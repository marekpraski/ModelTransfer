using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class Model2D
    {

        #region Region parametry opisowe modelu

        public object idModel { get; set; }
        public int IdModelAfterRestore { get; set; }        //przypisany po odtworzeniu deklaracji modeli do bazy; stary ID potrzebny do powiązania modelu z Powierzchnią, wczytywaną później

        public object nazwaModel { get; set; }
        public object opisModel { get; set; }
        public object dataModel { get; set; }
        public object idUzytk { get; set; }
        public object czyArch { get; set; }
        public object directoryId { get; set; }
        public object idUzytkWlasciciel { get; set; }

        public string idModel_dataType { get; set; }
        public string nazwaModel_dataType { get; set; }
        public string opisModel_dataType { get; set; }
        public string dataModel_dataType { get; set; }
        public string idUzytk_dataType { get; set; }
        public string czyArch_dataType { get; set; }
        public string directoryId_dataType { get; set; }
        public string idUzytkWlasciciel_dataType { get; set; }


        #endregion


        #region Region - elementy składowe modelu

        public List<ModelPowierzchnia> powierzchnieList { get; }

        public ModelDirectory modelDir  {get; set;}



        #endregion

        public Model2D()
        {
            powierzchnieList = new List<ModelPowierzchnia>();
        }

        public void addPowierzchnia(ModelPowierzchnia pow)
        {
            powierzchnieList.Add(pow);
        }



        public void setNewModelIdInPowierzchnia(int newModelId)
        {
            foreach (ModelPowierzchnia pow in powierzchnieList)
            {
                pow.idModel = newModelId;

                DataColumn col = pow.powDataTable.Columns[SqlQueries.getPowierzchnie_idModelIndex];
                foreach (DataRow row in pow.powDataTable.Rows)
                {
                    row[col] = newModelId;
                }
            }
        }

    }
}
