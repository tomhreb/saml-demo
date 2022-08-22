using ITfoxtec.Identity.Saml2;

namespace Saml;

public class CustomSaml2Configuration : Saml2Configuration
{
    public string SigningCertificateThumbprint { get; set; }
}