using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorClaimSurveyApp.Areas.Master.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Reflection.Emit;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Master.Controllers
{
    [Area("Master")]
    [Authorize]
    public class UserMasterController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserMasterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserMasterListing()
        {
            return View();
        }

        public async Task<IActionResult> UserMasterListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiUserMaster/GetAllCodes");
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
      (!DBNull.Value.Equals(x.USER_ID) && x.USER_ID?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.USER_NAME) && x.USER_NAME?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.USER_TYPE) && x.USER_TYPE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true)
  ).ToList();


                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                    data = data.OrderBy(o => "o.USER_ID" + sortColumn).ToList();
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
        public async Task<IActionResult> UserMasterEdit(string id)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
            UserMasterViewModel userMasterViewModel = new UserMasterViewModel
            {
                UserMasterEntity = new UserMasterEntity()
            };

            await FillDropDown("USER_TYPE", userMasterViewModel);

            if (!string.IsNullOrEmpty(id))
            {
                userMasterViewModel.Mode = "U";
                HttpResponseMessage response = await client.GetAsync($"/api/ApiUserMaster/GetById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    userMasterViewModel.UserMasterEntity = JsonConvert.DeserializeObject<UserMasterEntity>(result);
                    
                    return View("UserMasterEdit", userMasterViewModel);
                }
            }
            else
            {
               
                userMasterViewModel.Mode = "I";

            }

            return View(userMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserMasterEdit(UserMasterViewModel umVmodel)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };
            string userId = HttpContext.Session.GetString("UserId");
            
            if (umVmodel.Mode == "I")
            {
                if (ModelState.IsValid)
                {
                    umVmodel.UserMasterEntity.UserCrBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiUserMaster/AddNewUserMaster", umVmodel.UserMasterEntity);
                    if (response.IsSuccessStatusCode)
                    {

                        umVmodel.Mode = "U";
                        await FillDropDown("USER_TYPE", umVmodel);

                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2000}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                      
                        
                    }
                }
                return Redirect("~/Master/UserMaster/UserMasterEdit?id=" + umVmodel.UserMasterEntity.UserId);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    umVmodel.UserMasterEntity.UserUpBy = userId;
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/ApiUserMaster/UpdateUserMaster", umVmodel.UserMasterEntity);

                    if (response.IsSuccessStatusCode)
                    {
                        await FillDropDown("USER_TYPE", umVmodel);
                        umVmodel.Mode = "U";
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{2001}");
                        string stmsg = await response2.Content.ReadAsStringAsync();
                        TempData["SuccessMessage"] = stmsg;
                    }


                }
                return View(umVmodel);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCode(string userId)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiUserMaster/Delete/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{3000}");
                    string stmsg = await response2.Content.ReadAsStringAsync();
                    return Ok(new { success = true, message = stmsg });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting code. Please try again later."; return RedirectToAction("CodesMasterList");
                }
            }
        }

        public async Task FillDropDown(string msg, UserMasterViewModel model)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };

            UserMasterViewModel errorCodesMasterModel = new UserMasterViewModel();
            errorCodesMasterModel.ListUserType = new List<SelectListItem>();
            string errorMsg = msg;
            HttpResponseMessage response = await client.GetAsync($"/api/ApiUserMaster/GetDropDown/{errorMsg}");


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);

                model.ListUserType.Add(new SelectListItem
                {
                    Text = "--SELECT--",
                    Value = ""
                });

                foreach (DataRow dataRow in dt.Rows)
                {
                    model.ListUserType.Add(new SelectListItem
                    {
                        Text = dataRow["CM_DESC"].ToString(),
                        Value = dataRow["CM_CODE"].ToString()
                    });
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckIfIdExists(string pCode)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {

                HttpResponseMessage response = await client.GetAsync($"/api/ApiUserMaster/CheckIfIdExists/{pCode}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    int rowCount = int.Parse(responseData); // Convert the response to an integer

                    // Use `rowCount` as needed
                    if (rowCount > 0)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"/api/ApiErrorCodeMaster/GetErrorMessage/{4001}");
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
