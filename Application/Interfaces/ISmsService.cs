namespace ManageUsers.Application.Interfaces
{
    public interface ISmsService
    {
        Task<dynamic> Send(string mobileNumber, string message);

    }
}
