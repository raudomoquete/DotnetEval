using Application.Queries;
using ErrorOr;
using Infrastructure.External;

namespace Application.Handlers;

/// <summary>
/// Handler for GetPostsQuery
/// </summary>
public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, ErrorOr<GetPostsQueryResult>>
{
    private readonly IJsonPlaceholderClient _jsonPlaceholderClient;

    public GetPostsQueryHandler(IJsonPlaceholderClient jsonPlaceholderClient)
    {
        _jsonPlaceholderClient = jsonPlaceholderClient;
    }

    public async Task<ErrorOr<GetPostsQueryResult>> Handle(
        GetPostsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var posts = await _jsonPlaceholderClient.GetPostsAsync(cancellationToken);

            var postDtos = posts.Select(p => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Body = p.Body
            }).ToList();

            return new GetPostsQueryResult(Posts: postDtos);
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "ExternalApi.GetPostsError",
                description: $"Error al obtener los posts de la API externa: {ex.Message}");
        }
    }
}

