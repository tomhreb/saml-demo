using System.Security.Authentication;
using System.Security.Claims;
using System.ServiceModel.Security;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.Extensions.Options;
using Saml;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAuthentication("saml2").AddCookie("saml2");

services.Configure<CustomSaml2Configuration>(configuration.GetSection("Saml2"));

services.Configure<CustomSaml2Configuration>(config =>
{
    var entityDescriptor = new EntityDescriptor();
    entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(configuration["Saml2:IdPMetadata"]));
    if (entityDescriptor.IdPSsoDescriptor != null)
    {
        config.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
        config.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);

        config.CertificateValidationMode = X509CertificateValidationMode.Custom;
        config.CustomCertificateValidator = new CustomCertificateValidator(config.SigningCertificateThumbprint);
    }
    else
    {
        throw new Exception("IdPSsoDescriptor not loaded from metadata.");
    }
});

var app = builder.Build();

app.UseSaml2();

app.MapGet("/", () => "Well hello!");

app.MapGet("/user-info/", (ClaimsPrincipal user) =>
{
    if (user.Identity is { IsAuthenticated: true })
    {
        var claims = string.Join(Environment.NewLine, user.Claims.Select(claim => $"{claim.Type} = {claim.Value}"));

        return Results.Text($"Hi, this is {user.Identity.Name}{Environment.NewLine}{claims}");
    }

    return Results.Text("User is not authenticated");
});

app.MapPost("/saml-acs/", async ctx =>
{
    var binding = new Saml2PostBinding();
    var saml2AuthnResponse = new Saml2AuthnResponse(ctx.RequestServices.GetRequiredService<IOptions<CustomSaml2Configuration>>().Value);

    binding.ReadSamlResponse(ctx.Request.ToGenericHttpRequest(), saml2AuthnResponse);
    if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
    {
        throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
    }
    binding.Unbind(ctx.Request.ToGenericHttpRequest(), saml2AuthnResponse);
    await saml2AuthnResponse.CreateSession(ctx, claimsTransform: ClaimsTransform.Transform);

    ctx.Response.Redirect("/user-info/");
});

app.MapGet("/logout/", async ctx =>
{
    var logoutRequest = new Saml2LogoutRequest(ctx.RequestServices.GetRequiredService<IOptions<CustomSaml2Configuration>>().Value, ctx.User);
    await logoutRequest.DeleteSession(ctx);

    ctx.Response.Redirect("/user-info/");
});

app.Run();