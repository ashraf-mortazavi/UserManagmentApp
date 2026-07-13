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

            group.MapGet("/areas", GetAreas)
                .RequireAuthorization()
                .WithDisplayName("دریافت مناطق")
                .WithSummary("Get all areas");

            group.MapGet("/areas/{areaId:int}/zones", GetZonesByArea)
                .RequireAuthorization()
                .WithDisplayName("دریافت نواحی منطقه")
                .WithSummary("Get zones for an area");
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
                   AccessLevel: request.AccessLevel,
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
                var callerUserId = claimsPrincipal.FindFirst("nameid")?.Value
                    ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var enrichedQuery = query with
                {
                    CallerAccessLevel = AccessLevel.Setad,
                    CallerAreaId = null,
                    CallerRegionId = null
                };

                if (!string.IsNullOrEmpty(callerUserId))
                {
                    var getUserQuery = new GetUserByIdQuery(callerUserId);
                    var callerUser = await mediator.Send(getUserQuery, ct);
                    if (callerUser != null && string.IsNullOrEmpty(callerUser.FailedResult))
                    {
                        enrichedQuery = query with
                        {
                            CallerAccessLevel = callerUser.AccessLevel,
                            CallerAreaId = callerUser.AreaId,
                            CallerRegionId = callerUser.RegionId
                        };
                    }
                }

                GetUsersResponse getUsersResponse = await mediator.Send(enrichedQuery, ct);

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
            [FromBody] UpdateUserRequest request,
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
                    Position: request.Position,
                    Description: request.Description,
                    Enabled: request.Enabled,
                    AccessLevel: request.AccessLevel,
                    OrganizationId: request.OrganizationId,
                    AreaId: request.AreaId,
                    RegionId: request.RegionId,
                    UserRoleIds: request.UserRoleIds);

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

        private static async Task<Results<Ok<APIResponse<GetAreasResponse>>, BadRequest<APIResponse<GetAreasResponse>>>> GetAreas(
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetAreasResponse> response = new();

            try
            {
                var result = await sender.Send(new GetAreasQuery(), ct);
                response.StatusCode = HttpStatusCode.OK;
                response.Result.Data = result;
                return TypedResults.Ok(response);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<Results<Ok<APIResponse<GetRegionsByAreaResponse>>, BadRequest<APIResponse<GetRegionsByAreaResponse>>>> GetZonesByArea(
            [FromRoute] int areaId,
            ISender sender,
            CancellationToken ct)
        {
            APIResponse<GetRegionsByAreaResponse> response = new();

            try
            {
                var result = await sender.Send(new GetRegionsByAreaQuery(areaId), ct);
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
