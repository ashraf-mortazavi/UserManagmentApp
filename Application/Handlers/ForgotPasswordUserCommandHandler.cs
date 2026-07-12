using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers
{
    public sealed class ForgotPasswordUserCommandHandler : IRequestHandler<ForgotPasswordUserCommand, ForgotPasswordResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordUserCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordUserCommand request, CancellationToken cancellationToken)
        {
            ForgotPasswordResponse response = new();
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    response.FailedResult = "کاربر با این ایمیل یافت نشد!";
                    return response;
                }

                if (!user.Enabled)
                {
                    response.FailedResult = "حساب کاربری غیرفعال است!";
                    return response;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                //var encodedToken = Uri.EscapeDataString(token);
                var url = _configuration["AppSetting:Url"] ?? request.Context.Request.Scheme + "://" + request.Context.Request.Host;
                var resetLink = $"{url}/reset-password?{token}&email={Uri.EscapeDataString(user.Email)}";

                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    user.UserName ?? user.Email,
                    resetLink
                );

                if (!emailSent)
                {
                    response.FailedResult = "ارسال ایمیل با مشکل مواجه شد. لطفاً بعداً تلاش کنید.";
                    return response;
                }
                response.ResetLink = resetLink;
            }
            catch (Exception)
            {
                response.FailedResult = "خطایی در پردازش درخواست رخ داد.";
            }

            return response;
        }
    }
}
