namespace Lemax_Take_Home.Authorization
{
    public interface IApplicationUserService
    {
        bool ValidateCredentials(string username, string password);
    }
}
