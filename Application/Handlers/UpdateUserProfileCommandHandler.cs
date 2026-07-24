using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResponse>
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UpdateUserProfileCommandHandler(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        public async Task<UpdateUserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken ct)
        {
            var response = new UpdateUserProfileResponse();

            var user = await _userService.GetUserByIdAsync(request.UserId, ct);
            if (user == null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }

            if (request.Avatar is not null)
            {
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                    await _fileService.DeleteFileAsync(user.AvatarUrl, ct);

                user.AvatarUrl = await _fileService.UploadAvatarAsync(request.Avatar, ct);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (request.BirthDate.HasValue && request.BirthDate.Value != null)
                user.BirthDate = request.BirthDate;

            user.UpdatedAt = DateTime.UtcNow;

            _userService.UpdateUser(user, ct);

            response.Id = user.Id.ToString();
            return response;
        }
    }
}
