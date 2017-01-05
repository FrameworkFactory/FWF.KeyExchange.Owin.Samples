using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using FWF.KeyExchange.Owin;
using System.Net.Http;

namespace WebApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Provide a remote uri to point to
            Uri rootUrl = new Uri("http://localhost:23456/");

            // Create the key exchange environment
            var bootstrapper = new FWFKeyExchangeBootstrapper();
            var keyExchangeProvider = bootstrapper.Resolve<IKeyExchangeProvider>();
            var endpointIdProvider = bootstrapper.Resolve<IEndpointIdProvider>();
            var symmetricEncryptionProvider = bootstrapper.Resolve<ISymmetricEncryptionProvider>();

            // Create a random message
            var random = bootstrapper.Resolve<IRandom>();
            var plainTextMessage = random.AnyString(1024);
            var plainTextData = Encoding.UTF8.GetBytes(plainTextMessage);

            // Send the message to remote endpoint once the key exchange
            // has taken place and the message is encrypted over the wire
            using (var http = new KeyExchangeHttpClient(bootstrapper))
            {
                // Perform the key exchange first to make a shared key available
                http.SetupExchange(rootUrl);

                // Get a unique id for the remote endpoint
                var endpointId = endpointIdProvider.DetermineEndpointId(rootUrl);

                // Get the shared key from the key exchange
                var sharedKey = keyExchangeProvider.GetEndpointSharedKey(endpointId);

                // Encrypt the payload with some salt too
                byte[] iv;
                var encryptedData = symmetricEncryptionProvider.Encrypt(
                    sharedKey,
                    plainTextData,
                    out iv
                    );

                // Perpare the data
                var ivString = Convert.ToBase64String(iv);
                var encodedIv = HttpUtility.UrlEncode(ivString);

                // Use the url for the Web Api MessageController
                var messageUrl = new Uri(rootUrl.ToString() + "msg?iv=" + encodedIv);
                
                using (var httpResponse = http.PostAsync(messageUrl, new ByteArrayContent(encryptedData)).Result)
                {
                }
            }

            Console.WriteLine("Success!");
        }
    }
}
