using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public sealed record GetCaptchaQuery : IRequest<GetCaptchaResponse>;
}
