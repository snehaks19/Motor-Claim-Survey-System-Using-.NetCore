using BusinessEntity;
using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class UserMasterManager
    {
        public int FnSave(UserMasterEntity ObjUserMasterEntity)
        {
            ObjUserMasterEntity.UserCrDt = DateTime.Now;

            string insertQuery = $"INSERT INTO USER_MASTER (USER_ID, USER_TYPE,USER_NAME,USER_PASSWORD, USER_CR_BY, USER_CR_DT,USER_ACTIVE_YN)" +
                $" VALUES (:UserId,:UserType,:UserName,:UserPassword,:UserCrBy,:UserCrDt,:UserActiveYn)";


            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "UserId", ObjUserMasterEntity.UserId },
                { "UserType", ObjUserMasterEntity.UserType },
                { "UserName", ObjUserMasterEntity.UserName },
                { "UserPassword", ObjUserMasterEntity.UserPassword },
                { "UserCrBy", ObjUserMasterEntity.UserCrBy },
                { "UserCrDt", ObjUserMasterEntity.UserCrDt },
                { "UserActiveYn", ObjUserMasterEntity.UserActiveYn }
            };


            int rows = DatabaseClass.ExecuteQuery(dict, insertQuery);
            return rows;
        }

        public DataTable FnFetchAll()
        {
            try
            {
                string query = "SELECT USER_ID, USER_NAME, USER_PASSWORD,CASE WHEN USER_TYPE = 'S' THEN 'Surveyor' WHEN USER_TYPE = 'U' THEN 'User' ELSE ' ' " +
                        "END AS USER_TYPE, USER_ACTIVE_YN FROM USER_MASTER ORDER BY USER_ID";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnDelete(string userId)
        {
            string query = $"DELETE FROM USER_MASTER WHERE USER_ID='{userId}'";
            int rows = DatabaseClass.ExecuteQuery(query);
            return rows;
        }

        public int FnUpdate(UserMasterEntity objUserMasterEntity)
        {
            objUserMasterEntity.UserUpDt = DateTime.Now;
            string updateQuery = $"UPDATE USER_MASTER SET USER_NAME='{objUserMasterEntity.UserName}',USER_TYPE='{objUserMasterEntity.UserType}',USER_UP_BY='{objUserMasterEntity.UserUpBy}',USER_ACTIVE_YN='{objUserMasterEntity.UserActiveYn}',USER_UP_DT=TO_DATE('{objUserMasterEntity.UserUpDt:dd-MM-yyyy}', 'DD-MM-YYYY') where USER_ID='{objUserMasterEntity.UserId}'";
            int rows = DatabaseClass.ExecuteQuery(updateQuery);
            return rows;
        }

        public UserMasterEntity GetUserMasterById(string id)
        {
            UserMasterEntity userMaster = new UserMasterEntity();
            string query = $"SELECT USER_ID, USER_TYPE, USER_NAME,USER_PASSWORD,USER_ACTIVE_YN FROM USER_MASTER WHERE USER_ID='{id}'";
            DataTable dt = DatabaseClass.ExecuteDataset(query);
            if (dt.Rows.Count > 0)
            {
                userMaster.UserId = dt.Rows[0]["USER_ID"].ToString();
                userMaster.UserType = dt.Rows[0]["USER_TYPE"].ToString();
                userMaster.UserName = dt.Rows[0]["USER_NAME"].ToString();
                userMaster.UserPassword = dt.Rows[0]["USER_PASSWORD"].ToString();
                userMaster.UserActiveYn = dt.Rows[0]["USER_ACTIVE_YN"] as string;

            }
            return userMaster;


        }

        public DataTable Dropdown(string type)
        {
            string query = $"SELECT CM_CODE || '-' || CM_DESC AS CM_DESC,CM_CODE AS CM_CODE FROM SNEHA_CODES_MASTER WHERE CM_TYPE='{type}' AND CM_ACTIVE_YN='Y'";
            DataTable dt = DatabaseClass.ExecuteDataset(query);
            return dt;
        }

        public string CheckLogin(UserMasterEntity ObjUserMasterEntity)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["Userid"] = ObjUserMasterEntity.UserId;
            dict["Password"] = ObjUserMasterEntity.UserPassword;

            string query = "SELECT USER_TYPE,USER_ACTIVE_YN FROM USER_MASTER WHERE USER_ID =:Userid AND USER_PASSWORD = :Password";
            DataSet ds = DatabaseClass.ExecuteQuerySelect(dict, query);
          
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string userType = ds.Tables[0].Rows[0]["USER_TYPE"].ToString();
                string userActiveYn = ds.Tables[0].Rows[0]["USER_ACTIVE_YN"].ToString();
                if (userActiveYn == "Y")
                {
                    return userType;
                }
                else
                {
                    return "1";
                }
                
            }
            else
            {
                return null;
            }
        }
        public string GetName(string UserId,string Password)
        {
            
            string query = $"SELECT USER_NAME FROM USER_MASTER WHERE USER_ID ='{UserId}' AND USER_PASSWORD = '{Password}'";
            string name = DatabaseClass.ExecuteScalar(query).ToString();

            return name;
        }

        public int CheckDuplicateUser(String userId)
        {
            string query = $"SELECT COUNT(*) FROM USER_MASTER WHERE USER_ID='{userId}'";
            Object rows = DatabaseClass.ExecuteScalar(query);
            int row = Convert.ToInt32(rows);
            return row;
        }
    }
}
