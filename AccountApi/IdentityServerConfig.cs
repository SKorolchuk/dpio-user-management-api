using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.AccountApi
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("settings", "Settings API"),
                new ApiResource("operations", "Operations API"),
                new ApiResource("files", "Files API"),
                new ApiResource("auth", "Auth API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(configuration.GetValue<string>("Secret").Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = {"settings", "operations", "files", "auth"}
                },
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret(configuration.GetValue<string>("Secret").Sha256())
                    },
                    AllowedScopes = {"settings", "operations", "files", "auth"}
                },
                // SPA Client
                new Client
                {
                    ClientId = "dpio-application",
                    ClientName = "JS Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {$"{configuration.GetValue<string>("ApplicationAddress")}/home"},
                    PostLogoutRedirectUris = {$"{configuration.GetValue<string>("ApplicationAddress")}/login"},
                    AllowedCorsOrigins = {$"{configuration.GetValue<string>("ApplicationAddress")}"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "settings",
                        "operations",
                        "files",
                        "auth"
                    }
                }
            };
        }
    }
}
