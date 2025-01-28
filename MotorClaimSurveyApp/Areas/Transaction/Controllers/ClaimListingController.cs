using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Dynamic;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class ClaimListingController : Controller
    {
        private readonly IConfiguration _configuration;
        public ClaimListingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ClaimListing()
        {
            return View();
        }

        public async Task<IActionResult> ClaimListBinding()
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/ApiClaim/GetAllCodes");
                if (response.IsSuccessStatusCode)
                {
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
     (!DBNull.Value.Equals(x.CLM_NO) && x.CLM_NO?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_POL_NO) && x.CLM_POL_NO?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_POL_FM_DT) && x.CLM_POL_FM_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_POL_TO_DT) && x.CLM_POL_TO_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_POL_ASSR_NAME) && x.CLM_POL_ASSR_NAME?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_LOSS_DT) && x.CLM_LOSS_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_INTM_DT) && x.CLM_INTM_DT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_SUR_CR_YN) && x.CLM_SUR_CR_YN?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
     (!DBNull.Value.Equals(x.CLM_APPR_STATUS) && x.CLM_APPR_STATUS?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true)
 ).ToList();

                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                        data = data.OrderBy(o => "o.CLM_POL_ASSR_NAME" + sortColumn).ToList();
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

    }
}
