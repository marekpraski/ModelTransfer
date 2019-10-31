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
using System.Timers;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

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



        private ModelBundle modelBundle;            //modele wczytane z pliku
        private List<Model2D> selectedModels;       //modele wybrane przez użytkownika do zapisania w pliku

        string timeLog = "";

        private Thread computationsThread;

        private int saveModelOption = 0;        //ustawiany po naciśnięciu przycisku zapisu do pliku na podstawie wyboru opcji w kombo

        private string[] fileNames;             //pliki .bin z modelami wybrane przez użytkownika

        private bool directoriesChecked = false;        //aktualizowana przez zdarzenie zafajkowania checkboxa w drzewie katalogów; 
                                                        //powoduje zatrzymanie wypełniania okna listy modeli po zaznaczeniu katalogu
                                                        //jeżeli nie nałożę tej blokady, występuje konflikt pomiędzy wątkiem wczytującym modele w celu zapisania do pliku a wątkiem wypełniającym okno modeli
                                                        //co wywala błąd BDReadera (połączenie otwarte)

        private int pointNumberWarningLevel = 10000;         //wpisana tu liczba jest w tysiącach, bo rzeczywistą warstość dzielę na 1000 po odczycie; sumaryczna liczba punktów dla których wyświetla się ostrzeżenie, że plik może być bardzo duży
        private int numberOfTriangles = 0;                     //liczba trójkątów do zapisu z bazy danych do pliku; do sterowania paskiem postępu oraz ostrzeżeniem o dużej ilości danych do zapisu

        private int fileSize = 0;                       //wielkość pliku do odczytu
        int timerTickNumber = 0;

        #endregion



#region Region - konstruktor, ustawienia tej formatki

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
                saveModelOptionsCombo.Items.AddRange(modelSaveOptions);
#if DEBUG
                saveModelOptionsCombo.SelectedIndex = 0;                    //domyślna opcja kombo
#else
                saveModelOptionsCombo.SelectedIndex = 1;
#endif

                //ustawienia eksploratora plików
                openFileDialog1.Filter = "Pliki modeli (*.bin)|*.bin";
                openFileDialog1.InitialDirectory = currentPath;

                //ustawienia kontrolki drzewa katalogów
                directoryTreeControl1.directorySelectedEvent += onTreeviewDirectorySelected;
                directoryTreeControl1.directoryCheckedEvent += onTreeviewDirectoryChecked;

                directoryTreeControl1.turnTreeviewCheckboxesOn();
                directoryTreeControl1.showUncheckAllCheckboxesLabel();
                directoryTreeControl1.setUpThisForm(reader);
            }
        }


        private void resetParameters()
        {
            this.fileSize = 0;
            this.fileNames = null;
            this.timerTickNumber = 0;
            this.numberOfTriangles = 0;
        }


        #endregion



#region Region - start programu, utworzenie połączenia z bazą danych, utworzenie czytnika


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



        private void SaveToFileButton_Click(object sender, EventArgs e)
        {
            resetParameters();

            if (modelsListView.CheckedItems.Count > 0 || getModelsFromDirectories)
            {
                if (!cancelOperation())
                {                    
                    this.saveModelOption = saveModelOptionsCombo.SelectedIndex;
                    GetFileNameForm fnForm = new GetFileNameForm();
                    fnForm.GetFileNameEvent += onGetFileNameForm_ButtonClick;
                    fnForm.ShowDialog();
                }
            }
            else
            {
                MyMessageBox.display("Brak zaznaczonych modeli");
            }
        }


        private void SaveToDBButton_Click(object sender, EventArgs e)
        {
            resetParameters();
            openFileDialog1.ShowDialog();            
        }


        private void HelpButton_Click(object sender, EventArgs e)
        {
            string pomocInfo = "Pojedyncze modele można wybierać po zwykłym zaznaczeniu katalogu" +
                            "\r\nPo zaznaczeniu checkboxa przy katalogu kopiowane bedą wszystkie znajdujące się w nim modele;" + 
                            "\r\n    niemożliwe jest wtedy wybranie tylko pojedynczych modeli";
            MyMessageBox.display(pomocInfo);
        }


        //zatwierdzenie wyboru plików w eksploratorze
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileNames = openFileDialog1.FileNames;
            GetDirectoryAndUserForm getDirectoryAndUser = new GetDirectoryAndUserForm(reader);
            getDirectoryAndUser.acceptButtonClickedEvent += onGetUserAndDirectory_ButtonClick;
            getDirectoryAndUser.ShowDialog();
        }

        //
        //metody wspomagające powyższe zdarzenia
        //

        //sprawdzam sumaryczną liczbę punktów w modelach, żeby ostrzec przed możliwością dużego pliku
        private bool cancelOperation()
        {
            
            getNumberOfTriangles();
            MyMessageBoxResults result = MyMessageBoxResults.Yes;

            switch (saveModelOption)
            {
                case 0:
                    result = generateWarning(numberOfTriangles/4);      //liczbę trójkątów mam zapisaną w jednym polu, dla szybkości wykorzystuję to i nie zliczam; stąd jeżeli chcę zapisać tylko punkty to dzielę na 4
                                                                        //bo same punkty stanowią 1/4 ilości punkty + trójkąty
                    break;
                case 1:
                    result = generateWarning(numberOfTriangles);
                    break;
            }

            switch (result)
            {
                case MyMessageBoxResults.Yes:
                    return false;
                case MyMessageBoxResults.No:
                    return true;
            }
            return false;
        }

        private void getNumberOfTriangles()
        {
            string modelIds = getSelectedModelIds();
            string query = SqlQueries.getSumTriangles.Replace("@modelIds", modelIds);
            this.numberOfTriangles = int.Parse(reader.readFromDB(query).getQueryData()[0][0].ToString())/1000;     //wynikiem będzie jedna pozycja, stąd [0][0]
        }

        private MyMessageBoxResults generateWarning(int numberOfPoints)
        {

            if (numberOfPoints > pointNumberWarningLevel)
            {
               return MyMessageBox.display("Modele do zapisania zawierają łącznie " + numberOfPoints +" punktów.\r\nTworzenie pliku może potrwać kilka minut. Czy kontynuować?", MessageBoxType.YesNo);
            }
            return MyMessageBoxResults.Yes;
            
        }


    #endregion




#region Region - obsługa paska postępu

        //wskaźnik do funkcji sterującej paskiem postępu
        public delegate void showProgressDelegate(int number, int modelsTotal);

        //funkcja sterująca paskiem postępu
        private void showProgress(int number, int modelsTotal)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showProgressDelegate(showProgress), number, modelsTotal);
            }
            else
            {
                int percent = number * 100 / modelsTotal;
                progressBar1.Value = percent;
                label1.Text = number.ToString() + " / " + modelsTotal.ToString();
            }
        }


        public delegate void showProgressByStepDelegate(int progressBarValue);

        //funkcja sterująca paskiem postępu
        private void showProgress(int progressBarValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showProgressByStepDelegate(showProgress));
            }
            else
            {
                if (progressBar1.Value < 90 && progressBarValue<=100)    //postęp dobrany jest doświadczalnie, więc gdy transfer okaże się mniejszy niż zakladany i pasek za szybko dojdzie do końca to niech czeka na 90%
                                                                        //gdy powyżej 100 to wywala się
                {
                    progressBar1.Value = progressBarValue;
                }
            }
        }

        //timer używam do obsługi paska postępu podczas zapisywania modeli do pliku
        private void saveToFileTimer_Tick(object sender, ElapsedEventArgs e)
        {
            timerTickNumber++;
            decimal progressStep = 100*180 / (decimal)numberOfTriangles;                     //dobrane doświadczalnie, 200 oznacza szybkość zapisu do pliku - 200tys pkt/sekundę; 100 bo przeliczam na %
            int progressBarValue = Convert.ToInt32(Math.Round((timerTickNumber * progressStep),0));

            showProgress(progressBarValue);
        }


        //timer używam do obsługi paska postępu podczas czytania modeli z pliku
        private void ReadFromFileTimer_Tick(object sender, ElapsedEventArgs e)
        {
            timerTickNumber++;
            decimal progressStep = 100 * 5000 / (decimal)fileSize;                     //dobrane doświadczalnie, 10000 oznacza szybkość odczytu z pliku - 10 MB/sekundę; 100 bo przeliczam na %
            int progressBarValue = Convert.ToInt32(Math.Round((timerTickNumber * progressStep), 0));

            showProgress(progressBarValue);
        }


        private void AbortButton_Click(object sender, EventArgs e)
        {
            computationsThread.Abort();
            hideProgressItems();
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
                progressAreaPanel.Visible = false;
            }
        }


        //czytanie z bazy danych i zapisywanie do pliku
        private void showReadFromDBProgressItems()
        {
            progressAreaPanel.Visible = true;
            label1.Visible = true;
            label1.Text = "";
            label2.Visible = true;
            label2.Text = "wczytuję modele z bazy danych";
            progressBar1.Visible = true;
            progressBar1.Value = 0;
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
                progressAreaPanel.Visible = true;
                label1.Visible = true;
                label1.Text = "";
                label2.Visible = true;
                label2.Text = "zapisuję modele do pliku na dysku";
                progressBar1.Visible = true;
                progressBar1.Value = 0;
            }
        }


        //czytanie z pliku i zapisywanie do bazy
        private void showReadFromFileProgressItems()
        {
            progressAreaPanel.Visible = true;
            label1.Visible = true;
            label1.Text = "";
            label2.Visible = true;
            label2.Text = "wczytuję plik z dysku";
            progressBar1.Visible = true;
            progressBar1.Value = 0;
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
                progressAreaPanel.Visible = true;
                label1.Visible = true;
                label1.Text = "";
                label2.Visible = true;
                label2.Text = "zapisuję modele do bazy danych";
                progressBar1.Visible = true;
            }
        }


#endregion




#region Region - czytanie modeli z bazy danych i zapisywanie do pliku

        private void onTreeviewDirectorySelected(object sender, MyEventArgs args)
        {
            if (!directoriesChecked)
            {
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


        //zdarzenie w formatce GetFileNameForm
        private void onGetFileNameForm_ButtonClick(object sender, MyEventArgs args)
        {
            showReadFromDBProgressItems();

            //uruchamiam wątek po wyświetleniu elementów paska postępu
            computationsThread = new Thread(() => saveModelsFromDbToFile(args.fileName));
            computationsThread.Start();
        }



        //metoda uruchamiana w osobnym wątku
        private void saveModelsFromDbToFile(string fileName)
        {

            readSelectedModelsFromDB();

            showWriteToFileProgressItems();     //delegate

            // 
            // uruchamiam timer1 potrzebny do paska postępu; pasek popychany zdarzeniem Timer1_Tick
            // 
            this.saveToFileTimer.Elapsed += saveToFileTimer_Tick;
            this.saveToFileTimer.Interval = 1000;
            saveToFileTimer.Enabled = true;
            saveToFileTimer.Start();

            writeModelsToFile(fileName);

            hideProgressItems();                //delegate
            disableSaveToFileButton();          //delegate
            MyMessageBox.displayAndClose("Modele zapisane do pliku",1);
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
                showProgress(i+1, modelsTotal);
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

        private void writeModelsToFile(string fileName)
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
                using (var compressionStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(compressionStream, mb);
                }
            }
            //timer jest potrzebny do paska postępu, uruchomiony został w metodzie "saveModelsFromDbToFile", teraz po zapisaniu pliku zamykam go
            saveToFileTimer.Stop();
            saveToFileTimer.Enabled = false;
        }

#endregion



#region Region - czytanie modeli z pliku binarnego i zapisywanie do bazy danych


        //zdarzenie wywołane w formatce GetDirectoryAndUserForm
        private void onGetUserAndDirectory_ButtonClick(object sender, MyEventArgs args)
        {
            showReadFromFileProgressItems();

            computationsThread = new Thread(() => writeModelsFromFileToDB(args));
            computationsThread.Start();

        }


        //metoda uruchamiana w osobnym wątku
        private void writeModelsFromFileToDB(MyEventArgs args)
        {
            // 
            // readFromFileTimer
            // 
            readFromFileTimer.Elapsed += ReadFromFileTimer_Tick;
            readFromFileTimer.Interval = 1000;
            readFromFileTimer.Enabled = true;
            readFromFileTimer.Start();

            readModelsFromFile();

            if (modelBundle != null)
            {
                showWriteToDBProgressItems();       //delegat
                writeModelsToDB(args);
                MyMessageBox.displayAndClose("Modele wczytane", 1);
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



        private void readModelsFromFile()
        {

            FileManipulator fm = new FileManipulator();
            if (this.fileNames.Length > 0)
            {
                foreach (string fileName in this.fileNames)         //teroretycznie przygotowane do czytania z wielu plików, ale zapisywanie tego później do bazy może być problematyczne
                                                                    //bez każdorazowego wskazania, gdzie katalogi mają być zakotwiczone
                {

                    modelBundle = new ModelBundle();

                    try
                    {
                        if (fm.assertFileExists(fileName))
                        {
                            FileInfo info = new FileInfo(fileName);
                            this.fileSize = Convert.ToInt32(info.Length/1000);
                            //deserialize
                            using (Stream stream = File.Open(fileName, FileMode.Open))
                            {
                                using (GZipStream decompressedStream = new GZipStream(stream, CompressionMode.Decompress))
                                {
                                    BinaryFormatter bformatter = new BinaryFormatter();
                                    modelBundle = (ModelBundle)bformatter.Deserialize(decompressedStream);
                                }
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        MyMessageBox.display(ex.Message, MessageBoxType.Error);
                    }
                }
            }
            readFromFileTimer.Stop();
            readFromFileTimer.Enabled = false;
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

            int modelsTotal = models.Count;       //do paska postępu
            //po kolei wpisuję deklaracje wszystkich modeli, po jednym, do tabeli DefModel2D
            for (int i = 0; i < models.Count; i++)
            {
                Model2D model = models[i];

                string nazwaModel = converter.getConvertedValue(model.nazwaModel, model.nazwaModel_dataType);
                string opisModel = converter.getConvertedValue(model.opisModel, model.opisModel_dataType);
                string dataModel = converter.getConvertedValue(model.dataModel, model.dataModel_dataType);

                //do paska postępu
                showProgress(i+1, modelsTotal);

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

        }


        private void writeDirectoryTreeToDB(Dictionary<string, ModelDirectory> checkedDirectories, string selectedRootDirId)
        {
            ModelDirectory dir;
            ModelDirectory parentDir = null;
            int maxDirectoryIdInDB = 0;
            int i = 0;
            string query = "";
            foreach (string dirId in checkedDirectories.Keys)
            {
                checkedDirectories.TryGetValue(dirId, out dir);
                query = SqlQueries.insertDirectory.Replace("@directoryName", dir.name);
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
                if (dir.parentId != null && dir.parentId !="")      //występuje gdy wytnie się gałąź od pnia, tzn nie ma parenta
                {
                    if (parentDir != null)      //występuje to w sytuacji wycięcia kawałka gałęzi; po wycięciu najwyższa gałąź ma parenta, który jednak nie został zapisany do pliku
                    {
                        checkedDirectories.TryGetValue(dir.parentId, out parentDir);
                        query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", parentDir.newId) + dir.newId;
                    }
                    else
                    {           //gdy wytnę kawałek gałęzi, to muszę ją podpiąć pod nowego parenta, którym będzie katalog wybrany przez użytkownika
                        query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", selectedRootDirId) + dir.newId;
                    }
                }
                else
                {
                        //jeżeli parent Id był null, tzn gałąź została odcięta od pnia, to podpinam ten katalog pod wybrany przez użytkownika
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

    }
}
