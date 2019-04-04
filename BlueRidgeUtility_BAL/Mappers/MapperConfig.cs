using AutoMapper;
using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Helpers.MimeTypes;
using BlueRidgeUtility_BAL.Models;
using BlueRidgeUtility_BAL.SelectListItemModels;
using BlueRidgeUtility_BAL.SelectListItems;
using BlueRidgeUtility_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Mappers
{
    public static class Mapping
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<tblUser, UserModel>()
               . ForMember(o=>o.parsedJoiningDate,b=>b.MapFrom(z=>z.joiningDate!=null?new Datepart { day=z.joiningDate.Value.Day,month=z.joiningDate.Value.Month,year=z.joiningDate.Value.Year}:null))
               .ForMember(o => o.parsedExitDate, b => b.MapFrom(z => z.exitDate != null ? new Datepart { day = z.exitDate.Value.Day, month = z.exitDate.Value.Month, year = z.exitDate.Value.Year } : null))
               .ForMember(x => x.createdDate, opt => opt.Ignore())
               .ForMember(x => x.isDeleted, opt => opt.Ignore())
               .ForMember(x => x.password, opt => opt.Ignore())
               .ForMember(x => x.passwordSalt, opt => opt.Ignore());
              
            CreateMap<RoleSelectListModel, SelectListItem>()
                .ForMember(o => o.text, b => b.MapFrom(z => z.roleDescription))
                .ForMember(o => o.value, b => b.MapFrom(z => z.roleId.ToString()));
            CreateMap<UserSelectListItemModel, SelectListItem>()
                .ForMember(o => o.text, b => b.MapFrom(z => z.userName))
                .ForMember(o => o.value, b => b.MapFrom(z => z.userId));

            CreateMap<UserModel,tblUser >()
                .ForMember(o => o.joiningDate, b => b.MapFrom(z => z.parsedJoiningDate != null ? new DateTime(z.parsedJoiningDate.year, z.parsedJoiningDate.month, z.parsedJoiningDate.day):(DateTime?)null))
                .ForMember(o => o.exitDate, b => b.MapFrom(z => z.parsedExitDate != null ? new DateTime(z.parsedExitDate.year, z.parsedExitDate.month, z.parsedExitDate.day) : (DateTime?)null))
                .ForMember(x => x.createdDate, opt => opt.Ignore())
                .ForMember(x => x.modifiedDate, opt => opt.Ignore())
                .ForMember(x => x.isDeleted, opt => opt.Ignore())
                .ForMember(x => x.password, opt => opt.Ignore())
                .ForMember(x => x.passwordSalt, opt => opt.Ignore())
                .ForMember(x => x.userId, opt => opt.Ignore());

            CreateMap<tblDocument, AddEditUserDocumentModel>()
                .ForMember(o=>o.content_type,b=>b.MapFrom(z=>z.documentExt!=null? MimeTypeMap.GetMimeType(z.documentExt):String.Empty));
            CreateMap<AddEditUserDocumentModel,tblDocument>();

            CreateMap<BackupRestoreModel, tblBackupRestoreHistory>()
                   .ForMember(o => o.userId, b => b.MapFrom(z => z.userId))
                   .ForMember(o => o.sourceDatabaseName, b => b.MapFrom(z => z.sourceDatabaseName))
                   .ForMember(o => o.bkpFileName, b => b.MapFrom(z => z.bkpFilename))
                   .ForMember(o => o.destinationDatabaseName, b => b.MapFrom(z => z.destinationDatabaseName))
                   .ForMember(o => o.createdDate, b => b.MapFrom(z => DateTime.Now))
                   .ForMember(o => o.isDeleted, b => b.MapFrom(z => false))
                   .ForMember(o => o.status, b => b.MapFrom(z => Convert.ToInt32( BackRestoreStatus.InProgress)));



            CreateMap<tblAllDatabaseConnectionStrings, ConnectionStringModel>()
                .ForMember(o => o.databaseType, b => b.MapFrom(z => ((Backup_Restore_DatabaseType)z.databaseType)));





        }
    }
}
