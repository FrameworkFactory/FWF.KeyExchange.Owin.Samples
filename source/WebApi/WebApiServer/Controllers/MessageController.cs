using System;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using FWF.KeyExchange.Owin;

namespace WebApiServer.Controllers
{
    [RoutePrefix("msg")]
    public class MessageController : ApiController
    {
        
        // POST /
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromUri] string iv, [FromBody]byte[] encryptedMessage)
        {
            var owinContext = Request.GetOwinContext();

            // Because of the key exchange, I can expect that the message I recieve
            // here is encrypted with the runtime shared key.  Next steps are to:
            // - Determine the unique id property of the calling client
            // - Ensure that the remote endpoint has a shared key
            // - Decrypt the message with the shared key

            var boostrapper = Program.FwfKeyExchangeBootstrapper;

            var keyExchangeProvider = boostrapper.Resolve<IKeyExchangeProvider>();
            var endpointIdProvider = boostrapper.Resolve<IEndpointIdProvider>();
            var symmetricEncryptionProvider = boostrapper.Resolve<ISymmetricEncryptionProvider>();

            // Determine the unique id of the calling client
            var endpointId = endpointIdProvider.DetermineEndpointId(owinContext);

            // Determine if the client has a shared key
            var hasSharedKey = keyExchangeProvider.IsEndpointConfigured(endpointId);

            // Get the shared key
            var sharedKey = keyExchangeProvider.GetEndpointSharedKey(endpointId);

            // Assume the IV (salt) of the message was sent as Base64
            var ivData = Convert.FromBase64String(iv);

            var decryptedData = symmetricEncryptionProvider.Decrypt(
                sharedKey,
                ivData,
                encryptedMessage
                );

            return Json(string.Empty);
        }

    }

}
