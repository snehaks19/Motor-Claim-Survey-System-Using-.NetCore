using BusinessEntity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer;
using Newtonsoft.Json;

namespace BusinessLayer
{
    public class MotorPolicyManager
    {
        public int FnSave(MotorPolicyEntity objMotorPolicy)
        {
            try
            {
                objMotorPolicy.PolCrDt = DateTime.Now;

                string query = "SELECT SEQ_POLI_UID.NEXTVAL FROM dual";

                int polUid = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

                objMotorPolicy.PolUid = polUid;

                string policyNumber = $"P/{DateTime.Now.Year}/{polUid.ToString().PadLeft(5, '0')}";

                objMotorPolicy.PolNo = policyNumber;

                Dictionary<string, object> dict = new Dictionary<string, object>
                {
                    { "POL_UID", objMotorPolicy.PolUid },
                    { "POL_NO", objMotorPolicy.PolNo },
                    { "POL_ISS_DT", objMotorPolicy.PolIssDt },
                    { "POL_FM_DT", objMotorPolicy.PolFmDt },
                    { "POL_TO_DT", objMotorPolicy.PolToDt },
                    { "POL_ASSR_NAME", objMotorPolicy.PolAssrName.Trim() },
                    { "POL_ASSR_MOBILE", objMotorPolicy.PolAssrMobile.Trim() },
                    { "POL_CURR_CODE", objMotorPolicy.PolCurrCode },
                    { "POL_GROSS_FC_PREM", objMotorPolicy.PolGrossFcPrem },
                    { "POL_GROSS_LC_PREM", objMotorPolicy.PolGrossLcPrem },
                    { "POL_VEH_MAKE", objMotorPolicy.PolVehMake },
                    { "POL_VEH_MODEL", objMotorPolicy.PolVehModel },
                    { "POL_VEH_CHASSIS_NO", objMotorPolicy.PolVehChassisNo.Trim() },
                    { "POL_VEH_ENGINE_NO", objMotorPolicy.PolVehEngineNo.Trim() },
                    { "POL_VEH_REGN_NO", objMotorPolicy.PolVehRegnNo.Trim() },
                    { "POL_VEH_VALUE", objMotorPolicy.PolVehValue },
                    { "POL_APPR_STATUS", "N" },
                    { "POL_CR_BY", objMotorPolicy.PolCrBy },
                    { "POL_CR_DT", objMotorPolicy.PolCrDt }

                };

                string insertQuery = @"INSERT INTO MOTOR_POLICY (
                    POL_UID, POL_NO, POL_ISS_DT, POL_FM_DT, POL_TO_DT, POL_ASSR_NAME, 
                    POL_ASSR_MOBILE, POL_CURR_CODE, POL_GROSS_FC_PREM, POL_GROSS_LC_PREM, 
                    POL_VEH_MAKE, POL_VEH_MODEL, POL_VEH_CHASSIS_NO, POL_VEH_ENGINE_NO, 
                    POL_VEH_REGN_NO,POL_VEH_VALUE, POL_APPR_STATUS, POL_CR_BY, POL_CR_DT
                ) VALUES (
                    :POL_UID, :POL_NO, :POL_ISS_DT, :POL_FM_DT, :POL_TO_DT, :POL_ASSR_NAME, 
                    :POL_ASSR_MOBILE, :POL_CURR_CODE, :POL_GROSS_FC_PREM, :POL_GROSS_LC_PREM, 
                    :POL_VEH_MAKE, :POL_VEH_MODEL, :POL_VEH_CHASSIS_NO, :POL_VEH_ENGINE_NO, 
                    :POL_VEH_REGN_NO,:POL_VEH_VALUE, :POL_APPR_STATUS, :POL_CR_BY, :POL_CR_DT
                )";

                int rows = DatabaseClass.ExecuteQuery(dict, insertQuery);
                if (rows > 0)
                {
                    return polUid;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception err)
            {

                throw err;
            }
            
        }
        public int FnUpdate(MotorPolicyEntity objMotorPolicy)

        {
            objMotorPolicy.PolUpDt = DateTime.Now;

            string updateQuery = $@"UPDATE MOTOR_POLICY SET 
            Pol_Iss_Dt = TO_DATE('{objMotorPolicy.PolIssDt.ToString("dd-MM-yyyy")}', 'dd-MM-yyyy'),
            Pol_Fm_Dt = TO_DATE('{objMotorPolicy.PolFmDt.ToString("dd-MM-yyyy")}', 'dd-MM-yyyy'),
            Pol_To_Dt = TO_DATE('{objMotorPolicy.PolToDt.ToString("dd-MM-yyyy")}', 'dd-MM-yyyy'),
            Pol_Assr_Name = '{objMotorPolicy.PolAssrName.Trim()}',
            Pol_Assr_Mobile = '{objMotorPolicy.PolAssrMobile.Trim()}',
            Pol_Curr_Code = '{objMotorPolicy.PolCurrCode}',
            Pol_Gross_Fc_Prem = {objMotorPolicy.PolGrossFcPrem},
            Pol_Gross_Lc_Prem = {objMotorPolicy.PolGrossLcPrem},
            Pol_Veh_Make = '{objMotorPolicy.PolVehMake}',
            Pol_Veh_Model = '{objMotorPolicy.PolVehModel}',
            Pol_Veh_Chassis_No = '{objMotorPolicy.PolVehChassisNo.Trim()}',
            Pol_Veh_Engine_No = '{objMotorPolicy.PolVehEngineNo.Trim()}',
            Pol_Veh_Regn_No = '{objMotorPolicy.PolVehRegnNo.Trim()}',
            Pol_Veh_Value = {objMotorPolicy.PolVehValue},
            Pol_Up_By = '{objMotorPolicy.PolUpBy}',
            Pol_Up_Dt = SYSDATE
            WHERE Pol_Uid = '{objMotorPolicy.PolUid}'";


            int rows = DatabaseClass.ExecuteQuery(updateQuery);
            return rows;
        }

        public DataTable FnFetchAll()
        {

            string query = $@"
        SELECT 
            POL_UID,
            POL_NO, 
            TO_CHAR(POL_ISS_DT, 'DD-MM-YYYY') AS POL_ISS_DT, 
            TO_CHAR(POL_FM_DT, 'DD-MM-YYYY') AS POL_FM_DT, 
            TO_CHAR(POL_TO_DT, 'DD-MM-YYYY') AS POL_TO_DT, 
            POL_ASSR_NAME, 
            POL_ASSR_MOBILE, 
            POL_CURR_CODE, 
            POL_GROSS_FC_PREM,  
            POL_GROSS_LC_PREM, 
            (SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = P.POL_VEH_MAKE ) AS POL_VEH_MAKE, 
            (SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = P.POL_VEH_MODEL ) AS POL_VEH_MODEL, 
            POL_VEH_CHASSIS_NO, 
            POL_VEH_ENGINE_NO, 
            POL_VEH_REGN_NO, 
            POL_VEH_VALUE, 
            CASE 
                WHEN POL_APPR_STATUS = 'A' THEN 'Approved' 
                WHEN POL_APPR_STATUS = 'N' THEN 'Not Approved' 
                ELSE 'Unknown' 
            END AS POL_APPR_STATUS
         
            FROM 
                MOTOR_POLICY P 
            ORDER BY 
                POL_UID DESC
                ";


            DataTable dt = DatabaseClass.ExecuteDataset(query);
            return dt;
        }

        public MotorPolicyEntity GetPolicyById(int polUid)
        {
            try
            {
                CodesMasterManager codesMasterManager = new CodesMasterManager();

                MotorPolicyEntity motorPolicyEntity = new MotorPolicyEntity();  

                string query = $"SELECT POL_UID,POL_NO, POL_ISS_DT,POL_FM_DT, POL_TO_DT, POL_ASSR_NAME, POL_ASSR_MOBILE, POL_CURR_CODE," +
                        $"POL_GROSS_FC_PREM, POL_GROSS_LC_PREM, POL_VEH_MAKE, POL_VEH_MODEL,POL_VEH_CHASSIS_NO, POL_VEH_ENGINE_NO," +
                        $" POL_VEH_REGN_NO, POL_VEH_VALUE,POL_APPR_STATUS FROM MOTOR_POLICY WHERE POL_UID={polUid}";

                DataTable dt = DatabaseClass.ExecuteDataset(query);

                if (dt.Rows.Count > 0)
                {
                    motorPolicyEntity.PolUid = Convert.ToInt32(dt.Rows[0]["POL_UID"]);
                    motorPolicyEntity.PolNo = dt.Rows[0]["POL_NO"].ToString();
                    motorPolicyEntity.PolIssDt = Convert.ToDateTime(dt.Rows[0]["POL_ISS_DT"]);
                    motorPolicyEntity.PolFmDt = Convert.ToDateTime(dt.Rows[0]["POL_FM_DT"]);
                    motorPolicyEntity.PolToDt = Convert.ToDateTime(dt.Rows[0]["POL_TO_DT"]);
                    motorPolicyEntity.PolAssrName = dt.Rows[0]["POL_ASSR_NAME"].ToString();
                    motorPolicyEntity.PolAssrMobile = dt.Rows[0]["POL_ASSR_MOBILE"].ToString();
                    motorPolicyEntity.PolCurrCode = dt.Rows[0]["POL_CURR_CODE"].ToString();
                    motorPolicyEntity.PolGrossFcPrem = Convert.ToDouble(dt.Rows[0]["POL_GROSS_FC_PREM"]);
                    motorPolicyEntity.PolGrossLcPrem = Convert.ToDouble(dt.Rows[0]["POL_GROSS_LC_PREM"]);
                    motorPolicyEntity.PolVehMake = dt.Rows[0]["POL_VEH_MAKE"].ToString();
                    motorPolicyEntity.PolVehModel = dt.Rows[0]["POL_VEH_MODEL"].ToString();
                    motorPolicyEntity.PolVehChassisNo = dt.Rows[0]["POL_VEH_CHASSIS_NO"].ToString();
                    motorPolicyEntity.PolVehEngineNo = dt.Rows[0]["POL_VEH_ENGINE_NO"].ToString();
                    motorPolicyEntity.PolVehRegnNo = dt.Rows[0]["POL_VEH_REGN_NO"].ToString();
                    motorPolicyEntity.PolVehValue = Convert.ToDouble(dt.Rows[0]["POL_VEH_VALUE"]);
                    motorPolicyEntity.PolApprStatus = dt.Rows[0]["POL_APPR_STATUS"].ToString();
                }


                    return motorPolicyEntity;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

        public int FnDelete(int polUid)
        {
            string deleteQuery = $"DELETE FROM MOTOR_POLICY WHERE POL_UID= {polUid}";

            int rows = DatabaseClass.ExecuteQuery(deleteQuery);

            return rows;
        }

        public int FnUpdateApprStatus(long polUid)
        {

            string updateQuery = $"UPDATE MOTOR_POLICY SET POL_APPR_STATUS='A',POL_APPR_DT=SYSDATE WHERE POL_UID={polUid}";

            int rows = DatabaseClass.ExecuteQuery(updateQuery);

            return rows;

        }

        public int FnUpdateApprStatus2(long polUid, string userId)
        {

            string updateQuery = $"UPDATE MOTOR_POLICY SET POL_APPR_STATUS='A',POL_APPR_BY='{userId}',POL_APPR_DT=SYSDATE WHERE POL_UID={polUid}";
            
            int rows = DatabaseClass.ExecuteQuery(updateQuery);
            
            return rows;
        }

        public int FnFetchApprCount()
        {
            string query = "SELECT COUNT(*)  FROM Motor_policy WHERE POL_APPR_STATUS = 'A'";

            int rows =Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }

        public int FnFetchUnApprCount()
        {
            string query = "SELECT COUNT(*)  FROM Motor_policy WHERE POL_APPR_STATUS = 'N'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }

        public int FnFetchClaimApprCount()
        {
            string query = "SELECT COUNT(*)  FROM MOTOR_CLAIM WHERE CLM_APPR_STATUS = 'A'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }
        public int FnFetchClaimPendingCount()
        {
            string query = "SELECT COUNT(*)  FROM MOTOR_CLAIM WHERE CLM_APPR_STATUS = 'N'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }
        public int FnFetchClaimRejectedCount()
        {
            string query = "SELECT COUNT(*)  FROM MOTOR_CLAIM WHERE CLM_APPR_STATUS = 'R'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }

        public int FnFetchSurveyPendingCount()
        {
            string query = "SELECT COUNT(*)  FROM MOTOR_CLM_SUR_HDR WHERE SUR_STATUS = 'P'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }

        public int FnFetchSurveySubmittedCount()
        {
            string query = "SELECT COUNT(*)  FROM MOTOR_CLM_SUR_HDR WHERE SUR_STATUS = 'S'";

            int rows = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

            return rows;
        }

        public DataTable policyCount()
        {
            string query = "SELECT TRUNC(POL_ISS_DT) AS POL_ISS_DT,COUNT(*) AS NO_OF_POLICY  FROM MOTOR_POLICY GROUP BY TRUNC(POL_ISS_DT)  ORDER BY TRUNC(POL_ISS_DT)";

            DataTable dt = DatabaseClass.ExecuteDataset(query);

            return dt;
        }

      

    }
}
