using AzureQuest.Web.Data;
using AzureQuest.Web.Models;
using AzureQuest.Web.Services.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AzureQuest.Web.Services
{
    public class IdentityConfigurationService : IIdentityConfigurationService
    {
        public void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
        {

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public void ConfigureIdentityAuth0(IServiceCollection services, IConfiguration configuration)
        {
            // Add authentication services
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOAuth("Auth0", options => {
                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = configuration["Auth0:ClientId"];
                options.ClientSecret = configuration["Auth0:ClientSecret"];

                // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
                options.CallbackPath = new PathString("/signin-auth0");

                // Configure the Auth0 endpoints                
                options.AuthorizationEndpoint = $"https://{configuration["Auth0:Domain"]}/authorize";
                options.TokenEndpoint = $"https://{configuration["Auth0:Domain"]}/oauth/token";
                options.UserInformationEndpoint = $"https://{configuration["Auth0:Domain"]}/userinfo";

                // To save the tokens to the Authentication Properties we need to set this to true
                // See code in OnTicketReceived event below to extract the tokens and save them as Claims
                options.SaveTokens = true;

                // Set scope to openid. See https://auth0.com/docs/scopes
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.Events = new OAuthEvents
                {
                    // When creating a ticket we need to manually make the call to the User Info endpoint to retrieve the user's information,
                    // and subsequently extract the user's ID and email adddress and store them as claims
                    OnCreatingTicket = async context =>
                    {
                        // Retrieve user info
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        // Extract the user info object
                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        // Add the Name Identifier claim
                        var userId = user.Value<string>("sub");
                        if (!string.IsNullOrEmpty(userId))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }

                        // Add the Name claim
                        var email = user.Value<string>("name");
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }
                    }
                };
            });
        }
    }
}
