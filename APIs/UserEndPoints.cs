using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
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

            group.MapPost("/login", Login)
               .WithName("Login")
               .WithSummary("Login a new user")
               .Produces<LoginUserResponse>(StatusCodes.Status200OK)
               .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

            group.MapPut("/changepassword", ChangePassword)
                 .WithName("ChangePassword")
                .WithSummary("Change user password")
                .Produces<LoginUserResponse>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);


            group.MapPut("/forgotpassword", ForgotPassword)
                .WithName("ForgotPassword")
               .WithSummary("Forgot user password")
               .Produces<LoginUserResponse>(StatusCodes.Status200OK)
               .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);


            group.MapPut("/resetpassword", ResetPassword)
               .WithName("ResetPassword")
              .WithSummary("Reset user password")
              .Produces<LoginUserResponse>(StatusCodes.Status200OK)
              .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

            group.MapGet("/{roleId:int}/permissions", GetRolePermissions)
                .WithName("GetRolePermissions")
                .WithSummary("Get role Permissions")
                .Produces<GetRolePermissionsResponse>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);


            group.MapGet("/roles", GetRoles)
                .WithName("GetRoles")
                .WithSummary("Get roles")
                .Produces<RoleDto>(StatusCodes.Status200OK)
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

                if (string.IsNullOrEmpty(userId))
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
                   Position: request.Position,
                   PersonalCode: request.PersonalCode,
                   OrganizationId: request.OrganizationId,
                   AreaId: request.AreaId,
                   RegionId: request.RegionId,
                   UserName: request.UserName,
                   Password: request.Password,
                   Description: request.Description,
                   CreatedById: userId,
                   UserRoleIds: request.UserRoleIds,
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


        private static async Task<IResult> Login(
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


        private static async Task<IResult> ChangePassword(
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


        private static async Task<IResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest forgotPasswordRequest,
            HttpContext context,
            ISender sender)
        {
            try
            {
                var command = new ForgotPasswordUserCommand(forgotPasswordRequest.Email, context);
                var response = await sender.Send(command);

                if (!string.IsNullOrEmpty(response.FailedResult))
                {
                    return TypedResults.BadRequest(new { error = response.FailedResult });
                }

                return TypedResults.Ok(response);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(
                    detail: "????? ?? ?????? ??????? ?? ???.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> ResetPassword(
            [FromQuery] string token,
            [FromBody] ResetPasswordRequest request,
            HttpContext context,
            ISender sender)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return TypedResults.BadRequest(new { error = "???? ?????? ???" });
                }

                var command = new ResetPasswordUserCommand(
                    request.Email,
                    token,
                    request.NewPassword,
                    context
                );

                var response = await sender.Send(command);

                if (!string.IsNullOrEmpty(response.FailedResult))
                {
                    return TypedResults.BadRequest(new { error = response.FailedResult });
                }

                return TypedResults.Ok(response);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(
                    detail: "????? ?? ?????? ??????? ?? ???.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }


        private static async Task<IResult> GetRolePermissions(
            [FromQuery] List<int> roleIds,
            ISender sender,
            CancellationToken ct)
        {
            List<GetRolePermissionsResponse> response = new();
            try
            {
                GetRolePermissionsQuery getRolePermissionsQuery = new(roleIds: new List<int>(roleIds));
                response = await sender.Send(getRolePermissionsQuery);
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }


        private static async Task<IResult> GetRoles(
          ISender sender,
          ClaimsPrincipal claimsPrincipal,
          CancellationToken ct)
        {
            GetRolesResponse response = new();
            try
            {
                string userId = claimsPrincipal.FindFirst("nameid")?.Value!
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

                response = await sender.Send(new GetRolesQuery(userId: userId));
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }
    }
}
