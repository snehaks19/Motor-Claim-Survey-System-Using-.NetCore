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
    public class ApiCodesMasterController : ControllerBase
    {
        CodesMasterManager objCodesMasterManager = new CodesMasterManager();

        [HttpPost]
        [Route("AddNewCodesMaster")]
        public IActionResult AddNewCodesMaster(CodesMasterEntity objCodesMasterEntity)
        {
            try
            {
                return Ok(objCodesMasterManager.FnSave(objCodesMasterEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;//bad req
            }
        }

        [HttpGet]
        [Route("GetAllCodes")]
        public IActionResult GetAllCodes()
        {
            try
            {
                DataTable dt = objCodesMasterManager.FnFetchAll();
                string ds = JsonConvert.SerializeObject(dt);
                return Ok(ds);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //[HttpPost]
        //[Route("Delete/{code}/{type}")]
        //public IActionResult Delete(string code, string type)
        //{
        //    try
        //    {
        //        objCodesMasterManager.FnDelete(code, type);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(CodesMasterEntity codesMasterEntity)
        {
            try
            {
                objCodesMasterManager.FnDelete(codesMasterEntity.CmCode, codesMasterEntity.CmType);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }


        [HttpGet]
        [Route("GetById/{cmCode}/{cmType}")]
        public IActionResult GetById(string cmCode, string cmType)
        {
            try
            {
                return Ok(objCodesMasterManager.GetCodeMasterById(cmCode, cmType));
                
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [Route("UpdateCodesMaster")]
        public IActionResult UpdateCodesMaster(CodesMasterEntity objCodesMasterEntity)
        {
            try
            {
                
                return Ok(objCodesMasterManager.FnUpdate(objCodesMasterEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;  
            }
        }

        [HttpGet("CheckIfCodeExists/{code}/{type}")]
        public IActionResult CheckIfCodeExists(string code, string type)
        {
            try
            {
                int duplicateCount = objCodesMasterManager.FnDuplicateCheck(code.ToUpper().Trim(), type.ToUpper().Trim());
                return Ok(duplicateCount);
            }

            catch (Exception)
            {

                throw;
            }
        }




    }
}
