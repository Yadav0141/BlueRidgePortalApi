using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using BlueRidgeUtility.BlueRidgeUtility_BLL;
using BlueRidgeUtility.Security;
using BlueRidgeUtility.Validators;
using BlueRidgeUtility_BAL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BlueRidgeUtility.App_Start
{
    public class IOCConfig
    {
        public static void Configure(ContainerBuilder containerBuilder)
        {
            // Register Web API controller in executing assembly.
            
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterType<CurrentUserServiceProvider>().As<ICurrentUserServiceProvider>();
            containerBuilder.RegisterType<UrlTokenGenerator>().As<IUrlTokenGenerator>();
            containerBuilder.RegisterType<DBBackupRestoreManager>().As<IDBBackupRestoreManager>();
            containerBuilder.RegisterType<UserModelValidator>().As<IUserModelValidator>();
            containerBuilder.RegisterType<PasswordHasher>().As<IPasswordHasher>();
            containerBuilder.RegisterAssemblyTypes(Assembly.Load(nameof(BlueRidgeUtility_BAL)))
                .Where(t => t.Namespace.Contains("Managers"))
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));
        }
    }
}