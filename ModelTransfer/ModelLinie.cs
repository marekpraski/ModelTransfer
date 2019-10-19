using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelLinie
    {
        public DataTable breaklineData { get; set; }


        public bool setNewIdPow(uint newIdPow)
        {
            if (breaklineData.Rows.Count > 0)
            {
                DataColumn col = breaklineData.Columns[SqlQueries.getBreaklines_IdPowIndex];
                foreach (DataRow row in breaklineData.Rows)
                {
                    row[col] = newIdPow;
                }
                return true;
            }
            return false;
        }
    }
}
