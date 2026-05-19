using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Api;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        _http = new HttpClient
        {
            // This is dev only eh
            BaseAddress = new Uri("https://thirstforlime.servequake.com:55555/")
        };
    }

    protected void ApplyAuthToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    protected HttpClient Http => _http;
}