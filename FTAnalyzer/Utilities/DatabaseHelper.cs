using AppKit;
using Ionic.Zip;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Data.Sqlite;

namespace FTAnalyzer.Utilities
{
    public class DatabaseHelper : IDisposable
    {
        public string DatabaseFile { get; private set; }
        public string CurrentFilename { get; private set; }
        public string DatabasePath { get; private set; }
        string EmptyFile;
        string ResourcePath;
        static DatabaseHelper instance;
        static string connectionString;
        static SqliteConnection InstanceConnection { get; set; }
        Version ProgramVersion { get; set; }
 
        #region Constructor/Destructor
        DatabaseHelper()
        {
            DatabasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ResourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), @"../Resources");
            CurrentFilename = Path.Combine(DatabasePath, "FTA-RestoreTemp.s3db");
            CheckDatabaseConnection();
//            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
//            SQLitePCL.raw.FreezeProvider();
            InstanceConnection = new SqliteConnection(connectionString);
        }

        public static DatabaseHelper Instance => instance ??= new DatabaseHelper();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (InstanceConnection?.State == ConnectionState.Open)
                        InstanceConnection.Close();
                    InstanceConnection?.Dispose();
                    // dispose of things here
                }
                catch (Exception)
                { }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void CheckDatabaseConnection()
        {
            try
            {
                DatabaseFile = Path.Combine(DatabasePath, "Geocodes.s3db");
                if (!File.Exists(DatabaseFile))
                {
                    string path = DatabasePath;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    EmptyFile = Path.Combine(ResourcePath, "Geocodes-Empty.s3db");
                    File.Copy(EmptyFile, DatabaseFile);
                }
                connectionString = $"Data Source={DatabaseFile};";
            }
            catch(ArgumentNullException)
            {
                UIHelpers.ShowMessage($"Couldn't find Empty Database in Resources Directory at {EmptyFile}");
            }
            catch (Exception ex)
            {
                UIHelpers.ShowMessage($"Error opening database. Error is :{ex.Message}.\n\nDatabaseFile:{DatabaseFile}\nEmptyFile:{EmptyFile}", "FTAnalyzer");
            }
        }
        #endregion

        #region Database Update Functions
        public void CheckDatabaseVersion(Version programVersion)
        {
            try
            {
                ProgramVersion = programVersion;
                Version dbVersion = GetDatabaseVersion();
                if (dbVersion < programVersion)
                    UpgradeDatabase(dbVersion);
            }
            catch (SqliteException)
            {
                //                log.Debug("Caught Exception in CheckDatabaseVersion " + e.Message);
                UpgradeDatabase(new Version("0.0.0.0"));
            }
        }

        static Version GetDatabaseVersion()
        {
            string db = null;
            try
            {
                if (InstanceConnection.State != ConnectionState.Open)
                    InstanceConnection.Open();
                using (SqliteCommand cmd = new SqliteCommand("select Database from versions where platform='Mac'", InstanceConnection))
                {
                    db = (string)cmd.ExecuteScalar();
                }
                InstanceConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in GetDatabaseVersion " + e.Message);
            }
            Version dbVersion = db == null ? new Version("0.0.0.0") : new Version(db);
            return dbVersion;
        }

        public bool BackupDatabase(string saveDatabase, string comment)
        {
            //string directory = Application.UserAppDataRegistry.GetValue("Geocode Backup Directory", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).ToString();
            string FileName = $"FTAnalyzer-Geocodes-{DateTime.Now.ToString("yyyy-MM-dd")}-{FamilyTree.Instance.Version}.zip";
            //saveDatabase.InitialDirectory = directory;
            //DialogResult result = saveDatabase.ShowDialog();
            //if (result == DialogResult.OK)
            {
                StartBackupRestoreDatabase();
                if (File.Exists(FileName))
                    File.Delete(FileName);
                ZipFile zip = new ZipFile(FileName);
                zip.AddFile(DatabaseFile, string.Empty);
                zip.Comment = comment + " on " + DateTime.Now.ToString("dd MMM yyyy HH:mm");
                zip.Save();
                //EndBackupDatabase();
                //Application.UserAppDataRegistry.SetValue("Geocode Backup Directory", Path.GetDirectoryName(saveDatabase.FileName));
                UIHelpers.ShowMessage($"Database exported to {FileName}", "FTAnalyzer Database Export Complete");
                return true;
            }
            //return false;
        }

        void UpgradeDatabase(Version dbVersion)
        {
            try
            {
                Version v7_3_3_2 = new Version("7.3.3.2");
                Version v7_4_0_0 = new Version("7.4.0.0");
                Version v8_0_0_0 = new Version("8.0.0.0");
                if (InstanceConnection.State != ConnectionState.Open)
                    InstanceConnection.Open();
                if (dbVersion < v7_3_3_2)
                {
                    try
                    {
                        using (SqliteCommand cmd = new SqliteCommand("SELECT count(*) FROM LostCousins", InstanceConnection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (SqliteException)
                    {
                        using (SqliteCommand cmd = new SqliteCommand("create table IF NOT EXISTS LostCousins (CensusYear INTEGER(4), CensusCountry STRING (20), CensusRef STRING(25), IndID STRING(10), FullName String(80), constraint pkLostCousins primary key (CensusYear, CensusCountry, CensusRef, IndID))", InstanceConnection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    using (SqliteCommand cmd = new SqliteCommand("update versions set Database = '7.3.3.2'", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                if (dbVersion < v7_4_0_0)
                {

                    using (SqliteCommand cmd = new SqliteCommand("drop table versions", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS Versions(Platform VARCHAR(10) PRIMARY KEY, [Database] VARCHAR(10));", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqliteCommand cmd = new SqliteCommand("insert into Versions(platform, database) values('PC', '7.4.0.0')", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqliteCommand cmd = new SqliteCommand("insert into Versions(platform, database) values('Mac', '1.2.0.42')", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                if (dbVersion < v8_0_0_0)
                {
                    using (SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS CustomFacts (FactType STRING(60) PRIMARY KEY, [Ignore] BOOLEAN)", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqliteCommand cmd = new SqliteCommand("update Versions set database = '8.0.0.0' where platform = 'PC'", InstanceConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                }
            catch (Exception ex)
            {
                UIHelpers.ShowMessage($"Error upgrading database. Error is :{ex.Message}", "FTAnalyzer");
            }
        }
        #endregion

        #region LostCousins
        public static int AddLostCousinsFacts()
        {
            int count = 0;
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            using (SqliteCommand cmd = new SqliteCommand("select CensusYear, CensusCountry, CensusRef, IndID, FullName from LostCousins", InstanceConnection))
            {
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string indID = reader["IndID"].ToString();
                        string fullName = reader["FullName"].ToString();
                        Individual ind = FamilyTree.Instance.GetIndividual(indID);
                        if (ind?.Name == fullName) // only load if individual exists in this tree.
                        {
                            string CensusYear = reader["CensusYear"].ToString();
                            string CensusCountry = reader["CensusCountry"].ToString();
                            string CensusRef = reader["CensusRef"].ToString();
                            if (!ind.IsLostCousinsEntered(CensusDate.GetLostCousinsCensusYear(new FactDate(CensusYear), true)))
                            {
                                FactLocation location = FactLocation.GetLocation(CensusCountry);
                                Fact f = new Fact(CensusRef, Fact.LOSTCOUSINS, new FactDate(CensusYear), location, string.Empty, true, true);
                                ind?.AddFact(f);
                            }
                            count++;
                        }
                        else
                        {
                            Console.Write("name wrong");
                            // UpdateFullName(reader, ind.Name); needed during testing
                        }
                    }
                }
            }
            return count;
        }

        void UpdateFullName(SqliteDataReader reader, string name)
        {
            using (SqliteCommand updateCmd = new SqliteCommand("update LostCousins set FullName=? Where CensusYear=? and CensusCountry=? and CensusRef=? and IndID=?", InstanceConnection))
            {
                SqliteParameter param = updateCmd.CreateParameter();
                param.DbType = DbType.String;
                updateCmd.Parameters.Add(param);
                param = updateCmd.CreateParameter();
                param.DbType = DbType.Int32;
                updateCmd.Parameters.Add(param);
                param = updateCmd.CreateParameter();
                param.DbType = DbType.String;
                updateCmd.Parameters.Add(param);
                param = updateCmd.CreateParameter();
                param.DbType = DbType.String;
                updateCmd.Parameters.Add(param);
                param = updateCmd.CreateParameter();
                param.DbType = DbType.String;
                updateCmd.Parameters.Add(param);
                updateCmd.Prepare();

                updateCmd.Parameters[0].Value = name;
                updateCmd.Parameters[1].Value = reader["CensusYear"];
                updateCmd.Parameters[2].Value = reader["CensusCountry"];
                updateCmd.Parameters[3].Value = reader["CensusRef"];
                updateCmd.Parameters[4].Value = reader["IndID"];
                int rowsaffected = updateCmd.ExecuteNonQuery();
                if (rowsaffected != 1)
                    Console.WriteLine("Problem updating");
            }
        }

        public static bool LostCousinsExists(CensusIndividual ind)
        {
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            bool result = false;
            using (SqliteCommand cmd = new SqliteCommand("SELECT EXISTS(SELECT 1 FROM LostCousins where CensusYear=? and CensusCountry=? and CensusRef=? and IndID=?)", InstanceConnection))
            {
                SqliteParameter param = cmd.CreateParameter();
                param.DbType = DbType.Int32;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                cmd.Prepare();
                cmd.Parameters[0].Value = ind.CensusDate.BestYear;
                cmd.Parameters[1].Value = ind.CensusCountry;
                cmd.Parameters[2].Value = ind.CensusReference;
                cmd.Parameters[3].Value = ind.IndividualID;
                using (SqliteDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (reader.Read())
                        result = reader[0].ToString() == "1";
                }
            }
            return result;
        }

        public static void StoreLostCousinsFact(CensusIndividual ind, IProgress<string> outputText)
        {
            try
            {
                if (InstanceConnection.State != ConnectionState.Open)
                    InstanceConnection.Open();
                SqliteParameter param;

                using (SqliteCommand cmd = new SqliteCommand("insert into LostCousins (CensusYear, CensusCountry, CensusRef, IndID, FullName) values(?,?,?,?,?)", InstanceConnection))
                {
                    param = cmd.CreateParameter();
                    param.DbType = DbType.Int32;
                    cmd.Parameters.Add(param);
                    param = cmd.CreateParameter();
                    param.DbType = DbType.String;
                    cmd.Parameters.Add(param);
                    param = cmd.CreateParameter();
                    param.DbType = DbType.String;
                    cmd.Parameters.Add(param);
                    param = cmd.CreateParameter();
                    param.DbType = DbType.String;
                    cmd.Parameters.Add(param);
                    param = cmd.CreateParameter();
                    param.DbType = DbType.String;
                    cmd.Parameters.Add(param);
                    cmd.Prepare();

                    if (ind.CensusReference != null)
                    {
                        cmd.Parameters[0].Value = ind.CensusDate.BestYear;
                        cmd.Parameters[1].Value = ind.CensusCountry;
                        cmd.Parameters[2].Value = ind.CensusReference;
                        cmd.Parameters[3].Value = ind.IndividualID;
                        cmd.Parameters[4].Value = ind.Name;

                        int rowsaffected = cmd.ExecuteNonQuery();
                        if (rowsaffected != 1)
                            outputText.Report($"\nProblem updating record in database update affected {rowsaffected} records.");
                        else
                        {
                            FactLocation location = FactLocation.GetLocation(ind.CensusCountry);
                            Fact f = new Fact(ind.CensusRef, Fact.LC_FTA, ind.CensusDate, location, string.Empty, true, true);
                            Individual person = FamilyTree.Instance.GetIndividual(ind.IndividualID); // get the individual not the census indvidual
                            person?.AddFact(f);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                outputText.Report($"\nFailed to save Lost Cousins record in database error was: {e.Message}");
            }
        }
        #endregion

        #region Cursor Queries

        public void AddEmptyLocationsToQueue(ConcurrentQueue<FactLocation> queue)
        {
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            using (SqliteCommand cmd = new SqliteCommand("select location from geocode where foundlocation='' and geocodestatus in (3, 8, 9)", InstanceConnection))
            {
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FactLocation loc = FactLocation.LookupLocation(reader[0].ToString());
                        if (!queue.Contains(loc))
                            queue.Enqueue(loc);
                    }
                }
            }
            InstanceConnection.Close();
        }
        #endregion

        #region Custom Facts

        public static bool IgnoreCustomFact(string factType)
        {
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            bool result = false;
            using (SqliteCommand cmd = new SqliteCommand("SELECT EXISTS(SELECT ignore FROM CustomFacts where FactType=?)", InstanceConnection))
            {
                SqliteParameter param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                cmd.Prepare();
                cmd.Parameters[0].Value = factType;
                using (SqliteDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (reader.Read())
                        result = reader[0].ToString() == "1";
                }
            }
            return result;
        }

        public static void IgnoreCustomFact(string factType, bool ignore)
        {
            using (SqliteCommand cmd = new SqliteCommand("insert or replace into CustomFacts(FactType,Ignore) values(?,?)", InstanceConnection))
            {
                SqliteParameter param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.Boolean;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                cmd.Prepare();
                cmd.Parameters[0].Value = factType;
                cmd.Parameters[1].Value = ignore;
                int rowsaffected = cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region BackupRestore
        public bool StartBackupRestoreDatabase()
        {
            string tempFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer\Geocodes.s3db.tmp");
            try
            {
                GC.Collect(); // needed to force a cleanup of connections prior to replacing the file.
                if (File.Exists(tempFilename))
                    File.Delete(tempFilename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RestoreDatabase(IProgress<string> outputText)
        {
            bool result = true;
            try
            {
                // finally check for updates
                //restoring = true;
                CheckDatabaseVersion(ProgramVersion);
                //restoring = false;
                FamilyTree ft = FamilyTree.Instance;
                if (ft.DataLoaded)
                    return FamilyTree.LoadGeoLocationsFromDataBase(outputText);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region EventHandler
        public static event EventHandler GeoLocationUpdated;
        protected static void OnGeoLocationUpdated(FactLocation loc)
        {
            GeoLocationUpdated?.Invoke(loc, EventArgs.Empty);
        }
        #endregion
    }
}
