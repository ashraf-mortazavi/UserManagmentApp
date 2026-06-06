using Azure;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;




namespace ManageUsers.Application.Handlers
{

    public sealed class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, ChangeUserPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeUserPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ChangeUserPasswordResponse> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            User user;
            ChangeUserPasswordResponse changeUserPasswordResponse = new();

            user = await _userRepository.GetUserByIdAsync(request.UserId);

            if (user is null)
            {
                changeUserPasswordResponse.FailedResult = "کاربر یافت نشد!";
                return changeUserPasswordResponse;
            }

            bool isExist = await _userRepository.CheckPasswordAsync(user, request.CurrentPassword);

            if (!isExist)
            {
                changeUserPasswordResponse.FailedResult = "رمز عبور اشتباه است!";
                return changeUserPasswordResponse;
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                changeUserPasswordResponse.FailedResult = "رمز عبور فعلی و رمز عبور جدید نباید یکسان باشد!";
                return changeUserPasswordResponse;
            }

            IdentityResult identityResult = await _userRepository.SetPasswordByUserIdAsync(user.Id.ToString(), request.NewPassword);

            if (!identityResult.Succeeded)
            {
                changeUserPasswordResponse.FailedResult = "در تغییر رمزعبور مشکلی بوجود آمد!";
                return changeUserPasswordResponse;
            }

        
            List<IdentityUserRole<int>> identityUserRoles = await _userRepository.GetUserRole(user.Id);
            Dictionary<int, List<int>> mapRolePermissions = await _userRepository.GetRolePermissions(identityUserRoles, cancellationToken);
            List<int> roleIds = new();
            foreach (var item in mapRolePermissions)
            {
                roleIds.Add(item.Key);
            }

            List<RolePermission> rolePermissions = await _userRepository.GetRolePermisionsByRoleIds(roleIds, cancellationToken);


            CreateTokenContext ctx = new CreateTokenContext
            {
                User = user,
                ActiveRole = true,
                HttpContext = request.Context,
                RolePermissions = rolePermissions,
            };

            // JWT Code
            JWEToken tokenDTO = await _userRepository.CreateTokenAsync(ctx, cancellationToken: cancellationToken);

            changeUserPasswordResponse.Token = tokenDTO.Token;
            changeUserPasswordResponse.RefereshToken = tokenDTO.RefreshToken;
          
            return changeUserPasswordResponse;
        }
    }

}
