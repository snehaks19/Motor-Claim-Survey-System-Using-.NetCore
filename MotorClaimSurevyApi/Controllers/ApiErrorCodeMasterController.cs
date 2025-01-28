using BusinessEntity;
using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Reflection.Emit;

namespace MotorClaimSurevyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiErrorCodeMasterController : ControllerBase
    {
        ErrorCodeMasterManager objErrorCodeMasterManager =new ErrorCodeMasterManager();

        [HttpPost]
        [Route("AddNewErrorCodeMaster")]
        public IActionResult AddNewErrorCodeMaster(ErrorCodeMasterEntity objErrorCodeMasterEntity)
        {
            try
            {
                return Ok(objErrorCodeMasterManager.FnSave(objErrorCodeMasterEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        [HttpGet]
        [Route("GetAllCodes")]
        public IActionResult GetAllCodes()
        {
            try
            {
                DataTable dt = objErrorCodeMasterManager.FnFetchAll();
                string ds = JsonConvert.SerializeObject(dt);
                return Ok(ds);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //[HttpDelete]
        //[Route("Delete/{errCode}/{errType}")]
        //public IActionResult Delete(string errCode, string errType)
        //{
        //    try
        //    {
        //        objErrorCodeMasterManager.FnDelete(errCode, errType);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}


        [HttpGet]
        [Route("Delete/{errCode}/{errType}")]
        public IActionResult Delete(string errCode, string errType)
        {
            try
            {
                objErrorCodeMasterManager.FnDelete(errCode, errType);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetById/{errCode}/{errType}")]
        public IActionResult GetById(string errCode, string errType)
        {
            try
            {
                
                return Ok(objErrorCodeMasterManager.GetErrorCodeMasterById(errCode, errType));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [Route("UpdateErrorCodeMaster")]
        public IActionResult UpdateErrorCodeMaster(ErrorCodeMasterEntity objErrorCodeMasterEntity)
        
        {
            try
            {

                return Ok(objErrorCodeMasterManager.FnUpdate(objErrorCodeMasterEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }


        [HttpGet("CheckIfCodeExists/{code}")]
        public IActionResult CheckIfCodeExists(string code)
        {
            try
            {
                int duplicateCount = objErrorCodeMasterManager.FnDuplicateCheck(code.ToUpper().Trim());
                return Ok(duplicateCount);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]

        [Route("GetDropDown/{pErrType}")]
        public ActionResult GetDropDown(string pErrType)

        {
            ErrorCodeMasterManager errorCodeManager = new ErrorCodeMasterManager();
            DataTable dt = errorCodeManager.Dropdown(pErrType);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }

        [HttpGet]
        [Route("GetErrorMessage/{pCode}")]
        public IActionResult GetErrorMessage(string pCode)
        {
            try
            {
                ErrorCodeMasterManager errorCodeMasterManager = new ErrorCodeMasterManager();
                string msg = errorCodeMasterManager.GetErrorMsg(pCode);
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
