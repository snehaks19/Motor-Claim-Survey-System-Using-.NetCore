using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace MotorClaimSurevyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiDashboardController : ControllerBase
    {
        MotorPolicyManager motorPolicyManager=new MotorPolicyManager(); 

        [HttpGet]
        [Route("CheckApprPolicyCount")]
        public IActionResult CheckApprPolicyCount()
        {
            try
            {
                int count= motorPolicyManager.FnFetchApprCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckUnApprPolicyCount")]
        public IActionResult CheckUnApprPolicyCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchUnApprCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckApprClaimCount")]
        public IActionResult CheckApprClaimCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchClaimApprCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckRejClaimCount")]
        public IActionResult CheckRejClaimCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchClaimRejectedCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckPendngClaimCount")]
        public IActionResult CheckPendngClaimCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchClaimPendingCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckPendingSurveyCount")]
        public IActionResult CheckPendingSurveyCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchSurveyPendingCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("CheckSubmittedSurveyCount")]
        public IActionResult CheckSubmittedSurveyCount()
        {
            try
            {
                int count = motorPolicyManager.FnFetchSurveySubmittedCount();
                string pValues = JsonConvert.SerializeObject(count);
                return Ok(pValues);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetPolicyCount")]
        public IActionResult GetPolicyCount()
        
        {
            MotorPolicyManager objmotorPolicyManager = new MotorPolicyManager();
            DataTable dt = objmotorPolicyManager.policyCount();
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);

        }
    }
}
