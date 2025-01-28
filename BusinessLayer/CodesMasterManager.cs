using BusinessEntity;
using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CodesMasterManager
    {

        public int FnDuplicateCheck(string cmCode, string cmType)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM SNEHA_CODES_MASTER WHERE CM_CODE='{cmCode}' AND CM_TYPE='{cmType}'";
                Object rows = DatabaseClass.ExecuteScalar(query);
                int row = Convert.ToInt32(rows);
                return row;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public int FnSave(CodesMasterEntity objCodesMasterEntity)
        {
            try
            {
             

                objCodesMasterEntity.CmCode = objCodesMasterEntity.CmCode?.Trim().ToUpper();
                objCodesMasterEntity.CmType = objCodesMasterEntity.CmType?.Trim().ToUpper();
                objCodesMasterEntity.CmDesc = objCodesMasterEntity.CmDesc?.Trim();
                objCodesMasterEntity.CmCrDt = DateTime.Now;


                Dictionary<string, object> dict = new Dictionary<string, object>
                {
                { "CmCode", objCodesMasterEntity.CmCode},
                { "CmType", objCodesMasterEntity.CmType},
                { "CmDesc", objCodesMasterEntity.CmDesc },
                { "CmParentCode", objCodesMasterEntity.CmParentCode },
                { "CmValue", objCodesMasterEntity.CmValue },
                { "CmActiveYn", objCodesMasterEntity.CmActiveYn },
                { "CmCrBy", objCodesMasterEntity.CmCrBy },
                { "CmCrDt", objCodesMasterEntity.CmCrDt }
                };

                string insertQuery = $"INSERT INTO SNEHA_CODES_MASTER (CM_CODE, CM_TYPE, CM_DESC, CM_PARENT_CODE, CM_VALUE, CM_ACTIVE_YN, CM_CR_BY, CM_CR_DT) " +
                             $"VALUES ('{objCodesMasterEntity.CmCode}', '{objCodesMasterEntity.CmType}', '{objCodesMasterEntity.CmDesc}', '{objCodesMasterEntity.CmParentCode}'," +
                             $" '{objCodesMasterEntity.CmValue}', '{objCodesMasterEntity.CmActiveYn}', '{objCodesMasterEntity.CmCrBy}', TO_DATE('{objCodesMasterEntity.CmCrDt:yyyy-MM-dd}', 'YYYY-MM-DD'))";



                int rows = DatabaseClass.ExecuteQuery(dict, insertQuery);
                return rows;


            }
            catch (Exception)
            {

                return 0;
            }
        }

        public DataTable FnFetchAll()
        {
            try
            {
                string query = "SELECT CM_CODE, CM_TYPE, CASE WHEN CM_DESC IS NULL THEN ' ' ELSE CM_DESC END AS CM_DESC, CM_VALUE, CM_ACTIVE_YN FROM SNEHA_CODES_MASTER ORDER BY  CM_TYPE ,CM_CODE asc";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnDelete(string cmCode, string cmType)
        {
            try
            {
                string deleteQuery = $"DELETE FROM SNEHA_CODES_MASTER WHERE CM_CODE='{cmCode}' AND CM_TYPE='{cmType}'";
                int rows = DatabaseClass.ExecuteQuery(deleteQuery);
                return rows;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        

        public CodesMasterEntity GetCodeMasterById(string cmCode, string cmType)
        {
            CodesMasterEntity codeMaster = new CodesMasterEntity();
            string query = $"SELECT CM_CODE,CM_TYPE,CM_DESC,CM_VALUE,CM_PARENT_CODE,CM_ACTIVE_YN FROM SNEHA_CODES_MASTER WHERE CM_CODE='{cmCode}' AND CM_TYPE='{cmType}'";
            DataTable dt = DatabaseClass.ExecuteDataset(query);
            if (dt.Rows.Count > 0)
            {
                codeMaster.CmCode = dt.Rows[0]["CM_CODE"].ToString();
                codeMaster.CmDesc = dt.Rows[0]["CM_DESC"] as string; // Null-safe conversion
                codeMaster.CmValue = dt.Rows[0]["CM_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["CM_VALUE"]) : (double?)null;
                codeMaster.CmType = dt.Rows[0]["CM_TYPE"].ToString();
                codeMaster.CmActiveYn = dt.Rows[0]["CM_ACTIVE_YN"] as string;
               
            }
            return codeMaster;
        }

        public int FnUpdate(CodesMasterEntity objCodesMasterEntity)

        {
            try
            {
                objCodesMasterEntity.CmUpDt = DateTime.Now;
                string updateQuery = $"UPDATE SNEHA_CODES_MASTER SET CM_VALUE='{objCodesMasterEntity.CmValue}',CM_PARENT_CODE='{objCodesMasterEntity.CmParentCode}', CM_DESC='{objCodesMasterEntity.CmDesc}',CM_ACTIVE_YN='{objCodesMasterEntity.CmActiveYn}',CM_UP_BY='{objCodesMasterEntity.CmUpBy}',CM_UP_DT=TO_DATE('{objCodesMasterEntity.CmUpDt:yyyy-MM-dd}', 'YYYY-MM-DD') where CM_CODE='{objCodesMasterEntity.CmCode}' AND CM_TYPE='{objCodesMasterEntity.CmType}'";
                int rows = DatabaseClass.ExecuteQuery(updateQuery);
                return rows;
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
        public DataTable Dropdown(string type,string make)
        {
            try
            {
                string query = $"SELECT CM_CODE || '-' || CM_DESC AS CM_DESC,CM_CODE AS CM_CODE FROM SNEHA_CODES_MASTER WHERE CM_TYPE='{type}' AND CM_PARENT_CODE='{make}' AND CM_ACTIVE_YN='Y'";
                DataTable dt = DatabaseClass.ExecuteDataset(query);
                return dt;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public decimal ReturnVal(string currCode)
        
        {
            try
            {
                string query = $"SELECT CM_VALUE FROM SNEHA_CODES_MASTER WHERE CM_CODE='{currCode}'";
                object s = DatabaseClass.ExecuteScalar(query);

                return (decimal)s;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

    }
}
