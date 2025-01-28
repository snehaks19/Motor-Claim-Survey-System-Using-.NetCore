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
    public class ApiPolicyController : ControllerBase
    {
        MotorPolicyManager objMotorPolicyManager=new MotorPolicyManager();

        [HttpPost]
        [Route("SavePolicy")]
        public IActionResult SavePolicy(MotorPolicyEntity objMotorPolicyEntity)
        {
            try
            {
                return Ok(objMotorPolicyManager.FnSave(objMotorPolicyEntity));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpPost]
        [Route("UpdatePolicy")]
        public IActionResult UpdatePolicy(MotorPolicyEntity objMotorPolicyEntity)
        {
            try
            {

                return Ok(objMotorPolicyManager.FnUpdate(objMotorPolicyEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        [HttpGet]
        [Route("GetAllCodes")]
        public IActionResult GetAllCodes()
        {
            try
            {
                DataTable dt = objMotorPolicyManager.FnFetchAll();
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

        [Route("GetRate/{pCurrType}")]
        public ActionResult GetRate(string pCurrType)

        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            decimal dt = codesMasterManager.ReturnVal(pCurrType);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }

        [HttpGet]
        [Route("GetModel/{vehicleMake}")]
        public IActionResult GetModel(string vehicleMake)

        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            DataTable dt = codesMasterManager.Dropdown("VEH_MODEL", vehicleMake);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }

        [HttpGet]
        [Route("GetById/{polUid}")]
        public IActionResult GetById(int polUid)
        {
            try
            {
                
                return Ok(objMotorPolicyManager.GetPolicyById(polUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }


        [HttpDelete]
        [Route("Delete/{polUid}")]
        public IActionResult Delete(int polUid)
        {
            try
            {
                objMotorPolicyManager.FnDelete(polUid);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //[HttpPost]
        //[Route("UpdateApprStatus")]
        //public IActionResult UpdateApprStatus(MotorPolicyEntity objMotorPolicyEntity)
        //{
        //    try
        //    {

        //        return Ok(objMotorPolicyManager.FnUpdateApprStatus(objMotorPolicyEntity));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException ?? ex;
        //    }
        //}

        [HttpGet]
        [Route("UpdateApprStatus/{polUid}")]
        public IActionResult UpdateApprStatus(long polUid)
        {
            try
            {
                int rowsAffected = objMotorPolicyManager.FnUpdateApprStatus(polUid);

                if (rowsAffected > 0)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "No rows were updated." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("UpdateApprStatus2/{polUid}/{userId}")]
        public IActionResult UpdateApprStatus2(long polUid,string userId)
        {
            try
            {
                
                int rowsAffected = objMotorPolicyManager.FnUpdateApprStatus2(polUid, userId);

                if (rowsAffected > 0)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "No rows were updated." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       


    }
  }
