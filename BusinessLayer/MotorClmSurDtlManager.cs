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
    public class MotorClmSurDtlManager
    {
        public long FnSave(MotorClmSurDtlEntity objMotorClmSurDtl)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["SURD_UID"] = objMotorClmSurDtl.SurdUid > 0 ? objMotorClmSurDtl.SurdUid : 0;
            dict["SURD_SUR_UID"] = objMotorClmSurDtl.SurdSurUid;
            dict["ITEM_CODE"] = objMotorClmSurDtl.SurdItemCode;
            dict["TYPE"] = objMotorClmSurDtl.SurdType;
            dict["UNIT_PRICE"] = objMotorClmSurDtl.SurdUnitPrice;
            dict["QUANTITY"] = objMotorClmSurDtl.SurdQuantity;
            dict["FC_AMT"] = objMotorClmSurDtl.SurdFcAmt;
            dict["LC_AMT"] = objMotorClmSurDtl.SurdLcAmt;
            dict["SUR_CR_BY"] = objMotorClmSurDtl.SurdCrBy;
            dict["SUR_CR_BY"] = objMotorClmSurDtl.SurdCrBy;


            (int status, long uid, string errMsg) = DatabaseClass.ExecuteProc(dict);

            return (uid);
        }


        public DataTable GetDetailsById(long surUid)
        {
            MotorClmSurDtlEntity motorClmSurDtlEntity = new MotorClmSurDtlEntity();

            string query = $@" SELECT
                SURD_UID,
                SURD_SUR_UID,
                (SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = M.SURD_ITEM_CODE ) AS SURD_ITEM_CODE,
                CASE 
                    WHEN SURD_TYPE = 'R' THEN 'Repair'
                    WHEN SURD_TYPE = 'P' THEN 'Replace'
                END AS SURD_TYPE,
                SURD_UNIT_PRICE,
                SURD_QUANTITY,
                SURD_FC_AMT,
                SURD_LC_AMT ,
                (SELECT SUR_STATUS FROM MOTOR_CLM_SUR_HDR WHERE SUR_UID={surUid} )AS SURD_STATUS
            FROM
                MOTOR_CLM_SUR_DTL M
            WHERE SURD_SUR_UID='{surUid}'
            ORDER BY SURD_UID ";

            DataTable dt = DatabaseClass.ExecuteDataset(query);
            if (dt.Rows.Count > 0)
            {
                motorClmSurDtlEntity.SurdUid = Convert.ToInt32(dt.Rows[0]["SURD_UID"]);
                motorClmSurDtlEntity.SurdSurUid = Convert.ToInt32(dt.Rows[0]["SURD_SUR_UID"]);
                motorClmSurDtlEntity.SurdItemCode = dt.Rows[0]["SURD_ITEM_CODE"].ToString();
                motorClmSurDtlEntity.SurdType = dt.Rows[0]["SURD_TYPE"].ToString();
                motorClmSurDtlEntity.SurdUnitPrice = Convert.ToInt64(dt.Rows[0]["SURD_UNIT_PRICE"]);
                motorClmSurDtlEntity.SurdQuantity = Convert.ToInt32(dt.Rows[0]["SURD_QUANTITY"]);
                motorClmSurDtlEntity.SurdFcAmt = Convert.ToDouble(dt.Rows[0]["SURD_FC_AMT"]);
                motorClmSurDtlEntity.SurdLcAmt = Convert.ToDouble(dt.Rows[0]["SURD_LC_AMT"]);
            }
                return dt;

        }

        public MotorClmSurDtlEntity GetEachItemDetailsById(int surdUid, int surUid)
        {
            MotorClmSurDtlEntity motorClmSurDtlEntity = new MotorClmSurDtlEntity();

            try
            {
                string selectQuery = $@"SELECT
                                            SURD_UID,
                                            SURD_SUR_UID,
                                            SURD_ITEM_CODE,
                                            SURD_TYPE,
                                            SURD_UNIT_PRICE,
                                            SURD_QUANTITY,
                                            SURD_FC_AMT,
                                            SURD_LC_AMT
                                            
                                        FROM
                                            MOTOR_CLM_SUR_DTL
                                        WHERE
                                            SURD_SUR_UID = '{surUid}' and SURD_UID = '{surdUid}'";
                DataTable dt = DatabaseClass.ExecuteDataset(selectQuery);

                if (dt.Rows.Count > 0)
                {
                    motorClmSurDtlEntity.SurdUid = Convert.ToInt32(dt.Rows[0]["SURD_UID"]);
                    motorClmSurDtlEntity.SurdSurUid = Convert.ToInt32(dt.Rows[0]["SURD_SUR_UID"]);
                    motorClmSurDtlEntity.SurdItemCode = dt.Rows[0]["SURD_ITEM_CODE"].ToString();
                    motorClmSurDtlEntity.SurdType = dt.Rows[0]["SURD_TYPE"].ToString();
                    motorClmSurDtlEntity.SurdUnitPrice = Convert.ToDecimal(dt.Rows[0]["SURD_UNIT_PRICE"]);
                    motorClmSurDtlEntity.SurdQuantity = Convert.ToInt32(dt.Rows[0]["SURD_QUANTITY"]);
                    motorClmSurDtlEntity.SurdFcAmt = Convert.ToDouble(dt.Rows[0]["SURD_FC_AMT"]);
                    motorClmSurDtlEntity.SurdLcAmt = Convert.ToDouble(dt.Rows[0]["SURD_LC_AMT"]);
                }
                return motorClmSurDtlEntity;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

        public int FnDelete(int surdUid, int surUid)
        {
            try
            {
                string deleteQuery = $"DELETE FROM MOTOR_CLM_SUR_DTL WHERE SURD_UID = {surdUid} AND SURD_SUR_UID={surUid}";
                int rows = DatabaseClass.ExecuteQuery(deleteQuery);

                if (rows >= 1)
                {
                    string updateQuery = $"UPDATE MOTOR_CLM_SUR_HDR SET " +
                         $"SUR_FC_AMT = (SELECT SUM(SURD_FC_AMT) FROM MOTOR_CLM_SUR_DTL WHERE SURD_SUR_UID = {surUid}), " +
                         $"SUR_LC_AMT = (SELECT SUM(SURD_LC_AMT) FROM MOTOR_CLM_SUR_DTL WHERE SURD_SUR_UID = {surUid}) " +
                         $"WHERE SUR_UID = {surUid}";

                    int row = DatabaseClass.ExecuteQuery(updateQuery);

                    if (row >= 1)
                    {
                        return row;
                    }
                    else
                        return 0;
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

        public int CheckDuplicate(string item, long surUid)
        {
            string query = $"SELECT COUNT(*) FROM MOTOR_CLM_SUR_DTL WHERE SURD_ITEM_CODE='{item}' AND SURD_SUR_UID={surUid}";
            object rows = DatabaseClass.ExecuteScalar(query);
            return Convert.ToInt32(rows);
        }
    }
}
