using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ModelTransfer
{
    public partial class MainForm : Form
    {

        private string userLogin = "";
        private string userPassword = "";

        private string currentPath = "";     //katalog z którego uruchamiany jest program, wykrywany przez DBConnector i ustawiany tutaj
                                             //dla DEBUGA ustawiony jest w metodzie ReadAllData
     

        private DBReader reader;
        DBWriter writer;
        DBConnector dbConnector;
        private SqlConnection dbConnection;

        bool userClicked = false;



        public MainForm(string login, string pass)
        {
            InitializeComponent();
            this.userLogin = login;
            this.userPassword = pass;
            setupThisForm();
        }


        #region Region - start programu, połączenie z bazą danych


        private bool establishConnection()
        {
            dbConnector = new DBConnector(userLogin, userPassword);
#if DEBUG
            currentPath = @"C:\testDesktop\conf";
#else
            currentPath = Application.StartupPath;
#endif
            if (dbConnector.validateConfigFile(currentPath))
            {
                dbConnection = dbConnector.getDBConnection(ConnectionSources.serverNameInFile, ConnectionTypes.sqlAuthorisation);
                reader = new DBReader(dbConnection);
                return true;
            }
            return false;
        }

        #endregion


        #region Region - zdarzenia wywołane przez interakcję użytkownika

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            userClicked = true;
        }


        private void HelpButton_Click(object sender, EventArgs e)
        {
            string pomocInfo = "dir = currentPath \r\n" + "fileName = modele.bin";
            MyMessageBox.display(pomocInfo);
        }


        private void SaveToDBButton_Click(object sender, EventArgs e)
        {
            GetDirectoryAndUserForm getDir = new GetDirectoryAndUserForm(reader);
            getDir.ShowDialog();
            List<Model2D> models = readModelsFromFile();
            if (models != null)
            {
                writeModelsToDB(models);
            }
            else
            {
                MyMessageBox.display("Nie można było odczytać pliku źródłowego", MessageBoxType.Error);
            }
        }


        private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (userClicked)
            {
                toolStripSaveToFileButton.Enabled = true;
            }
        }


        private void SaveToFileButton_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count > 0)
            {
                GetFileNameForm fnForm = new GetFileNameForm();
                fnForm.GetFileName += getFileNameForm_ButtonClick;
                fnForm.ShowDialog();
                
            }
            else
            {
                MyMessageBox.display("Brak zaznaczonych modeli");
            }
        }

        private void getFileNameForm_ButtonClick(object sender, MyEventArgs args)
        {
            List<Model2D> selectedModels = readSelectedModelsFromDB();
            saveModelsToFile(selectedModels, args.fileName);
            toolStripSaveToFileButton.Enabled = false;
        }


        #endregion


        #region Region - czytanie modeli z bazy danych i zapisywanie do pliku

        private QueryData readModelsFromDB(string queryFilter = "")
        {
            string query = SqlQueries.getModels + queryFilter;
            return reader.readFromDB(query);
        }


        private void setupThisForm()
        {
            if (establishConnection())
            {
                directoryTreeControl1.directorySelectedEvent += onDirectorySelected_treeViewNodeSelected;
                directoryTreeControl1.setUpTreeview(reader);
            }
        }

        private void onDirectorySelected_treeViewNodeSelected(object sender, MyEventArgs args)
        {
            populateModelListview(args.selectedDirectoryId);
        }

        private void populateModelListview(string selectedDirectoryId)
        {
            listView1.Items.Clear();
            string queryFilter = SqlQueries.getModelsByDirectory + selectedDirectoryId;
            QueryData modelData = readModelsFromDB(queryFilter);
                if (modelData.getHeaders().Count > 0)
                {
                    foreach (string[] model in modelData.getQueryDataAsStrings())
                    {
                        ListViewItem item = new ListViewItem(model);
                        listView1.Items.Add(item);
                    }
                listView1.Refresh();
                }
                else
                {
                    MyMessageBox.display("Nie można było załadować modeli", MessageBoxType.Error);
                }
        }



        private List<Model2D> readSelectedModelsFromDB()
        {
            List<Model2D> selectedModels = new List<Model2D>();
            string modelIds = "";
            foreach (ListViewItem checkedModel in listView1.CheckedItems)
            {
                modelIds += (checkedModel.Text + ",");
            }
            int index = modelIds.LastIndexOf(",");
            string queryFilter = SqlQueries.getModelsByIdFilter.Replace("@iDs", modelIds.Remove(index, 1));
            QueryData modelData = readModelsFromDB(queryFilter);
            List<object[]> models = modelData.getQueryData();
            List<string> paramTypes = modelData.getDataTypes();

            for(int i=0; i< models.Count; i++)
            {
                object[] model = models[i];
                Model2D model2D = new Model2D();

                model2D.czyArch = model[SqlQueries.getModels_czyArchIndex];
                model2D.czyArch_dataType = paramTypes[SqlQueries.getModels_czyArchIndex];

                model2D.dataModel = model[SqlQueries.getModels_dataModelIndex];
                model2D.dataModel_dataType = paramTypes[SqlQueries.getModels_dataModelIndex];

                model2D.directoryId = model[SqlQueries.getModels_directoryIdIndex];
                model2D.directoryId_dataType = paramTypes[SqlQueries.getModels_directoryIdIndex];

                model2D.idModel = model[SqlQueries.getModels_idModelIndex];
                model2D.idModel_dataType =  paramTypes[SqlQueries.getModels_idModelIndex];

                model2D.idUzytk = model[SqlQueries.getModels_idUzytkIndex];
                model2D.idUzytk_dataType = paramTypes[SqlQueries.getModels_idUzytkIndex];

                model2D.idUzytkWlasciciel = model[SqlQueries.getModels_idUzytkWlascicielIndex];
                model2D.idUzytkWlasciciel_dataType = paramTypes[SqlQueries.getModels_idUzytkWlascicielIndex];

                model2D.nazwaModel = model[SqlQueries.getModels_nazwaModelIndex];
                model2D.nazwaModel_dataType = paramTypes[SqlQueries.getModels_nazwaModelIndex];

                model2D.opisModel = model[SqlQueries.getModels_opisModelIndex];
                model2D.opisModel_dataType = paramTypes[SqlQueries.getModels_opisModelIndex];

                readPowierzchniaFromDB(model2D);
                selectedModels.Add(model2D);
            }
            return selectedModels;
        }


        private void readPowierzchniaFromDB(Model2D model)
        {
            string query = SqlQueries.getPowierzchnie + SqlQueries.getPowierzchnie_FilterAllInModel + model.idModel;
            QueryData powierzchnieData = reader.readFromDB(query);
            List<string> paramTypes = powierzchnieData.getDataTypes();

            for(int i=0; i < powierzchnieData.getDataRowsNumber(); i++)
            {
                Powierzchnia pow = new Powierzchnia();

                pow.idPow = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_idPowIndex];

                pow.idModel = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_idModelIndex];
                pow.idModel_dataType = paramTypes[SqlQueries.getPowierzchnie_idModelIndex];

                pow.nazwaPow = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_nazwaPowIndex];
                pow.nazwaPow_dataType = paramTypes[SqlQueries.getPowierzchnie_nazwaPowIndex];

                pow.powierzchniaData = powierzchnieData.getQueryData()[i];
                pow.columnHeaders = powierzchnieData.getHeaders();
                pow.columnDataTypes = powierzchnieData.getDataTypes();
                pow.powDataTable = reader.readFromDBToDataTable(SqlQueries.getPowierzchnie + SqlQueries.getPowierzchnie_FilterSingleById + pow.idPow);
                readPowierzchniaDataFromDB(pow);
                model.addPowierzchnia(pow);
            }
        }

        private void readPowierzchniaDataFromDB(Powierzchnia pow)
        {
            string query = "";

            ModelPunkty points = new ModelPunkty();
            query = SqlQueries.getPoints + pow.idPow;
            points.pointData = reader.readFromDBToDataTable(query);
            pow.points = points;

            ModelTriangles triangles = new ModelTriangles();
            query = SqlQueries.getTriangles + pow.idPow;
            triangles.triangleData = reader.readFromDBToDataTable(query);
            pow.triangles = triangles;

            ModelLinie breaklines = new ModelLinie();
            query = SqlQueries.getBreaklines + pow.idPow;
            breaklines.breaklineData = reader.readFromDBToDataTable(query);
            pow.breaklines = breaklines;

            ModelGrid grids = new ModelGrid();
            query = SqlQueries.getGrids + pow.idPow;
            grids.gridData = reader.readFromDBToDataTable(query);
            pow.grids = grids;

        }

        private void saveModelsToFile(List<Model2D> selectedModels, string fileName)
        {
            string fileSaveDir = currentPath;

            if (fileName == null || fileName == "")
            {
                fileName = "modele.bin";
            }
            else
            {
               fileName += ".bin";
            }

            string serializationFile = Path.Combine(fileSaveDir, fileName);

            //serialize
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, selectedModels);
            }
        }

        #endregion


        #region Region - czytanie modeli z pliku binarnego i zapisywanie do bazy danych

        private List<Model2D> readModelsFromFile()
        {
            FileManipulator fm = new FileManipulator();
            string fileName = "modele.bin";
            string filePath = currentPath;
            List<Model2D> models = new List<Model2D>();

            try
            {
                if (fm.assertFileExists(filePath + @"\" + fileName))
                {
                    //deserialize
                    using (Stream stream = File.Open(filePath + @"\" + fileName, FileMode.Open))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                        models = (List<Model2D>)bformatter.Deserialize(stream);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MyMessageBox.display(ex.Message, MessageBoxType.Error);
                return null;
            }
            return models;
        }


        private void writeModelsToDB(List<Model2D> models)
        {
            writer = new DBWriter(dbConnection);
            DBValueTypeConverter converter = new DBValueTypeConverter();
            int maxModelIdInDB = 0;

            int newDirectoryId = 1;
            int newIdWlasciciel = 1;
            string newCzyArch = "0";        //wczytywane modele nie będą archiwalne
            string newIdUzytk = "null";     //wczytywane modele nie będą oznaczone jako wczytane do pamięci


            //po kolei wpisuję deklaracje wszystkich modeli, po jednym, do tabeli DefModel2D
            for(int i =0; i< models.Count; i++)
            {
                Model2D model = models[i];
                
                string nazwaModel = converter.getConvertedValue(model.nazwaModel, model.nazwaModel_dataType);
                string opisModel = converter.getConvertedValue(model.opisModel, model.opisModel_dataType);
                string dataModel = converter.getConvertedValue(model.dataModel, model.dataModel_dataType);

                string query = SqlQueries.insertModel.Replace("@nazwaModel", nazwaModel).Replace("@opisModel", opisModel).Replace("@dataModel", dataModel).Replace("@idUzytk", newIdUzytk).Replace("@czyArch", newCzyArch).Replace("@directoryId", newDirectoryId.ToString()).Replace("@idWlasciciel", newIdWlasciciel.ToString());
                writer.writeToDB(query);

                if (i == 0)         //wpis modelu robię przez insert, baza danych automatycznie nadaje mu ID, które teraz odczytuję
                {
                    maxModelIdInDB = getMaxModelIdFromDB();
                }
                else
                {
                    maxModelIdInDB++;       //kolejne modele będą miały kolejne ID, nie muszę za każdym razem czytać tylko inkrementuję
                }

                //w każdym modelu w powierzchniach zmieniam Id modelu na nowy, w nowej bazie danych
                model.setNewModelId(maxModelIdInDB);

                //po zapisaniu deklaracji modelu zapisuję dane szczegółowe tego modelu, tzn powierzchnie itp, które są w osobnych tablicach
                writePowierzchniaToDB(model);
            }
            MyMessageBox.display("Modele wczytane");
        }

        private void writePowierzchniaToDB(Model2D model)
        {
            uint maxPowId = 0;
            string tableName = dbConnector.getTableNameFromQuery(SqlQueries.getPowierzchnie);

            for(int i=0; i < model.powierzchnieList.Count; i++)
            {
                Powierzchnia pow = model.powierzchnieList[i];

                writer.writeBulkDataToDB(pow.powDataTable, tableName);

                if (i==0)       //analogicznie jak w przypadku wpisywania deklaracji modeli, po dodaniu pierwszej powierzchni odczytuję jej ID z bazy
                {
                    maxPowId = getMaxPowierzchniaIdFromDB();
                }
                else            //kolejne ID tworzę sam
                {
                    maxPowId++;
                }
                    //w każdej powierzchni, w danych składowych tj trójkątów, punktów itd  zmieniam ID powierzchni na nowy, w nowej bazie danych
                pow.idPow = maxPowId;

                //zapisuję dane szczegółowe każdej powierzchni do bazy, tj. punkty, trójkąty itd
                writePowierzchniaDataToDB(pow);
            }

        }

        

        private int getMaxModelIdFromDB()
        {
            string query = SqlQueries.getMaxModelId;
            QueryData res = reader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }


        private void writePowierzchniaDataToDB(Powierzchnia pow)
        {
            string tableName = "";
            uint newIdPow = uint.Parse(pow.idPow.ToString());

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getPoints);
            ModelPunkty points = pow.points;
            if (points.setNewIdPow(newIdPow))
            {
                writer.writeBulkDataToDB(points.pointData, tableName);
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getTriangles);
            ModelTriangles triangles = pow.triangles;
            if (triangles.setNewIdPow(newIdPow))
            {
                writer.writeBulkDataToDB(triangles.triangleData, tableName);
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getGrids);
            ModelGrid grids = pow.grids;
            if (grids.setNewIdPow(newIdPow))
            {
                writer.writeBulkDataToDB(grids.gridData, tableName);
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getBreaklines);
            ModelLinie breaklines = pow.breaklines;
            if (breaklines.setNewIdPow(newIdPow))
            {
                writer.writeBulkDataToDB(breaklines.breaklineData, tableName);
            }

        }



        private uint getMaxPowierzchniaIdFromDB()
        {
            string query = SqlQueries.getMaxPowierzchniaId;
            QueryData res = reader.readFromDB(query);
            return uint.Parse(res.getQueryData()[0][0].ToString());
        }

        #endregion

    }
}
