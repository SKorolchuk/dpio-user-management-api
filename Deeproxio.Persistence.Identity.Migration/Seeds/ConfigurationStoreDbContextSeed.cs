using System;
using Deeproxio.Persistence.Identity.Context;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Serilog;

namespace Deeproxio.Persistence.Identity.Migrator.Seeds
{
    public static class ConfigurationStoreDbContextSeed
    {
        public static void SeedData(this ConfigurationStoreDbContext context)
        {
            try
            {
                context.IdentityResources.Add(new IdentityResources.OpenId().ToEntity());
                context.IdentityResources.Add(new IdentityResources.Profile().ToEntity());

                context.ApiResources.Add(new ApiResource("api1", "My API #1").ToEntity());
                // client credentials flow client
                context.Clients.Add(new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                }.ToEntity());
                // SPA client using code flow + pkce
                context.Clients.Add(
                    new Client
                    {
                        ClientId = "spa",
                        ClientName = "SPA Client",
                        ClientUri = "http://identityserver.io",

                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true,
                        RequireClientSecret = false,

                        RedirectUris =
                        {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                        },

                        PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                        AllowedCorsOrigins = { "http://localhost:5002" },

                        AllowedScopes = { "openid", "profile", "api1" }
                    }.ToEntity());
                // MVC client using code flow + pkce
                context.Clients.Add(new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5003/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
                }.ToEntity());

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error occured while saving data.");
            }
        }
    }
}
