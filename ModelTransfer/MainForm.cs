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
     

        private DBReader dbReader;
        private DBWriter dbWriter;
        private DBConnector dbConnector;
        private SqlConnection dbConnection;

        private bool userClicked = false;
        private bool getModelsFromDirectories = false;           //true jeżeli istnieje gałąź w drzewie katalogów, która ma zaznaczony checkbox


        private List<Model2D> selectedModelDeclarations = new List<Model2D>();       //modele wybrane przez użytkownika do zapisania w pliku
        private Dictionary<object, int> modelIdsAfterRestoreDict = new Dictionary<object, int>();     //kluczem jest Id modelu w starej bazie, wartością Id modelu po przeniesieniu do nowej bazy


        private Thread computationsThread;

        private int saveModelOption = 0;        //ustawiany po naciśnięciu przycisku zapisu do pliku na podstawie wyboru opcji w kombo


        private bool directoriesChecked = false;        //aktualizowana przez zdarzenie zafajkowania checkboxa w drzewie katalogów; 
                                                        //powoduje zatrzymanie wypełniania okna listy modeli po zaznaczeniu katalogu
                                                        //jeżeli nie nałożę tej blokady, występuje konflikt pomiędzy wątkiem wczytującym modele w celu zapisania do pliku a wątkiem wypełniającym okno modeli
                                                        //co wywala błąd DBReadera (połączenie otwarte)


        private string[] modelSaveOptions = { "same punkty", "pełne modele" };      //opcje kombo

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
                directoryTreeControl1.setUpThisForm(dbReader);
            }
        }


        private void resetParameters()
        {
            modelIdsAfterRestoreDict.Clear();
            selectedModelDeclarations.Clear();
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
                dbReader = new DBReader(dbConnection);
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
                this.saveModelOption = saveModelOptionsCombo.SelectedIndex;
                GetFileNameForm fnForm = new GetFileNameForm();
                fnForm.GetFileNameEvent += onGetFileNameForm_ButtonClick;
                fnForm.ShowDialog();
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
            string pomocInfo = "1. Pojedyncze modele można wybierać po zwykłym zaznaczeniu katalogu" +
                            "\r\nPo zaznaczeniu checkboxa przy katalogu kopiowane bedą wszystkie znajdujące się w nim modele;" +
                            "\r\n    niemożliwe jest wtedy wybranie tylko pojedynczych modeli\r\n" +
                            "\r\n2. Anulowanie operacji podczas zapisu modeli do bazy nie wycofuje z bazy modeli już zapisanych";
            MyMessageBox.display(pomocInfo);
        }


        //zatwierdzenie wyboru plików w eksploratorze
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = openFileDialog1.FileName;
            GetDirectoryAndUserForm getDirectoryAndUser = new GetDirectoryAndUserForm(dbReader, fileName);
            getDirectoryAndUser.acceptButtonClickedEvent += onGetUserAndDirectory_ButtonClick;
            getDirectoryAndUser.ShowDialog();
        }


    #endregion




#region Region - obsługa paska postępu

        //wskaźnik do funkcji sterującej paskiem postępu
        public delegate void showProgressDelegate(int number, int modelsTotal, string modelName);

        //funkcja sterująca paskiem postępu
        private void showProgress(int number, int modelsTotal, string modelName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showProgressDelegate(showProgress), number, modelsTotal, modelName);
            }
            else
            {
                int percent = number * 100 / modelsTotal;
                if (percent <= 100)
                {
                    progressBar1.Value = percent;
                }
                else
                {
                    progressBar1.Value = 100;
                }
                numberLabel.Text = number.ToString() + " / " + modelsTotal.ToString();
                modelNameLabel.Text = modelName;
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
                numberLabel.Visible = false;
                infoLabel.Visible = false;
                modelNameLabel.Visible = false;
                progressBar1.Visible = false;
                progressAreaPanel.Visible = false;
            }
        }



        //elementy paska postępu
        private void showProgressItems(string info)
        {
            progressAreaPanel.Visible = true;
            numberLabel.Visible = true;
            numberLabel.Text = "";
            modelNameLabel.Visible = true;
            modelNameLabel.Text = "";
            infoLabel.Visible = true;
            infoLabel.Text = info;
            progressBar1.Visible = true;
            progressBar1.Value = 0;
        }



        private void AbortButton_Click(object sender, EventArgs e)
        {
            computationsThread.Abort();
            hideProgressItems();
            resetParameters();
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


        //zdarzenie w formatce GetFileNameForm, tu uruchamiam osobny wątek
        private void onGetFileNameForm_ButtonClick(object sender, MyEventArgs args)
        {
            showProgressItems("zapisuję do pliku");
            string selectedModelIds = getSelectedModelIds();        //zanim uruchomię wątek, bo czytam z listy w tej formatce
            //uruchamiam wątek po wyświetleniu elementów paska postępu
            computationsThread = new Thread(() => saveModelsFromDbToFile(args.fileName, selectedModelIds));
            computationsThread.Start();
        }



        //metoda uruchamiana w osobnym wątku
        private void saveModelsFromDbToFile(string fileName, string selectedModelIds)
        {
            readSelectedModelDeclarationsFromDB(selectedModelIds);

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
            return dbReader.readFromDB(query);
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


        //czyta tylko z tablicy DefModel2D, dane zostaną dodane później do każdego modelu osobno i będą dopisywane do pliku osobno
        //dane z tej metody będą użyte do odtworzenia struktury, do której następnie będą iteracyjnie dopisane dane 
        private void readSelectedModelDeclarationsFromDB(string selectedModelIds)
        {

            //selectedModelDeclarations = new List<Model2D>();
            //string selectedModelIds = getSelectedModelIds();
            
            string queryFilter = SqlQueries.getModelsByIdFilter.Replace("@iDs", selectedModelIds);

            QueryData modelData = readModelsFromDB(queryFilter);
            List<object[]> models = modelData.getQueryData();
            List<string> paramTypes = modelData.getDataTypes();

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

                selectedModelDeclarations.Add(model2D);

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
                    List<object> modelIdsInDirList = dbReader.readFromDB(SqlQueries.getModels + SqlQueries.getModelsByDirectoryFilter + modelDir.id).getColumnDataAsList(SqlQueries.getModels_idModelIndex);
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

            //najpierw potrzebuję jedynie utworzyć obiekty ModelPowierzchnia, potrzebuję do tego tylko niektóre dane
            query = SqlQueries.getPowierzchnieNoBlob + SqlQueries.getPowierzchnie_byIdModelFilter + model.idModel;

            QueryData powierzchnieData = dbReader.readFromDB(query);
            List<string> paramTypes = powierzchnieData.getDataTypes();

            for(int i=0; i < powierzchnieData.getDataRowsNumber(); i++)
            {
                ModelPowierzchnia pow = new ModelPowierzchnia();

                pow.idPow = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_idPowIndex];

                pow.idModel = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_idModelIndex];
                pow.idModel_dataType = paramTypes[SqlQueries.getPowierzchnie_idModelIndex];

                pow.nazwaPow = powierzchnieData.getQueryData()[i][SqlQueries.getPowierzchnie_nazwaPowIndex];
                pow.nazwaPow_dataType = paramTypes[SqlQueries.getPowierzchnie_nazwaPowIndex];

                pow.powierzchniaData = powierzchnieData.getQueryData()[i];
                pow.columnHeaders = powierzchnieData.getHeaders();
                pow.columnDataTypes = powierzchnieData.getDataTypes();

                //teraz w zależności od opcji czytam pełne dane powierzchni i zapisuję do DataTable
                if (saveModelOption == 0)
                {
                    pow.powDataTable = dbReader.readFromDBToDataTable(SqlQueries.getPowierzchnieNoBlob + SqlQueries.getPowierzchnie_byIdPowFilter + pow.idPow);
                }
                else
                {
                    pow.powDataTable = dbReader.readFromDBToDataTable(SqlQueries.getPowierzchnieFull + SqlQueries.getPowierzchnie_byIdPowFilter + pow.idPow);
                }
                readPowierzchniaDataFromDB(pow);
                model.addPowierzchnia(pow);
            }
        }

        private void readPowierzchniaDataFromDB(ModelPowierzchnia pow)
        {
            string query = "";

            ModelPunkty points = new ModelPunkty();
            query = SqlQueries.getPoints + pow.idPow;
            points.pointData = dbReader.readFromDBToDataTable(query);
            pow.points = points;

            if (saveModelOption == 1)           //tj pełne modele, tylko wtedy wczytuję trójkąty
            {
                ModelTriangles triangles = new ModelTriangles();
                query = SqlQueries.getTriangles + pow.idPow;
                triangles.triangleData = dbReader.readFromDBToDataTable(query);
                pow.triangles = triangles;                
            }

            ModelLinie breaklines = new ModelLinie();
            query = SqlQueries.getBreaklines + pow.idPow;
            breaklines.breaklineData = dbReader.readFromDBToDataTable(query);
            pow.breaklines = breaklines;

            ModelGrid grids = new ModelGrid();
            query = SqlQueries.getGrids + pow.idPow;
            grids.gridData = dbReader.readFromDBToDataTable(query);
            pow.grids = grids;

        }


        //w pliku zapisane są ciągiem pary: deklaracja długości danych(4 bajty) + buffer zawierający dane
        private void writeModelsToFile(string fileName)
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

            initializeFile(serializationFile);

            int modelsTotal = selectedModelDeclarations.Count;       //do paska postępu
            int i = 0;
            foreach (Model2D model in selectedModelDeclarations)
            {
                showProgress(i + 1, modelsTotal, model.nazwaModel.ToString());

                readPowierzchniaFromDB(model);
                addModelToFile(serializationFile, model);
                //do paska postępu
                i++;
            }
        }



        //tworzy plik a następnie zapisuje strukturę danych, tj deklaracje modeli i słownik katalogów
        private void initializeFile(string serializationFile)
        {
            ModelBundle modelBundle = new ModelBundle();
            modelBundle.models = selectedModelDeclarations;
            modelBundle.checkedDirectories = directoryTreeControl1.checkedDirectories;
            
            try
            {
                //serialize
                using (FileStream stream = new FileStream(serializationFile, FileMode.Create))
                {

                    MemoryStream originalMemoryStream = new MemoryStream();
                    MemoryStream compressedMemoryStream = new MemoryStream();

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(originalMemoryStream, modelBundle);

                    using(GZipStream gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                        originalMemoryStream.WriteTo(gzipStream);
                    }

                    byte[] buffer = compressedMemoryStream.ToArray();
                    int bufferSize = buffer.Length;
                    compressedMemoryStream.Close();
                    originalMemoryStream.Close();

                    using (BinaryWriter binWriter = new BinaryWriter(stream))
                    {
                        binWriter.Write(bufferSize);            //pierwsze 4 bajty
                        binWriter.Write(buffer);
                    }
                }
            }
            catch (OutOfMemoryException exc)
            {
                MyMessageBox.display(exc.Message + "\r\nwriteModelsToFile");
            }
        }


        private void addModelToFile(string serializationFile, Model2D model)
        {
            ModelBundle modelDataBundle = new ModelBundle();
            modelDataBundle.addModel(model);
            try
            {
                //serialize
                using (FileStream stream = new FileStream(serializationFile, FileMode.Open))
                {

                    MemoryStream originalMemoryStream = new MemoryStream();
                    MemoryStream compressedMemoryStream = new MemoryStream();

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(originalMemoryStream, modelDataBundle);

                    using (GZipStream gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                        originalMemoryStream.WriteTo(gzipStream);
                    }

                    byte[] buffer = compressedMemoryStream.ToArray();
                    int bufferSize = buffer.Length;
                    compressedMemoryStream.Close();
                    originalMemoryStream.Close();

                    using (BinaryWriter binWriter = new BinaryWriter(stream))
                    {
                        stream.Position = stream.Length;
                        binWriter.Write(bufferSize);            //pierwsze 4 bajty
                        binWriter.Write(buffer);
                    }
                }
            }
            catch (OutOfMemoryException exc)
            {
                MyMessageBox.display(exc.Message + "\r\nwriteModelsToFile");
            }
        }


        #endregion



        #region Region - czytanie modeli z pliku binarnego i zapisywanie do bazy danych


        //zdarzenie wywołane w formatce GetDirectoryAndUserForm
        private void onGetUserAndDirectory_ButtonClick(object sender, MyEventArgs args)
        {
            showProgressItems("zapisuję do bazy danych");

            computationsThread = new Thread(() => writeModelsFromFileToDB(args));
            computationsThread.Start();

        }


        //metoda uruchamiana w osobnym wątku
        private void writeModelsFromFileToDB(MyEventArgs args)
        {
            int streamPosition = 0;
            int dataPacketNumber = 0;
            int totalNumberOfPackets = 0;                   //do paska postępu
            ModelBundle modelDataBundle = new ModelBundle();
            try
            {
                do {
                    //deserialize
                    using (FileStream stream = new FileStream(args.fileName, FileMode.Open))
                    {

                        int dataPacketLength = 0;
                        byte[] buffer;
                        using (BinaryReader bReader = new BinaryReader(stream))
                        {
                            stream.Position = streamPosition;
                            dataPacketLength = bReader.ReadInt32();        //najpierw wpis 4 bajty określa długość paczki, która po nim następuje
                            buffer = new byte[dataPacketLength];
                            bReader.Read(buffer, 0, dataPacketLength);
                        }
                        MemoryStream compressedStream = new MemoryStream(buffer);
                        MemoryStream decompressedStream = new MemoryStream();

                        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(decompressedStream);
                        }
                        decompressedStream.Position = 0;
                        BinaryFormatter bformatter = new BinaryFormatter();
                        modelDataBundle = (ModelBundle)bformatter.Deserialize(decompressedStream);

                        if (dataPacketNumber == 0)
                        {
                            totalNumberOfPackets = modelDataBundle.models.Count;
                            if (modelDataBundle.checkedDirectories.Count == 0 && args.selectedDirectoryId == "")
                            {
                                MyMessageBox.display("Nie można odtworzyć modeli gdyż nie wybrano katalogu docelowego \r\na modele w pliku źródłowym wczytane zostały bez katalogu.\r\nSpróbuj wczytać ponownie wybierając katalog docelowy", MessageBoxType.Error);
                                break;
                            }
                            else
                            {
                                writeModelDeclarationsToDB(args, modelDataBundle);
                            }
                        }
                        else
                        {
                            showProgress(dataPacketNumber, totalNumberOfPackets, modelDataBundle.models[0].nazwaModel.ToString());       //pasek postępu
                            writePowierzchniaToDB(modelDataBundle);
                        }
                        compressedStream.Close();
                        decompressedStream.Close();
                        streamPosition += dataPacketLength + 4;     //tj. przesuwam o nagłówek (int czyli 4 bajty) i długość właśnie przeczytanego pakietu

                        dataPacketNumber++;
                    }
                }
                while (dataPacketNumber <= totalNumberOfPackets);

                if (dataPacketNumber >= totalNumberOfPackets)       //komunikat o sukcesie tylko wtedy, gdy cała pętla przeszła
                {
                    MyMessageBox.displayAndClose("Modele wczytane", 1);
                }

                hideProgressItems();            //delegat
                refreshDirectoryTree();      //delegat          
            }
            catch (ArgumentException ex)
            {
                MyMessageBox.display(ex.Message, MessageBoxType.Error);
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
                directoryTreeControl1.setUpThisForm(dbReader);
            }
        }



        private void writeModelDeclarationsToDB(MyEventArgs args, ModelBundle modelDeclarationsFromFileBundle)
        {
            List<Model2D> models = modelDeclarationsFromFileBundle.models;
            Dictionary<string, ModelDirectory> checkedDirectories = modelDeclarationsFromFileBundle.checkedDirectories;

            int maxModelIdInDB = 0;
            string newDirectoryId = "";
            string newIdWlasciciel = args.selectedUserId;
            bool restoreDirectoryTree = args.restoreDirectoryTree;

            string newCzyArch = "0";        //wczytywane modele nie będą archiwalne
            string newIdUzytk = "null";     //wczytywane modele nie będą oznaczone jako wczytane do pamięci

            dbWriter = new DBWriter(dbConnection);
            DBValueTypeConverter converter = new DBValueTypeConverter();

            if (restoreDirectoryTree && checkedDirectories.Count>0)
            {
                writeDirectoryTreeToDB(checkedDirectories, args.selectedDirectoryId);
            }


            //po kolei wpisuję deklaracje wszystkich modeli, po jednym, do tabeli DefModel2D
            int loopNumber = 0;
            foreach (Model2D model in models)
            {
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
                dbWriter.writeToDB(query);

                if (loopNumber == 0)         //wpis modelu robię przez insert, baza danych automatycznie nadaje mu ID, które teraz odczytuję
                {
                    maxModelIdInDB = getMaxModelIdFromDB();
                }
                else
                {
                    maxModelIdInDB++;       //kolejne modele będą miały kolejne ID, nie muszę za każdym razem czytać tylko inkrementuję
                }

                //w każdym modelu w powierzchniach zmieniam Id modelu na nowy, w nowej bazie danych
                model.IdModelAfterRestore = maxModelIdInDB;
                modelIdsAfterRestoreDict.Add(model.idModel, model.IdModelAfterRestore);
                loopNumber++;
            }
        }


        private void writeDirectoryTreeToDB(Dictionary<string, ModelDirectory> checkedDirectories, string selectedRootDirId)
        {
            ModelDirectory dir;
            ModelDirectory parentDir = null;
            int maxDirectoryIdInDB = 0;
            int i = 0;
            string query = "";
            if (selectedRootDirId == "") selectedRootDirId = "null";      //gdy użytkownik nie wybierze folderu docelowego, przypinam gałąź do pnia

            foreach (string dirId in checkedDirectories.Keys)
            {
                checkedDirectories.TryGetValue(dirId, out dir);
                query = SqlQueries.insertDirectory.Replace("@directoryName", dir.name);
                dbWriter.writeToDB(query);

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

            // aktualizuję id parenta każdego katalogu - dopiero teraz,  gdy już dodałem wszystkie katalogi do bazy i mają one przypisane nowe id
            //na tym etapie każdy katalog ma parenta null
            foreach(string dirId in checkedDirectories.Keys)
            {
                checkedDirectories.TryGetValue(dirId, out dir);
                if (dir.parentId !="")                       //tzn gdy gałąź miała parenta w oryginalnej bazie danych
                {
                    checkedDirectories.TryGetValue(dir.parentId, out parentDir);

                    if (parentDir != null)      // parentDir==null występuje to w sytuacji wycięcia kawałka gałęzi; po wycięciu najwyższa gałąź miałaa parenta w oryginalnej bazie danych, jednak nie został on zapisany do pliku
                                                //tak więc w tej bazie gałąź musi zostać przypięta do pnia
                    {
                        query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", parentDir.newId) + dir.newId;
                    }
                    else              //gdy wytnę kawałek gałęzi, to muszę ją podpiąć pod nowego parenta, którym będzie katalog wybrany przez użytkownika
                    {           
                        query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", selectedRootDirId) + dir.newId;
                    }
                }
                else            //tzn gdy odetnie się gałąź od pnia, tzn nie ma parenta
                {
                        //jeżeli parent Id był null, tzn gałąź została odcięta od pnia, to podpinam ten katalog pod wybrany przez użytkownika
                    query = SqlQueries.updateDirectoryParentId.Replace("@newParentId", selectedRootDirId) + dir.newId;
                }
                dbWriter.writeToDB(query);
            }
        }


        private int getMaxDirectoryIdFromDB()
        {
            string query = SqlQueries.getMaxDirectoryId;
            QueryData res = dbReader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }

        private void writePowierzchniaToDB(ModelBundle modelDataBundle)
        {
            Model2D model = modelDataBundle.models[0];              //każda paczka zawiera tylko jeden model
            int newModelId = 0;
            modelIdsAfterRestoreDict.TryGetValue(model.idModel, out newModelId);
            model.setNewModelIdInPowierzchnia(newModelId);

            uint maxPowId = 0;
            string tableName = dbConnector.getTableNameFromQuery(SqlQueries.getPowierzchnieNoBlob);

            for(int i=0; i < model.powierzchnieList.Count; i++)
            {
                ModelPowierzchnia pow = model.powierzchnieList[i];

                dbWriter.writeBulkDataToDB(pow.powDataTable, tableName);

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
            QueryData res = dbReader.readFromDB(query);
            return int.Parse(res.getQueryData()[0][0].ToString());
        }


        private void writePowierzchniaDataToDB(ModelPowierzchnia pow)
        {
            string tableName = "";
            uint newIdPow = uint.Parse(pow.idPow.ToString());

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getPoints);
            ModelPunkty points = pow.points;
            if (points.setNewIdPow(newIdPow))
            {
                dbWriter.writeBulkDataToDB(points.pointData, tableName);
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getTriangles);
            ModelTriangles triangles = pow.triangles;
            if (triangles != null)                              //jest null jeżeli zapisuję same punkty
            {
                if (triangles.setNewIdPow(newIdPow))
                {
                    dbWriter.writeBulkDataToDB(triangles.triangleData, tableName);
                }
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getGrids);
            ModelGrid grids = pow.grids;
            if (grids.setNewIdPow(newIdPow))
            {
                dbWriter.writeBulkDataToDB(grids.gridData, tableName);
            }

            tableName = dbConnector.getTableNameFromQuery(SqlQueries.getBreaklines);
            ModelLinie breaklines = pow.breaklines;
            if (breaklines.setNewIdPow(newIdPow))
            {
                dbWriter.writeBulkDataToDB(breaklines.breaklineData, tableName);
            }

        }



        private uint getMaxPowierzchniaIdFromDB()
        {
            string query = SqlQueries.getMaxPowierzchniaId;
            QueryData res = dbReader.readFromDB(query);
            return uint.Parse(res.getQueryData()[0][0].ToString());
        }



        #endregion

    }
}
