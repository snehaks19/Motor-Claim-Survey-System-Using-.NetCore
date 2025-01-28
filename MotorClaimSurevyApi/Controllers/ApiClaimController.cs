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
    public class ApiClaimController : ControllerBase
    {
        MotorClaimManager objMotorClaimManager = new MotorClaimManager();

        [HttpPost]
        [Route("SaveClaim")]
        public IActionResult SaveClaim(MotorClaimEntity motorClaimEntity)
        {
            try
            {
                return Ok(objMotorClaimManager.FnSave(motorClaimEntity));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        [HttpPost]
        [Route("UpdateClaim")]
        public IActionResult UpdatePolicy(MotorClaimEntity motorClaimEntity)
        {
            try
            {

                return Ok(objMotorClaimManager.FnUpdate(motorClaimEntity));
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
                DataTable dt = objMotorClaimManager.FnFetchAll();
                string ds = JsonConvert.SerializeObject(dt);
                return Ok(ds);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]

        [Route("GetDropDown")]
        public IActionResult GetDropDown()

        {

            DataTable dt = objMotorClaimManager.Dropdown();
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }


        [HttpGet]
        [Route("GetPolicyDetail/{policyNumber}")]
        public IActionResult GetPolicyDetail(string policyNumber)
        {
            try
            {             
                string formattedPolicyNumber = policyNumber.Replace("_", "/");
                return Ok(objMotorClaimManager.LoadPolicyDetails(formattedPolicyNumber));
               
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error.", details = ex.Message });
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
        [Route("GetModel/{pMake}")]
        public ActionResult GetModel(string pMake)

        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            DataTable dt = codesMasterManager.Dropdown("VEH_MODEL", pMake);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }
        [HttpGet]
        [Route("GetById/{clmUid}")]
        public IActionResult GetById(int clmUid)
        {
            try
            {

                return Ok(objMotorClaimManager.GetClaimById(clmUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("SaveClaim/{clmUid}/{userId}")]
        public IActionResult SaveClaim(long clmUid,string userId)
        {
            try
            {
                return Ok(objMotorClaimManager.SurveyRequest(clmUid, userId));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("ApproveClaim/{clmUid}")]
        public IActionResult ApproveClaim(long clmUid)
        {
            try
            {
                int rowsAffected = objMotorClaimManager.FnApprove(clmUid);

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
        [Route("RejectClaim/{clmUid}")]
        public IActionResult RejectClaim(long clmUid)
        {
            try
            {
                int rowsAffected = objMotorClaimManager.FnReject(clmUid);

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
        [Route("CheckSurveyStatus/{clmUid}")]
        public IActionResult CheckSurveyStatus(int clmUid)
        {
            MotorClmSurHdrManager motorClmSurHdrManager = new MotorClmSurHdrManager();
            try
            {

                return Ok(motorClmSurHdrManager.CheckSurveyStatus(clmUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetSurUid/{clmUid}")]
        public IActionResult GetSurUid(int clmUid)
        {
            MotorClmSurHdrManager motorClmSurHdrManager = new MotorClmSurHdrManager();
            try
            {

                return Ok(motorClmSurHdrManager.GetSurUid(clmUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetPoliceNumber/{clmUid}")]
        public IActionResult GetPoliceNumber(int clmUid)
        {
            MotorClaimManager motorClmManager = new MotorClaimManager();
            try
            {

                return Ok(motorClmManager.GetPoliceNumber(clmUid));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckDuplicatePoliceNumber/{rptNo}")]
        public IActionResult CheckDuplicatePoliceNumber(string rptNo)
        {
            try
            {
                MotorClaimManager motorClmManager = new MotorClaimManager();
                int duplicateCount = motorClmManager.CheckDuplicate(rptNo);
                return Ok(duplicateCount);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetPolicyUid/{policyNumber}")]
        public IActionResult GetPolicyUid(string policyNumber)
        {

            string formattedPolicyNumber = policyNumber.Replace("-", "/");

            MotorClaimManager motorClaimManager = new MotorClaimManager();
            try
            {

                return Ok(motorClaimManager.GetPolUid(formattedPolicyNumber));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }



    }
}
