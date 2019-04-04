using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Configuration;
using System.Data.SqlClient;
using BlueRidgeUtility.BlueRidge_SignalR_Hub;
using BlueRidgeUtility_BAL.Models;
using BlueRidgeUtility_BAL.Managers;
using System.Management;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Security;

namespace BlueRidgeUtility.BlueRidgeUtility_BLL
{
    public class DBBackupRestoreManager : IDBBackupRestoreManager
    {
        IBackupRestoreService _backupRestoreService;
        public string BKP_FOLDER_PATH = ConfigurationManager.AppSettings["BKP_FOLDER_PATH"];
        public DBBackupRestoreManager(IBackupRestoreService backupRestoreService)
        {
            _backupRestoreService = backupRestoreService;
        }


        public IList<DatabaseSelectItems> GetAllSourceDatabaseNames()
        {


            try
            {
                List<DatabaseSelectItems> databaseSelectItems = new List<DatabaseSelectItems>();
                var databases = _backupRestoreService.getAllDatabaseServers(1);  // 1 = Source Database
                if (databases != null)
                {
                    foreach (var database in databases)
                    {
                        var lstDatabases = GetAllDatabaseNameFromConnectionString(database.databaseConnectionString);
                        if (lstDatabases != null)
                        {
                            lstDatabases.ForEach(x => { x.server_id = database.connectionStringId; x.value = $"{x.server_id}_{x.database_id}"; });
                            databaseSelectItems.AddRange(lstDatabases);

                        }

                    }

                }
                return databaseSelectItems;


            }
            catch (Exception ex)
            {
                return null;
            }



        }






        public List<DatabaseSelectItems> GetAllDatabaseNameFromConnectionString(string connectionString)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = conn.CreateCommand())
                    {
                        sqlCommand.CommandTimeout = 0;
                        sqlCommand.CommandText = @"SELECT name,database_id FROM sys.databases where name not in ('master', 'tempdb', 'model', 'msdb');";
                        conn.Open();
                        using (SqlDataReader row = sqlCommand.ExecuteReader())
                        {
                            return row.MapToList<DatabaseSelectItems>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public GetSourceDatabaseInitials GetDatabase_Backup_Utility_Details()
        {
            GetSourceDatabaseInitials getSourceDatabaseInitials = new GetSourceDatabaseInitials();
            getSourceDatabaseInitials.sourceServerSelectList = GetAllSourceDatabaseNames();
            var destinationDBServers = _backupRestoreService.getAllDatabaseServers(2); // 2 = Destination Database
            if (destinationDBServers != null && destinationDBServers.Count > 0)
            {
                getSourceDatabaseInitials.destinationDBServers = destinationDBServers.Select(x => new DatabaseSelectItems { name = x.databaseServerName, server_id = x.connectionStringId }).ToList();

            }
            return getSourceDatabaseInitials;

        }


        public string Backup_Restore(BackupRestoreModel backupModel)
        {

            backupModel.bkpFilename = $"{ backupModel.uniqueTaskId}.bkp";
            var backupHistoryId = _backupRestoreService.InsertNewBackupRestoreEntry(backupModel);
            if (backupHistoryId != null)
            {
                try
                {
                    
                    backupModel.bkpHistoryId = backupHistoryId;

                    var sourceConnectionStringModel = _backupRestoreService.getDatabaseConnectionStringModel(backupModel.sourceServerId);
                    var sourceConnectionBuilder = GetConnectionBuilder(sourceConnectionStringModel.databaseConnectionString);
                    backupModel.sourceServerBKPPath = sourceConnectionStringModel.bkpFilePath;
                    backupModel.sourceServerUsername = sourceConnectionBuilder.UserID;
                    backupModel.sourceServerPassword = sourceConnectionBuilder.Password;
                   
                    var destinationConnectionStringModel = _backupRestoreService.getDatabaseConnectionStringModel(backupModel.destinationServerId);
                    var destinationConnectionBuilder = GetConnectionBuilder(destinationConnectionStringModel.databaseConnectionString);
                    backupModel.destinationServerBKPPath = destinationConnectionStringModel.bkpFilePath;
                    backupModel.destinationServerUsername = destinationConnectionBuilder.UserID;
                    backupModel.destinationServerPassword = destinationConnectionBuilder.Password;


                    _backupRestoreService.updateBKP_RestoreHistoryStatus(backupModel.bkpHistoryId, BlueRidgeUtility_BAL.Enums.BackRestoreStatus.Backup);

                    string backupFilePath = BackupDataBase(backupModel, sourceConnectionBuilder.ConnectionString);
                    if (backupFilePath != null)
                    {
                         PowershellBKPFileCopyModel model = new PowershellBKPFileCopyModel(); // To Do
                        model.FromRemoteServer = sourceConnectionBuilder.DataSource;
                        model.ToRemoteServer = destinationConnectionBuilder.DataSource;
                        model.sourceusername = ConfigurationManager.AppSettings["SOURCE_USERNAME"];
                        model.sourcepassword= ConfigurationManager.AppSettings["SOURCE_PASSWORD"];
                        model.destinationusername = ConfigurationManager.AppSettings["DESTINATION_USERNAME"];
                        model.destinationpassword = ConfigurationManager.AppSettings["DESTINATION_PASSWORD"];
                        model.source = $"{backupModel.sourceServerBKPPath}{backupModel.bkpFilename}";
                        model.target = $"{backupModel.destinationServerBKPPath}";
                        model.FolderBKPPath = BKP_FOLDER_PATH;
                        model.SourceBKPPath = $"{BKP_FOLDER_PATH}{backupModel.bkpFilename}";
                        _backupRestoreService.updateBKP_RestoreHistoryStatus(backupModel.bkpHistoryId, BlueRidgeUtility_BAL.Enums.BackRestoreStatus.Copying);
                        // CopyBKPFileToServer(model);
                        _backupRestoreService.updateBKP_RestoreHistoryStatus(backupModel.bkpHistoryId, BlueRidgeUtility_BAL.Enums.BackRestoreStatus.Restore);

                        CreateDestinationDatabase(destinationConnectionBuilder.ConnectionString, backupModel.destinationDatabaseName);
                         RestoreDataBase(backupModel, destinationConnectionBuilder.ConnectionString);
                    }

                }
                catch (Exception ex)
                {

                    _backupRestoreService.InsertErrorInformation(backupHistoryId, ex);
                    throw ex;
                }

            }



            return null;
        }

        public int CreateDestinationDatabase(string connectionString,string databaseName)
        {
            try
            {

                  using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = conn.CreateCommand())
                    {
                        sqlCommand.CommandTimeout = 0;
                        sqlCommand.CommandText = $"CREATE DATABASE [{databaseName}];";
                        conn.Open();
                        return sqlCommand.ExecuteNonQuery();


                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //public string CopyBKPFileToServer(PowershellBKPFileCopyModel model)
        //{
        //    using (PowerShell PowerShellInstance = PowerShell.Create())
        //    {
        //        PowerShellInstance.AddScript("CopyFile.ps1");
        //        PowerShellInstance.AddParameter("$UserName", @"dev.corp.blueridgeglobal.com\svc_scheduledtask");
        //        PowerShellInstance.AddParameter("$Password", @"DbBac@$123");
        //        PowerShellInstance.AddParameter("$Source", "source");
        //        PowerShellInstance.AddParameter("$Target", "target");
        //        PowerShellInstance.Invoke();
        //    }
        //    return String.Empty;
        //}

        private void CopyBKPFileToServer(PowershellBKPFileCopyModel model)
        {
          using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(@"Param(
            [parameter(Mandatory=$true)]
            $FromRemoteServer,
            [parameter(Mandatory=$true)]
            $ToRemoteServer,
            [parameter(Mandatory=$true)]
            $SourceUserName,
            [parameter(Mandatory=$true)]
            $SourcePassword,
            [parameter(Mandatory=$true)]
            $DestinationUserName,
            [parameter(Mandatory=$true)]
            $DestinationPassword,
            [parameter(Mandatory=$true)]
            $Source,
            [parameter(Mandatory=$true)]
            $Target,
            [parameter(Mandatory=$true)]
            $FolderBKPPath,
            [parameter(Mandatory=$true)]
            $SourceBKPPath

            )

            $SecureSourcePassword = ConvertTo-SecureString $SourcePassword -AsPlainText -Force
            $SourceCredential = New-Object System.Management.Automation.PSCredential $SourceUserName, $SecureSourcePassword
            $SecureDestinationPassword = ConvertTo-SecureString $DestinationPassword -AsPlainText -Force
            $DestinationCredential = New-Object System.Management.Automation.PSCredential $DestinationUserName, $SecureDestinationPassword
            $FromSession = New-PSSession -ComputerName $FromRemoteServer  -Credential $SourceCredential
            $ToSession = New-PSSession -ComputerName $ToRemoteServer  -Credential $DestinationCredential
            Copy-Item $Source -Destination $FolderBKPPath -FromSession  $FromSession -Recurse
            Copy-Item $SourceBKPPath -Destination $Target -ToSession  $ToSession -Recurse
            Remove-Item –Path $SourceBKPPath –Recurse
            ");
                PowerShellInstance.AddParameter("FromRemoteServer", model.FromRemoteServer);
                PowerShellInstance.AddParameter("ToRemoteServer", model.ToRemoteServer);
                PowerShellInstance.AddParameter("SourceUserName", model.sourceusername);
                PowerShellInstance.AddParameter("SourcePassword", $"{model.sourcepassword}");
                PowerShellInstance.AddParameter("DestinationUserName", model.destinationusername);
                PowerShellInstance.AddParameter("DestinationPassword", $"{model.destinationpassword}");
                PowerShellInstance.AddParameter("Source", model.source);
                PowerShellInstance.AddParameter("Target", model.target);
                PowerShellInstance.AddParameter("FolderBKPPath", model.FolderBKPPath);
                PowerShellInstance.AddParameter("SourceBKPPath", model.SourceBKPPath);
                



                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                if (PowerShellInstance.HadErrors)
                {
                    if (PowerShellInstance.Streams.Error.Count > 0)
                    {
                        throw PowerShellInstance.Streams.Error[0].Exception;

                    }

                }
               
            }

        }


        public string BackupDataBase(BackupRestoreModel backupModel, string connectionString)
        {

            Server sourceServer = GetServerUsingConnectionString(connectionString);
            Backup backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.Database = backupModel.sourceDatabaseName;
            backup.BackupSetDescription = "BackUp of:" + backupModel.sourceDatabaseName + "on" + DateTime.Now.ToShortDateString();
            backup.BackupSetName = "FullBackUp";
            var bkpFilePath=  System.IO.Path.Combine(backupModel.sourceServerBKPPath, backupModel.bkpFilename);
            BackupDeviceItem deviceItem = new BackupDeviceItem(bkpFilePath, DeviceType.File);
            backup.Devices.Add(deviceItem);
            backup.CopyOnly = true;
            backup.Initialize = true;
            backup.Checksum = true;
            backup.ContinueAfterError = true;
            backup.PercentCompleteNotification = 20;
            backup.Incremental = false;
            backup.FormatMedia = false;
            backup.ExpirationDate = DateTime.Now.AddDays(3);
            backup.LogTruncation = BackupTruncateLogType.Truncate;
            backup.PercentComplete += new PercentCompleteEventHandler((sender, e) => backup_PercentComplete(sender, e, backupModel.uniqueTaskId, backupModel.bkpHistoryId));
            backup.Complete +=
                new Microsoft.SqlServer.Management.Common.ServerMessageEventHandler
                ((sender, e) => backup_Complete(sender, e, backupModel.uniqueTaskId));
            // Perform backup.
            backup.SqlBackup(sourceServer);
            backup.Devices.Remove(deviceItem);
            return bkpFilePath;
        }

        //The event handlers
        public void backup_Complete(object sender, Microsoft.SqlServer.Management.Common.ServerMessageEventArgs e, string taskId)
        {
            BlueRidgePortalHub.hubContext.Clients.All.backup_Complete(new { taskId = taskId, percent = 100 });

            //  WriteToLogAndConsole(e.ToString() + "% Complete");
        }
        public void backup_PercentComplete(object sender, PercentCompleteEventArgs e, string taskId, int? bkpHistoryId)
        {
            //   ((Backup)sender).Abort();
            BlueRidgePortalHub.hubContext.Clients.All.backup_PercentComplete(new { taskId = taskId, percent = e.Percent });
            _backupRestoreService.updateBackupPercentage(bkpHistoryId, e.Percent);
            //  WriteToLogAndConsole(e.Percent.ToString() + "% Complete");
        }


        public void RestoreDataBase(BackupRestoreModel backupModel, string connectionString)
        {
            Server myServer = GetServerUsingConnectionString(connectionString);
            Restore myRestore = new Restore();
            myRestore.Database = backupModel.destinationDatabaseName;
            Database currentDb = myServer.Databases[backupModel.destinationDatabaseName];
            if (currentDb != null)
                myServer.KillAllProcesses(backupModel.destinationDatabaseName);
            var bkpFilePath = System.IO.Path.Combine(backupModel.destinationServerBKPPath, backupModel.bkpFilename); 
            myRestore.Devices.AddDevice(bkpFilePath, DeviceType.File);
            RelocateFile DataFile = new RelocateFile();
            string MDF = myRestore.ReadFileList(myServer).Rows[0][1].ToString();
            DataFile.LogicalFileName = myRestore.ReadFileList(myServer).Rows[0][0].ToString();
            DataFile.PhysicalFileName = myServer.Databases[backupModel.destinationDatabaseName].FileGroups[0].Files[0].FileName;

            RelocateFile LogFile = new RelocateFile();
            string LDF = myRestore.ReadFileList(myServer).Rows[1][1].ToString();
            LogFile.LogicalFileName = myRestore.ReadFileList(myServer).Rows[1][0].ToString();
            LogFile.PhysicalFileName = myServer.Databases[backupModel.destinationDatabaseName].LogFiles[0].FileName;

            myRestore.RelocateFiles.Add(DataFile);
            myRestore.RelocateFiles.Add(LogFile);

            myRestore.Database = backupModel.destinationDatabaseName;
            myRestore.ReplaceDatabase = true;
            myRestore.PercentCompleteNotification = 20;
            myRestore.PercentComplete +=
                new PercentCompleteEventHandler((sender, e) => myRestore_PercentComplete(sender, e, backupModel.uniqueTaskId, backupModel.bkpHistoryId));
            myRestore.Complete += new ServerMessageEventHandler((sender, e) => myRestore_Complete(sender, e, backupModel.uniqueTaskId));

            myRestore.SqlRestore(myServer);
            currentDb = myServer.Databases[backupModel.destinationDatabaseName];
            currentDb.SetOnline();
        }
        public void myRestore_Complete
            (object sender, Microsoft.SqlServer.Management.Common.ServerMessageEventArgs e, string taskId)
        {
            BlueRidgePortalHub.hubContext.Clients.All.myRestore_Complete(new { taskId = taskId, percent = 100 });
            //  WriteToLogAndConsole(e.ToString() + " Complete");
        }
        public void myRestore_PercentComplete(object sender, PercentCompleteEventArgs e, string taskId, int? bkpHistoryId)
        {
            BlueRidgePortalHub.hubContext.Clients.All.myRestore_PercentComplete(new { taskId = taskId, percent = e.Percent });
            _backupRestoreService.updateRestorePercentage(bkpHistoryId, e.Percent);
            //  WriteToLogAndConsole(e.Percent.ToString() + "% Complete");
        }

        public bool isDatabaseNameExistOnDestination(string databaseName, int connectionStringId)
        {
            var connectionString = "";

            return GetAllDatabaseNameFromConnectionString(connectionString)?.Any(x => x.name.ToLower() == databaseName.ToLower()) ?? false;
        }
        public Server GetServerUsingConnectionString(String connectionString)
        {
            var builder = GetConnectionBuilder(connectionString);
            ServerConnection ServerConnection = new ServerConnection(builder.DataSource);
            ServerConnection.LoginSecure = false;
            ServerConnection.Login = builder.UserID;
            ServerConnection.Password = builder.Password;
            Server server = new Server(ServerConnection);
            server.ConnectionContext.StatementTimeout = 60 * 60;
            return server;
        }

        private SqlConnectionStringBuilder GetConnectionBuilder(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString);
        }




    }
}