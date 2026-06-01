using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace ManageUsers.Controllers
{
    public static class UserEndPoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {

            var group = app.MapGroup("/api/users")
                .WithTags("Users")
                .WithOpenApi();

            group.MapGet("/me", (ClaimsPrincipal user) =>
            {
                var claims = user.Claims.Select(c => new { c.Type, c.Value });
                return Results.Ok(claims);
            }).RequireAuthorization();

            group.MapPost("/createuser", CreateUser)
                .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"))
                .WithName("CreateUser")
                .WithSummary("Create a new user")
                .Produces<CreateUserResponse>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

            group.MapPost("/login", LogIn)
               .WithName("Login")
               .WithSummary("Login a new user")
               .Produces<LoginUserResponse>(StatusCodes.Status200OK)
               .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

            group.MapPut("/resetpassword", ResetPassword)
                 .WithName("ResetPassword")
                .WithSummary("Reset user password")
                .Produces<LoginUserResponse>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        }

        public static async Task<IResult> CreateUser(
            [FromBody] CreateUserRequest request,
            IMediator mediator,
            ClaimsPrincipal claimsPrincipal,
            CancellationToken ct)
        {

            try
            {
                var userId = claimsPrincipal.FindFirst("nameid")?.Value
                ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var createdById))
                {
                    return Results.Problem("Invalid user identity.", statusCode: StatusCodes.Status401Unauthorized);
                }

                var command = new CreateUserCommand(
                   FirstName: request.FirstName,
                   LastName: request.LastName,
                   PhoneNumber: request.PhoneNumber,
                   NationalCode: request.NationalCode,
                   Email: request.Email,
                   PostalCode: request.PostalCode,
                   UserName: request.UserName,
                   Password: request.Password,
                   CreatedById: createdById,
                   UserRoles: request.UserRoles,
                   Enabled: true,
                   CreatedAt: DateTime.UtcNow);

                var result = await mediator.Send(command, ct);
                return Results.Created($"{result.Id}", result);
            }
            catch (DbUpdateException ex)
            {
                return Results.Problem(
                    detail: "Duplicated",
                    statusCode: StatusCodes.Status409Conflict
                    );
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        }


        private static async Task<IResult> LogIn(
            [FromBody] LoginUserRequest crmLoginApplicationUserDTO,
            ISender sender,
            HttpContext context)
        {
            try
            {
                LoginUserResponse response = new LoginUserResponse();
                LoginUserCommand loginUserCommand = new(context, crmLoginApplicationUserDTO);
                response = await sender.Send(loginUserCommand);
                if (!string.IsNullOrEmpty(response.FailedResult))
                {
                    return TypedResults.BadRequest(response);
                }

                return TypedResults.Ok(response);


            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }


        private static async Task<IResult> ResetPassword(
           [FromBody] ChangeUserPasswordRequest changeUserPasswordRequest,
           ClaimsPrincipal claimsPrincipal,
           HttpContext context,
           ISender sender)
        {
            try
            {
                string userId = claimsPrincipal.FindFirst("nameid")?.Value!
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

                ChangeUserPasswordResponse response;
                ChangeUserPasswordCommand changeUserPasswordCommand = new(userId: userId, context: context, changeUserPasswordRequest: changeUserPasswordRequest);
                response = await sender.Send(changeUserPasswordCommand);
                if (!string.IsNullOrEmpty(response.FailedResult))
                {
                    return TypedResults.BadRequest(response);
                }

                return TypedResults.Ok(response);


            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}
