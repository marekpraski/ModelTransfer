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
using System.Diagnostics;
using System.Threading;

namespace ModelTransfer
{
    public partial class MainForm : Form
    {

        #region Region - parametry

        private string userLogin = "";
        private string userPassword = "";

        private string currentPath = "";     //katalog z którego uruchamiany jest program, wykrywany przez DBConnector i ustawiany tutaj
                                             //dla DEBUGA ustawiony jest w metodzie ReadAllData
     

        private DBReader reader;
        private DBWriter writer;
        private DBConnector dbConnector;
        private SqlConnection dbConnection;

        private bool userClicked = false;
        private bool getModelsFromDirectories = false;           //true jeżeli istnieje gałąź w drzewie katalogów, która ma zaznaczony checkbox

        private string[] modelSaveOptions = { "same punkty", "pełne modele" };


        //private MyEventArgs myEventArgs;
        private BackgroundWorker bgw;

        private ModelBundle modelBundle;            //modele wczytane z pliku
        private List<Model2D> selectedModels;       //modele wybrane przez użytkownika do zapisania w pliku

        string timeLog = "";

        private Thread computationsThread;

        private int saveModelOption = 0;        //odczyt z kombo, ustawiany po naciśnięciu przycisku zapisu do pliku

        private string log = "";

        private bool directoriesChecked = false;        //aktualizowana przez zdarzenie zafajkowania checkboxa w drzewie katalogów; 
                                                        //powoduje zatrzymanie wypełniania okna listy modeli po zaznaczeniu katalogu
                                                        //jeżeli nie nałożę tej blokady, występuje konflikt pomiędzy wątkiem wczytującym modele w celu zapisania do pliku a wątkiem wypełniającym okno modeli
                                                        //co wywala błąd BDReadera (połączenie otwarte)

        #endregion


        public MainForm(string login, string pass)
        {
            InitializeComponent();
            this.userLogin = login;
            this.userPassword = pass;
            setupThisForm();
        }


        private void setupThisForm()
        {
            if (establishConnection())
            {
                hideProgressItems();
                directoryTreeControl1.directorySelectedEvent += onTreeviewDirectorySelected;
                directoryTreeControl1.directoryCheckedEvent += onTreeviewDirectoryChecked;

                saveModelOptionsCombo.Items.AddRange(modelSaveOptions);
                saveModelOptionsCombo.SelectedIndex = 0;
                directoryTreeControl1.turnTreeviewCheckboxesOn();
                directoryTreeControl1.setUpThisForm(reader);
            }
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


        #region Region - zdarzenia wywołane  przez interakcję użytkownika w tej formatce

        private void modelsListView_MouseClick(object sender, MouseEventArgs e)
        {
            userClicked = true;
        }


        private void modelsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (userClicked)
            {
                toolStripSaveToFileButton.Enabled = true;
            }
        }



        private void HelpButton_Click(object sender, EventArgs e)
        {
            string pomocInfo = "dir = currentPath \r\n" + "fileName = modele.bin";
            MyMessageBox.display(pomocInfo);
        }



        private void SaveToDBButton_Click(object sender, EventArgs e)
        {
            GetDirectoryAndUserForm getDirectoryAndUser = new GetDirectoryAndUserForm(reader);
            getDirectoryAndUser.acceptButtonClickedEvent += onGetUserAndDirectory_ButtonClick;
            getDirectoryAndUser.ShowDialog();
            
        }



        private void SaveToFileButton_Click(object sender, EventArgs e)
        {
            if (modelsListView.CheckedItems.Count > 0 || getModelsFromDirectories)
            {
                saveModelOption = saveModelOptionsCombo.SelectedIndex;
                GetFileNameForm fnForm = new GetFileNameForm();
                fnForm.GetFileNameEvent += onGetFileNameForm_ButtonClick;
                fnForm.ShowDialog();                
            }
            else
            {
                MyMessageBox.display("Brak zaznaczonych modeli");
            }
        }


        #endregion


        #region Region - zdarzenia wywołane przez interakcję użytkownika w innych formatkach, mające wpływ na akcje tej formatki


        private void onTreeviewDirectorySelected(object sender, MyEventArgs args)
        {
            if (!directoriesChecked) {
                populateModelListview(args.selectedDirectoryId);
            }
        }


        private void onTreeviewDirectoryChecked(object sender, MyEventArgs args)
        {
            if (args.checkedDirectoriesExist)
            {
                modelsListView.Enabled = false;
                getModelsFromDirectories = true;
                toolStripSaveToFileButton.Enabled = true;
                directoriesChecked = true;
            }
            else
            {
                modelsListView.Enabled = true;
                getModelsFromDirectories = false;
                toolStripSaveToFileButton.Enabled = false;
                directoriesChecked = false;
            }
        }


        #endregion


        #region Region - wątek czytania modeli z pliku i zapisywania do bazy danych

        //formatka GetDirectoryAndUserForm
        private void onGetUserAndDirectory_ButtonClick(object sender, MyEventArgs args)
        {
            showReadFromFileProgressItems();

            computationsThread = new Thread(() => writeModelsFromFileToDB(args));
            computationsThread.Start();

        }

        private void writeModelsFromFileToDB(MyEventArgs args)
        {
            readModelsFromFile();

            if (modelBundle != null)
            {
                showWriteToDBProgressItems();       //delegat
                writeModelsToDB(args);
                hideProgressItems();            //delegat
                refreshDirectoryTree();      //delegat          
            }
            else
            {
                MyMessageBox.display("Nie można było odczytać modeli z pliku źródłowego", MessageBoxType.Error);
            }
        }


        public delegate void refreshDirectoryTreeDelegate();
        private void refreshDirectoryTree()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new refreshDirectoryTreeDelegate(refreshDirectoryTree));
            }
            else
            {
                directoryTreeControl1.resetThisForm();
                directoryTreeControl1.setUpThisForm(reader);
            }
        }


        #endregion


        #region Region - wątek czytania modeli z bazy i zapisywania do pliku,

        //formatka GetFileNameForm
        private void onGetFileNameForm_ButtonClick(object sender, MyEventArgs args)
        {
            showReadFromDBProgressItems();

            //uruchamiam wątek po wyświetleniu elementów paska postępu
            computationsThread = new Thread(() => saveModelsFromDbToFile(args.fileName));
            computationsThread.Start();
        }


        private void saveModelsFromDbToFile(string fileName)
        {

            readSelectedModelsFromDB();

            showWriteToFileProgressItems();     //delegate

            saveModelsToFile(fileName);

            hideProgressItems();                //delegate
            disableSaveToFileButton();          //delegate
            MyMessageBox.display("Modele zapisane do pliku");
        }

        public delegate void disableSaveToFileButtonDelegate();

        private void disableSaveToFileButton()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new disableSaveToFileButtonDelegate(disableSaveToFileButton));
            }
            else
            {
                toolStripSaveToFileButton.Enabled = false;
            }
        }

        #endregion



        #region Region - obsługa paska postępu

        //wskaźnik do funkcji sterującej paskiem postępu
        public delegate void showProgressDelegate(int percent, int number);

        //funkcja sterująca paskiem postępu
        private void showProgress(int percent, int number)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showProgressDelegate(showProgress), percent, number);
            }
            else
            {
                progressBar1.Value = percent;
                label1.Text = number.ToString();
            }
        }

        public delegate void hideProgressItemsDelegate();
        private void hideProgressItems()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new hideProgressItemsDelegate(hideProgressItems));
            }
            else
            {
                label1.Visible = false;
                label2.Visible = false;
                progressBar1.Visible = false;
                groupBox1.Visible = false;
            }
        }


        //czytanie z bazy danych i zapisywanie do pliku
        private void showReadFromDBProgressItems()
        {
            groupBox1.Visible = true;
            groupBox1.Text = "wczytuję modele z bazy danych";
            label1.Visible = true;
            label1.Text = "";
            label2.Visible = true;
            label2.Text = "";
            progressBar1.Visible = true;
        }


        public delegate void showWriteToFileProgressItemsDelegate();

        private void showWriteToFileProgressItems()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showWriteToFileProgressItemsDelegate(showWriteToFileProgressItems));
            }
            else
            {
                groupBox1.Visible = true;
                groupBox1.Text = "";
                label1.Visible = true;
                label1.Text = "zapisuję modele do pliku na dysku";
                label2.Visible = false;
                label2.Text = "";
                progressBar1.Visible = false;
            }
        }


        //czytanie z pliku i zapisywanie do bazy
        private void showReadFromFileProgressItems()
        {
            groupBox1.Visible = true;
            label1.Visible = true;
            label1.Text = "wczytuję pliki z dysku";
            label2.Visible = false;
            label2.Text = "";
            progressBar1.Visible = false;
        }

        public delegate void showWriteToDBProgressItemsDelegate();
        private void showWriteToDBProgressItems()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showWriteToDBProgressItemsDelegate(showWriteToDBProgressItems));
            }
            else
            {
                groupBox1.Visible = true;
                groupBox1.Text = "zapisuję modele do bazy danych";
                label1.Visible = true;
                label1.Text = "";
                label2.Visible = true;
                label2.Text = "";
                progressBar1.Visible = true;
            }
        }


        #endregion




        #region Region - czytanie modeli z bazy danych i zapisywanie do pliku



        private QueryData readModelsFromDB(string queryFilter = "")
        {
            string query = SqlQueries.getModels + queryFilter;
            return reader.readFromDB(query);
        }



        private void populateModelListview(string selectedDirectoryId)
        {
            modelsListView.Items.Clear();
            string queryFilter = SqlQueries.getModelsByDirectoryFilter + selectedDirectoryId;
            QueryData modelData = readModelsFromDB(queryFilter);
                if (modelData.getHeaders().Count > 0)
                {
                    foreach (string[] model in modelData.getQueryDataAsStrings())
                    {
                        ListViewItem item = new ListViewItem(model);
                        modelsListView.Items.Add(item);
                    }
                modelsListView.Refresh();
                }
                else
                {
                    MyMessageBox.display("Nie można było załadować modeli", MessageBoxType.Error);
                }
        }


        private void readSelectedModelsFromDB()
        {

            selectedModels = new List<Model2D>();
            string selectedModelIds = getSelectedModelIds();;
            
            string queryFilter = SqlQueries.getModelsByIdFilter.Replace("@iDs", selectedModelIds);

            QueryData modelData = readModelsFromDB(queryFilter);
            List<object[]> models = modelData.getQueryData();
            List<string> paramTypes = modelData.getDataTypes();

            int modelsTotal = models.Count;       //do paska postępu

            for (int i=0; i< models.Count; i++)
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

                ModelDirectory modelDir;
                directoryTreeControl1.checkedDirectories.TryGetValue(model2D.directoryId.ToString(), out modelDir);
                model2D.modelDir = modelDir;

                readPowierzchniaFromDB(model2D);
                selectedModels.Add(model2D);

                //do paska postępu
                int percents = (i + 1) * 100 / modelsTotal;
                showProgress(percents, i);
            }

        }

        private string getSelectedModelIds()
        {
            string modelIds = "";
            if (getModelsFromDirectories)
            {
                ModelDirectory modelDir;
                foreach(string dirId in directoryTreeControl1.checkedDirectories.Keys)
                {
                    directoryTreeControl1.checkedDirectories.TryGetValue(dirId, out modelDir);
                    List<object> modelIdsInDirList = reader.readFromDB(SqlQueries.getModels + SqlQueries.getModelsByDirectoryFilter + modelDir.id).getColumnDataAsList(SqlQueries.getModels_idModelIndex);
                    foreach(object modelId in modelIdsInDirList)
                    {
                        modelIds += modelId.ToString() + ",";
                    } 
                }
            }
            else
            {
                foreach (ListViewItem checkedModel in modelsListView.CheckedItems)
                {
                    modelIds += (checkedModel.Text + ",");
                }
            }
            int index = modelIds.LastIndexOf(",");
            return modelIds.Remove(index, 1);
        }

        private void readPowierzchniaFromDB(Model2D model)
        {
            string query = "";
            if (saveModelOption == 0)       
            {
                query = SqlQueries.getPowierzchnieNoBlob + SqlQueries.getPowierzchnie_byIdModelFilter + model.idModel;
            }
            else
            {
                query = SqlQueries.getPowierzchnieFull + SqlQueries.getPowierzchnie_byIdModelFilter + model.idModel;
            }

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
                pow.powDataTable = reader.readFromDBToDataTable(SqlQueries.getPowierzchnieNoBlob + SqlQueries.getPowierzchnie_byIdPowFilter + pow.idPow);
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

            if (saveModelOption == 1)           //tj pełne modele, tylko wtedy wczytuję trójkąty
            {
                ModelTriangles triangles = new ModelTriangles();
                query = SqlQueries.getTriangles + pow.idPow;
                triangles.triangleData = reader.readFromDBToDataTable(query);
                pow.triangles = triangles;                
            }

            ModelLinie breaklines = new ModelLinie();
            query = SqlQueries.getBreaklines + pow.idPow;
            breaklines.breaklineData = reader.readFromDBToDataTable(query);
            pow.breaklines = breaklines;

            ModelGrid grids = new ModelGrid();
            query = SqlQueries.getGrids + pow.idPow;
            grids.gridData = reader.readFromDBToDataTable(query);
            pow.grids = grids;

        }

        private void saveModelsToFile(string fileName)
        {
            ModelBundle mb = new ModelBundle();
            mb.models = selectedModels;
            mb.checkedDirectories = directoryTreeControl1.checkedDirectories;
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

                bformatter.Serialize(stream, mb);
            }
        }

        #endregion



        #region Region - czytanie modeli z pliku binarnego i zapisywanie do bazy danych

        private void readModelsFromFile()
        {
            FileManipulator fm = new FileManipulator();
            string fileName = "modele.bin";
            string filePath = currentPath;
            modelBundle = new ModelBundle();

            try
            {
                if (fm.assertFileExists(filePath + @"\" + fileName))
                {

                    //deserialize
                    using (Stream stream = File.Open(filePath + @"\" + fileName, FileMode.Open))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        modelBundle = (ModelBundle)bformatter.Deserialize(stream);
                    }
                }

            }
            catch (ArgumentException ex)
            {
                MyMessageBox.display(ex.Message, MessageBoxType.Error);
            }
        }



        private void writeModelsToDB(MyEventArgs args)
        {
            List<Model2D> models = modelBundle.models;

            Dictionary<string, ModelDirectory> checkedDirectories = modelBundle.checkedDirectories;

            writer = new DBWriter(dbConnection);
            DBValueTypeConverter converter = new DBValueTypeConverter();
            int maxModelIdInDB = 0;

            string newDirectoryId = "";
            string newIdWlasciciel = args.selectedUserId;

            bool restoreDirectoryTree = args.restoreDirectoryTree;

            string newCzyArch = "0";        //wczytywane modele nie będą archiwalne
            string newIdUzytk = "null";     //wczytywane modele nie będą oznaczone jako wczytane do pamięci

            if (restoreDirectoryTree && checkedDirectories.Count>0)
            {
                writeDirectoryTreeToDB(checkedDirectories, args.selectedDirectoryId);
            }

            int modelsTotal = models.Count-1;       //do paska postępu
            //po kolei wpisuję deklaracje wszystkich modeli, po jednym, do tabeli DefModel2D
            for (int i = 0; i < models.Count; i++)
            {
                //do paska psotępu
                int percents = (i * 100) / modelsTotal;
                showProgress(percents, i);

                Model2D model = models[i];

                string nazwaModel = converter.getConvertedValue(model.nazwaModel, model.nazwaModel_dataType);
                string opisModel = converter.getConvertedValue(model.opisModel, model.opisModel_dataType);
                string dataModel = converter.getConvertedValue(model.dataModel, model.dataModel_dataType);

            if(restoreDirectoryTree && checkedDirectories.Count > 0)
            {
                newDirectoryId = model.modelDir.newId;
            }
            else
            {
                newDirectoryId = args.selectedDirectoryId;
            }

                string query = SqlQueries.insertModel.Replace("@nazwaModel", nazwaModel).Replace("@opisModel", opisModel).Replace("@dataModel", dataModel).Replace("@idUzytk", newIdUzytk).Replace("@czyArch", newCzyArch).Replace("@directoryId", newDirectoryId).Replace("@idWlasciciel", newIdWlasciciel);
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


        private void writeDirectoryTreeToDB(Dictionary<string, ModelDirectory> checkedDirectories, string selectedRootDirId)
        {
            ModelDirectory dir;
            ModelDirectory parentDir;
            int maxDirectoryIdInDB = 0;
            int i = 0;
            foreach(string dirId in checkedDirectories.Keys)
            {
                checkedDirectories.TryGetValue(dirId, out dir);
                string query = SqlQueries.insertDirectory.Replace("@directoryName", dir.name);
                writer.writeToDB(query);

                if (i == 0)         //wpis robię przez insert, baza danych automatycznie nadaje kolejne ID, które teraz odczytuję
                {
                    maxDirectoryIdInDB = getMaxDirectoryIdFromDB();
                }
                else
                {
                    maxDirectoryIdInDB++;       //kolejne modele będą miały kolejne ID, nie muszę za każdym razem czytać tylko inkrementuję
                }
                dir.newId = maxDirectoryIdInDB.ToString();
                i++;
            }

            //id parenta każdego katalogu aktualizuję dopiero wtedy  gdy już dodam wszystkie katalogi do bazy i mają one przypisane nowe id

            foreach(string dirId in checkedDirectories.Keys)
            {
                checkedDirectories.TryGetValue(dirId, out dir);
                string query;
                if (dir.parentId != null && dir.parentId !="")
                {
                    checkedDirectories.TryGetValue(dir.parentId, out parentDir);
                    query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", parentDir.newId) + dir.newId;
                }
                else
                {
                        //jeżeli parent był null, to podpinam ten katalog pod wybrany przez użytkownika
                    query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", selectedRootDirId) + dir.newId;
                }
                writer.writeToDB(query);
            }
        }


        private int getMaxDirectoryIdFromDB()
        {
            string query = SqlQueries.getMaxDirectoryId;
            QueryData res = reader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }

        private void writePowierzchniaToDB(Model2D model)
        {
            uint maxPowId = 0;
            string tableName = dbConnector.getTableNameFromQuery(SqlQueries.getPowierzchnieNoBlob);

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
            if (triangles != null)                              //jest null jeżeli zapisuję same punkty
            {
                if (triangles.setNewIdPow(newIdPow))
                {
                    writer.writeBulkDataToDB(triangles.triangleData, tableName);
                }
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

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Increment(1);
        }

    }
}
