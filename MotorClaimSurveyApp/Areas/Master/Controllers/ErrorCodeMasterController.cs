using BusinessEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Master.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Master.Controllers
{
    [Area("Master")]
    [Authorize]
    public class ErrorCodeMasterController : Controller
    {
        private readonly IConfiguration _configuration;
        public ErrorCodeMasterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ErrorCodeMasterListing()
        {
            return View();
        }

        public async Task<IActionResult> ErrorCodeMasterListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiErrorCodeMaster/GetAllCodes");
                if (response.IsSuccessStatusCode)
                {
                    //DataTable dt = await httpResponseMessage.Content.ReadFromJsonAsync<DataTable>();
                    var result = await response.Content.ReadAsStringAsync();
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);
                    int totalRecord = 0;
                    int filterRecord = 0;
                    var draw = Request.Form["draw"].FirstOrDefault();
                    var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                    var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                    var searchValue = Request.Form["search[value]"].FirstOrDefault();
                    int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                    int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                    var data = ConvertDataTableToList(dt);
                    //get total count of data in table
                    totalRecord = data.Count();
                    // search data when search value found
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        data = data.Where(x => (x.ERR_CODE != null && x.ERR_CODE.Trim().ToLower().Contains(searchValue.Trim().ToLower()))
                                               || (x.ERR_TYPE != null && x.ERR_TYPE.Trim().ToLower().Contains(searchValue.Trim().ToLower()))
                                               || (x.ERR_DESC != null && x.ERR_DESC.Trim().ToLower().Contains(searchValue.Trim().ToLower()))
                                              ).ToList();
                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                        data = data.OrderBy(o => "o.ERR_CODE" + sortColumn).ToList();
                    //pagination
                    var empList = data.Skip(skip).Take(pageSize).ToList();
                    var returnObj = new
                    {
                        draw = draw,
                        recordsTotal = totalRecord,
                        recordsFiltered = filterRecord,
                        data = empList
                    };
                    return Ok(returnObj);
                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public static List<dynamic> ConvertDataTableToList(DataTable pDataTable)
        {
            var data = new List<dynamic>();
            if (pDataTable != null)
            {
                foreach (DataRow item in pDataTable.Rows)
                {
                    IDictionary<string, object> dn = new ExpandoObject();

                    foreach (var column in pDataTable.Columns.Cast<DataColumn>())
                    {
                        dn[column.ColumnName] = item[column];
                    }
                    data.Add(dn);
                }
            }
            return data;
        }

        [HttpGet]
        public async Task<IActionResult> ErrorCodeMasterEdit(string errCode, string errType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            ErrorCodeMasterViewModel errorCodeMasterViewModel = new ErrorCodeMasterViewModel
            {
                 ErrorCodeMasterEntity =new ErrorCodeMasterEntity()
            };
            errorCodeMasterViewModel.ErrorCodeMasterEntity.ErrType = errType;
            await FillDropDown("ERROR_TYPE", errorCodeMasterViewModel);

            if (!string.IsNullOrEmpty(errCode) && !string.IsNullOrEmpty(errType))
            {
                errorCodeMasterViewModel.Mode = "U";
                HttpResponseMessage response = await client.GetAsync($"/api/ApiErrorCodeMaster/GetById/{errCode}/{errType}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    errorCodeMasterViewModel.ErrorCodeMasterEntity = JsonConvert.DeserializeObject<ErrorCodeMasterEntity>(result);
                   
                    return View("ErrorCodeMasterEdit", errorCodeMasterViewModel);
                }
            }
            else
            {
                
                errorCodeMasterViewModel.Mode = "I";

            }
            return View(errorCodeMasterViewModel);
        }

        [HttpPost]
        
        public async Task<IActionResult> ErrorCodeMasterEdit(ErrorCodeMasterViewModel emVmodel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            string userId = HttpContext.Session.GetString("UserId");
            if (emVmodel.Mode == "I")
            {
                if (ModelState.IsValid)
                {
                    emVmodel.ErrorCodeMasterEntity.ErrCrBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiErrorCodeMaster/AddNewErrorCodeMaster", emVmodel.ErrorCodeMasterEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        emVmodel.Mode = "U";
                        await FillDropDown("ERROR_TYPE", emVmodel);

                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;

                    }


                }
                return Redirect("~/Master/ErrorCodeMaster/ErrorCodeMasterEdit?errCode=" + emVmodel.ErrorCodeMasterEntity.ErrCode + "&errType=" + emVmodel.ErrorCodeMasterEntity.ErrType);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    emVmodel.ErrorCodeMasterEntity.ErrUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiErrorCodeMaster/UpdateErrorCodeMaster", emVmodel.ErrorCodeMasterEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        await FillDropDown("ERROR_TYPE", emVmodel);
                        emVmodel.Mode = "U";
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                    }


                }
                return View(emVmodel);
            }



        }

        [HttpPost]
        public async Task<IActionResult> DeleteCode(string code, string type)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {
                ErrorCodeMasterViewModel errorCodeMasterViewModel = new ErrorCodeMasterViewModel
                {
                    ErrorCodeMasterEntity = new ErrorCodeMasterEntity()
                };
                errorCodeMasterViewModel.ErrorCodeMasterEntity.ErrCode = code;
                errorCodeMasterViewModel.ErrorCodeMasterEntity.ErrType = type;
                HttpResponseMessage response = await client.GetAsync($"/api/ApiErrorCodeMaster/Delete/{code}/{type}");

                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3000}");
                    string stmsg = await response2.Content.ReadAsStringAsync();
                    
                    return Ok(new { success = true, message = stmsg });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting code. Please try again later."; return RedirectToAction("ErrorCodeMasterListing");
                }
            }
        }

        public async Task<IActionResult> ErrorCodeMasterUpdate(string errCode, string errType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            HttpResponseMessage response = await client.GetAsync($"/api/ApiErrorCodeMaster/GetById/{errCode}/{errType}");
            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();
                ErrorCodeMasterEntity errorCodeMasterEntity = JsonConvert.DeserializeObject<ErrorCodeMasterEntity>(result);
                ErrorCodeMasterViewModel model = new ErrorCodeMasterViewModel
                {
                     ErrorCodeMasterEntity=new ErrorCodeMasterEntity()
                };
                return View("ErrorCodeMasterEdit", model);
            }

            TempData["ErrorMessage"] = "Code not found.";
            return RedirectToAction("CodesMaster");
        }
        [HttpPost]
        public async Task<IActionResult> CheckIfCodeExists(string pCode)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {

                HttpResponseMessage response = await client.GetAsync($"/api/ApiErrorCodeMaster/CheckIfCodeExists/{pCode}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    int rowCount = int.Parse(responseData); // Convert the response to an integer

                    // Use `rowCount` as needed
                    if (rowCount > 0)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{4000}");
                        string stmsg = await response2.Content.ReadAsStringAsync();

                        return Ok(new { success = true, message = stmsg });
                    }
                    else
                    {
                        return Ok(new { success = false });
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error while checking code existence");
                }

            }
        }

        public async Task FillDropDown(string msg, ErrorCodeMasterViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            ErrorCodeMasterViewModel errorCodesMasterModel = new ErrorCodeMasterViewModel();
            errorCodesMasterModel.ListErrType = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiErrorCodeMaster/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListErrType.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListErrType.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }

        public async Task<IActionResult> GetDeleteConfirmationMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3001}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the confirmation message." });
            }
        }
        public async Task<IActionResult> GetApprovalConfirmationMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2007}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the confirmation message." });
            }
        }



        public async Task<IActionResult> GetsubmitSurveyConfirmationMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2008}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the confirmation message." });
            }
        }

        public async Task<IActionResult> GetClaimApprConfirmationMsg()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3005}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the confirmation message." });
            }
        }

        public async Task<IActionResult> GetClaimRejectionMsg()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2009}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the confirmation message." });
            }
        }

        public async Task<IActionResult> GetDateErrorMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{8001}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Date message." });
            }
        }

        public async Task<IActionResult> GetLossDateErrorMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9000}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }
        public async Task<IActionResult> GetLossDateErrorMessage1()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9013}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }
        public async Task<IActionResult> GetLossDateErrorMessage2()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9003}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }

        public async Task<IActionResult> GetLossDateErrorMessage3()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9008}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }

        public async Task<IActionResult> GetLossDateErrorMessage4()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9014}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }

        public async Task<IActionResult> GetLossDateErrorMessage5()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9006}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }
        public async Task<IActionResult> GetMobileErrorMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{8010}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Loss Date message." });
            }
        }
        public async Task<IActionResult> GetPolFcErrorMessage()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{1200}");

            // Check if the response is successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the message content
                string stmsg = await response2.Content.ReadAsStringAsync();

                // Return the message as part of the JSON response
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                // If the API request fails, return an error message
                return Json(new { success = false, message = "Failed to fetch the Policy Fc message." });
            }
        }



    }
}
