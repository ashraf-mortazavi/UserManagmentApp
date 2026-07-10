using MediatR;

namespace ManageUsers.Application.Queries
{
    public sealed record GetCaptchaQuery : IRequest<(string, string)>;
}
