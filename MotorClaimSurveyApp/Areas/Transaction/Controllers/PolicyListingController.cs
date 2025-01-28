using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class PolicyListingController : Controller
    {
        private readonly IConfiguration _configuration;
        public PolicyListingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PolicyListing()
        {
            return View();
        }

        public async Task<IActionResult> PolicyListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiPolicy/GetAllCodes");
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
                    
                    totalRecord = data.Count();
                   
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        data = data.Where(x =>
    (!DBNull.Value.Equals(x.POL_NO) && x.POL_NO?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_ASSR_NAME) && x.POL_ASSR_NAME?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_VEH_MAKE) && x.POL_VEH_MAKE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_FM_DT) && x.POL_FM_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_VEH_MODEL) && x.POL_VEH_MODEL?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_TO_DT) && x.POL_TO_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_ISS_DT) && x.POL_ISS_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
    (!DBNull.Value.Equals(x.POL_APPR_STATUS) && x.POL_APPR_STATUS?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true)
).ToList();

                    }

                    filterRecord = data.Count();
                   
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                        data = data.OrderBy(o => "o.POL_UID" + sortColumn).ToList();
                    
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

        //[HttpPost]
        //public async Task<IActionResult> DeletePolicy(int code)
        //{
        //    string baseAddress = _configuration.GetValue<string>("ApiUrl");
        //    using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
        //    {
        //        HttpResponseMessage response = await client.DeleteAsync($"/api/ApiPolicy/Delete/{code}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            //TempData["SuccessMessage"] = "Code deleted successfully.";
        //            return Ok(new { success = true });
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Error deleting code. Please try again later.";
        //            return RedirectToAction("CodesMasterList");
        //        }
        //    }
        //}
    }
}
