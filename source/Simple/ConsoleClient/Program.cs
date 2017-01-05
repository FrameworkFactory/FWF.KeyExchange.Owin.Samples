using System;
using FWF.KeyExchange.Owin;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Provide a remote uri to point to
            Uri rootUrl = new Uri("http://localhost:12345/");

            // Create the key exchange environment
            var bootstrapper = new FWFKeyExchangeBootstrapper();

            // Create a random message
            var random = bootstrapper.Resolve<IRandom>();
            var plainTextMessage = random.AnyString(1024);

            // Send the message to remote endpoint once the key exchange
            // has taken place and the message is encrypted over the wire
            using (var http = new KeyExchangeHttpClient(bootstrapper))
            {
                http.SendPayload(rootUrl, plainTextMessage);
            }

            Console.WriteLine("Success!");
        }
    }
}
