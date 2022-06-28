namespace Lemax_Take_Home.Authorization
{
    /// <summary>
    /// Simple service for authentication used only for PoC
    /// </summary>
    public class SimpleApplicationUserService : IApplicationUserService
    {
        private readonly string _username = "take";
        private readonly string _password = "home";

        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <returns>True if username is 'take' and password is 'home', false otherwise</returns>
        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            return await Task.Run(() =>
            {
                return username.Equals(_username) && password.Equals(_password);
            });
        }
    }
}
