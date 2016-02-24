using DryIoc;
using DryIoc.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.StaticFiles;
using Owin;
using POC.Owin.AspNet.Infrastructure;
using POC.Owin.AspNet.Infrastructure.QueryStrings;
using POC.Owin.AspNet.Models.Abstractions;
using POC.Owin.AspNet.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

using AppFunc = System.Func<
    System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task
>;

[assembly: OwinStartup(typeof(POC.Owin.AspNet.Startup))]

namespace POC.Owin.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var webApiConfig = WebApiConfiguration();
            var hubConfig = SignalRConfiguration();
            var staticFilesConfig = StaticFilesConfiguration();

            ContainerConfiguration(webApiConfig);   

            app.Use(new Func<AppFunc, AppFunc>(next => env => Invoke(next, env)))
                .UseErrorPage(ErrorPageOptions.ShowAll)
                .UseCors(CorsOptions.AllowAll)
                .Use(typeof(OwinMiddleWareQueryStringExtractor))
                .UseOAuthAuthorizationServer(new OAuthOptions())
                .UseJwtBearerAuthentication(new JwtOptions())
                .UseAngularServer("/", "/index.html")
                .UseFileServer(staticFilesConfig)
                .UseWebApi(webApiConfig)
                .MapSignalR(hubConfig);
        }

        private FileServerOptions StaticFilesConfiguration()
        {
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = new WebPhysicalFileSystem(".\\wwwroot")
            };

            return options;
        }

        private HubConfiguration SignalRConfiguration()
        {
            var hubConfig = new HubConfiguration
            {
                EnableJSONP = true,
                EnableJavaScriptProxies = true
            };

            return hubConfig;
        }

        private HttpConfiguration WebApiConfiguration()
        {
            var webApiConfig = new HttpConfiguration();
            webApiConfig.MapHttpAttributeRoutes();
            webApiConfig.Routes.MapHttpRoute(
                "default",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            return webApiConfig;
        }

        private void ContainerConfiguration(HttpConfiguration webApiConfig)
        {
            var container = new Container()
                .WithWebApi(webApiConfig);

            container.Register<ITest, Test>();

            GlobalHost.DependencyResolver.Register(
                typeof(IHubActivator),
                () => new DryIocHubActivator(container)
            );

            var implementingClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type =>
                    type.BaseType == typeof(Hub)
                );

            foreach (var implementingClass in implementingClasses)
                container.Register(implementingClass, setup: Setup.With(allowDisposableTransient: true));
        }

        private async Task Invoke(AppFunc next, IDictionary<string, object> env)
        {
            var diag = new Stopwatch();
            diag.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            await Console.Out.WriteLineAsync($"[BEGIN] :: #{env["owin.RequestPath"]}");

            await next.Invoke(env);

            Console.ForegroundColor = ConsoleColor.Green;
            diag.Stop();
            await Console.Out.WriteLineAsync($"[-END-] :: #{env["owin.RequestPath"]} :: {diag.ElapsedMilliseconds}ms");
        }
    }

    public sealed class DryIocHubActivator : IHubActivator
    {
        private readonly IContainer _container;

        public DryIocHubActivator(IContainer container)
        {
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            _container.OpenScope();
            return _container.Resolve<IHub>(descriptor.HubType);
        }
    }
}
