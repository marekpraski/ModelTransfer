using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    public class SqlQueries
    {

        //
        //pobieranie danych modeli
        //


        public static string getModels = "select IDModel, NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel from DefModel2D ";
        public static string getModelsByIdFilter = "where IDModel in(@iDs)";
  
        public static int getModels_idModelIndex = 0;
        public static int getModels_nazwaModelIndex = 1;
        public static int getModels_opisModelIndex = 2;
        public static int getModels_dataModelIndex = 3;
        public static int getModels_idUzytkIndex = 4;
        public static int getModels_czyArchIndex = 5;
        public static int getModels_directoryIdIndex = 6;
        public static int getModels_idUzytkWlascicielIndex = 7;


        public static string getMaxModelId = "select MAX(IDModel) as maxModelId from DefModel2D"; 


        public static string getPowierzchnie = "select IDPow, IDModel, NazwaSkr, NazwaPow, Promien, PoczWspY, PoczWspX, RozmOczekY, RozmOczekX, LbaOczekY, LbaOczekX, IlPkt, IlSektor, Wykladnik, PowObrys, " +
                                                "DataPowierzchni, dane_bin, dane_bin_rozmiar, dane_bin_index, dane_bin_index_rozmiar, minZ, maxZ, ileTri, ileGrd from DefPowierzchni where IDModel = ";

        public static int getPowierzchnie_idPowIndex = 0;
        public static int getPowierzchnie_idModelIndex = 1;
        public static int getPowierzchnie_nazwaSkrIndex = 2;
        public static int getPowierzchnie_nazwaPowIndex = 3;
        public static int getPowierzchnie_promienIndex = 4;
        public static int getPowierzchnie_poczWspYIndex = 5;
        public static int getPowierzchnie_poczWspXIndex = 6;
        public static int getPowierzchnie_rozmOczekYIndex = 7;
        public static int getPowierzchnie_rozmOczekXIndex = 8;
        public static int getPowierzchnie_lbaOczekYIndex = 9;
        public static int getPowierzchnie_lbaOczekXIndex = 10;
        public static int getPowierzchnie_ilPktIndex = 11;
        public static int getPowierzchnie_ilSektorIndex = 12;
        public static int getPowierzchnie_wykladnikIndex = 13;
        public static int getPowierzchnie_powObrysIndex = 14;
        public static int getPowierzchnie_dataPowierzchniIndex = 15;
        public static int getPowierzchnie_daneBinIndex = 16;
        public static int getPowierzchnie_daneBinRozmiarIndex = 17;
        public static int getPowierzchnie_daneBinIndexIndex = 18;
        public static int getPowierzchnie_daneBinIndexRozmiarIndex = 19;
        public static int getPowierzchnie_minZIndex = 20;
        public static int getPowierzchnie_maxZIndex = 21;
        public static int getPowierzchnie_ileTriIndex = 22;
        public static int getPowierzchnie_ileGridIndex = 23;

        public static string getMaxPowierzchniaId = "select MAX(IDPow) as maxPowId from DefPowierzchni ";

        //
        //zapisywanie modeli
        //

        public static string insertModel = "insert into DefModel2D(NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel) " + 
                                            "values (@nazwaModel, @opisModel, @dataModel, @idUzytk, @czyArch, @directoryId, @idWlasciciel);\r\n ";



    }
}
