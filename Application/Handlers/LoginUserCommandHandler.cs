using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoginUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            User? user;
            LoginUserResponse response = new();

            user = await _userRepository.GetByUserNameAsync(userName: request.UserName, ct: cancellationToken);

            if (user is null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }
            if (!user.Enabled)
            {
                response.FailedResult = "کاربر غیر فعال است!";
                return response;
            }

            bool checkPassword = await _userRepository.CheckPasswordAsync(user: user, password: request.Password);

            if (!checkPassword)
            {
                response.FailedResult = "رمز ورود اشتباه است!";
                return response;
            }
            List<IdentityUserRole<int>> identityUserRoles = await _userRepository.GetUserRole(user.Id);
            Dictionary<int, List<int>> mapRolePermissions = await _userRepository.GetRolePermissions(identityUserRoles, cancellationToken);
            List<int> roleNames = new();
            foreach (var item in mapRolePermissions)
            {
                roleNames.Add(item.Key);
            }

            List<RolePermission> rolePermissions = await _userRepository.GetRolePermisionsByRoleIds(roleNames, cancellationToken);

            CreateTokenContext ctx = new CreateTokenContext
            {
                User = user,
                ActiveRole = true,
                HttpContext = request.Context,
                RolePermissions = rolePermissions,
            };

            // JWT Code
            JWEToken tokenDTO = await _userRepository.CreateTokenAsync(ctx, cancellationToken: cancellationToken);
       
            response.Token = tokenDTO.Token;
            response.RefreshToken = tokenDTO.RefreshToken;

            return response;
        }
    }
}
