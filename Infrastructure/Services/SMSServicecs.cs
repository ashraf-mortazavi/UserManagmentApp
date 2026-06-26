using ManageUsers.Application.Interfaces;

namespace ManageUsers.Infrastructure.Services
{
    public class SMSServicecs(IHostEnvironment hostEnvironment) : ISmsService
    {
        public Task<dynamic> Send(string mobileNumber, string message)
        {
            /// to do for spical Provider///
            return null;
        }
    }
}
