using BlueRidgeUtility_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL
{
   public class BlueRidgeUtilityDBContext : DbContext
    {
        public BlueRidgeUtilityDBContext() : base("name = BlueRidgeUtitlityDBConnectionString")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<tblUser> TblUsers { get; set; }
        public DbSet<tblRole> TblRoles { get; set; }
        public DbSet<tblModules> TblModules { get; set; }
        public DbSet<tblPermissions> TblPermissions { get; set; }
        public DbSet<tblBackupRestoreHistory> TblBackupRestoreHistory { get; set; }
        public DbSet<tblDocumentType> TblDocumentTypes { get; set; }
        public DbSet<tblDocument> TblDocuments { get; set; }
        public DbSet<tblAllDatabaseConnectionStrings> TblAllDatabaseConnectionStrings { get; set; }
    }
}
