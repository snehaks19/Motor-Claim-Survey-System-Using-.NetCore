using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MotorClaimSurveyApp.Areas.Transaction.ViewModels;
using MotorClaimSurveyApp.Models;
using Newtonsoft.Json;

namespace MotorClaimSurveyApp.Controllers
{

    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }   
        
        public async Task<IActionResult> Index()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            StatisticsModel statisticsModel = new StatisticsModel();

            HttpResponseMessage httpResponse = await client.GetAsync($"/api/ApiDashboard/CheckApprPolicyCount/");
            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await httpResponse.Content.ReadAsStringAsync();
                statisticsModel.ApprPolicyCount=Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse2 = await client.GetAsync($"/api/ApiDashboard/CheckUnApprPolicyCount/");
            if (httpResponse2.IsSuccessStatusCode)
            {
                var result = await httpResponse2.Content.ReadAsStringAsync();
                statisticsModel.UnApprPolicyCount = Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse3 = await client.GetAsync($"/api/ApiDashboard/CheckApprClaimCount/");
            if (httpResponse3.IsSuccessStatusCode)
            {
                var result = await httpResponse3.Content.ReadAsStringAsync();
                statisticsModel.ApprClaimCount = Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse4 = await client.GetAsync($"/api/ApiDashboard/CheckRejClaimCount/");
            if (httpResponse4.IsSuccessStatusCode)
            {
                var result = await httpResponse4.Content.ReadAsStringAsync();
                statisticsModel.RejClaimCount = Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse5 = await client.GetAsync($"/api/ApiDashboard/CheckPendngClaimCount/");
            if (httpResponse5.IsSuccessStatusCode)
            {
                var result = await httpResponse5.Content.ReadAsStringAsync();
                statisticsModel.PendingClaimCount = Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse6 = await client.GetAsync($"/api/ApiDashboard/GetPolicyCount/");
            if (httpResponse6.IsSuccessStatusCode)
            {
                var result = await httpResponse6.Content.ReadAsStringAsync();
                statisticsModel.PolicyCount = Convert.ToInt32(result);
            }

            return View(statisticsModel);
        }

        public async Task<IActionResult> SurveyIndex()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            StatisticsModel statisticsModel = new StatisticsModel();

            HttpResponseMessage httpResponse = await client.GetAsync($"/api/ApiDashboard/CheckPendingSurveyCount/");
            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await httpResponse.Content.ReadAsStringAsync();
                statisticsModel.PendingSurveyCount = Convert.ToInt32(result);
            }
            HttpResponseMessage httpResponse2 = await client.GetAsync($"/api/ApiDashboard/CheckSubmittedSurveyCount/");
            if (httpResponse2.IsSuccessStatusCode)
            {
                var result = await httpResponse2.Content.ReadAsStringAsync();
                statisticsModel.SubmittedSurveyCount = Convert.ToInt32(result);
            }
            

            return View("SurveyStatistics",statisticsModel);
        }
    }
}
