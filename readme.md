# Biblioteka ułatwiająca integrację z profilem zaufanym

*Disclaimer*
Biblioteka powstała na potrzeby jednego z systemów jakie tworzyłem. Więc może nie uwzględniać częśći API. Chętnie zaakceptuję pull-request

## Użycie
```
var certificatePath = "PATH_TO_PFX_OR_P12_FILE";
var certificateData = File.ReadAllBytes(certificatePath);
var certificate = new X509Certificate2(certificateData, "password");
using (var client = SoapClient.Prepare())
{
	client.WithBinarySecurityTokenHeader(certificate);

	var envelope = SoapEnvelope
		.Prepare()
		.Body(new SampleRequest());

	var soapResponse = await client.SendAsync("https://some-server.org", "action", )
}
```