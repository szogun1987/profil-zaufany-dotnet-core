using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleSOAPClient;
using SimpleSOAPClient.Helpers;

namespace ProfilZaufany.Helpers
{
    public static class SoapClientExtensions
    {
        private static readonly Random Random = new Random();

        public static SoapClient AddCommonHeaders(this SoapClient soapClient)
        {
            return soapClient
                .OnSoapEnvelopeRequest((sClient, arguments, token) =>
                {
                    var bodyElement = arguments.Envelope.Body.Value;

                    bodyElement.Add(new XAttribute("callId", Random.Next()));
                    bodyElement.Add(new XAttribute("requestTimestamp", DateTime.Now));

                    return Task.CompletedTask;
                });
        }
    }
}