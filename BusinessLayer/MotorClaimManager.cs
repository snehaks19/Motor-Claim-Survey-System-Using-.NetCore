using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer;
using BusinessEntity;

namespace BusinessLayer
{
    public class MotorClaimManager
    {
        public DataTable FnFetchAll()
        {
            string query = $" SELECT CLM_UID,CLM_NO, CLM_POL_NO,TO_CHAR(CLM_POL_FM_DT, 'DD-MM-YYYY') AS CLM_POL_FM_DT, " +
                $"TO_CHAR(CLM_POL_TO_DT, 'DD-MM-YYYY') AS CLM_POL_TO_DT, CLM_POL_ASSR_NAME,  CLM_POL_ASSR_MOB, " +
                $"TO_CHAR(CLM_LOSS_DT, 'DD-MM-YYYY') AS CLM_LOSS_DT, TO_CHAR(CLM_INTM_DT, 'DD-MM-YYYY') AS CLM_INTM_DT, " +
                $"TO_CHAR(CLM_REG_DT, 'DD-MM-YYYY') AS CLM_REG_DT,CLM_POL_REP_NO,CLM_POL_REP_DTL,CLM_LOSS_DESC," +
                $"(SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = CLM_VEH_MAKE) AS CLM_VEH_MAKE, " +
                $"(SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = CLM_VEH_MODEL) AS CLM_VEH_MODEL," +
                $"CLM_VEH_CHASSIS_NO,  CLM_VEG_ENGINE_NO,  CLM_VEH_REGN_NO, CLM_VEH_VALUE ,CLM_SUR_CR_YN," +
                $"CASE " +
                $"WHEN CLM_APPR_STATUS = 'A' THEN 'Approved' " +
                $"WHEN CLM_APPR_STATUS = 'R' THEN 'Rejected' " +
                $"ELSE ' ' " +
                $"END AS CLM_APPR_STATUS " +
                $" FROM  MOTOR_CLAIM ORDER BY CLM_UID DESC";

            DataTable dt = DatabaseClass.ExecuteDataset(query);
            return dt;

        }


        public DataTable Dropdown()
        {
            string query = $"SELECT POL_NO FROM MOTOR_POLICY WHERE POL_APPR_STATUS='A' ORDER BY POL_NO DESC";
            DataTable dt = DatabaseClass.ExecuteDataset(query);
            return dt;
        }

        public MotorClaimEntity LoadPolicyDetails(string polNo)
        {
            try
            {
                MotorClaimEntity motorClaimEntity = new MotorClaimEntity();

                string query = $"SELECT POL_FM_DT,POL_TO_DT,POL_ASSR_NAME,POL_ASSR_MOBILE,(SELECT CM_CODE || '-' || CM_DESC FROM SNEHA_CODES_MASTER WHERE CM_CODE= P.POL_VEH_MAKE) AS POL_VEH_MAKE ,(SELECT CM_CODE || '-' || CM_DESC FROM SNEHA_CODES_MASTER WHERE CM_CODE= P.POL_VEH_MODEL) AS POL_VEH_MODEL" +
                        $",POL_VEH_CHASSIS_NO,POL_VEH_ENGINE_NO,POL_VEH_REGN_NO,POL_VEH_VALUE FROM MOTOR_POLICY P WHERE POL_NO='{polNo}'";
                DataTable dt = DatabaseClass.ExecuteDataset(query);

                if (dt.Rows.Count > 0)
                {
                    motorClaimEntity.ClmPolFmDt = Convert.ToDateTime(dt.Rows[0]["POL_FM_DT"]);
                    motorClaimEntity.ClmPolToDt = Convert.ToDateTime(dt.Rows[0]["POL_TO_DT"]);
                    motorClaimEntity.ClmPolAssrName = dt.Rows[0]["POL_ASSR_NAME"].ToString();
                    motorClaimEntity.ClmPolAssrMob = dt.Rows[0]["POL_ASSR_MOBILE"].ToString();
                    motorClaimEntity.ClmVehMake = dt.Rows[0]["POL_VEH_MAKE"].ToString();
                    motorClaimEntity.ClmVehModel = dt.Rows[0]["POL_VEH_MODEL"].ToString();
                    motorClaimEntity.ClmVehChassisNo = dt.Rows[0]["POL_VEH_CHASSIS_NO"].ToString();
                    motorClaimEntity.ClmVehEngineNo = dt.Rows[0]["POL_VEH_ENGINE_NO"].ToString();
                    motorClaimEntity.ClmVehRegnNo = dt.Rows[0]["POL_VEH_REGN_NO"].ToString();
                    motorClaimEntity.ClmVehValue = Convert.ToDouble(dt.Rows[0]["POL_VEH_VALUE"]);

                }
                return motorClaimEntity;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public int FnSave(MotorClaimEntity objMotorClaim)
        {
            try
            {
                objMotorClaim.ClmCrDt = DateTime.Now;
                string query = "SELECT seq_clm_uid.NEXTVAL FROM dual";
                int clmUid = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));

                string claimNumber = $"C/{DateTime.Now.Year}/{clmUid.ToString().PadLeft(5, '0')}";

                Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "CLM_UID",  clmUid},
                { "CLM_NO", claimNumber },
                { "CLM_POL_NO", objMotorClaim.ClmPolNo },
                { "CLM_POL_FM_DT", objMotorClaim.ClmPolFmDt},
                { "CLM_POL_TO_DT", objMotorClaim.ClmPolToDt },
                { "CLM_POL_ASSR_NAME", objMotorClaim.ClmPolAssrName },
                { "CLM_POL_ASSR_MOB", objMotorClaim.ClmPolAssrMob},
                { "CLM_LOSS_DT", objMotorClaim.ClmLossDt },
                { "CLM_INTM_DT", objMotorClaim.ClmIntmDt },
                { "CLM_REG_DT", objMotorClaim.ClmRegDt },
                { "CLM_POL_REP_NO", objMotorClaim.ClmPolRepNo.Trim() },
                { "CLM_POL_REP_DTL", objMotorClaim.ClmPolRepDtl.Trim() },
                { "CLM_LOSS_DESC", objMotorClaim.ClmLossDesc.Trim()},
                { "CLM_VEH_MAKE", objMotorClaim.ClmVehMake},
                { "CLM_VEH_MODEL", objMotorClaim.ClmVehModel},
                { "CLM_VEH_CHASSIS_NO", objMotorClaim.ClmVehChassisNo},
                { "CLM_VEG_ENGINE_NO", objMotorClaim.ClmVehEngineNo},
                { "CLM_VEH_REGN_NO", objMotorClaim.ClmVehRegnNo },
                { "CLM_VEH_VALUE", objMotorClaim.ClmVehValue },
                { "CLM_SUR_CR_YN", "N" },
                { "CLM_SUR_APPR_YN", "N" },
                { "CLM_APPR_STATUS", "N" },
                { "CLM_CR_BY", objMotorClaim.ClmCrBy },
                { "CLM_CR_DT", objMotorClaim.ClmCrDt}
            };

                string insertQuery = @"
            INSERT INTO MOTOR_CLAIM (CLM_UID, CLM_NO, CLM_POL_NO, CLM_POL_FM_DT, CLM_POL_TO_DT, 
                CLM_POL_ASSR_NAME, CLM_POL_ASSR_MOB, CLM_LOSS_DT, CLM_INTM_DT, CLM_REG_DT, 
                CLM_POL_REP_NO, CLM_POL_REP_DTL, CLM_LOSS_DESC, CLM_VEH_MAKE, CLM_VEH_MODEL, 
                CLM_VEH_CHASSIS_NO, CLM_VEG_ENGINE_NO, CLM_VEH_REGN_NO, CLM_VEH_VALUE, 
                CLM_SUR_CR_YN, CLM_SUR_APPR_YN, CLM_APPR_STATUS,CLM_CR_BY,CLM_CR_DT) 
            VALUES (:CLM_UID, :CLM_NO, :CLM_POL_NO, :CLM_POL_FM_DT, :CLM_POL_TO_DT, 
                :CLM_POL_ASSR_NAME, :CLM_POL_ASSR_MOB, :CLM_LOSS_DT, :CLM_INTM_DT, :CLM_REG_DT, 
                :CLM_POL_REP_NO, :CLM_POL_REP_DTL, :CLM_LOSS_DESC, :CLM_VEH_MAKE, :CLM_VEH_MODEL,
                :CLM_VEH_CHASSIS_NO, :CLM_VEG_ENGINE_NO, :CLM_VEH_REGN_NO, :CLM_VEH_VALUE,
                :CLM_SUR_CR_YN, :CLM_SUR_APPR_YN, :CLM_APPR_STATUS,:CLM_CR_BY,:CLM_CR_DT)";


                int rows = DatabaseClass.ExecuteQuery(dict, insertQuery);
                if (rows > 0)
                {
                    return clmUid;
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
        public MotorClaimEntity GetClaimById(int claimUid)
        {
            try
            {
                CodesMasterManager codesMasterManager = new CodesMasterManager();
                MotorClaimEntity motorClaimEntity= new MotorClaimEntity();

                string selectQuery = $"SELECT CLM_UID,CLM_NO, CLM_POL_NO, CLM_POL_FM_DT, CLM_POL_TO_DT," +
                    $"CLM_POL_ASSR_NAME, CLM_POL_ASSR_MOB, CLM_LOSS_DT, CLM_INTM_DT," +
                    $"CLM_REG_DT, CLM_POL_REP_NO, CLM_POL_REP_DTL, CLM_LOSS_DESC," +
                    $"CLM_VEH_MAKE, CLM_VEH_MODEL, CLM_VEH_CHASSIS_NO, " +
                    $"CLM_VEG_ENGINE_NO, CLM_VEH_REGN_NO, CLM_VEH_VALUE,CLM_SUR_CR_YN,CLM_SUR_NO,CLM_APPR_STATUS FROM MOTOR_CLAIM WHERE CLM_UID = {claimUid}";
                DataTable dt = DatabaseClass.ExecuteDataset(selectQuery);
                if (dt.Rows.Count > 0)
                {
                   motorClaimEntity.ClmUid = Convert.ToInt32(dt.Rows[0]["CLM_UID"]);
                   motorClaimEntity.ClmNo = dt.Rows[0]["CLM_NO"].ToString();
                   motorClaimEntity.ClmPolNo = dt.Rows[0]["CLM_POL_NO"].ToString();                   
                   motorClaimEntity.ClmPolFmDt = Convert.ToDateTime(dt.Rows[0]["CLM_POL_FM_DT"]);
                   motorClaimEntity.ClmPolToDt = Convert.ToDateTime(dt.Rows[0]["CLM_POL_TO_DT"]);
                   motorClaimEntity.ClmPolAssrName = dt.Rows[0]["CLM_POL_ASSR_NAME"].ToString();
                   motorClaimEntity.ClmPolAssrMob = dt.Rows[0]["CLM_POL_ASSR_MOB"].ToString();                   
                   motorClaimEntity.ClmLossDt = Convert.ToDateTime(dt.Rows[0]["CLM_LOSS_DT"]);
                   motorClaimEntity.ClmIntmDt = Convert.ToDateTime(dt.Rows[0]["CLM_INTM_DT"]);
                   motorClaimEntity.ClmRegDt = Convert.ToDateTime(dt.Rows[0]["CLM_REG_DT"]);
                   motorClaimEntity.ClmPolRepNo = dt.Rows[0]["CLM_POL_REP_NO"].ToString();
                   motorClaimEntity.ClmPolRepDtl = dt.Rows[0]["CLM_POL_REP_DTL"].ToString();
                   motorClaimEntity.ClmLossDesc = dt.Rows[0]["CLM_LOSS_DESC"].ToString();
                   motorClaimEntity.ClmVehMake = dt.Rows[0]["CLM_VEH_MAKE"].ToString();
                   motorClaimEntity.ClmVehModel = dt.Rows[0]["CLM_VEH_MODEL"].ToString();
                   motorClaimEntity.ClmVehChassisNo = dt.Rows[0]["CLM_VEH_CHASSIS_NO"].ToString();
                   motorClaimEntity.ClmVehEngineNo = dt.Rows[0]["CLM_VEG_ENGINE_NO"].ToString();
                   motorClaimEntity.ClmVehRegnNo = dt.Rows[0]["CLM_VEH_REGN_NO"].ToString();
                   motorClaimEntity.ClmVehValue = Convert.ToDouble(dt.Rows[0]["CLM_VEH_VALUE"]);
                   motorClaimEntity.ClmSurCrYn = dt.Rows[0]["CLM_SUR_CR_YN"].ToString();
                   motorClaimEntity.ClmSurNo = dt.Rows[0]["CLM_SUR_NO"].ToString();
                   motorClaimEntity.ClmApprStatus = dt.Rows[0]["CLM_APPR_STATUS"].ToString();
                }
                return motorClaimEntity;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

        public int FnUpdate(MotorClaimEntity objMotorClaim)
        {
            try
            {
                objMotorClaim.ClmUpDt = DateTime.Now;

                string updateQuery = $@"
        UPDATE MOTOR_CLAIM SET 
            CLM_NO = '{objMotorClaim.ClmNo}',
            CLM_POL_NO = '{objMotorClaim.ClmPolNo}',
            CLM_POL_FM_DT = TO_DATE('{objMotorClaim.ClmPolFmDt:dd-MM-yyyy}', 'dd-MM-yyyy'),
            CLM_POL_TO_DT = TO_DATE('{objMotorClaim.ClmPolToDt:dd-MM-yyyy}', 'dd-MM-yyyy'),
            CLM_POL_ASSR_NAME = '{objMotorClaim.ClmPolAssrName}',
            CLM_POL_ASSR_MOB = '{objMotorClaim.ClmPolAssrMob}',
            CLM_LOSS_DT = TO_DATE('{objMotorClaim.ClmLossDt:dd-MM-yyyy}', 'dd-MM-yyyy'),
            CLM_INTM_DT = TO_DATE('{objMotorClaim.ClmIntmDt:dd-MM-yyyy}', 'dd-MM-yyyy'),
            CLM_REG_DT = TO_DATE('{objMotorClaim.ClmRegDt:dd-MM-yyyy}', 'dd-MM-yyyy'),
            CLM_POL_REP_NO = '{objMotorClaim.ClmPolRepNo.Trim()}',
            CLM_POL_REP_DTL = '{objMotorClaim.ClmPolRepDtl.Trim()}',
            CLM_LOSS_DESC = '{objMotorClaim.ClmLossDesc.Trim()}',
            CLM_VEH_MAKE = '{objMotorClaim.ClmVehMake}',
            CLM_VEH_MODEL = '{objMotorClaim.ClmVehModel}',
            CLM_VEH_CHASSIS_NO = '{objMotorClaim.ClmVehChassisNo}',
            CLM_VEG_ENGINE_NO = '{objMotorClaim.ClmVehEngineNo}',
            CLM_VEH_REGN_NO = '{objMotorClaim.ClmVehRegnNo}',
            CLM_VEH_VALUE = {objMotorClaim.ClmVehValue},
            CLM_SUR_CR_YN = 'N',
            CLM_SUR_APPR_YN = 'N',
            CLM_APPR_STATUS = 'N',
            CLM_UP_BY = '{objMotorClaim.ClmUpBy}',
            CLM_UP_DT = SYSDATE
        WHERE CLM_UID = '{objMotorClaim.ClmUid}'";

                int rows = DatabaseClass.ExecuteQuery(updateQuery);
                return rows;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        public (int, string) SurveyRequest(long clmUid, string userId)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["CLMUID"] = clmUid;
            dict["USERID"] = userId;

            (int status, string errMsg) = DatabaseClass.ExecuteProc(clmUid, userId);

            return (status, errMsg);
        }

        public int FnApprove(long clmUid)
        {
            string query = $"UPDATE MOTOR_CLAIM SET CLM_APPR_STATUS='A' WHERE CLM_UID={clmUid}";
            int rows = DatabaseClass.ExecuteQuery(query);
            return rows;
        }
        public int FnReject(long clmUid)
        {
            string query = $"UPDATE MOTOR_CLAIM SET CLM_APPR_STATUS='R' WHERE CLM_UID={clmUid}";
            int rows = DatabaseClass.ExecuteQuery(query);
            return rows;
        }

        public string GetPoliceNumber(int clmUid)
        {
            string query = $"SELECT CLM_POL_REP_NO FROM MOTOR_CLAIM WHERE CLM_UID='{clmUid}'";
            string reptNo = DatabaseClass.ExecuteScalar(query).ToString();
            return reptNo;
        }

        public int CheckDuplicate(string reptNo)
        {
            string query = $"SELECT COUNT(*) FROM MOTOR_CLAIM WHERE CLM_POL_REP_NO='{reptNo}'";
            int res = Convert.ToInt32(DatabaseClass.ExecuteScalar(query));
            return res;
        }
        public int GetPolUid(string polNo)
        {
            try
            {
                string query = $"SELECT POL_UID FROM MOTOR_POLICY WHERE POL_NO='{polNo}'";
                object value = DatabaseClass.ExecuteScalar(query);
                int val = Convert.ToInt32(value);
                return val;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
    }
}


