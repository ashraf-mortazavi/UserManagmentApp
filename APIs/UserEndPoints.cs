using Azure.Core;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Domain;
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
                .AllowAnonymous()
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
                .WithSummary("Send OTP code to complete 2FA login")
                .Validator<RequestOTPCode>();

            group.MapPost("/verify", VerifyOTPCode)
                .AllowAnonymous()
                .WithDisplayName("تایید کد")
                .WithSummary("Verify OTP code to complete 2FA login and receive JWT token")
                .Validator<VerifyOtpRequest>();

            group.MapPut("/changepassword", ChangePassword)
                 .RequireAuthorization()
                 .WithDisplayName("تغییر رمز عبور")
                 .WithSummary("Change user password")
                 .Validator<ChangeUserPasswordRequest>();

            group.MapPut("/forgotpassword", ForgotPassword)
                .WithDisplayName("فراموشی رمز عبور")
                .WithSummary("ForgotPassword user with email")
                .Validator<ForgotPasswordRequest>();

            group.MapPut("/resetpassword", ResetPassword)
                .RequireAuthorization()
                .WithDisplayName("تغییر کامل رمز عبور")
                .WithSummary("Reset user password")
                .Validator<ResetPasswordRequest>();

            group.MapPost("/{id}/admin-reset-password", AdminResetPassword)
                .RequireAuthorization()
                .WithDisplayName("بازنشانی رمز عبور توسط ادمین")
                .WithSummary("Admin generates a new random password for a user");

            group.MapGet("/{roleId:int}/permissions", GetRolePermissions)
                .RequireAuthorization()
                .WithDisplayName("دریافت دسترسی های نقش مشخص")
                .WithSummary("Get permissions for a specific role");

            group.MapGet("/roles", GetRoles)
                .RequireAuthorization()
                .WithDisplayName("دریافت نقش های کاربر")
                .WithSummary("Get roles for current user");

            app.MapGet("/api/roles", GetAllRoles)
                .WithDisplayName("دریافت تمام نقش ها")
                .WithSummary("Get all available roles for assignment.");

            group.MapGet("/captcha", GenerateCaptchCode)
               .AllowAnonymous()
               .WithDisplayName("دریافت کد امنیتی")
               .WithSummary("Generate Captch Code");

            group.MapGet("", GetUsers)
                .RequireAuthorization()
                .WithDisplayName("دریافت کاربران")
                .WithSummary("Get All Users");

            group.MapGet("/{id}", GetUserById)
                .RequireAuthorization()
                .WithDisplayName("دریافت کاربر")
                .WithSummary("Get user by ID");

            group.MapPut("/{id}", UpdateUser)
                .RequireAuthorization()
                .WithDisplayName("ویرایش کاربر")
                .WithSummary("Update user")
                .Validator<UpdateUserRequest>();

            group.MapGet("/zones", GetZones)
                .WithDisplayName("دریافت مناطق")
                .WithSummary("Get all zones");

            group.MapGet("/areas/{zoneId:int}/zones", GetAreasByZone)
                .RequireAuthorization()
                .WithDisplayName("دریافت نواحی منطقه")
                .WithSummary("Get zones for an area");

            group.MapGet("/profile", GetProfile)
                .RequireAuthorization()
                .WithDisplayName("دریافت پروفایل کاربر")
                .WithSummary("Get current user profile info");
        }

        public static async Task<Results<Ok<APIResponse<CreateUserResponse>>, BadRequest<APIResponse<CreateUserResponse>>>> CreateUser(
            [FromForm] CreateUserRequest request,
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
                   PersonalCode: request.PersonalCode,
                   AccessLevel: request.AccessLevel,
                   AreaId: request.AreaId,
                   ZoneId: request.ZoneId,
                   UserName: request.UserName,
                   Password: request.Password,
                   CreatedById: userId,
                   UserRoleId: request.UserRoleId,
                   Enabled: false,
                   CreatedAt: DateTime.UtcNow,
                   BirthDate: request.BirthDate,
                   Avatar: request.Avatar);

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

        private static async Task<Results<Ok<APIResponse<AdminResetPasswordResponse>>, BadRequest<APIResponse<AdminResetPasswordResponse>>>> AdminResetPassword(
            [FromRoute] string id,
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<AdminResetPasswordResponse> response = new();

            try
            {
                var command = new AdminResetPasswordCommand(id);
                AdminResetPasswordResponse result = await sender.Send(command, ct);

                if (!string.IsNullOrEmpty(result.FailedResult))
                {
                    response.ErrorMessage = [result.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponseList<List<GetRolePermissionsResponse>>>, NotFound<APIResponseList<List<GetRolePermissionsResponse>>>>> GetRolePermissions(
            [FromRoute] string roleId,
            ISender sender,
            CancellationToken ct,
             int pageNumber = 1,
             int pageSize = 10)
        {
            APIResponseList<List<GetRolePermissionsResponse>> response = new();
            try
            {
                GetRolePermissionsQuery getRolePermissionsQuery = new(roleId: roleId);
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

        private static async Task<Results<Ok<APIResponseList<GetRolesResponse>>, NotFound<APIResponseList<List<GetRolesResponse>>>>> GetRoles(
          ISender sender,
          ClaimsPrincipal claimsPrincipal,
          CancellationToken ct,
          int pageNumber = 1,
          int pageSize = 10)
        {
            APIResponseList<GetRolesResponse> response = new();
            try
            {
                string userId = claimsPrincipal.FindFirst("nameid")?.Value!
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

                GetRolesResponse rolesResponse = await sender.Send(new GetRolesQuery(userId: userId));

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = rolesResponse;
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

        private static async Task<Results<Ok<APIResponse<GetCaptchaResponse>>, NotFound<APIResponse<GetCaptchaResponse>>>> GenerateCaptchCode(
           ISender sender,
           HttpContext context,
           CancellationToken ct)
        {
            APIResponse<GetCaptchaResponse> response = new();
            try
            {
                GetCaptchaResponse result = await sender.Send(new GetCaptchaQuery(), ct);
                context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
                context.Response.Headers.Pragma = "no-cache";
                context.Response.Headers.Expires = "0";

                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }


        private static async Task<Results<Ok<APIResponse<GetAllRolesResponse>>, BadRequest<APIResponse<GetAllRolesResponse>>>> GetAllRoles(
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetAllRolesResponse> response = new();

            GetAllRolesQuery query = new();
            GetAllRolesResponse result = await sender.Send(query, ct);

            response.StatusCode = HttpStatusCode.OK;
            response.Result.Data = result;
            return TypedResults.Ok(response);
        }


        private static async Task<Results<Ok<APIResponse<VerifyOtpResponse>>, BadRequest<APIResponse<VerifyOtpResponse>>>> VerifyOTPCode(
          [FromBody] VerifyOtpRequest verifyOtpRequest,
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<VerifyOtpResponse> response = new();

            try
            {

                var command = new VerifyOtpCodeCommand(verifyOtpRequest.PhoneNumber, verifyOtpRequest.OtpCode);
                VerifyOtpResponse verifyOtpResponse = await sender.Send(command);

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


        public static async Task<Results<Ok<APIResponse<GetUsersResponse>>, BadRequest<APIResponse<GetUsersResponse>>>> GetUsers(
            [AsParameters] GetUsersQuery query,
            IMediator mediator,
            ClaimsPrincipal claimsPrincipal,
            CancellationToken ct)
        {
            APIResponse<GetUsersResponse> response = new();

            try
            {
                var userId = claimsPrincipal.FindFirst("nameid")?.Value
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                GetUserByIdResponse getUserByIdResponse = await mediator.Send(new GetUserByIdQuery(userId), ct);
                if (getUserByIdResponse != null && Convert.ToInt32(getUserByIdResponse.RoleId) > 0 && string.IsNullOrEmpty(getUserByIdResponse.FailedResult))
                {

                    query = query with
                    {
                        AccessLevel = getUserByIdResponse.AccessLevel,
                        AreaId = getUserByIdResponse.AreaId,
                        ZoneId = getUserByIdResponse.ZoneId,
                        RoleId = Convert.ToInt32(getUserByIdResponse.RoleId)
                    };
                }

                GetUsersResponse getUsersResponse = await mediator.Send(query, ct);

                if (!string.IsNullOrEmpty(getUsersResponse.FailedResult))
                {
                    response.ErrorMessage = [getUsersResponse.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = getUsersResponse;
                return TypedResults.Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetUserByIdResponse>>, BadRequest<APIResponse<GetUserByIdResponse>>>> GetUserById(
            [FromRoute] string id,
            IMediator mediator,
            CancellationToken ct)
        {
            APIResponse<GetUserByIdResponse> response = new();

            try
            {
                var query = new GetUserByIdQuery(id);
                var result = await mediator.Send(query, ct);

                if (!string.IsNullOrEmpty(result.FailedResult))
                {
                    response.ErrorMessage = [result.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<UpdateUserResponse>>, BadRequest<APIResponse<UpdateUserResponse>>>> UpdateUser(
            [FromRoute] string id,
            [FromForm] UpdateUserRequest request,
            IMediator mediator,
            CancellationToken ct)
        {
            APIResponse<UpdateUserResponse> response = new();

            try
            {
                var command = new UpdateUserCommand(
                    UserId: id,
                    FirstName: request.FirstName,
                    LastName: request.LastName,
                    PhoneNumber: request.PhoneNumber,
                    NationalCode: request.NationalCode,
                    Email: request.Email,
                    PostalCode: request.PostalCode,
                    PersonalCode: request.PersonalCode,
                    Enabled: request.Enabled,
                    IsApprovedByAdmin: request.IsApprovedByAdmin,
                    AccessLevel: request.AccessLevel,
                    AreaId: request.AreaId,
                    ZoneId: request.RegionId,
                    RoleId: request.RoleId,
                    BirthDate: request.BirthDate,
                    Avatar: request.Avatar);

                var result = await mediator.Send(command, ct);

                if (!string.IsNullOrEmpty(result.FailedResult))
                {
                    response.ErrorMessage = [result.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetZonesResponse>>, BadRequest<APIResponse<GetZonesResponse>>>> GetZones(
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetZonesResponse> response = new();

            try
            {
                var result = await sender.Send(new GetZonesQuery(), ct);
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetAreasByZoneResponse>>, BadRequest<APIResponse<GetAreasByZoneResponse>>>> GetAreasByZone(
            [FromRoute] int zoneId,
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetAreasByZoneResponse> response = new();

            try
            {
                var result = await sender.Send(new GetAreasByZoneQuery(zoneId), ct);
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetUserProfileResponse>>, BadRequest<APIResponse<GetUserProfileResponse>>>> GetProfile(
            ClaimsPrincipal claimsPrincipal,
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetUserProfileResponse> response = new();

            try
            {
                var userId = claimsPrincipal.FindFirst("nameid")?.Value
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await sender.Send(new GetUserProfileQuery(userId), ct);

                if (!string.IsNullOrEmpty(result.FailedResult))
                {
                    response.ErrorMessage = [result.FailedResult];
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return TypedResults.BadRequest(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

    }
}
