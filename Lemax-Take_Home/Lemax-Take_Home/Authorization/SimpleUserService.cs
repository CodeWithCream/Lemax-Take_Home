namespace Lemax_Take_Home.Authorization
{
    public class SimpleUserService : IApplicationUserService
    {
        private readonly string _username = "take";
        private readonly string _password = "home";

        public bool ValidateCredentials(string username, string password)
        {
            return username.Equals(_username) && password.Equals(_password);
        }
    }
}
