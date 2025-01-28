using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer;
using BusinessEntity;
using System.Reflection.Emit;

namespace BusinessLayer
{
    public class ErrorCodeMasterManager
    {
        public DataTable LoadGrid()
        {
            try
            {
                string query = "SELECT ERR_CODE, ERR_TYPE, ERR_DESC FROM SNEHA_ERROR_CODE_MASTER ORDER BY ERR_CODE,ERR_TYPE";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnSave(ErrorCodeMasterEntity objErrorCodeMasterEntity)
        {
            try
            {
                objErrorCodeMasterEntity.ErrCrDt = DateTime.Now;

                string insertQuery = $"INSERT INTO SNEHA_ERROR_CODE_MASTER (ERR_CODE, ERR_TYPE, ERR_DESC, ERR_CR_BY, ERR_CR_DT)" +
                        $" VALUES (:ErrCode,:ErrType,:ErrDesc,:ErrCrBy,:ErrCrDt)";


                Dictionary<string, object> dict = new Dictionary<string, object>
{
                { "ErrCode", objErrorCodeMasterEntity.ErrCode.ToUpper().Trim() },
                { "ErrType", objErrorCodeMasterEntity.ErrType.ToUpper() },
                { "ErrDesc", objErrorCodeMasterEntity.ErrDesc },
                { "ErrCrBy", objErrorCodeMasterEntity.ErrCrBy },
                { "ErrCrDt", objErrorCodeMasterEntity.ErrCrDt }
            };

                int rows = DatabaseClass.ExecuteQuery(dict, insertQuery);
                return rows;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public DataTable FnFetchAll()
        {
            try
            {
                string query = "SELECT ERR_CODE, ERR_TYPE, CASE WHEN ERR_DESC IS NULL THEN ' ' ELSE ERR_DESC END AS ERR_DESC FROM SNEHA_ERROR_CODE_MASTER ORDER BY ERR_CODE,ERR_TYPE";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnDelete(string errCode, string errType)
        {
            try
            {
                string query = $"DELETE FROM SNEHA_ERROR_CODE_MASTER WHERE ERR_CODE='{errCode}' AND ERR_TYPE='{errType}'";
                int rows = DatabaseClass.ExecuteQuery(query);
                return rows;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public ErrorCodeMasterEntity GetErrorCodeMasterById(string errCode, string errType)
        {
           
            ErrorCodeMasterEntity errorCode = new ErrorCodeMasterEntity();
            string query = $"SELECT ERR_CODE,ERR_TYPE,ERR_DESC FROM SNEHA_ERROR_CODE_MASTER WHERE ERR_CODE='{errCode}' AND ERR_TYPE='{errType}'";
            DataTable dt = DatabaseClass.ExecuteDataset(query);
            if (dt.Rows.Count > 0)
            {
                errorCode.ErrCode = dt.Rows[0]["ERR_CODE"].ToString();
                errorCode.ErrType = dt.Rows[0]["ERR_TYPE"].ToString();
                errorCode.ErrDesc = dt.Rows[0]["ERR_DESC"].ToString();
            }
            return errorCode;
        }
        public int FnUpdate(ErrorCodeMasterEntity objErrorCodeMasterEntity)

        {
            try
            {
                objErrorCodeMasterEntity.ErrUpDt = DateTime.Now;
                string updateQuery = $"UPDATE SNEHA_ERROR_CODE_MASTER SET ERR_DESC='{objErrorCodeMasterEntity.ErrDesc}',ERR_UP_BY='{objErrorCodeMasterEntity.ErrUpBy}',ERR_UP_DT=TO_DATE('{objErrorCodeMasterEntity.ErrUpDt:dd-MM-yyyy}', 'DD-MM-YYYY') where ERR_CODE='{objErrorCodeMasterEntity.ErrCode}'/* AND ERR_TYPE='{objErrorCodeMasterEntity.ErrType}'*/";
                int rows = DatabaseClass.ExecuteQuery(updateQuery);
                return rows;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnDuplicateCheck(string code)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM SNEHA_ERROR_CODE_MASTER WHERE ERR_CODE='{code}'";
                Object rows = DatabaseClass.ExecuteScalar(query);
                int row = Convert.ToInt32(rows);
                return row;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public DataTable Dropdown(string type)
        {
            try
            {
                string query = $"SELECT CM_CODE || '-' || CM_DESC AS CM_DESC,CM_CODE AS CM_CODE FROM SNEHA_CODES_MASTER WHERE CM_TYPE='{type}' AND CM_ACTIVE_YN='Y'";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public string GetErrorMsg(string code)
        {
            try
            {
                string query = $"SELECT ERR_DESC FROM SNEHA_ERROR_CODE_MASTER WHERE ERR_CODE='{code}'";
                string desc = DatabaseClass.ExecuteScalar(query).ToString();
                return desc;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

    }
}
