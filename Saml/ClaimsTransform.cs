﻿using System.Security.Claims;
using ITfoxtec.Identity.Saml2.Claims;

namespace Saml;

public static class ClaimsTransform
{
    public static ClaimsPrincipal Transform(ClaimsPrincipal incomingPrincipal)
    {
        if (!incomingPrincipal.Identity.IsAuthenticated)
        {
            return incomingPrincipal;
        }

        return CreateClaimsPrincipal(incomingPrincipal);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(ClaimsPrincipal incomingPrincipal)
    {
        var claims = new List<Claim>();

        // All claims
        claims.AddRange(incomingPrincipal.Claims);

        // Or custom claims
        //claims.AddRange(GetSaml2LogoutClaims(incomingPrincipal));
        //claims.Add(new Claim(ClaimTypes.NameIdentifier, GetClaimValue(incomingPrincipal, ClaimTypes.NameIdentifier)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, incomingPrincipal.Identity.AuthenticationType, ClaimTypes.NameIdentifier, ClaimTypes.Role)
        {
            BootstrapContext = ((ClaimsIdentity)incomingPrincipal.Identity).BootstrapContext
        });
    }

    private static IEnumerable<Claim> GetSaml2LogoutClaims(ClaimsPrincipal principal)
    {
        yield return GetClaim(principal, Saml2ClaimTypes.NameId);
        yield return GetClaim(principal, Saml2ClaimTypes.NameIdFormat);
        yield return GetClaim(principal, Saml2ClaimTypes.SessionIndex);
    }

    private static Claim GetClaim(ClaimsPrincipal principal, string claimType)
    {
        return ((ClaimsIdentity)principal.Identity).Claims.FirstOrDefault(c => c.Type == claimType);
    }

    private static string GetClaimValue(ClaimsPrincipal principal, string claimType)
    {
        var claim = GetClaim(principal, claimType);
        return claim != null ? claim.Value : null;
    }
}
