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
    public partial class Form1 : Form
    {

        private string userLogin = "";
        private string userPassword = "";

        private string currentPath = "";     //katalog z którego uruchamiany jest program, wykrywany przez DBConnector i ustawiany tutaj
                                             //dla DEBUGA ustawiony jest w metodzie ReadAllData
     

        private DBReader dbReader;
        private SqlConnection dbConnection;

        bool userClicked = false;


        public Form1(string login, string pass)
        {
            InitializeComponent();
            this.userLogin = login;
            this.userPassword = pass;
            populateModelListView();
        }


        #region Region - start programu, połączenie z bazą danych


        private bool establishConnection()
        {
            DBConnector dbConnector = new DBConnector(userLogin, userPassword);
#if DEBUG
            currentPath = @"C:\testDesktop\conf";
#else
            currentPath = Application.StartupPath;
#endif
            if (dbConnector.validateConfigFile(currentPath))
            {
                dbConnection = dbConnector.getDBConnection(ConnectionSources.serverNameInFile, ConnectionTypes.sqlAuthorisation);
                dbReader = new DBReader(dbConnection);
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

        private void ModelsFromFileButton_Click(object sender, EventArgs e)
        {
            List<Model2D> models = readModelsFromFile();
            writeModelsToDB(models);
        }


        private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (userClicked)
            {
                toolStripSaveToFileButton.Enabled = true;
            }
        }


        private void ToolStripSaveToFileButton_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count > 0)
            {
                List<Model2D> selectedModels = readSelectedModelsFromDB();
                saveModelsToFile(selectedModels);
                toolStripSaveToFileButton.Enabled = false;
            }
            else
            {
                MyMessageBox.display("Brak zaznaczonych modeli");
            }
        }


        #endregion


        #region Region - czytanie modeli z bazy danych i zapisywanie do pliku

        private QueryData readModelsFromDB(string queryFilter = "")
        {
            string query = SqlQueries.getModels + queryFilter;
            return dbReader.readFromDB(query);
        }


        private void populateModelListView()
        {
            if (establishConnection())
            {
                QueryData modelData = readModelsFromDB();
                if (modelData.getHeaders().Count > 0)
                {
                    foreach (string[] model in modelData.getQueryDataAsStrings())
                    {
                        ListViewItem item = new ListViewItem(model);
                        listView1.Items.Add(item);
                    }
                }
                else
                {
                    MyMessageBox.display("Nie można było załadować modeli", MessageBoxType.Error);
                }
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

                readPowierzchniaDataFromDB(model2D);
                selectedModels.Add(model2D);
            }
            return selectedModels;
        }


        private void readPowierzchniaDataFromDB(Model2D model)
        {
            string query = SqlQueries.getPowierzchnie + model.idModel;
            QueryData powierzchniaData = dbReader.readFromDB(query);

            for(int i=0; i < powierzchniaData.getDataRowsNumber(); i++)
            {
                Powierzchnia pow = new Powierzchnia();

                object idPowierzchni = powierzchniaData.getQueryData()[i][SqlQueries.getPowierzchnie_idPowIndex];
                object idModel = powierzchniaData.getQueryData()[i][SqlQueries.getPowierzchnie_idModelIndex];

                pow.idPow = idPowierzchni;
                pow.idModel = idModel;
                model.addPowierzchnia(pow);
            }
            model.powierzchnieBulkData = powierzchniaData;
        }


        private void saveModelsToFile(List<Model2D> selectedModels)
        {
            string fileSaveDir = currentPath;
            string fileName = "modele.bin";
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

            if (fm.assertFileExists(filePath + @"\" + fileName))
            {
                //deserialize
                using (Stream stream = File.Open(filePath + @"\" + fileName, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    models = (List<Model2D>)bformatter.Deserialize(stream);
                }
            }
            return models;
        }


        private void writeModelsToDB(List<Model2D> models)
        {
            DBWriter writer = new DBWriter(dbConnection);
            DBValueTypeConverter converter = new DBValueTypeConverter();
            int maxModelIdInDB = getMaxModelIdFromDB();

            int newDirectoryId = 1;
            int newIdWlasciciel = 1;
            string newCzyArch = "0";        //wczytywane modele nie będą archiwalne
            string newIdUzytk = "null";     //wczytywane modele nie będą oznaczone jako wczytane do pamięci

            foreach(Model2D model in models)
            {
                ++maxModelIdInDB;
                modifyPowierzchniaData(model, maxModelIdInDB);
                string nazwaModel = converter.getConvertedValue(model.nazwaModel, model.nazwaModel_dataType);
                string opisModel = converter.getConvertedValue(model.opisModel, model.opisModel_dataType);
                string dataModel = converter.getConvertedValue(model.dataModel, model.dataModel_dataType);

                string query = SqlQueries.insertModel.Replace("@nazwaModel", nazwaModel).Replace("@opisModel", opisModel).Replace("@dataModel", dataModel).Replace("@idUzytk", newIdUzytk).Replace("@czyArch", newCzyArch).Replace("@directoryId", newDirectoryId.ToString()).Replace("@idWlasciciel", newIdWlasciciel.ToString());
                writer.writeToDB(query);
                writeModelDataToDB(model);
            }
        }

        private void writeModelDataToDB(Model2D model)
        {
            MyMessageBox.display("finito");
            //throw new NotImplementedException();
        }

        private int getMaxModelIdFromDB()
        {
            string query = SqlQueries.getMaxModelId;
            QueryData res = dbReader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }


        private void modifyPowierzchniaData(Model2D model, int newModelId)
        {
            int newPowierzchniaId = getMaxPowierzchniaIdFromDB();
            model.powierzchnieBulkData.replaceDataColumnValue(SqlQueries.getPowierzchnie_idModelIndex, newModelId);

            foreach (Powierzchnia pow in model.powierzchnieList)
            {
                ++newPowierzchniaId;
                pow.idModel = newModelId;
                pow.idPow = newPowierzchniaId;
            }
        }


        private int getMaxPowierzchniaIdFromDB()
        {
            string query = SqlQueries.getMaxPowierzchniaId;
            QueryData res = dbReader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }

        #endregion

    }
}
