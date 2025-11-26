using Application.Handlers;
using Application.Queries;
using Infrastructure.External;

namespace Application.Tests.Handlers;

/// <summary>
/// Unit tests for GetPostsQueryHandler
/// </summary>
public class GetPostsQueryHandlerTests
{
    private readonly Mock<IJsonPlaceholderClient> _mockJsonPlaceholderClient;
    private readonly GetPostsQueryHandler _handler;

    public GetPostsQueryHandlerTests()
    {
        _mockJsonPlaceholderClient = new Mock<IJsonPlaceholderClient>();
        _handler = new GetPostsQueryHandler(_mockJsonPlaceholderClient.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPosts()
    {
        // Arrange
        var query = new GetPostsQuery();
        var expectedPosts = new List<JsonPlaceholderPost>
        {
            new() { Id = 1, UserId = 1, Title = "Post 1", Body = "Body 1" },
            new() { Id = 2, UserId = 1, Title = "Post 2", Body = "Body 2" },
            new() { Id = 3, UserId = 2, Title = "Post 3", Body = "Body 3" }
        };

        _mockJsonPlaceholderClient
            .Setup(x => x.GetPostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPosts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Posts.Should().HaveCount(3);
        result.Value.Posts.Should().BeEquivalentTo(expectedPosts, options => 
            options.ExcludingMissingMembers());

        _mockJsonPlaceholderClient.Verify(
            x => x.GetPostsAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyPostsList_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetPostsQuery();
        var emptyPosts = new List<JsonPlaceholderPost>();

        _mockJsonPlaceholderClient
            .Setup(x => x.GetPostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPosts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Posts.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExternalApiError_ReturnsError()
    {
        // Arrange
        var query = new GetPostsQuery();
        var errorMessage = "External API error";

        _mockJsonPlaceholderClient
            .Setup(x => x.GetPostsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Failure);
        result.FirstError.Code.Should().Be("ExternalApi.GetPostsError");
        result.FirstError.Description.Should().Contain(errorMessage);
    }
}

