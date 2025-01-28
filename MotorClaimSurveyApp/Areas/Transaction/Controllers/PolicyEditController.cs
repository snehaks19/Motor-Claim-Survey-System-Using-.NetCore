using BusinessEntity;
using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Master.ViewModels;
using MotorClaimSurveyApp.Areas.Transaction.ViewModels;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Reflection;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class PolicyEditController : Controller
    {
        private readonly IConfiguration _configuration;
        public PolicyEditController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PolicyEdit(int polUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            PolicyViewModel policyViewModel = new PolicyViewModel
            {
                objMotorPolicyEntity = new MotorPolicyEntity()
            };

            await FillCurrencyDropDown("CURRENCY", policyViewModel);
            await FillVehMakeDropDown("VEH_MAKE", policyViewModel);
            await FillVehModelDropDown(policyViewModel.objMotorPolicyEntity.PolVehMake, policyViewModel);

            if (!string.IsNullOrEmpty(polUid.ToString()) && polUid != 0)
            {

                policyViewModel.Mode = "U";

                HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetById/{polUid}");

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadAsStringAsync();
                    policyViewModel.objMotorPolicyEntity = JsonConvert.DeserializeObject<MotorPolicyEntity>(result);

                    await FillVehModelDropDown(policyViewModel.objMotorPolicyEntity.PolVehMake, policyViewModel);

                    //TempData["ErrorMessage"] = "Policy not found.";
                    return View("PolicyEdit", policyViewModel);
                }
            }
            else
            {
                //TempData["ErrorMessage"] = "Error";
                policyViewModel.Mode = "I";
            }
            return View(policyViewModel);
        }

        public async Task FillCurrencyDropDown(string msg, PolicyViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            PolicyViewModel policyViewModel = new PolicyViewModel();
            policyViewModel.ListCurrCode = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetDropDown/{errorMsg}");


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

        public async Task FillVehMakeDropDown(string msg, PolicyViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            PolicyViewModel policyViewModel = new PolicyViewModel();
            policyViewModel.ListVehMake = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetDropDown/{errorMsg}");

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

        private async Task FillVehModelDropDown(string vehicleMake, PolicyViewModel policyViewModel)
       {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            PolicyViewModel model = new PolicyViewModel();
            policyViewModel.ListVehModel = new List<SelectListItem>();

            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetModel/{vehicleMake}");
            
            if (response.IsSuccessStatusCode)
            {
                var vehicleModelList = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(vehicleModelList);

                //policyViewModel.ListVehModel.Add(new SelectListItem
                //{
                //    Text = "--SELECT--",
                //    Value = ""
                //});

                foreach (DataRow dataRow in dt.Rows)
                {
                    policyViewModel.ListVehModel.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetConversionRate(string currencyCode, decimal fcAmount)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            PolicyViewModel policyViewModel = new PolicyViewModel
            {
                currencyCode = currencyCode,
                fcAmount = fcAmount
            };

            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetRate/{currencyCode}");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                decimal conversionRate = Convert.ToDecimal(responseContent);
                decimal lcAmount = fcAmount * conversionRate;

                return Json(new { success = true, lcAmount = lcAmount });
            }
            else
            {
                return Json(new { success = false, message = "Failed to fetch conversion rate." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetVehicleModels(string make)
        {
            try
            {
                CodesMasterManager codesMasterManager = new CodesMasterManager();
                string baseAddress = _configuration.GetValue<string>("ApiUrl");
                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                };

                PolicyViewModel policyViewModel = new PolicyViewModel();
                policyViewModel.ListVehModel = new List<SelectListItem>();

                HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/GetModel/{make}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                    //policyViewModel.ListVehModel.Add(new SelectListItem
                    //{
                    //    Text = "--SELECT--",
                    //    Value = ""
                    //});

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        policyViewModel.ListVehModel.Add(new SelectListItem
                        {
                            Text = dataRow["CM_DESC"].ToString(),
                            Value = dataRow["CM_CODE"].ToString()
                        });
                    }
                    return Json(policyViewModel.ListVehModel);  
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PolicyEdit(long poluid, PolicyViewModel pModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            PolicyViewModel policyViewModel = new PolicyViewModel()
            {
                objMotorPolicyEntity = new MotorPolicyEntity()
            };
            string userId = HttpContext.Session.GetString("UserId");
            if (pModel.Mode == "I")
            {
                if (ModelState.IsValid)
                {
                    pModel.objMotorPolicyEntity.PolCrBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiPolicy/SavePolicy", pModel.objMotorPolicyEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var polUid = JsonConvert.DeserializeObject<long>(result);

                        pModel.Mode = "U";
                        
                        HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                        string stmsg = await response1.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;

                        return Redirect("~/Transaction/PolicyEdit/PolicyEdit?polUid=" + polUid);
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
                    pModel.objMotorPolicyEntity.PolUid = poluid;
                    pModel.objMotorPolicyEntity.PolUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiPolicy/UpdatePolicy", pModel.objMotorPolicyEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        pModel.Mode = "U";

                        HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                        string stmsg = await response1.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;

                        return Redirect("~/Transaction/PolicyEdit/PolicyEdit?polUid=" + poluid);
                    }
                    else
                    {
                        //TempData["ErrorMessage"] = "Error updating the code. Please try again.";
                    }

                }
                return Redirect("~/Transaction/PolicyEdit/PolicyEdit");
            }



        }

        [HttpPost]
        public async Task<IActionResult> ChangeApprovalStatus(long polUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            PolicyViewModel policyViewModel = new PolicyViewModel()
            {
                objMotorPolicyEntity = new MotorPolicyEntity()
            };
            
            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/UpdateApprStatus/{polUid}");

            if (response.IsSuccessStatusCode)
            {
                policyViewModel.Mode = "A";
                policyViewModel.objMotorPolicyEntity.PolApprStatus = "A";

                HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2002}");
                string stmsg = await response1.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = stmsg;

                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false, message = "Error updating approval status." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeApprovalStatus2(long polUid)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            PolicyViewModel policyViewModel = new PolicyViewModel()
            {
                objMotorPolicyEntity = new MotorPolicyEntity()
            };
            string userId = HttpContext.Session.GetString("UserId");

            
            HttpResponseMessage response = await client.GetAsync($"/api/ApiPolicy/UpdateApprStatus2/{polUid}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                policyViewModel.Mode = "A";
                policyViewModel.objMotorPolicyEntity.PolApprStatus = "A";

                HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2002}");
                string stmsg = await response1.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = stmsg;

                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false, message = "Error updating approval status." });
            }
        }
    }



    }

