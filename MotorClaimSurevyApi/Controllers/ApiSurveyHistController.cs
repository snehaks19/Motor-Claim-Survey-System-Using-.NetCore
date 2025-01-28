using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace MotorClaimSurevyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSurveyHistController : ControllerBase
    {
        MotorClmSurHistManager motorClmSurHistManager=new MotorClmSurHistManager();

        [HttpGet]
        [Route("GetDetailsById/{surdUid}")]
        public IActionResult GetDetailsById(int surdUid)
        {
            try
            {
                DataTable dt = motorClmSurHistManager.GetSurveyDetails(surdUid);
                string pValues = JsonConvert.SerializeObject(dt);
                return Ok(pValues);
                

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
