using Application.Commands;
using Application.Handlers;
using Infrastructure.External;

namespace Application.Tests.Handlers;

/// <summary>
/// Unit tests for CreatePostCommandHandler
/// </summary>
public class CreatePostCommandHandlerTests
{
    private readonly Mock<IJsonPlaceholderClient> _mockJsonPlaceholderClient;
    private readonly CreatePostCommandHandler _handler;

    public CreatePostCommandHandlerTests()
    {
        _mockJsonPlaceholderClient = new Mock<IJsonPlaceholderClient>();
        _handler = new CreatePostCommandHandler(_mockJsonPlaceholderClient.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsCreatedPost()
    {
        // Arrange
        var command = new CreatePostCommand(
            UserId: 1,
            Title: "Test Post",
            Body: "This is a test post body");

        var expectedPost = new JsonPlaceholderPost
        {
            Id = 101,
            UserId = command.UserId,
            Title = command.Title,
            Body = command.Body
        };

        _mockJsonPlaceholderClient
            .Setup(x => x.CreatePostAsync(
                It.Is<CreateJsonPlaceholderPostRequest>(r => 
                    r.UserId == command.UserId &&
                    r.Title == command.Title &&
                    r.Body == command.Body),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPost);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(expectedPost.Id);
        result.Value.UserId.Should().Be(command.UserId);
        result.Value.Title.Should().Be(command.Title);
        result.Value.Body.Should().Be(command.Body);

        _mockJsonPlaceholderClient.Verify(
            x => x.CreatePostAsync(
                It.IsAny<CreateJsonPlaceholderPostRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ExternalApiError_ReturnsError()
    {
        // Arrange
        var command = new CreatePostCommand(
            UserId: 1,
            Title: "Test Post",
            Body: "Test Body");

        var errorMessage = "External API error";

        _mockJsonPlaceholderClient
            .Setup(x => x.CreatePostAsync(
                It.IsAny<CreateJsonPlaceholderPostRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Failure);
        result.FirstError.Code.Should().Be("ExternalApi.CreatePostError");
        result.FirstError.Description.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task Handle_ValidRequest_MapsCommandToRequest()
    {
        // Arrange
        var command = new CreatePostCommand(
            UserId: 5,
            Title: "My Post Title",
            Body: "My post body content");

        var expectedPost = new JsonPlaceholderPost
        {
            Id = 200,
            UserId = 5,
            Title = "My Post Title",
            Body = "My post body content"
        };

        CreateJsonPlaceholderPostRequest? capturedRequest = null;

        _mockJsonPlaceholderClient
            .Setup(x => x.CreatePostAsync(
                It.IsAny<CreateJsonPlaceholderPostRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<CreateJsonPlaceholderPostRequest, CancellationToken>((req, ct) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(expectedPost);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserId.Should().Be(command.UserId);
        capturedRequest.Title.Should().Be(command.Title);
        capturedRequest.Body.Should().Be(command.Body);
    }
}

