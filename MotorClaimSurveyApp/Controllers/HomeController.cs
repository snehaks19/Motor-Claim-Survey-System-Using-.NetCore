using Microsoft.AspNetCore.Mvc;
using MotorClaimSurveyApp.Models;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace MotorClaimSurveyApp.Controllers
{
    public class HomeController : Controller
    {

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                // If the session is not present, redirect to the login page
                return RedirectToAction("Login", "Login");
            }
            else if (HttpContext.Session.GetString("PtyType") == "U")
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
                    statisticsModel.ApprPolicyCount = Convert.ToInt32(result);
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

                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                    // Create an instance of StatisticsModel to store the data
                    //var statisticsModel = new StatisticsModel();

                    // Initialize the lists to hold the dates and counts
                    statisticsModel.PolicyDates = new List<string>();
                    statisticsModel.PolicyCounts = new List<int>();

                    // Loop through the DataTable rows and map the values to the model
                    foreach (DataRow row in dt.Rows)
                    {
                        // Extract the date and policy count from each row
                        DateTime policyDate = Convert.ToDateTime(row["POL_ISS_DT"]);
                        string formattedPolicyDate = policyDate.ToString("dd-MM-yyyy");
                        int policyCount = Convert.ToInt32(row["NO_OF_POLICY"]);

                        // Add the date and policy count to the respective lists
                        statisticsModel.PolicyDates.Add(formattedPolicyDate);
                        statisticsModel.PolicyCounts.Add(policyCount);
                    }

                    // Optionally, calculate the total number of policies
                    statisticsModel.PolicyCount = statisticsModel.PolicyCounts.Sum();
                }

                return View(statisticsModel);
            }
            else if (HttpContext.Session.GetString("PtyType") == "S")
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


                return View("SurveyStatistics", statisticsModel);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

    }
}
