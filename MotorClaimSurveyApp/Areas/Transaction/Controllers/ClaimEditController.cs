using BusinessEntity;
using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Transaction.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Json;
using System.Reflection;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class ClaimEditController : Controller
    {
        private readonly IConfiguration _configuration;
        public ClaimEditController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ClearSuccessMessage()
        {
            TempData.Remove("SuccessMessage");
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ClaimEdit(long clmUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            ClaimViewModel claimViewModel = new ClaimViewModel()
            {
                motorClaimEntity = new MotorClaimEntity(),
                motorClmSurHdrEntity = new MotorClmSurHdrEntity()
            };
            claimViewModel.motorClaimEntity.ClmUid = clmUid;

            await FillPolicyNoDropDown(claimViewModel);
            await FillVehMakeDropDown("VEH_MAKE", claimViewModel);
            await FillVehModelDropDown(claimViewModel.motorClaimEntity.ClmVehMake, claimViewModel);


            if (!string.IsNullOrEmpty(clmUid.ToString()) && clmUid != 0)
            {
                claimViewModel.Mode = "U";
                
                HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetById/{clmUid}");
                HttpResponseMessage response1 = await client.GetAsync($"/api/ApiClaim/CheckSurveyStatus/{clmUid}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    claimViewModel.motorClaimEntity = JsonConvert.DeserializeObject<MotorClaimEntity>(result);
                    
                    await FillVehModelDropDown(claimViewModel.motorClaimEntity.ClmVehMake, claimViewModel);
                    
                    if (response1.IsSuccessStatusCode)
                    {
                        var result2 = await response1.Content.ReadAsStringAsync();
                        claimViewModel.motorClmSurHdrEntity.SurStatus = result2;
                        
                        return View("ClaimEdit", claimViewModel);


                    }
                    return View("ClaimEdit", claimViewModel);
                }
            }
            else
            {            
                claimViewModel.Mode = "I";
            }
            return View(claimViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ClaimEdit(long clmuid, string userType, ClaimViewModel cModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            ClaimViewModel claimViewModel = new ClaimViewModel()
            {
                motorClaimEntity = new MotorClaimEntity()
            };
            string userId = HttpContext.Session.GetString("UserId");

            if (userType == "U")
            {
                if (cModel.Mode == "I")
                {
                    if (ModelState.IsValid)
                    {
                        cModel.motorClaimEntity.ClmCrBy = userId;
                        HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiClaim/SaveClaim", cModel.motorClaimEntity);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var clmUid = JsonConvert.DeserializeObject<long>(result);

                            cModel.Mode = "U";
                            HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                            string stmsg = await response1.Content.ReadAsStringAsync();
                            TempData["SuccessMessage"] = stmsg;

                            return Redirect("~/Transaction/ClaimEdit/ClaimEdit?clmUid=" + clmUid);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error saving the code. Please try again.";
                        }

                    }
                    return Redirect("~/Transaction/ClaimEdit/ClaimEdit");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        cModel.motorClaimEntity.ClmUid = clmuid;
                        cModel.motorClaimEntity.ClmUpBy = userId;
                        HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiClaim/UpdateClaim", cModel.motorClaimEntity);

                        if (response.IsSuccessStatusCode)
                        {
                            cModel.Mode = "U";
                            HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                            string stmsg = await response1.Content.ReadAsStringAsync();
                            TempData["SuccessMessage"] = stmsg;
                            return Redirect("~/Transaction/ClaimEdit/ClaimEdit?clmUid=" + clmuid);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error updating the code. Please try again.";
                        }

                    }
                    return Redirect("~/Transaction/ClaimEdit/ClaimEdit");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Error in Saving the Code";
                return Redirect("~/Transaction/ClaimEdit/ClaimEdit?clmUid=" + clmuid);

            }

        }

        public async Task FillPolicyNoDropDown(ClaimViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            model.ListPolicyNo = new List<SelectListItem>();
            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetDropDown");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListPolicyNo.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListPolicyNo.Add(new SelectListItem
                    {
                        Text = dataRow["POL_NO"].ToString(),
                        Value = dataRow["POL_NO"].ToString()
                    });
                }
            }
        }

        public async Task FillVehMakeDropDown(string msg, ClaimViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            ClaimViewModel claimViewModel = new ClaimViewModel();
            claimViewModel.ListVehMake = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListVehMake.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListVehMake.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }

        private async Task FillVehModelDropDown(string vehicleMake, ClaimViewModel claimViewModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            ClaimViewModel model = new ClaimViewModel();
            model.ListVehModel = new List<SelectListItem>();

            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetModel/{vehicleMake}");
           
            if (response.IsSuccessStatusCode)
            {
                var vehicleModelList = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(vehicleModelList);
                claimViewModel.ListVehModel.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    claimViewModel.ListVehModel.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetPolicyDetails(string policyNumber)
        {
            string formattedPolicyNumber = policyNumber.Replace("/", "_");
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            ClaimViewModel model = new ClaimViewModel()
            {
                motorClaimEntity = new MotorClaimEntity()
            };
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetPolicyDetail/{formattedPolicyNumber}");
                if (response.IsSuccessStatusCode)
                { 
                    
                    var result = await response.Content.ReadAsStringAsync();
                    model.motorClaimEntity = JsonConvert.DeserializeObject<MotorClaimEntity>(result);
                    await FillVehMakeDropDown("VEH_MAKE", model);
                    await FillVehModelDropDown(model.motorClaimEntity.ClmVehMake, model);
                    return Content(result, "application/json");
                   
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
        [HttpPost]
        public async Task<IActionResult> IntimateToSurvey(long clmUid, string userId, string userType)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            if (userType == "U")
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/SaveClaim/{clmUid}/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2003}");
                    string stmsg = await response1.Content.ReadAsStringAsync();

                    return Json(new { success = true, message = stmsg, hideButton = true, icon = "success" , title = "Success" });
                }
                return Json(new { success = false, message = "Failed to Save Claim" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Sent for Survey",icon="error" ,title="Error"});
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> GetVehicleModels(string vehicleMake)
        {
            try
            {
                CodesMasterManager codesMasterManager = new CodesMasterManager();
                string baseAddress = _configuration.GetValue<string>("ApiUrl");
                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                };

                ClaimViewModel claimViewModel = new ClaimViewModel();
                claimViewModel.ListVehModel = new List<SelectListItem>();

                HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetModel/{vehicleMake}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                    claimViewModel.ListVehModel.Add(new SelectListItem
                    {
                        Text = "--SELECT--",
                        Value = ""
                    });

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        claimViewModel.ListVehModel.Add(new SelectListItem
                        {
                            Text = dataRow["CM_DESC"].ToString(),
                            Value = dataRow["CM_CODE"].ToString()
                        });
                    }
                    return Json(claimViewModel.ListVehModel);  
                }

                return Json(new { success = false, message = "Failed to retrieve vehicle models." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error fetching models: " + ex.Message
                });
            }

        }

        
        [HttpPost]
        public async Task<IActionResult> ViewSurvey(int clmUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response1 = await client.GetAsync($"/api/ApiClaim/GetSurUid/{clmUid}");

            if (response1.IsSuccessStatusCode)
            {
                var result = await response1.Content.ReadAsStringAsync();
                long surUid = Convert.ToInt64(result);              
                return Json(new { success = true, surUid = surUid });
            }

            return Json(new { success = false, message = "Failed to retrieve survey UID." });
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(long clmUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            ClaimViewModel claimViewModel = new ClaimViewModel()
            {
                motorClaimEntity = new MotorClaimEntity()
            };

            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/ApproveClaim/{clmUid}");

            if (response.IsSuccessStatusCode)
            {
                claimViewModel.Mode = "A";
                claimViewModel.motorClaimEntity.ClmApprStatus = "A";

                HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2005}");
                string stmsg = await response2.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = stmsg;
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                return BadRequest(new { success = false, message = "Error updating approval status." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int clmUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            ClaimViewModel claimViewModel = new ClaimViewModel()
            {
                motorClaimEntity = new MotorClaimEntity()
            };

            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/RejectClaim/{clmUid}");

            if (response.IsSuccessStatusCode)
            {
                claimViewModel.Mode = "R";
                claimViewModel.motorClaimEntity.ClmApprStatus = "R";
                HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2006}");
                string stmsg = await response2.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = stmsg;
                return Json(new { success = true, message = stmsg });
            }
            else
            {
                return BadRequest(new { success = false, message = "Error updating approval status." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckPoliceReportNumber(string reportNo, string clmUid)
        {
            try
            {
                string baseAddress = _configuration.GetValue<string>("ApiUrl");

                HttpClient client = new HttpClient()
                {
                    BaseAddress = new System.Uri(baseAddress)
                };

                ClaimViewModel claimViewModel = new ClaimViewModel()
                {
                    motorClaimEntity = new MotorClaimEntity()
                };

                if (clmUid == "0")
                {
                    clmUid = null;
                }

                if (!string.IsNullOrEmpty(clmUid))
                {
                    HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetPoliceNumber/{clmUid}");
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        // Retrieve the old Police Report Number for the provided claim UID
                        string oldNo = result;

                        // If the new report number matches the old, no further checks are needed
                        if (reportNo == oldNo)
                        {
                            return Json(new { success = false });
                        }
                        else
                        {

                            // Check if the Police Report Number is a duplicate

                            HttpResponseMessage response1 = await client.GetAsync($"/api/ApiClaim/CheckDuplicatePoliceNumber/{reportNo}");
                            if (response1.IsSuccessStatusCode)
                            {
                                var result1 = await response1.Content.ReadAsStringAsync();
                                int rows = Convert.ToInt32(result1);

                                if (rows > 0)
                                {
                                    // Retrieve the error message
                                    HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9010}");
                                    string stmsg = await response2.Content.ReadAsStringAsync();

                                    return Json(new { success = true, message = stmsg, OldNo= oldNo });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        success = false
                                    });
                                }

                            }
                            else
                            {
                                return Ok();
                            }
                        }
                    }
                    else
                    {
                        return Ok();
                    }
                }
            
                else
                {
                    HttpResponseMessage response1 = await client.GetAsync($"/api/ApiClaim/CheckDuplicatePoliceNumber/{reportNo}");
                    if (response1.IsSuccessStatusCode)
                    {
                        var result1 = await response1.Content.ReadAsStringAsync();
                        int rows = Convert.ToInt32(result1);

                        if (rows > 0)
                        {
                            // Retrieve the error message
                            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9010}");
                            string stmsg = await response2.Content.ReadAsStringAsync();

                            return Json(new { success = true, message = stmsg });
                        }
                        else
                        {
                            return Json(new
                            {
                                success = false
                            });
                        }

                    }
                    else
                    {
                        return BadRequest();
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return Json(new { exists = false, message = "An error occurred while processing your request." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPolUid(string policyNumber)
        {
            string formattedPolicyNumber = policyNumber.Replace("/", "-");
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpResponseMessage response = await client.GetAsync($"/api/ApiClaim/GetPolicyUid/{formattedPolicyNumber}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                long polUid = Convert.ToInt64(result);
                return Json(new { success = true, polUid = polUid });
            }

            return Json(new { success = false, message = "Failed to retrieve Policy UID." });
        }


    }
}


    

