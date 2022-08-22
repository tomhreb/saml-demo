using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace Saml;

public class CustomCertificateValidator : X509CertificateValidator
{
    private readonly string _signingCertificateThumbprint;

    public CustomCertificateValidator(string signingCertificateThumbprint)
    {
        _signingCertificateThumbprint = signingCertificateThumbprint;
    }

    public override void Validate(X509Certificate2 certificate)
    {
        if (certificate.Thumbprint != _signingCertificateThumbprint)
        {
            throw new SecurityTokenValidationException("Invalid X509 certificate chain.");
        }
    }
}