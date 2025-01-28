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
    public class ApiSurveyDetailsController : ControllerBase
    {

        MotorClmSurDtlManager objMotorClmSurDtlManager = new MotorClmSurDtlManager();

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

        [Route("GetRate/{pCurrType}")]
        public ActionResult GetRate(string pCurrType)

        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            decimal dt = codesMasterManager.ReturnVal(pCurrType);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }

        [HttpPost]
        [Route("SaveDetails")]
        public IActionResult SaveDetails(MotorClmSurDtlEntity Entity)
        {
            try
            {
                return Ok(objMotorClmSurDtlManager.FnSave(Entity));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        [HttpPost]
        [Route("UpdateDetails")]
        public IActionResult UpdateDetails(MotorClmSurDtlEntity Entity)
        {
            try
            {
                return Ok(objMotorClmSurDtlManager.FnSave(Entity));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        [HttpGet]
        [Route("GetDetailsById/{surUid}")]
        public IActionResult GetDetailsById(int surUid)
        {

            try
            {
                DataTable dt = objMotorClmSurDtlManager.GetDetailsById(surUid);
                string pValues = JsonConvert.SerializeObject(dt);
                return Ok(pValues);


            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetEachById/{surdUid}/{surUid}")]
        public IActionResult GetEachById(int surdUid,int surUid)
        {
            try
            {
                
                return Ok(objMotorClmSurDtlManager.GetEachItemDetailsById(surdUid, surUid));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //[HttpDelete]
        //[Route("Delete/{surdUid}/{surUid}")]
        //public IActionResult Delete(int surdUid,int surUid)
        //{
        //    try
        //    {
        //        objMotorClmSurDtlManager.FnDelete(surdUid, surUid);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        [HttpGet]
        [Route("Delete/{surdUid}/{surUid}")]
        public IActionResult Delete(int surdUid, int surUid)
        {
            try
            {
                objMotorClmSurDtlManager.FnDelete(surdUid, surUid);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckDuplicateItem/{item}/{surUid}")]
        public IActionResult CheckDuplicateItem(string item, long surUid)
        {
            try
            {
                int duplicateCount = objMotorClmSurDtlManager.CheckDuplicate(item, surUid);
                return Ok(duplicateCount);
            }

            catch (Exception)
            {

                throw;
            }
        }


    }
}
