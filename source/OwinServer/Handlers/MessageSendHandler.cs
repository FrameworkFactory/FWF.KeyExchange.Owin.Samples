using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using FWF.KeyExchange.Owin;

namespace FWF.KeyExchange.Sample.OwinApi.Handlers
{
    internal class MessageSendHandler
    {

        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public Task Handle(IOwinContext context, Func<Task> next)
        {
            var requestPath = context.Request.Path.Value.ToLowerInvariant();
            var requestMethod = context.Request.Method;

            if (requestMethod == "POST" && requestPath == "/send")
            {
                // Get the KeyExchange components from the AutoFac container saved in OWIN context
                var keyExchangeProvider = context.Environment[OwinKeyExchangeEnvironment.KeyExchangeProvider] as IKeyExchangeProvider;
                var owinEndpointIdProvider = context.Environment[OwinKeyExchangeEnvironment.EndpointIdProvider] as IOwinEndpointIdProvider;
                var symmetricEncryptionProvider = context.Environment[OwinKeyExchangeEnvironment.SymmetricEncryptionProvider] as ISymmetricEncryptionProvider;
                                
                // Get encrypted message from POST data
                var ivString = context.Request.Query["iv"];

                if (ivString.IsMissing())
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Headers.Set("Content-Type", "text/plain");
                    context.Response.ContentType = "text/plain";

                    context.Response.Write("Must supply the iv query parameter");

                    return Task.CompletedTask;
                }

                //
                var iv = Convert.FromBase64String(ivString);

                // NOTE: Not the best implementation to receive the entire request and parse
                // as a string...
                byte[] messageData;
                using (var stream = new MemoryStream())
                {
                    context.Request.Body.CopyTo(stream);

                    messageData = stream.ToArray();
                }

                //
                var endpointId = owinEndpointIdProvider.DetermineEndpointId(context);

                // 
                var sharedKey = keyExchangeProvider.GetEndpointSharedKey(endpointId);

                // Decrypt the message
                var decryptedData = symmetricEncryptionProvider.Decrypt(
                    sharedKey,
                    iv,
                    messageData
                    );

                string decryptedMessage = _defaultEncoding.GetString(decryptedData);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Headers.Set("Content-Type", "text/plain");
                context.Response.ContentType = "text/plain";

                context.Response.Write(decryptedMessage);

                return Task.CompletedTask;
            }

            return next();
        }


    }
}
