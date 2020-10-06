using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelPowierzchnia
    {

        #region Region - parametry opisowe powierzchni
        public object idPow { get; set; }
        public object idModel { get; set; }
        public object nazwaPow { get; set; }

        public string idPow_dataType { get; set; }
        public string idModel_dataType { get; set; }
        public string nazwaPow_dataType { get; set; }
 

        #endregion


        #region Region - parametry elementy składowe powierzchni

        public ModelGrid grids { get; set; }
        public ModelTriangles triangles { get; set; }
        public ModelPunkty points { get; set; }
        public ModelLinie breaklines { get; set; }

        public object[] powierzchniaData { get; set; }
        public DataTable powDataTable { get; set; }
        public string powObrys { get; set; }
        public List<string> columnHeaders { get; set; }
        public List<string> columnDataTypes { get; set; }

        #endregion

        public void setPowId(uint newPowId)
        {
            this.idPow = newPowId;
            DataColumn col = powDataTable.Columns[SqlQueries.getPowierzchnie_idPowIndex];
            foreach (DataRow row in powDataTable.Rows)
            {
                row[col] = newPowId;
            }
        }
    }
}
