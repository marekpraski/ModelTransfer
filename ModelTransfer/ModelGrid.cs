using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelGrid
    {
        public DataTable gridData { get; set; }


        public bool setNewIdPow(uint newIdPow)
        {
            if (gridData.Rows.Count > 0)
            {
                DataColumn col = gridData.Columns[SqlQueries.getGrids_IdPowIndex];
                foreach (DataRow row in gridData.Rows)
                {
                    row[col] = newIdPow;
                }
                return true;
            }
            return false;
        }
    }
}
