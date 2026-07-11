using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public sealed class GetCaptchaQueryHandler
     : IRequestHandler<GetCaptchaQuery, GetCaptchaResponse>
    {
        private readonly ICaptchaService _captchaService;

        public GetCaptchaQueryHandler(ICaptchaService captchaService)
            => _captchaService = captchaService;

        public async Task<GetCaptchaResponse> Handle(
            GetCaptchaQuery request, CancellationToken ct)
        {
            return await _captchaService.GenerateCaptchaImageAsync(ct);
        }
    }
}
