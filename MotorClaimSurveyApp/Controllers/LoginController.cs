using Microsoft.AspNetCore.Mvc;
using MotorClaimSurveyApp.Areas.Master.ViewModels;
using MotorClaimSurveyApp.Models;

namespace MotorClaimSurveyApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            var userId = loginModel.UserMasterEntity.UserId.ToUpper().Trim();
            var password = loginModel.UserMasterEntity.UserPassword;
           
            try
            {
                if (userId != null && password != null)
                {
                    var loginRequest = new { UserId = userId, Password = password };
                    //var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await client.GetAsync($"/api/ApiUserMaster/CheckLoginInfo/{userId}/{password}");
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        HttpResponseMessage httpResponse2 = await client.GetAsync($"/api/ApiUserMaster/GetUserName/{userId}/{password}");
                        var name = await httpResponse2.Content.ReadAsStringAsync();
                        HttpContext.Session.SetString("UserName", name);
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        string userType = responseContent.Trim();

                        //string userId = loginResult.UserId;

                        if (userType == "U")
                        {
                            HttpContext.Session.SetString("UserId", userId);
                            HttpContext.Session.SetString("PtyType", userType);

                            return RedirectToAction("Index", "Home");

                        }
                        else if (userType == "S")
                        {
                            HttpContext.Session.SetString("UserId", userId);
                            HttpContext.Session.SetString("PtyType", userType);
                            return RedirectToAction("Index", "Home");
                        }
                        else if (userType == "1")
                        {
                            HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{1003}");
                            string stmsg = await response2.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = stmsg;
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ViewBag.Message = "Invalid credentials";
                        }
                    }
                    ViewBag.Message = "Invalid credentials";
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Ok();
            }
        }
    }
}
    