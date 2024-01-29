
using System.IO;
using System;
using System.Text;
using System.Xml.Linq;

namespace DatabaseInterface
{
    /// <summary>
    /// zawiera dane umożliwiające utworzenia połaczenia z bazą danych
    /// </summary>
    public class DBConnectionData
    {
        private readonly char separator = '{';
        private string _connectionString;

        #region właściwości publiczne
        public string serverName { get; set; } = "";
        public string dbName { get; set; } = "";
        public string login { get; set; } = "";
        public string password { get; set; } = "";
        /// <summary>
        /// zwraca true jeżeli zdefiniowany jest serwer i baza danych w przypadku połączenia windows; 
        /// musi mieć też login i hasło w przypadku połączenia sql
        /// </summary>
        public bool isDefined { get => !String.IsNullOrEmpty(_connectionString); }
        public ConnectionTypes connectionType { get; set; } = ConnectionTypes.sqlAuthentication;
        /// <summary>
        /// zwraca connection string w formacie Data Source=xxxx;Initial Catalog=yyyy;User ID=zzzz;Password=pppp
        /// </summary>
        public string connectionString { get { return getConnectionString(); } set { _connectionString = value; } }
        /// <summary>
        /// należy przekazać jeżeli aplikacja otrzymująca te dane musi odczytać jakieś ustawienia w pliku znajdującym się w katalogu określonym przez ścieżkę względną w stosunku do położenia aplikacji; 
        /// np. add-in Microstation musi przeczytać plik znajdujący się w katalogu programu głównego
        /// </summary>
        public string applicationStartupPath { get; set; } = "";
        #endregion

        #region konstruktory
        public DBConnectionData() { }
		/// <summary>
		/// przyjmuje obiekt tej klasy w postaci stringa xml, który musi być utworzony metodą tej klasy getConnectionDataAsXml().ToString();
		/// </summary>
		public DBConnectionData(string connectionDataAsXml)
        {
            setConnectionDataFromXml(connectionDataAsXml);
        }
        /// <summary>
        /// przyjmuje obiekt tej klasy w postaci tabeli byte[]; musi być utworzony metodą tej klasy toByteArray()
        /// </summary>
        public DBConnectionData(byte[] connectionDataAsBytes)
        {
            MemoryStream stream = new MemoryStream(connectionDataAsBytes);

            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadInt32();                 //pierwszy jest długość elementu, nie potrzebuję go przy odtwarzaniu

                this.serverName = readParameter(reader);
                this.dbName = readParameter(reader);
                this.login = readParameter(reader);
                this.password = readParameter(reader);
                this.connectionString = readParameter(reader);
                this.applicationStartupPath = readParameter(reader);
                this.connectionType = (ConnectionTypes)reader.ReadInt32();
            }
            stream.Close();
        }
        #endregion

        #region metody publiczne
        /// <summary>
        /// zwraca obiekt tej klasy w postaci xml; po przekształceniu w stringa metodą ToString() może być przekazany jako parametr key-ina
        /// </summary>
        public XElement getConnectionDataAsXml()
        {
            XElement parameters =
               new XElement("Conn",
                   new XElement("S", parameterValuesToString()));      //zapisuję w ten sposób żeby do minimum ograniczyć długość tworzonego stringa, bo długość stringa który można wysłać z key-inem jest ograniczona
            return parameters;
        }

        /// <summary>
        /// przyjmuje obiekt tej klasy w postaci stringa xml, który musi być utworzony metodą tej klasy getConnectionDataAsXml().ToString()
        /// </summary>
        public void setConnectionDataFromXml(string connectionDataAsXml)
        {
            XElement parameters = XElement.Parse(connectionDataAsXml);
            XElement connectionParams = parameters.Element("Conn");

            try
            {
                string values = connectionParams != null ? connectionParams.Element("S").Value : parameters.Element("S").Value;     //connectionParams jest null gdy ten xml nie jest częścią innego, lecz samodzielnym
                parametersValuesFromString(values);
            }
            catch (NullReferenceException) { }   //te parametry nie zostały przekazane przez użytkownika, nic nie robię
        }

        /// <summary>
        /// zwraca obiekt tej klasy w postaci tabeli byte[]
        /// </summary>
        public byte[] toByteArray()
        {
            byte[] serverNameBytes = Encoding.UTF8.GetBytes(this.serverName);
            byte[] dbNameBytes = Encoding.UTF8.GetBytes(this.dbName);
            byte[] loginBytes = Encoding.UTF8.GetBytes(this.login);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(this.password);
            byte[] dbconnectionStringBytes = Encoding.UTF8.GetBytes(this.connectionString);
            byte[] appStartPathBytes = Encoding.UTF8.GetBytes(this.applicationStartupPath);
            int bufferLength = 8 * sizeof(Int32) + serverNameBytes.Length + dbNameBytes.Length + loginBytes.Length + 
                passwordBytes.Length + dbconnectionStringBytes.Length + appStartPathBytes.Length;
            byte[] buffer = new byte[bufferLength];

            MemoryStream stream = new MemoryStream(buffer);
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(bufferLength);              //int
                writer.Write(serverNameBytes.Length);     //int
                writer.Write(serverNameBytes);
                writer.Write(dbNameBytes.Length);       //int
                writer.Write(dbNameBytes);
                writer.Write(loginBytes.Length);       //int
                writer.Write(loginBytes);
                writer.Write(passwordBytes.Length);       //int
                writer.Write(passwordBytes);
                writer.Write(dbconnectionStringBytes.Length);       //int
                writer.Write(dbconnectionStringBytes);
                writer.Write(appStartPathBytes.Length);       //int
                writer.Write(appStartPathBytes);
                writer.Write((int)this.connectionType);    //int
            }
            return buffer;
        }
        #endregion

        #region metody prywatne
        private string readParameter(BinaryReader reader)
        {
            int bufferLength = reader.ReadInt32();
            byte[] bytes = new byte[bufferLength];
            reader.Read(bytes, 0, bufferLength);
            return Encoding.UTF8.GetString(bytes);
        }

        private void parametersValuesFromString(string values)
        {
            string[] valuesTable = values.Split(this.separator);
            this.serverName = valuesTable[0];
            this.dbName = valuesTable[1];
            this.login = valuesTable[2];
            this.password = valuesTable[3];
            this.applicationStartupPath = valuesTable[4];
            this.connectionType = (ConnectionTypes)Enum.Parse(typeof(ConnectionTypes), valuesTable[5]);
        }

        private string parameterValuesToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.serverName); sb.Append(this.separator);
            sb.Append(this.dbName); sb.Append(this.separator);
            sb.Append(this.login); sb.Append(this.separator);
            sb.Append(this.password); sb.Append(this.separator);
            sb.Append(this.applicationStartupPath); sb.Append(this.separator);
            sb.Append((int)this.connectionType);

			return sb.ToString();
        }

        private string getConnectionString()
		{
            if (!requiredItemsDefined())
                return "";

            if (this.connectionType == ConnectionTypes.sqlAuthentication)
			    return getSqlConnectionString();
            return getTrustedConnectionConnectionString();
		}

		private bool requiredItemsDefined()
		{
			if (connectionType == ConnectionTypes.windowsAuthentication && 
                (String.IsNullOrEmpty(serverName) || String.IsNullOrEmpty(dbName)))
				return false;
			if (connectionType == ConnectionTypes.sqlAuthentication &&
	            (String.IsNullOrEmpty(serverName) || String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password)))
				return false;
            return true;
		}

		private string getSqlConnectionString()
		{
			//przykładowy connection string
			//Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root
			if (!string.IsNullOrEmpty(_connectionString))
				return _connectionString;

			StringBuilder sb = new StringBuilder();
			sb.Append("Data Source=");
			sb.Append(this.serverName);
			sb.Append(";Initial Catalog=");
			sb.Append(this.dbName);
			sb.Append(";User ID=");
			sb.Append(this.login);
			sb.Append(";Password=");
			sb.Append(this.password);
            sb.Append(";MultipleActiveResultSets=true;");

			return sb.ToString();
		}

		private string getTrustedConnectionConnectionString()
		{
			//przykładowy connection string
			//Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;Integrated Security=True;
			if (!string.IsNullOrEmpty(_connectionString))
				return _connectionString;

			StringBuilder sb = new StringBuilder();
			sb.Append("Data Source=");
			sb.Append(this.serverName);
			sb.Append(";Initial Catalog=");
			sb.Append(this.dbName);
			sb.Append(";Integrated Security=True;MultipleActiveResultSets=true;");

			return sb.ToString();
		}
		#endregion
	}
}
