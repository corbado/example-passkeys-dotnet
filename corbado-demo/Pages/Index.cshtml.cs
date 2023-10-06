using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Diagnostics;

namespace corbado_demo.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
	public string? RequestId { get; set; }

	public string? userID { get; set; }
	public string? userName { get; set; }
	public string? userEmail { get; set; }

	public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

	private readonly ILogger<IndexModel> _logger;

	private readonly IHttpContextAccessor _httpContextAccessor;

	public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContextAccessor)
	{
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
	}


	public async Task OnGet()
	{

        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
		try
		{
			string projectID = Environment.GetEnvironmentVariable("CORBADO_PROJECT_ID");

			string issuer = $"https://{projectID}.frontendapi.corbado.io";
			string jwksUri = $"https://{projectID}.frontendapi.corbado.io/.well-known/jwks";

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
					ValidateAudience = false,
					ValidateLifetime = true,
					IssuerSigningKey = new RsaSecurityKey(rsaParameters),
					ClockSkew = TimeSpan.Zero,
					RequireSignedTokens = true,
					RequireExpirationTime = true,
					ValidateIssuerSigningKey = true
				};

				try
				{
					var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

					userID = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
					userName = claimsPrincipal.FindFirst("name")?.Value;
					userEmail = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
				}
				catch (SecurityTokenValidationException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
	   }
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
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


