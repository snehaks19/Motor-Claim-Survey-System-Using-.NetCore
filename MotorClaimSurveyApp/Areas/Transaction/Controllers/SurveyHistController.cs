using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using static MotorClaimSurveyApp.Filters.AuthorizeFilter;

namespace MotorClaimSurveyApp.Areas.Transaction.Controllers
{
    [Area("Transaction")]
    [Authorize]
    public class SurveyHistController : Controller
    {
        private readonly IConfiguration _configuration;
        public SurveyHistController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> HistoryTableBinding(int code)
        {
            string baseAddress = _configuration.GetValue<string>("ApiUrl");

            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(baseAddress)
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/api/ApiSurveyHist/GetDetailsById/{code}");
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
      (!DBNull.Value.Equals(x.SURDH_ITEM_CODE) && x.SURDH_ITEM_CODE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_TYPE) && x.SURDH_TYPE?.Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_UNIT_PRICE) && x.SURDH_UNIT_PRICE?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_QUANTITY) && x.SURDH_QUANTITY?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_FC_AMT) && x.SURDH_FC_AMT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_LC_AMT) && x.SURDH_LC_AMT?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true) ||
      (!DBNull.Value.Equals(x.SURDH_ACTION) && x.SURDH_ACTION?.ToString().Trim().ToLower().Contains(searchValue.Trim().ToLower()) == true)
  ).ToList();


                    }
                    // get total count of records after search
                    filterRecord = data.Count();
                    //sort data
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))


                        data = data.OrderBy(o => "o.SURDH_ITEM_CODE" + sortColumn).ToList();
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
