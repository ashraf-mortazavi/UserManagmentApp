using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using MediatR;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;

namespace ManageUsers.Application.Handlers
{
    public sealed class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, SetPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public SetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<SetPasswordResponse> Handle(SetPasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserName, ct);

            if (user is null)
            {
                return new SetPasswordResponse(false, "User not found.");
            }

            //user.Password = _passwordHasher.HashPassword(request.Password);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new SetPasswordResponse(true, "Password set successfully.");
        }
    }

}
