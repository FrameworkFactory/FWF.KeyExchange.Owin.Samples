using System;
using FWF.KeyExchange.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace FWF.KeyExchange.Sample.OwinApi
{
    class Program
    {

        static void Main(string[] args)
        {
            string url = "http://localhost:12345";

            using (WebApp.Start(url, WebAppConfig))
            {
                Console.WriteLine("Listening on uri: {0}", url);
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            
        }

        private static void WebAppConfig(IAppBuilder appBuilder)
        {
            var bootstrapper = new FWFKeyExchangeBootstrapper();

            // Use KeyExchange middleware to handle the key exchange
            appBuilder.UseKeyExchange(bootstrapper);

            // Handle any incoming message that has been encrypted with the shared key
            appBuilder.UseKeyExchangeMessage(bootstrapper);
        }
    }
}
