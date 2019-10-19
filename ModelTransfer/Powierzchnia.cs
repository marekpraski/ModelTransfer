using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    [Serializable]
    public class Powierzchnia
    {

        #region Region - parametry opisowe powierzchni
        public object idPow { get; set; }
        public object idModel { get; set; }
        public object nazwaSkr { get; set; }
        public object nazwaPow { get; set; }
        public object promien { get; set; }
        public object poczWspY { get; set; }
        public object poczWspX { get; set; }
        public object rozmOczekY { get; set; }
        public object rozmOczekX { get; set; }
        public object lbaOczekY { get; set; }
        public object lbaOczekX { get; set; }
        public object ilPkt { get; set; }
        public object ilSektor { get; set; }
        public object wykladnik { get; set; }
        public object powObrys { get; set; }
        public object dataPowierzchni { get; set; }
        public object daneBin { get; set; }
        public object daneBinRozmiar { get; set; }
        public object daneBinIndex { get; set; }
        public object daneBinIndexRozmiar { get; set; }
        public object minZ { get; set; }
        public object maxZ { get; set; }
        public object ileTri { get; set; }
        public object ileGrid { get; set; }

        public string idPow_dataType { get; set; }
        public string idModel_dataType { get; set; }
        public string nazwaSkr_dataType { get; set; }
        public string nazwaPow_dataType { get; set; }
        public string promien_dataType { get; set; }
        public string poczWspY_dataType { get; set; }
        public string poczWspX_dataType { get; set; }
        public string rozmOczekY_dataType { get; set; }
        public string rozmOczekX_dataType { get; set; }
        public string lbaOczekY_dataType { get; set; }
        public string lbaOczekX_dataType { get; set; }
        public string ilPkt_dataType { get; set; }
        public string ilSektor_dataType { get; set; }
        public string wykladnik_dataType { get; set; }
        public string powObrys_dataType { get; set; }
        public string dataPowierzchni_dataType { get; set; }
        public string daneBin_dataType { get; set; }
        public string daneBinRozmiar_dataType { get; set; }
        public string daneBinIndex_dataType { get; set; }
        public string daneBinIndexRozmiar_dataType { get; set; }
        public string minZ_dataType { get; set; }
        public string maxZ_dataType { get; set; }
        public string ileTri_dataType { get; set; }
        public string ileGrid_dataType { get; set; }

        #endregion


        #region Region - parametry elementy składowe powierzchni

        public ModelGrid grids { get; set; }
        public ModelTriangles triangles { get; set; }
        public ModelPunkty points { get; set; }
        public ModelLinie breaklines { get; set; }

        public object[] powierzchniaData { get; set; }
        public List<string> columnHeaders { get; set; }
        public List<string> columnDataTypes { get; set; }

        #endregion

        internal void modifyData(uint newValue)
        {
            
        }
    }
}
