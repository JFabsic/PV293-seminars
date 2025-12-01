using Library.BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Endpoints;

public static class AuthorsEndpoints
{
    public static void MapAuthorsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authors").WithTags("Authors");

        group.MapGet("/", GetAuthors)
            .WithName("GetAuthors")
            .WithSummary("Get all authors with their statistics")
            .WithOpenApi();
    }

    private static async Task<IResult> GetAuthors([FromServices] IAuthorService authorService)
    {
        var authors = await authorService.GetAllAuthorsAsync();
        return Results.Ok(authors);
    }
}