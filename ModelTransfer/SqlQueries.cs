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


        public static string getDirectories = "select DirectoryId, DirectoryName, ParentDirectoryId from ModelDirectories where Archiwum = 0 and DirectoryId>1 ";

        public static short getDirectories_directoryIdIndex = 0;
        public static short getDirectories_directoryNameIndex = 1;
        public static short getDirectories_parentIdIndex = 2;


        public static string getModels = "select IDModel, NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel from DefModel2D ";
        public static string getModelsByIdFilter = "where IDModel in(@iDs)";
        public static string getModelsByDirectoryFilter = "where DirectoryId =";


        public static int getModels_idModelIndex = 0;
        public static int getModels_nazwaModelIndex = 1;
        public static int getModels_opisModelIndex = 2;
        public static int getModels_dataModelIndex = 3;
        public static int getModels_idUzytkIndex = 4;
        public static int getModels_czyArchIndex = 5;
        public static int getModels_directoryIdIndex = 6;
        public static int getModels_idUzytkWlascicielIndex = 7;


        public static string getMaxModelId = "select MAX(IDModel) as maxModelId from DefModel2D"; 


        public static string getPowierzchnieNoBlob = "select IDPow, IDModel, NazwaSkr, NazwaPow, Promien, PoczWspY, PoczWspX, RozmOczekY, RozmOczekX, LbaOczekY, LbaOczekX, IlPkt, IlSektor, Wykladnik, null as PowObrys, " +
                                                "DataPowierzchni, null as dane_bin, dane_bin_rozmiar, null as dane_bin_index, dane_bin_index_rozmiar, minZ, maxZ, ileTri, ileGrd from DefPowierzchni  ";

        public static string getPowierzchnieFull = "select IDPow, IDModel, NazwaSkr, NazwaPow, Promien, PoczWspY, PoczWspX, RozmOczekY, RozmOczekX, LbaOczekY, LbaOczekX, IlPkt, IlSektor, Wykladnik, PowObrys, " +
                                               "DataPowierzchni, dane_bin, dane_bin_rozmiar, dane_bin_index, dane_bin_index_rozmiar, minZ, maxZ, ileTri, ileGrd from DefPowierzchni  ";

        public static string getPowierzchnie_byIdModelFilter = " where IDModel =";
        public static string getPowierzchnie_byIdPowFilter = " where IDPow =";

        public static int getPowierzchnie_idPowIndex = 0;
        public static int getPowierzchnie_idModelIndex = 1;
        public static int getPowierzchnie_nazwaSkrIndex = 2;
        public static int getPowierzchnie_nazwaPowIndex = 3;


        public static string getMaxPowierzchniaId = "select MAX(IDPow) as maxPowId from DefPowierzchni ";



        public static string getPoints = "select IDPunkty, WspX, WspY, Rzedna, IDPow from Model_punkty where IDPow = ";
        public static int getPoints_idPowIndex = 4;


        public static string getTriangles = "select IDTrojkat, IDPunkty1, IDPunkty2, IDPunkty3, IDPow from Model_trojkaty where IDPow =";
        public static int getTriangles_IdPowIndex = 4;


        public static string getGrids = "select ID_gr, WspX, WspY, Rzedna, IDPow from Model_grid where IDPow = ";
        public static int getGrids_IdPowIndex = 4;


        public static string getBreaklines = "select IDLinia, IDPunkt1, IDPunkt2, IDPow from Model_linie where IDPow = ";
        public static int getBreaklines_IdPowIndex = 3;


        //
        //zapisywanie modeli
        //

        public static string insertModel = "insert into DefModel2D(NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel) " + 
                                            "values (@nazwaModel, @opisModel, @dataModel, @idUzytk, @czyArch, @directoryId, @idWlasciciel);\r\n ";


        public static string getUsers = "select IDUzytk, Uzytkownik from Uzytkownik ";

        public static short getUsers_idUzytkownikIndex = 0;
        public static short getUsers_uzytkownikIndex = 1;


        public static string insertDirectory = "insert into ModelDirectories(DirectoryName, ParentDirectoryId, Archiwum) values ('@directoryName', null, 0) ";

        public static string updateDirectoryParentId = "update ModelDirectories set ParentDirectoryId = @newParentId where DirectoryId = ";


    }
}
