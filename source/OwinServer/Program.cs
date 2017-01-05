using System;
using FWF.KeyExchange.Owin;
using FWF.KeyExchange.Sample.OwinApi.Handlers;
using Microsoft.Owin.Hosting;
using Owin;

namespace FWF.KeyExchange.Sample.OwinApi
{
    class Program
    {
        private static RootHandler _rootHandler = new RootHandler();
        private static MessageSendHandler _messageSendHandler = new MessageSendHandler();

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
            // Use KeyExchange middleware to handle the key exchange
            var options = new OwinKeyExchangeOptions();
            appBuilder.UseKeyExchange(options);

            // Handle any request to the root path
            appBuilder.Use(_rootHandler.Handle);

            // DEMO: Handle any incoming message that has been encrypted with the shared key
            appBuilder.Use(_messageSendHandler.Handle);
        }
    }
}
