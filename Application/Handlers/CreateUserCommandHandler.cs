using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;




namespace ManageUsers.Application.Handlers
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken ct)
        {
            List<Role?> roles = await _userRepository.GetRolesAsync(request.UserRoles, ct);
            if (!roles.Any() || roles is null || roles.Count ==0)
            {
                throw new Exception("RoleIds are not Valid!");
            }

            User newUser = new User();
            newUser.FirstName = request.FirstName;
            newUser.LastName = request.LastName;
            newUser.NationalCode = request.NationalCode;
            newUser.PhoneNumber = request.PhoneNumber;
            newUser.Email = request.Email;
            newUser.PostalCode = request.PostalCode;
            newUser.CreatedById = request.CreatedById;
            newUser.Enabled = true;
            newUser.CreatedAt = DateTime.UtcNow;

            newUser.CreatedById = request.CreatedById;
            newUser.UserName = request.UserName;
            newUser = await _userRepository.AssignUserRolesAsync(user: newUser, request.Password, roles.Select(x=> x.Name!).ToList() , cancellationToken: ct);

            return new CreateUserResponse(
                newUser.Id
            );
        }
    }

}
