namespace Infrastructure.External;

/// <summary>
/// Interface for consuming JsonPlaceholder API
/// </summary>
public interface IJsonPlaceholderClient
{
    /// <summary>
    /// Gets all posts from JsonPlaceholder API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of posts</returns>
    Task<List<JsonPlaceholderPost>> GetPostsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new post in JsonPlaceholder API
    /// </summary>
    /// <param name="request">Post data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created post</returns>
    Task<JsonPlaceholderPost> CreatePostAsync(CreateJsonPlaceholderPostRequest request, CancellationToken cancellationToken = default);
}

