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

        public static async Task<Results<Ok<APIResponse<CreateUserResponse>>, BadRequest<APIResponse<CreateUserResponse>>>> CreateUser(
            [FromBody] CreateUserRequest request,
            IMediator mediator,
            ClaimsPrincipal claimsPrincipal,
            CancellationToken ct)
        {
            APIResponse<CreateUserResponse> response = new();

            try
            {
                var userId = claimsPrincipal.FindFirst("nameid")?.Value
                ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

                CreateUserResponse createUserResponse = await mediator.Send(command, ct);
                if (!string.IsNullOrEmpty(createUserResponse.FailedResult))
                {
                    response.ErrorMessage = [createUserResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = createUserResponse;
                return TypedResults.Ok(response);
            }

            catch
            {
                throw;
            }

        }


        private static async Task<Results<Ok<APIResponse<LoginUserResponse>>, BadRequest<APIResponse<LoginUserResponse>>>> Login(
            [FromBody] LoginUserRequest loginUserDTO,
            ISender sender,
            HttpContext context)
        {
            APIResponse<LoginUserResponse> response = new();
            try
            {
                LoginUserResponse loginUserResponse = new LoginUserResponse();
                LoginUserCommand loginUserCommand = new(context, loginUserDTO);
                loginUserResponse = await sender.Send(loginUserCommand);
                if (!string.IsNullOrEmpty(loginUserResponse.FailedResult))
                {
                    response.ErrorMessage = [loginUserResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Result.Data = loginUserResponse;
                    return TypedResults.BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                return TypedResults.Ok(response);


            }
            catch
            {
                throw;
            }
        }


        private static async Task<Results<Ok<APIResponse<ChangeUserPasswordResponse>>, BadRequest<APIResponse<ChangeUserPasswordResponse>>>> ChangePassword(
           [FromBody] ChangeUserPasswordRequest changeUserPasswordRequest,
           ClaimsPrincipal claimsPrincipal,
           HttpContext context,
           ISender sender)
        {
            APIResponse<ChangeUserPasswordResponse> response = new();

            try
            {
                string userId = claimsPrincipal.FindFirst("nameid")?.Value!
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

                ChangeUserPasswordCommand changeUserPasswordCommand = new(userId: userId, context: context, changeUserPasswordRequest: changeUserPasswordRequest);
                ChangeUserPasswordResponse changeUserPasswordResponse = await sender.Send(changeUserPasswordCommand);
                if (!string.IsNullOrEmpty(changeUserPasswordResponse.FailedResult))
                {
                    response.ErrorMessage = [changeUserPasswordResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = changeUserPasswordResponse;
                return TypedResults.Ok(response);


            }
            catch
            {
                throw;
            }
        }


        private static async Task<Results<Ok<APIResponse<ForgotPasswordResponse>>, BadRequest<APIResponse<ForgotPasswordResponse>>>> ForgotPassword(
            [FromBody] ForgotPasswordRequest forgotPasswordRequest,
            HttpContext context,
            ISender sender)
        {
            APIResponse<ForgotPasswordResponse> response = new();

            try
            {
                var command = new ForgotPasswordUserCommand(forgotPasswordRequest.Email, context);
                ForgotPasswordResponse forgotPasswordResponse = await sender.Send(command);

                if (!string.IsNullOrEmpty(forgotPasswordResponse.FailedResult))
                {
                    response.ErrorMessage = [forgotPasswordResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = forgotPasswordResponse;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<ResetPasswordResponse>>, BadRequest<APIResponse<ResetPasswordResponse>>>> ResetPassword(
            [FromQuery] string token,
            [FromBody] ResetPasswordRequest request,
            HttpContext context,
            ISender sender)
        {
            APIResponse<ResetPasswordResponse> response = new();

            try
            {
                var command = new ResetPasswordUserCommand(
                    request.Email,
                    token,
                    request.NewPassword,
                    context
                );

                ResetPasswordResponse resetPasswordResponse = await sender.Send(command);

                if (!string.IsNullOrEmpty(resetPasswordResponse.FailedResult))
                {
                    response.ErrorMessage = [resetPasswordResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = resetPasswordResponse;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }


        private static async Task<Results<Ok<APIResponse<List<GetRolePermissionsResponse>>>, NotFound<APIResponse<List<GetRolePermissionsResponse>>>>> GetRolePermissions(
            [FromQuery] List<int> roleIds,
            ISender sender,
            CancellationToken ct,
             int pageNumber = 1,
             int pageSize = 10)
        {
            APIResponse<List<GetRolePermissionsResponse>> response = new();
            try
            {
                GetRolePermissionsQuery getRolePermissionsQuery = new(roleIds: new List<int>(roleIds));
                List<GetRolePermissionsResponse> rolePermisionsResponse = await sender.Send(getRolePermissionsQuery);

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = rolePermisionsResponse;
                response.Result.pagination.TotalRecords = rolePermisionsResponse.Count() == 0 ? 0 : rolePermisionsResponse.First().TotalRecords;
                response.Result.pagination.PageNumber = pageNumber;
                response.Result.pagination.PageSize = pageSize;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetRolesResponse>>, NotFound<APIResponse<List<GetRolesResponse>>>>> GetRoles(
          ISender sender,
          ClaimsPrincipal claimsPrincipal,
          CancellationToken ct,
          int pageNumber = 1,
          int pageSize = 10)
        {
            APIResponse<GetRolesResponse> response = new();
            try
            {
                string userId = claimsPrincipal.FindFirst("nameid")?.Value!
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

                GetRolesResponse rolesResponse = await sender.Send(new GetRolesQuery(userId: userId));

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data.Items = rolesResponse.Items;
                response.Result.pagination.TotalRecords = rolesResponse.Items.Count() == 0 ? 0 : rolesResponse.Items.First().TotalRecords;
                response.Result.pagination.PageNumber = pageNumber;
                response.Result.pagination.PageSize = pageSize;
                return TypedResults.Ok(response);

            }
            catch
            {
                throw;
            }
        }
    }
}
