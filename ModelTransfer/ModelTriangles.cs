using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelTriangles
    {
        public DataTable triangleData { get; set; }


        public bool setNewIdPow(uint newIdPow)
        {
            if (triangleData.Rows.Count > 0)
            {
                DataColumn col = triangleData.Columns[SqlQueries.getTriangles_IdPowIndex];
                foreach (DataRow row in triangleData.Rows)
                {
                    row[col] = newIdPow;
                }
                return true;
            }
            return false;
        }
    }
}
