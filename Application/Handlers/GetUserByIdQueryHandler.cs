using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserService _userService;

        public GetUserByIdQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var response = new GetUserByIdResponse();

            var user = await _userService.GetUserByIdWithRolesAsync(request.UserId, ct);

            if (user == null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }

            var roleIds = await _userService.GetUserRoleIdsAsync(user.Id, ct);

            response.Id = user.Id.ToString();
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.UserName = user.UserName!;
            response.PhoneNumber = user.PhoneNumber!;
            response.NationalCode = user.NationalCode;
            response.Email = user.Email;
            response.PostalCode = user.PostalCode;
            response.PersonalCode = user.PersonalCode;
            response.Position = user.Position;
            response.Description = user.Description;
            response.Enabled = user.Enabled;
            response.OrganizationId = user.OrganizationId;
            response.AreaId = user.AreaId;
            response.RegionId = user.RegionId;
            response.UserRoleIds = roleIds;
            response.CreatedAt = user.CreatedAt;
            response.UpdatedAt = user.UpdatedAt;

            return response;
        }
    }
}
