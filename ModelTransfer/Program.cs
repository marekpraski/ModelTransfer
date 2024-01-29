using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using DatabaseInterface;
using UtilityTools;
using System.Data.SqlClient;

namespace ModelTransfer
{
	static class Loader
	{
		public static string mainPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.ToString();
		private static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();  //kluczem jest pełna ścieżka do dll-a; zapobiega kilkukrotnemu ładowaniu tego samego dll-a
		/// <summary>
		/// Główny punkt wejścia dla aplikacji.
		///https://stackoverflow.com/questions/6972050/net-reference-dll-from-other-location
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
#if DEBUG
#else
            AppDomain.CurrentDomain.AssemblyResolve += findAssembly;
#endif

			Program.Go(args);
		}

		static Assembly findAssembly(object sender, ResolveEventArgs args)
		{
			string simpleName = new AssemblyName(args.Name).Name;
			string path = Loader.mainPath + @"..\lib\" + simpleName + ".dll";

			if (path.Contains(".resources"))
			{
				return null;
			}
			else if (!File.Exists(path))
			{
				MessageBox.Show("Nie znaleziono biblioteki " + path);
				return null;
			}
			return getAssembly(path);
		}

		private static Assembly getAssembly(string path)
		{
			if (Loader.loadedAssemblies.ContainsKey(path))
				return Loader.loadedAssemblies[path];
			else
			{
				Assembly a = Assembly.LoadFrom(path);
				Loader.loadedAssemblies.Add(path, a);
				return a;
			}
		}
	}
	static class Program
    {
		public static string userName;
		public static string userPassword;
		public static string mainPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.ToString();
		public static SqlConnection dbConnection { get; private set; }

		/// <summary>
		/// Główny punkt wejścia dla aplikacji.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void Go(string[] args)
		{
			if (!assignProgramParameters(args))
				return;

			createSqlConnection(args);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				Application.Run(new MainForm());
			}
			catch (Exception e)
			{
				MessageBoxError.ShowBox(e);
			}
		}

		private static bool assignProgramParameters(string[] args)
		{
			if (args.Length < 2)
			{
				MessageBox.Show("Błędna liczba parametrów programu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			Program.userName = args[0];
			Program.userPassword = args[1];
			return true;
		}

		private static void createSqlConnection(string[] args)
		{
			XmlReader confReader = new XmlReader(mainPath + @"..\conf\config.xml");
			DBConnectionData connData = new DBConnectionData()
			{
				serverName = confReader.getNodeValue("server"),
				dbName = confReader.getNodeValue("db_modeler"),
				login = Program.userName,
				password = Program.userPassword
			};

			if (args.Length > 2 && args[2] == "windowsAuthentication")
				connData.connectionType = ConnectionTypes.windowsAuthentication;

			Program.dbConnection = new DBConnector().getDBConnection(connData);
		}
	}
}
