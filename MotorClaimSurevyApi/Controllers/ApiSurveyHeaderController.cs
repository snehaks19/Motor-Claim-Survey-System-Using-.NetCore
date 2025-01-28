using BusinessEntity;
using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace MotorClaimSurevyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSurveyHeaderController : ControllerBase
    {
        MotorClmSurHdrManager motorClmSurHdrManager = new MotorClmSurHdrManager();


        [HttpGet]
        [Route("GetAllCodes")]
        public IActionResult GetAllCodes()
        {
            try
            {
                DataTable dt = motorClmSurHdrManager.FnFetchAll();
                string ds = JsonConvert.SerializeObject(dt);
                return Ok(ds);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetDropDown/{pCurrType}")]
        public ActionResult GetDropDown(string pCurrType)

        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            DataTable dt = codesMasterManager.Dropdown(pCurrType);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);
        }

        [HttpGet]
        [Route("GetSurveyDetails/{surUid}")]
        public IActionResult GetDetailsById(int surUid)
        {
            try
            {

                return Ok(motorClmSurHdrManager.GetSurveyById(surUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [Route("SaveSurvey")]
        public IActionResult SaveSurvey(MotorClmSurHdrEntity entity)
        {
            try
            {
                return Ok(MotorClmSurHdrManager.FnSave(entity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetClmUid/{surUid}")]
        public IActionResult GetClmUid(long surUid)
        {
            try
            {

                return Ok(motorClmSurHdrManager.GetClmUid(surUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("SubmitSurvey/{surUid}/{surClmUid}/{userID}")]
        public IActionResult SubmitSurvey(long surUid,long surClmUid, string userID)
        {
            try
            {
                return Ok(motorClmSurHdrManager.SubmitSurvey(surUid, surClmUid, userID));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
