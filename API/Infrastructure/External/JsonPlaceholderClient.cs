using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.External;

/// <summary>
/// Implementation of JsonPlaceholder API client
/// </summary>
public class JsonPlaceholderClient : IJsonPlaceholderClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonPlaceholderClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // BaseAddress and Timeout are configured in DependencyInjection
        // HttpClient is configured via AddHttpClient extension method

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<JsonPlaceholderPost>> GetPostsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/posts", cancellationToken);
            response.EnsureSuccessStatusCode();

            var posts = await response.Content.ReadFromJsonAsync<List<JsonPlaceholderPost>>(_jsonOptions, cancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize posts response");

            return posts;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Error calling JsonPlaceholder API: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new InvalidOperationException("Request to JsonPlaceholder API timed out", ex);
        }
    }

    public async Task<JsonPlaceholderPost> CreatePostAsync(CreateJsonPlaceholderPostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/posts", request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var createdPost = await response.Content.ReadFromJsonAsync<JsonPlaceholderPost>(_jsonOptions, cancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize create post response");

            return createdPost;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Error calling JsonPlaceholder API: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new InvalidOperationException("Request to JsonPlaceholder API timed out", ex);
        }
    }
}

