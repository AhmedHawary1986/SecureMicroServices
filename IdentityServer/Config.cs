using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients => new Client[] { new Client { 
        ClientId="movieClient",
        AllowedGrantTypes = GrantTypes.ClientCredentials,
        ClientSecrets = {new Secret("secret".Sha256())},
        AllowedScopes= { "movieAPI" }
        },
         new Client
                   {
                       ClientId = "movies_mvc_client",
                       ClientName = "Movies MVC Web App",
                       AllowedGrantTypes = GrantTypes.Hybrid,
                       RequirePkce = false,
                       AllowRememberConsent = false,
              
                       RedirectUris = new List<string>()
                       {
                           "https://localhost:5002/signin-oidc"
                       },
                       PostLogoutRedirectUris = new List<string>()
                       {
                           "https://localhost:5002/signout-callback-oidc"
                       },
                       ClientSecrets = new List<Secret>
                       {
                           new Secret("secret".Sha256())
                       },
                       AllowedScopes = new List<string>
                       {
                           IdentityServerConstants.StandardScopes.OpenId,
                           IdentityServerConstants.StandardScopes.Profile,
                           IdentityServerConstants.StandardScopes.Address,
                           IdentityServerConstants.StandardScopes.Email,
                         
                           "movieAPI",
                           "roles"
                       }
                   }};

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[] { 
            new IdentityResources.OpenId(),
              new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Address(),
             new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" })
            };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[] { };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[] { new ApiScope("movieAPI","movie API")};

        public static List<TestUser> TestUsers => new List<TestUser> { new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "ahawary",
                    Password = "swn",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Ahmed"),
                        new Claim(JwtClaimTypes.FamilyName, "Elhawary")
                    }
                } };
    }
}
