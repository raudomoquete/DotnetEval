using API.DTOs;
using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for posts operations (requires JWT authentication)
/// </summary>
[Authorize]
public class PostsController : ApiController
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all posts from external API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of posts</returns>
    /// <response code="200">Returns the list of posts</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPosts(CancellationToken cancellationToken)
    {
        var query = new GetPostsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            value => Ok(value.Posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Body = p.Body
            }).ToList()),
            errors => Problem(errors));
    }

    /// <summary>
    /// Creates a new post in external API
    /// </summary>
    /// <param name="request">Post data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created post</returns>
    /// <response code="200">Returns the created post</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePost(
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePostCommand(
            UserId: request.UserId,
            Title: request.Title,
            Body: request.Body
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            value => Ok(new PostResponse
            {
                Id = value.Id,
                UserId = value.UserId,
                Title = value.Title,
                Body = value.Body
            }),
            errors => Problem(errors));
    }
}

