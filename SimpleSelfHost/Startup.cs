using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SimpleSelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            

            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            ConfigureWebApi(config);
            app.UseWebApi(config);


        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            var formatters = config.Formatters;

            //Remove o XML
            formatters.Remove(formatters.XmlFormatter);

            //Config. o json de resposta
            var jsonSettings = formatters.JsonFormatter.SerializerSettings;

            //Deixa a resposta identada e mais amigável (apenas p/ seres humanos)
            jsonSettings.Formatting = Formatting.Indented;

            //Deixa a serialização dos objetos em CamelCase (Default na Web)
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //Evita o loop infinito no case de objetvos encadeados
            formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //Permite a configuração de rotas através do decorator [Route]
            config.MapHttpAttributeRoutes();

            var cors = new EnableCorsAttribute("*", headers: "*", methods: "*");

            config.EnableCors(cors);

            //Nossa rota default
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
        }

    }
}
