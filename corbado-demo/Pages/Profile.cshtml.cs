using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace corbado_demo.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ProfileModel : PageModel
{
    public string? RequestId { get; set; }

    public string? userID { get; set; }
    public string? userName { get; set; }
    public string? userEmail { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ProfileModel> _logger;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileModel(ILogger<ProfileModel> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;

        Console.WriteLine("TEST ERROR CREATED");
        System.Diagnostics.Debug.WriteLine("TEST ERROR CREATED");



        // Get all environment variables and print them to the console
        foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
        {
            string key = envVar.ToString();
            string value = Environment.GetEnvironmentVariable(key);
            Console.WriteLine($"{key} = {value}");
        }

        string? v = Environment.GetEnvironmentVariable("CORBADO_PROJECT_ID");
        Console.WriteLine(v);
    }


    public async Task OnGet()
    {
        try
        {
            string projectID = Environment.GetEnvironmentVariable("CORBADO_PROJECT_ID");

            string issuer = $"https://{projectID}.frontendapi.corbado.io";
            string jwksUri = $"https://{projectID}.frontendapi.corbado.io/.well-known/jwks";

            // Fetch JSON from the jwksUri
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(jwksUri);
                var json = JObject.Parse(response);
                var publicKey = json["keys"][0]["n"].ToString();
                var publicKeyBase64 = Base64UrlToBase64(publicKey);

                var rsaParameters = new RSAParameters
                {
                    Exponent = Convert.FromBase64String(json["keys"][0]["e"].ToString()),
                    Modulus = Convert.FromBase64String(publicKeyBase64)
                };

                var token = _httpContextAccessor.HttpContext.Request.Cookies["cbo_short_session"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = false, // Set to true if audience validation is required
                    ValidateLifetime = true,
                    IssuerSigningKey = new RsaSecurityKey(rsaParameters),
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true
                };

                Console.WriteLine("validating...");

                try
                {
                    var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                    // Print all claims to the console
                    foreach (var claim in claimsPrincipal.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }

                    userID = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                    userName = claimsPrincipal.FindFirst("name")?.Value;
                    userEmail = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                }
                catch (SecurityTokenValidationException ex)
                {
  //                  return BadRequest(new { error = "JWT token is not valid!" });
                }
            }

  //         return new JsonResult(new { user_id = userID, user_name = userName, user_email = userEmail });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
  //          return BadRequest(new { error = ex.Message });
        }
    }

    // Helper function to convert Base64Url to Base64
    private string Base64UrlToBase64(string base64Url)
    {
        base64Url = base64Url.Replace('-', '+').Replace('_', '/');
        while (base64Url.Length % 4 != 0)
        {
            base64Url += '=';
        }
        return base64Url;
    }

}


