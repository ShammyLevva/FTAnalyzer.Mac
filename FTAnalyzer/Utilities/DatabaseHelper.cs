using AppKit;
using Foundation;
using Ionic.Zip;
using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FTAnalyzer.Utilities
{
    public class DatabaseHelper : IDisposable
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

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
            InstanceConnection = new SqliteConnection(connectionString);
        }

        public static DatabaseHelper Instance => instance = instance ?? new DatabaseHelper();

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
                connectionString = $"Data Source={DatabaseFile};Version=3;";
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
            string FileName = $"FTAnalyzer-Geocodes-{DateTime.Now.ToString("yyyy-MM-dd")}-{App.Version}.zip";
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
                InstanceConnection.Close();
            }
            catch (Exception ex)
            {
                UIHelpers.ShowMessage($"Error upgrading database. Error is :{ex.Message}", "FTAnalyzer");
            }
        }
        #endregion

        #region LostCousins
        public int AddLostCousinsFacts()
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

        public void StoreLostCousinsFact(CensusIndividual ind)
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
                            Console.WriteLine("Problem updating");
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
                Console.WriteLine(e.Message);
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
                    return ft.LoadGeoLocationsFromDataBase(outputText);
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
