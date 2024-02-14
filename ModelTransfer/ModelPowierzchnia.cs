using System;
using System.Data;

namespace ModelTransfer
{
    [Serializable]
    public class ModelPowierzchnia
    {
        public object idPow { get; set; }
        public object idModel { get; set; }

        public DataTable powDataTable { get; set; }
        /// <summary>
        /// w DataTable pole geometry podczas serializacji-deserializacji jest źle zapisywane-odczytywane, 
        /// więc muszę zapisać dodatkowo powObrys do string i osobno zapisywać do bazy
        /// </summary>
        public string powObrys { get; set; }
    }
}
