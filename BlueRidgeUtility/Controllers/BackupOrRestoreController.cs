using BlueRidgeUtility.BlueRidgeUtility_BLL;
using BlueRidgeUtility.Security;
using BlueRidgeUtility_BAL.Managers;
using BlueRidgeUtility_BAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlueRidgeUtility.Controllers
{
    [Authorize]
    [RoutePrefix("api/BackupOrRestore")]
    public class BackupOrRestoreController : ApiController
    {
        IDBBackupRestoreManager _dBBackupRestoreManager;
        ICurrentUserServiceProvider _currentUserServiceProvider;
        IBackupRestoreService _backupRestoreService;
        public BackupOrRestoreController(IDBBackupRestoreManager dBBackupRestoreManager,
             ICurrentUserServiceProvider currentUserServiceProvider,
             IBackupRestoreService backupRestoreService)
        {
            _dBBackupRestoreManager = dBBackupRestoreManager;
            _currentUserServiceProvider = currentUserServiceProvider;
            _backupRestoreService = backupRestoreService;
        }
        
        [Route("GetAllSourceDatabaseNames"), HttpGet]
        public IHttpActionResult GetAllSourceDatabaseNames()
        {
           

            return Ok(_dBBackupRestoreManager.GetDatabase_Backup_Utility_Details());

        }

        [Route("Backup_Restore"), HttpPost]
        public IHttpActionResult Backup_Restore(BackupRestoreModel backupModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             if (_dBBackupRestoreManager.isDatabaseNameExistOnDestination(backupModel.destinationDatabaseName, backupModel.destinationServerId))
            {
                ModelState.AddModelError("Error_Already_Exist", "Database name already exist.");
                return BadRequest(ModelState);
            }
            backupModel.userId = _currentUserServiceProvider.GetCurrentUserId();
            return Ok(_dBBackupRestoreManager.Backup_Restore(backupModel));
            
        }


        [Route("getbackuphistorybyuser"),HttpGet]
          public IHttpActionResult GetBackupHistorybyUser(int pageNumber, int pageCount)
        {
            var userId = _currentUserServiceProvider.GetCurrentUserId();
            var lstHistory=  _backupRestoreService.GetBackupRestoreHistoryListByUser(userId, pageNumber, pageCount);
            return Ok(lstHistory);

        }


        [Route("abortbackuphistory"), HttpGet]
        public IHttpActionResult AbortBackupHistory(int bkpHistoryId)
        {
            if (bkpHistoryId == 0)
            {
                return BadRequest("Parameter bkpHistoryId not found.");
            }
            _backupRestoreService.updateBKP_RestoreHistoryStatus(bkpHistoryId, BlueRidgeUtility_BAL.Enums.BackRestoreStatus.Aborted);
             return Ok();

        }

        [Route("deletebackuphistory"), HttpGet]
        public IHttpActionResult DeleteBackupHistory(int bkpHistoryId)
        {
           
            var result = _backupRestoreService.DeleteBackupHistory(bkpHistoryId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Unable to delete this record.Please contact administrator.");
            }
            

        }



    }
}
