using BusinessEntity;
using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class MotorClmSurHistManager
    {
        MotorClmSurDtlHistEntity motorClmSurDtlHistEntity = new MotorClmSurDtlHistEntity();
        public DataTable GetSurveyDetails(long id)
        {
            string query = $"SELECT CASE WHEN SURDH_ACTION = 'I' THEN 'Insert' " +
                $"WHEN SURDH_ACTION = 'U' THEN 'Update' WHEN SURDH_ACTION = 'D' THEN 'Delete' END AS SURDH_ACTION," +
                $"(SELECT C.CM_DESC FROM SNEHA_CODES_MASTER C WHERE C.CM_CODE = M.SURDH_ITEM_CODE ) AS SURDH_ITEM_CODE, " +
                $"(SELECT CM_DESC FROM SNEHA_CODES_MASTER WHERE CM_CODE=SURDH_TYPE) AS SURDH_TYPE,SURDH_UNIT_PRICE,SURDH_QUANTITY,SURDH_FC_AMT,SURDH_SRL," +
                $"SURDH_LC_AMT FROM MOTOR_CLM_SUR_DTL_HIST M WHERE SURDH_UID={id}";
            DataTable dt = DatabaseClass.ExecuteDataset(query);

            if (dt.Rows.Count > 0)
            {
                motorClmSurDtlHistEntity.SurdhAction= dt.Rows[0]["SURDH_ACTION"].ToString();
                motorClmSurDtlHistEntity.SurdhItemCode = dt.Rows[0]["SURDH_ITEM_CODE"].ToString();
                motorClmSurDtlHistEntity.SurdhType= dt.Rows[0]["SURDH_TYPE"].ToString();
                motorClmSurDtlHistEntity.SurdhSrl= Convert.ToInt32(dt.Rows[0]["SURDH_SRL"]);

                motorClmSurDtlHistEntity.SurdhUnitPrice = Convert.ToInt32(dt.Rows[0]["SURDH_UNIT_PRICE"]);
                motorClmSurDtlHistEntity.SurdhQuantity = Convert.ToInt32(dt.Rows[0]["SURDH_QUANTITY"]);
                motorClmSurDtlHistEntity.SurdhFcAmt = Convert.ToDouble(dt.Rows[0]["SURDH_FC_AMT"]);
                motorClmSurDtlHistEntity.SurdhLcAmt = Convert.ToDouble(dt.Rows[0]["SURDH_LC_AMT"]);

            }
            return dt;
        }
    }
}
