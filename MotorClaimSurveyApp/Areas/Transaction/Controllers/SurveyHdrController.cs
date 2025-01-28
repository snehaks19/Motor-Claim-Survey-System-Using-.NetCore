using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Transaction.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Reflection;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class SurveyHdrController : Controller
    {
        private readonly IConfiguration _configuration;
        public SurveyHdrController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult SurveyListing()
        {
            return View();
        }
        public async Task<IActionResult> SurveyListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiSurveyHeader/GetAllCodes");
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
                        data = data.Where(x =>
                        (!DBNull.Value.Equals(x.SUR_NO) && x.SUR_NO?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_CLM_NO) && x.SUR_CLM_NO?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_DATE) && x.SUR_DATE?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_LOCATION) && x.SUR_LOCATION?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_FC_AMT) && x.SUR_FC_AMT?.ToString().Trim().Contains(searchValue.Trim()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_LC_AMT) && x.SUR_LC_AMT?.ToString().Trim().Contains(searchValue.Trim()) == true) ||
                        (!DBNull.Value.Equals(x.SUR_STATUS) && x.SUR_STATUS?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true)
                    ).ToList();


                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                        data = data.OrderBy(o => "o.SUR_UID" + sortColumn).ToList();
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
        public async Task<IActionResult> SurveyEdit(int surUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            SurveyHeaderViewModel surveyHeader = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity(),
                MotorClmSurDtlEntity =new MotorClmSurDtlEntity()

            };


            await FillLocationDropDown("LOCATION", surveyHeader);
            await FillCurrencyDropDown("CURRENCY", surveyHeader);

            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetSurveyDetails/{surUid}");
            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();
                surveyHeader.MotorClmSurHdrEntity = JsonConvert.DeserializeObject<MotorClmSurHdrEntity>(result);
                if (!string.IsNullOrEmpty(surveyHeader.MotorClmSurHdrEntity.SurCurr) && !string.IsNullOrEmpty(surveyHeader.MotorClmSurHdrEntity.SurLocation))
                {
                    surveyHeader.Mode = "U";

                    return View("Survey", surveyHeader);
                }
                else
                {
                    surveyHeader.Mode = "I";
                    return View("Survey", surveyHeader);
                }
                
                
            }
            return View("Survey", surveyHeader);
        }

        

        [HttpGet]
        public async Task<IActionResult> SurveyEdit2(int surUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            SurveyHeaderViewModel surveyHeader = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity(),
                
            };

            await FillLocationDropDown("LOCATION", surveyHeader);
            await FillCurrencyDropDown("CURRENCY", surveyHeader);

            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetSurveyDetails/{surUid}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                surveyHeader.MotorClmSurHdrEntity = JsonConvert.DeserializeObject<MotorClmSurHdrEntity>(result);
                surveyHeader.Mode = !string.IsNullOrEmpty(surveyHeader.MotorClmSurHdrEntity.SurCurr) &&
                                    !string.IsNullOrEmpty(surveyHeader.MotorClmSurHdrEntity.SurLocation)
                                    ? "U" : "I";
            }

            return View("Survey", surveyHeader); // Render the view with data
        }


        [HttpPost]
        public async Task<IActionResult> SurveyEdit(long surUid, SurveyHeaderViewModel sModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            SurveyHeaderViewModel model = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity()
            };
            string userId = HttpContext.Session.GetString("UserId");


            if (sModel.Mode == "I")
            {
                if (ModelState.IsValid)
                {
                    sModel.MotorClmSurHdrEntity.SurUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiSurveyHeader/SaveSurvey", sModel.MotorClmSurHdrEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var clmUid = JsonConvert.DeserializeObject<long>(result);

                        sModel.Mode = "U";
                        HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                        string stmsg = await response1.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;

                        return Redirect("~/Transaction/SurveyHdr/SurveyEdit?surUid=" + surUid);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error saving the code. Please try again.";
                    }

                }
                return Redirect("~/Transaction/PolicyEdit/PolicyEdit");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    sModel.MotorClmSurHdrEntity.SurUid = surUid;
                    sModel.MotorClmSurHdrEntity.SurUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiSurveyHeader/SaveSurvey", sModel.MotorClmSurHdrEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        sModel.Mode = "U";
                        HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                        string stmsg = await response1.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                        return Redirect("~/Transaction/SurveyHdr/SurveyEdit?surUid=" + surUid);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating the code. Please try again.";
                    }

                }
                return Redirect("~/Transaction/SurveyHdr/SurveyEdit");
            }



        }
        public async Task FillCurrencyDropDown(string msg, SurveyHeaderViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyHeaderViewModel surveyHeader = new SurveyHeaderViewModel();
            surveyHeader.ListCurrCode = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListCurrCode.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListCurrCode.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }

        public async Task FillLocationDropDown(string msg, SurveyHeaderViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyHeaderViewModel surveyHeader = new SurveyHeaderViewModel();
            surveyHeader.ListLocation = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetDropDown/{errorMsg}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListLocation.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListLocation.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(long surUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyHeaderViewModel model = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity()
            };

            try
            {

                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetSurveyDetails/{surUid}");
                if (response.IsSuccessStatusCode)
                {

                    //var policyDetails = await response.Content.ReadAsStringAsync();
                    //return Content(policyDetails, "application/json");
                    var result = await response.Content.ReadAsStringAsync();
                    model.MotorClmSurHdrEntity = JsonConvert.DeserializeObject<MotorClmSurHdrEntity>(result);
                    return Content(result, "application/json");
                    //return View("ClaimEdit", model);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error fetching policy details.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }


        }
       
        public async Task<IActionResult> SurveyDetailsListBinding(int code)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/GetDetailsById/{code}");
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
                        data = data.Where(x =>
    (!DBNull.Value.Equals(x.SURD_ITEM_CODE) && x.SURD_ITEM_CODE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.SURD_TYPE) && x.SURD_TYPE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.SURD_UNIT_PRICE) && x.SURD_UNIT_PRICE?.ToString().Trim().Contains(searchValue.Trim()) == true) ||
    (!DBNull.Value.Equals(x.SURD_QUANTITY) && x.SURD_QUANTITY?.ToString().Trim().Contains(searchValue.Trim()) == true) ||
    (!DBNull.Value.Equals(x.SURD_LC_AMT) && x.SURD_LC_AMT?.ToString().Trim().Contains(searchValue.Trim()) == true) ||
    (!DBNull.Value.Equals(x.SURD_FC_AMT) && x.SURD_FC_AMT?.ToString().Trim().Contains(searchValue.Trim()) == true)
).ToList();

                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                        data = data.OrderBy(o => "o.SURD_UID" + sortColumn).ToList();
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

        [HttpGet]
        public async Task<IActionResult> FetchClmUid(long surUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyHeaderViewModel model = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity()
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/GetClmUid/{surUid}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    //model.MotorClmSurHdrEntity.SurClmUid = JsonConvert.DeserializeObject<long>(result);
                    //var clmUid = model.MotorClmSurHdrEntity.SurClmUid;
                    return Content(result, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error fetching claim UID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }


        
        [HttpPost]
        public async Task<IActionResult> SubmitSurvey(long surUid ,string userID)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            SurveyHeaderViewModel model = new SurveyHeaderViewModel()
            {
                MotorClmSurHdrEntity = new MotorClmSurHdrEntity()
            };

            HttpResponseMessage response1 = await client.GetAsync($"/api/ApiSurveyHeader/GetClmUid/{surUid}");
            if (response1.IsSuccessStatusCode)
            {
                var surClmUid = await response1.Content.ReadAsStringAsync();
                
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHeader/SubmitSurvey/{surUid}/{surClmUid}/{userID}");
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2004}");
                    string stmsg = await response2.Content.ReadAsStringAsync();
                    

                    // Include the message in the JSON response
                    return Json(new { success = true, message = stmsg });

                }
                return Json(new { success = false, message = "Failed to Save Claim" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Save Claim" });
            }
        }

            
        }
    
}
