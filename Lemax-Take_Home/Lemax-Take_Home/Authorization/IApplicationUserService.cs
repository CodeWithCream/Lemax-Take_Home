namespace Lemax_Take_Home.Authorization
{
    /// <summary>
    /// Service for user authentication used only for PoC
    /// </summary>
    public interface IApplicationUserService
    {
        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <param name="username">username (plain text)</param>
        /// <param name="password">password (plain text)</param>
        /// <returns>True if credentials valid, false otherwise</returns>
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}
