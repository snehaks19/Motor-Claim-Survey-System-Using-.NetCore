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
    public class MotorClmSurHdrManager
    {
        public DataTable FnFetchAll()
        {
            string query = $@"
            SELECT 
                SUR_UID,         
                SUR_CLM_NO, 
                NVL(SUR_NO, ' ') AS SUR_NO, 
                CASE 
         WHEN SUR_DATE IS NULL THEN ' ' 
         ELSE TO_CHAR(SUR_DATE, 'DD-MM-YYYY') 
     END AS SUR_DATE,
               CASE 
         WHEN (SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = S.SUR_LOCATION) IS NULL THEN ' '
         ELSE (SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = S.SUR_LOCATION)
     END AS SUR_LOCATION,
                SUR_CHASSIS_NO, 
                SUR_REGN_NO, 
                SUR_ENGINE_NO,
                CASE 
                    WHEN SUR_STATUS = 'P' THEN 'PENDING'
                    WHEN SUR_STATUS = 'S' THEN 'SUBMITTED'
                   
                END AS SUR_STATUS,
                CASE 
                    WHEN SUR_APPR_STS = 'A' THEN 'APPROVED'
                    WHEN SUR_APPR_STS = 'R' THEN 'REJECTED'
                    WHEN SUR_APPR_STS = 'N' THEN 'NOT APPROVED'
                    
                END AS SUR_APPR_STS,          
                SUR_CURR, 
                CASE WHEN SUR_FC_AMT IS NULL THEN ' ' ELSE TO_CHAR(SUR_FC_AMT) END AS SUR_FC_AMT,
     CASE WHEN SUR_LC_AMT IS NULL THEN ' ' ELSE TO_CHAR(SUR_LC_AMT) END AS SUR_LC_AMT       
            FROM 
                MOTOR_CLM_SUR_HDR S
            ORDER BY 
                SUR_UID DESC
            ";

            DataTable dt = DatabaseClass.ExecuteDataset(query);
            return dt;
        }

        public MotorClmSurHdrEntity GetSurveyById(long surUid)
        {
            CodesMasterManager codesMasterManager = new CodesMasterManager();
            MotorClmSurHdrEntity motorClmSurHdrEntity = new MotorClmSurHdrEntity();
            try
            {
                string query = $"SELECT SUR_UID,SUR_NO, SUR_CLM_NO, SUR_DATE, " +
                $" SUR_LOCATION, SUR_CHASSIS_NO, SUR_REGN_NO, SUR_ENGINE_NO, SUR_CURR, " +
                $"SUR_FC_AMT, SUR_LC_AMT,TO_CHAR(SUR_CR_DT,'DD-MM-YYYY') AS SUR_CR_DT,SUR_STATUS FROM MOTOR_CLM_SUR_HDR WHERE SUR_UID = {surUid}";

                DataTable dt = DatabaseClass.ExecuteDataset(query);
                if (dt.Rows.Count > 0)
                {
                    motorClmSurHdrEntity.SurUid = Convert.ToInt32(dt.Rows[0]["SUR_UID"]);
                    motorClmSurHdrEntity.SurNo = dt.Rows[0]["SUR_NO"].ToString();
                    motorClmSurHdrEntity.SurClmNo = dt.Rows[0]["SUR_CLM_NO"].ToString();

                    motorClmSurHdrEntity.SurDate = System.DateTime.Now;

                    motorClmSurHdrEntity.SurLocation = dt.Rows[0]["SUR_LOCATION"].ToString();
                    motorClmSurHdrEntity.SurChassisNo = dt.Rows[0]["SUR_CHASSIS_NO"].ToString();
                    motorClmSurHdrEntity.SurRegnNo = dt.Rows[0]["SUR_REGN_NO"].ToString();
                    motorClmSurHdrEntity.SurEngineNo = dt.Rows[0]["SUR_ENGINE_NO"].ToString();
                    motorClmSurHdrEntity.SurCurr = dt.Rows[0]["SUR_CURR"].ToString();
                    motorClmSurHdrEntity.SurFcAmt = dt.Rows[0]["SUR_FC_AMT"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["SUR_FC_AMT"]) : (double?)null;
                    motorClmSurHdrEntity.SurLcAmt = dt.Rows[0]["SUR_LC_AMT"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["SUR_LC_AMT"]) : (double?)null;
                    motorClmSurHdrEntity.SurCrDt = System.DateTime.Now;
                    motorClmSurHdrEntity.SurStatus= dt.Rows[0]["SUR_STATUS"].ToString();

                }
                return motorClmSurHdrEntity;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public static int FnSave(MotorClmSurHdrEntity objMotorClmSurHdr)
        {
            string updateQuery = $"UPDATE MOTOR_CLM_SUR_HDR SET " +
                     $"SUR_DATE = TO_DATE('{objMotorClmSurHdr.SurDate:dd-MM-yyyy}', 'DD-MM-YYYY'), " +
                     $"SUR_LOCATION = '{objMotorClmSurHdr.SurLocation}', " +
                     $"SUR_CURR = '{objMotorClmSurHdr.SurCurr}', " +
                     $"SUR_UP_BY = '{objMotorClmSurHdr.SurUpBy}', " +
                     $"SUR_UP_DT = SYSDATE " +
                     $"WHERE SUR_UID = '{objMotorClmSurHdr.SurUid}'";

            int rows = DatabaseClass.ExecuteQuery(updateQuery);
            if (rows > 0)
            {
                return rows;
            }
            else
            {
                return 0;
            }
        }

        public int GetClmUid(long surUid)
        {
            string query = $"SELECT SUR_CLM_UID FROM MOTOR_CLM_SUR_HDR WHERE SUR_UID={surUid}";
            int clmuid = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));
            return clmuid;
        }

        public (int, string) SubmitSurvey(long surUid, long clmUid, string userId)
        {
            (int status, string errMsg) = DatabaseClass.ExecuteProcForApproval(surUid, clmUid, userId);

            return (status, errMsg);
        }

        public string CheckSurveyStatus(int clmNo)
        {
            string query = $"SELECT SUR_STATUS FROM  MOTOR_CLM_SUR_HDR WHERE SUR_CLM_UID='{clmNo}'";
            object status = DatabaseClass.ExecuteScalar(query);
            if (status != null)
            {
                if (status.ToString() == "S")
                {
                    return status.ToString();
                }
                else
                {
                    return status.ToString();
                }

            }
            else
            {
                return "0";
            }
        }

        public long GetSurUid(int  clmUid)
        {
            string query = $"SELECT SUR_UID FROM MOTOR_CLM_SUR_HDR WHERE SUR_CLM_UID='{clmUid}'";
            long surUid = Convert.ToInt64(DatabaseClass.ExecuteScalar(query));
            return surUid;
        }
    }

}
