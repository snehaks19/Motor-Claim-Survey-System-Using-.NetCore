using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Transaction.ViewModels;
using Newtonsoft.Json;
using System.Data;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class SurveyDtlController : Controller
    {
        private readonly IConfiguration _configuration;
        public SurveyDtlController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddDetails(int surdUid,int surUid,string currencyCode)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            SurveyDetailsViewModel detailsModel = new SurveyDetailsViewModel()
            {
                MotorClmSurDtlEntity = new MotorClmSurDtlEntity(),
                
                currencyCode = currencyCode
            };
            detailsModel.MotorClmSurDtlEntity.SurdSurUid = surUid;
            //string surUid = HttpContext.Request.Query["surUid"];

            await FillItemCodeDropDown("VEH_PARTS", detailsModel);
            await FillTypeDropDown("ITEM_TYPE", detailsModel);
           


            if (!string.IsNullOrEmpty(surdUid.ToString()) && surdUid != 0)
            {
                //detailsModel.Mode = "U";
                
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/GetEachById/{surdUid}/{surUid}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    detailsModel.MotorClmSurDtlEntity = JsonConvert.DeserializeObject<MotorClmSurDtlEntity>(result);
                    detailsModel.currencyCode = currencyCode;
                    
                    return View("SurveyDetails", detailsModel);
                }
            }
            else
            {

                
                detailsModel.Mode = "I";
            }
            return View("SurveyDetails",detailsModel);
        }

        public async Task FillItemCodeDropDown(string msg, SurveyDetailsViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyDetailsViewModel detailsViewModel = new SurveyDetailsViewModel();
            detailsViewModel.ListItemCode = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListItemCode.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListItemCode.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }
        public async Task FillTypeDropDown(string msg, SurveyDetailsViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            SurveyDetailsViewModel detailsViewModel = new SurveyDetailsViewModel();
            detailsViewModel.ListItemType = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListItemType.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListItemType.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetConversionRate(string currencyCode, decimal lcAmount)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            SurveyDetailsViewModel detailModel = new SurveyDetailsViewModel
            {
                currencyCode = currencyCode,
                lcAmount = lcAmount
            };

            HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/GetRate/{currencyCode}");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                decimal conversionRate = Convert.ToDecimal(responseContent);

                decimal fcAmount = lcAmount / conversionRate;

                return Json(new { success = true, fcAmount = fcAmount });

            }
            else
            {

                return Json(new { success = false, message = "Failed to fetch conversion rate." });
            }



        }

        
     
        [HttpPost]
        public async Task<IActionResult> AddDetails(long surdUid, SurveyDetailsViewModel vModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            //string querySurdUid = HttpContext.Request.Query["SurUid"];

            SurveyDetailsViewModel detailsModel = new SurveyDetailsViewModel()
            {
                MotorClmSurDtlEntity = new MotorClmSurDtlEntity()
            };

            //if (vModel.Mode == "I")
            //{
                if (ModelState.IsValid)
                {

                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiSurveyDetails/SaveDetails", vModel.MotorClmSurDtlEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        surdUid = JsonConvert.DeserializeObject<long>(result);
                        var surUid = vModel.MotorClmSurDtlEntity.SurdSurUid;
                        var currencyCode = vModel.currencyCode;
                        detailsModel.MotorClmSurDtlEntity.SurdUid = surdUid;
                    //vModel.Mode = "U";

                    HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                    string stmsg = await response1.Content.ReadAsStringAsync();
                    TempData["DtlSuccessMessage"] = stmsg;

                    return Redirect("~/Transaction/SurveyDtl/AddDetails?surdUid=" + surdUid + "&surUid=" + surUid + "&currencyCode=" + currencyCode);
                       
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error saving the code. Please try again.";
                    }

                }
                return Redirect("~/Transaction/SurveyDtl/AddDetails");
            //}
            //else
            //{
            //    if (ModelState.IsValid)
            //    {
            //        //vModel.MotorClmSurDtlEntity.SurdUid = surdUid;
            //        HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiSurveyDetails/UpdateDetails", vModel.MotorClmSurDtlEntity);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            vModel.Mode = "U";
            //            TempData["SuccessMessage"] = "Code Updated Successfully!";
            //            return Redirect("~/Transaction/SurveyHdr/AddDetails?surdUid=" + surdUid);
            //        }
            //        else
            //        {
            //            TempData["ErrorMessage"] = "Error updating the code. Please try again.";
            //        }

            //    }
            //    return Redirect("~/Transaction/SurveyHdr/AddDetails");
            //}
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int code,int code2)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/Delete/{code}/{code2}");

                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response1 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3000}");
                    string stmsg = await response1.Content.ReadAsStringAsync();
                    TempData["SuccessMessage"] = stmsg;
                    return Json(new { success = true, message = stmsg });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting code. Please try again later.";
                    return RedirectToAction("CodesMasterList");
                }
            }
        }

        public async Task<IActionResult> CheckIfItemExists(string itemCode, long surveyId)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {

                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyDetails/CheckDuplicateItem/{itemCode}/{surveyId}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    int rowCount = int.Parse(responseData);

                    if (rowCount > 0)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{9012}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                       
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

    }
}
