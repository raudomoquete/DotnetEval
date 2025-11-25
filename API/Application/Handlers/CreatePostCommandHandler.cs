using Application.Commands;
using ErrorOr;
using Infrastructure.External;

namespace Application.Handlers;

/// <summary>
/// Handler for CreatePostCommand
/// </summary>
public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, ErrorOr<CreatePostCommandResult>>
{
    private readonly IJsonPlaceholderClient _jsonPlaceholderClient;

    public CreatePostCommandHandler(IJsonPlaceholderClient jsonPlaceholderClient)
    {
        _jsonPlaceholderClient = jsonPlaceholderClient;
    }

    public async Task<ErrorOr<CreatePostCommandResult>> Handle(
        CreatePostCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var createRequest = new CreateJsonPlaceholderPostRequest
            {
                UserId = request.UserId,
                Title = request.Title,
                Body = request.Body
            };

            var createdPost = await _jsonPlaceholderClient.CreatePostAsync(createRequest, cancellationToken);

            return new CreatePostCommandResult(
                Id: createdPost.Id,
                UserId: createdPost.UserId,
                Title: createdPost.Title,
                Body: createdPost.Body
            );
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "ExternalApi.CreatePostError",
                description: $"Error al crear el post en la API externa: {ex.Message}");
        }
    }
}

