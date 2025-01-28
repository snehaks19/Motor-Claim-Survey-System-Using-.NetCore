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
    public class ApiUserMasterController : ControllerBase
    {

        UserMasterManager objUserMasterManager = new UserMasterManager();

        [HttpPost]
        [Route("AddNewUserMaster")]
        public IActionResult AddNewUserMaster(UserMasterEntity objUserMasterEntity)
        {
            try
            {

                return Ok(objUserMasterManager.FnSave(objUserMasterEntity));
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
                DataTable dt = objUserMasterManager.FnFetchAll();
                string ds = JsonConvert.SerializeObject(dt);

                return Ok(ds);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("Delete/{userId}")]
        public IActionResult Delete(string userId)
        {
            try
            {
                objUserMasterManager.FnDelete(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                return Ok(objUserMasterManager.GetUserMasterById(id));

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        
        [HttpPost]
        [Route("UpdateUserMaster")]
        public IActionResult UpdateUserMaster(UserMasterEntity objUserMasterEntity)
        {
            try
            {

                return Ok(objUserMasterManager.FnUpdate(objUserMasterEntity));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        [HttpGet]

        [Route("GetDropDown/{pUserType}")]
        public ActionResult GetDropDown(string pUserType)
        {
            UserMasterManager objUserMasterManager = new UserMasterManager();
            DataTable dt = objUserMasterManager.Dropdown(pUserType);
            string pValues = JsonConvert.SerializeObject(dt);
            return Ok(pValues);
        }


        [HttpGet("CheckLoginInfo/{UserId}/{Password}")]
        public IActionResult CheckLoginInfo(string UserId, string Password)
        {
            try
            {
                UserMasterEntity userMasterEntity=new UserMasterEntity();
                userMasterEntity.UserId = UserId;
                userMasterEntity.UserPassword = Password;
                string userType  = objUserMasterManager.CheckLogin(userMasterEntity);
                if (userType == "U")
                {
                    return Ok(userType);

                }
                else if(userType == "S")
                {
                    return Ok(userType);
                }
                else if (userType == "1")
                {
                    return Ok(userType);
                }

                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("GetUserName/{UserId}/{Password}")]
        public IActionResult GetUserName(string UserId, string Password)
        {
            try
            {
                
                string userName = objUserMasterManager.GetName(UserId, Password);
                return Ok(userName);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet("CheckIfIdExists/{code}")]
        public IActionResult CheckIfIdExists(string code)
        {
            try
            {
                int duplicateCount = objUserMasterManager.CheckDuplicateUser(code.ToUpper().Trim());
                return Ok(duplicateCount);
            }

            catch (Exception)
            {

                throw;
            }
        }


    }
}
