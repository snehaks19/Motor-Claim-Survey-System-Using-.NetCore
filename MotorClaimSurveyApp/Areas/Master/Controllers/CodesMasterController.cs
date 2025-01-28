using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using MotorClaimSurveyApp.Areas.Master.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Master.Controllers
{
    [Area("Master")]
    [Authorize]
    public class CodesMasterController : Controller
    {
        private readonly IConfiguration _configuration;
        public CodesMasterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CodesMasterListing()
        {
            return View();
        }

        public async Task<IActionResult> CodesMasterListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiCodesMaster/GetAllCodes");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result);

                    DataTable dt = new DataTable();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var key in list[0].Keys)
                        {
                            dt.Columns.Add(key);
                        }

                        foreach (var dict in list)
                        {
                            var row = dt.NewRow();
                            foreach (var key in dict.Keys)
                            {
                                var value = dict[key];

                                if (value != null && value.GetType().IsClass && value.GetType() != typeof(string))
                                {
                                    row[key] = string.Empty; // Or use: row[key] = "Object";
                                }
                                else
                                {
                                    row[key] = value?.ToString() ?? string.Empty;
                                }
                            }
                            dt.Rows.Add(row);
                        }
                    }

                    int totalRecord = 0;
                    int filterRecord = 0;
                    var draw = Request.Form["draw"].FirstOrDefault();
                    var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                    var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                    var searchValue = Request.Form["search[value]"].FirstOrDefault();
                    int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                    int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                    var data = ConvertDataTableToList(dt);

                    // Total records before filtering
                    totalRecord = data.Count();

                    // Apply search filter first
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        data = data.Where(x => x.CM_CODE.Trim().ToLower().Contains(searchValue.Trim().ToLower())
                         || x.CM_TYPE.Trim().ToLower().Contains(searchValue.Trim().ToLower())
                         || x.CM_DESC.Trim().ToLower().Contains(searchValue.Trim().ToLower())
                         || x.CM_VALUE.Trim().ToLower().Contains(searchValue.Trim().ToLower())
                        ).ToList();

                    }

                    // Total records after filtering
                    filterRecord = data.Count();

                    // Apply sorting (dynamically)
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                    {
                        data = data.OrderBy(o => o.GetType().GetProperty(sortColumn)?.GetValue(o, null)).ToList();
                    }

                    // Pagination after sorting and filtering
                    var empList = data.Skip(skip).Take(pageSize).ToList();

                    var returnObj = new
                    {
                        draw = draw,
                        recordsTotal = totalRecord,
                        recordsFiltered = filterRecord,
                        data = empList // Use empList for paginated data
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
        public async Task<IActionResult> CodesMasterEdit(string cmCode,string cmType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            CodesMasterViewModel codesMasterViewModel = new CodesMasterViewModel
            {
                CodesMasterEntity = new CodesMasterEntity()
            };
            if (!string.IsNullOrEmpty(cmCode) && !string.IsNullOrEmpty(cmType))
            {
                codesMasterViewModel.Mode = "U";
                HttpResponseMessage response = await client.GetAsync($"/api/ApiCodesmaster/GetById/{cmCode}/{cmType}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    codesMasterViewModel.CodesMasterEntity = JsonConvert.DeserializeObject<CodesMasterEntity>(result);
                                    
                    return View("CodesMasterEdit", codesMasterViewModel);
                }
            }
            else
            {
                
                codesMasterViewModel.Mode = "I";

            }
            return View(codesMasterViewModel);
        }

        [HttpPost]
        
        public async Task<IActionResult> CodesMasterEdit(CodesMasterViewModel cmVmodel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            string userId = HttpContext.Session.GetString("UserId");

            if (cmVmodel.Mode == "I")                        
            {
                if (ModelState.IsValid)
                {
                    
                    cmVmodel.CodesMasterEntity.CmCrBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiCodesMaster/AddNewCodesMaster", cmVmodel.CodesMasterEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        cmVmodel.Mode = "U";
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error saving the code. Please try again.";
                    }

                }
                return Redirect("~/Master/CodesMaster/CodesMasterEdit?cmCode=" + cmVmodel.CodesMasterEntity.CmCode + "&cmType=" + cmVmodel.CodesMasterEntity.CmType);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    cmVmodel.CodesMasterEntity.CmUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiCodesMaster/UpdateCodesMaster", cmVmodel.CodesMasterEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        cmVmodel.Mode = "U";
                        
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating the code. Please try again.";
                    }

                }
                return View(cmVmodel);
            }

            
            
        }

        //[HttpPost]
        public async Task<IActionResult> DeleteCode(string code, string type)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {
                CodesMasterViewModel codesMasterViewModel = new CodesMasterViewModel
                {
                    CodesMasterEntity = new CodesMasterEntity()
                };

                codesMasterViewModel.CodesMasterEntity.CmCode = code;
                codesMasterViewModel.CodesMasterEntity.CmType = type;
                HttpResponseMessage response = await client.PostAsJsonAsync($"/api/ApiCodesMaster/Delete",codesMasterViewModel.CodesMasterEntity);

                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3000}");
                    string stmsg = await response2.Content.ReadAsStringAsync();
                    
                    return Ok(new { success = true, message = stmsg });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting code. Please try again later."; 
                    return RedirectToAction("CodesMasterList");
                }
            }
        }

        public async Task<IActionResult> CodesMasterUpdate(string cmCode, string cmType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            HttpResponseMessage response = await client.GetAsync($"/api/ApiCodesMaster/GetById/{cmCode}/{cmType}");
            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();
                CodesMasterEntity codeMaster = JsonConvert.DeserializeObject<CodesMasterEntity>(result);
                CodesMasterViewModel model = new CodesMasterViewModel
                {
                   CodesMasterEntity= codeMaster
                };
                return View("CodesMasterEdit", model);
            }

            //TempData["ErrorMessage"] = "Code not found.";
            return RedirectToAction("CodesMaster");
        }
        [HttpPost]
        public async Task<IActionResult> CheckIfCodeExists(string pCode, string pType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {

                HttpResponseMessage response = await client.GetAsync($"/api/ApiCodesMaster/CheckIfCodeExists/{pCode}/{pType}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    int rowCount = int.Parse(responseData); 
                   
                    if (rowCount > 0)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{4000}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        //TempData["SuccessMessage"] = stmsg;
                        return Ok(new { exists = true, message = stmsg });
                    }
                    else
                    {
                        return Ok(new { exists = false });
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error while checking code existence");
                }

            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CodesMasterUpdate(CodesMasterViewModel viewModel)
        //{
        //    string baseAddress = _configuration.GetValue<string>("ApiUrl");

        //    HttpClient client = new HttpClient()
        //    {
        //        BaseAddress = new Uri(baseAddress)
        //    };

        //    if (ModelState.IsValid)
        //    {
        //        HttpResponseMessage response = await client.PutAsJsonAsync("/api/ApiCodesMaster/CodeEditing", viewModel.codesMaster);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            TempData["SuccessMessage"] = "Code updated successfully.";
        //            return RedirectToAction("CodesMaster");
        //        }
        //    }
        //    return View(viewModel);
        //}



    }
}
