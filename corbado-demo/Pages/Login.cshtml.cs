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

namespace corbado_demo.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    public string? RequestId { get; set; }


    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<LoginModel> _logger;


    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;

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

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }

}


