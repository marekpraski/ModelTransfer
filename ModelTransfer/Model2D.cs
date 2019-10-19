using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    [Serializable]
    public class Model2D
    {

        #region Region parametry opisowe modelu

        public object idModel { get; set; }
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

        public List<Powierzchnia> powierzchnieList { get; }
        public List<object[]> powierzchnieBulkData { get; set; }


        #endregion

        public Model2D()
        {
            powierzchnieList = new List<Powierzchnia>();
        }

        public void addPowierzchnia(Powierzchnia pow)
        {
            powierzchnieList.Add(pow);
        }

        public void changePowierzchnieBulkDataColumnValue(int colIndex, object newValue)
        {
            foreach (object[] row in powierzchnieBulkData)
            {
                row[colIndex] = newValue;
            }
        }

        public void modifyPowierzchniaData(int newModelId)
        {
            changePowierzchnieBulkDataColumnValue(SqlQueries.getPowierzchnie_idModelIndex, newModelId);

            for (int i = 0; i < powierzchnieList.Count; i++)
            {
                Powierzchnia pow = powierzchnieList[i];
                pow.idModel = newModelId;                       //powinienem tutaj też zmienić IdPow ale nie mogę, bo żeby to zrobić muszę najpierw dodać powierzchnię do bazy danych
                                                                //dlatego to mogę zrobić dopiero po dodaniu
            }
        }

    }
}
