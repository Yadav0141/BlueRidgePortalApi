using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using BlueRidgeUtility.Security;
using System.Configuration;
using Autofac;
using System.Reflection;
using Autofac.Integration.WebApi;
using BlueRidgeUtility.App_Start;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using Autofac.Integration.Mvc;

[assembly: OwinStartup(typeof(BlueRidgeUtility.Startup))]

namespace BlueRidgeUtility
{
    public class Startup
    {
        private readonly string _appDomain = ConfigurationManager.AppSettings["JWT_DOMAIN"]; 
        private readonly string _jwtSecret = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
        public const string JsonWebTokenCookieName = "api-jwt";
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var builder = new ContainerBuilder();
            IOCConfig.Configure(builder);
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            app.MapSignalR();
          

            var provider = new OAuthBearerAuthenticationProvider();
            provider.OnRequestToken = GetRequestTokenFromCookie;

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "jwt",
                Provider = provider,
                TokenValidationParameters = new TokenValidationParameters
                {
                    // The same _jwtSecret and _appDomain as in JwtTokenProvider were used here
                    IssuerSigningKey = _jwtSecret.ToSymmetricSecurityKey(),
                    ValidIssuer = _appDomain,
                    ValidAudience = _appDomain,
                    ValidateLifetime = true,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(0)
                }
            });
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

           
           
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
        private static Task GetRequestTokenFromCookie(OAuthRequestTokenContext context)
        {
            context.Token = context.Request.Cookies[JsonWebTokenCookieName];
            return Task.FromResult<object>(null);
        }

      

    }
}
