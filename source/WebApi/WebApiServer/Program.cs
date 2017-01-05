using System;
using System.Web.Http;
using FWF.KeyExchange.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace WebApiServer
{
    class Program
    {

        public static FWFKeyExchangeBootstrapper FwfKeyExchangeBootstrapper;

        static void Main(string[] args)
        {
            string url = "http://localhost:23456";

            FwfKeyExchangeBootstrapper = new FWFKeyExchangeBootstrapper();

            using (WebApp.Start(url, WebAppConfig))
            {
                Console.WriteLine("Listening on uri: {0}", url);
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }

        }

        private static void WebAppConfig(IAppBuilder appBuilder)
        {
            // Use KeyExchange middleware to handle the key exchange
            appBuilder.UseKeyExchange(FwfKeyExchangeBootstrapper);

            // Handle any incoming message that has been encrypted with the shared key
            appBuilder.UseKeyExchangeMessage(FwfKeyExchangeBootstrapper);

            var config = new HttpConfiguration();

            config.Formatters.Add(new BinaryMediaTypeFormatter());

            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }


    }
}
