using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Extentions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

            group.MapPost("/createuser", CreateUser)
                .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"))
                .WithName("CreateUser")
                .WithSummary("Create a new user")
                .Validator<CreateUserRequest>();


            group.MapPost("/login", Login)
               .WithDisplayName("ورود")
               .WithSummary("Login with username/password")
               .Validator<LoginUserRequest>();


            group.MapPost("/request-otp", RequestOTPCode)
                 .AllowAnonymous()
                 .WithDisplayName("درخواست کد")
                .WithSummary("Verify OTP code to complete 2FA login and receive JWT token")
                .Validator<RequestOTPCode>();


            group.MapPut("/changepassword", ChangePassword)
                 .RequireAuthorization()
                 .WithDisplayName("تغییر رمز عبور")
                 .WithSummary("Change user password")
                 .Validator<ChangeUserPasswordRequest>();

            group.MapPut("/forgotpassword", ForgotPassword)
                .WithDisplayName("فراموشی رمز عبور")
                .WithSummary("Request a password reset email")
                .Validator<ForgotPasswordRequest>();

            group.MapPut("/resetpassword", ResetPassword)
                .WithDisplayName("تغییر کامل رمز عبور")
              .WithSummary("Reset user password")
              .Validator<ResetPasswordRequest>();

            group.MapGet("/{roleId:int}/permissions", GetRolePermissions)
                .RequireAuthorization()
                .WithDisplayName("دریافت دسترسی های نقش مشخص")
                .WithSummary("Get permissions for a specific role");

            group.MapGet("/roles", GetRoles)
                .RequireAuthorization()
                .WithDisplayName("دریافت نقش ها")
                .WithSummary("Get roles for current user");
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
                response.Result.Data = loginUserResponse;
                return TypedResults.Ok(response);
            }

            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<RequestOTPCodeResponse>>, BadRequest<APIResponse<RequestOTPCodeResponse>>>> RequestOTPCode(
            [FromBody] RequestOTPCode request,
            ISender sender)
        {
            APIResponse<RequestOTPCodeResponse> response = new();

            try
            {

                var command = new RequestOtpCodeCommand(request.PhoneNumber);
                RequestOTPCodeResponse verifyOtpResponse = await sender.Send(command);

                if (!string.IsNullOrEmpty(verifyOtpResponse.FailedResult))
                {
                    response.ErrorMessage = [verifyOtpResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Result.Data = verifyOtpResponse;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = verifyOtpResponse;
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
            [FromQuery] string[] roleIds,
            ISender sender,
            CancellationToken ct,
             int pageNumber = 1,
             int pageSize = 10)
        {
            APIResponse<List<GetRolePermissionsResponse>> response = new();
            try
            {
                GetRolePermissionsQuery getRolePermissionsQuery = new(roleIds: roleIds.ToList());
                List<GetRolePermissionsResponse> rolePermisionsResponse = await sender.Send(getRolePermissionsQuery);

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = rolePermisionsResponse;
                response.Result.pagination.TotalRecords = rolePermisionsResponse.Count == 0 ? 0 : rolePermisionsResponse.First().TotalRecords;
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
                response.Result.pagination.TotalRecords = rolesResponse.Items.Count == 0 ? 0 : rolesResponse.Items.First().TotalRecords;
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
