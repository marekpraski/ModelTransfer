using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelPunkty
    {
        public DataTable pointData { get; set; }


        public bool setNewIdPow(uint newIdPow)
        {
            if (pointData.Rows.Count > 0)
            {
                DataColumn col = pointData.Columns[SqlQueries.getPoints_idPowIndex];
                foreach (DataRow row in pointData.Rows)
                {
                    row[col] = newIdPow;
                }
                return true;
            }
            return false;
        }

    }
}
