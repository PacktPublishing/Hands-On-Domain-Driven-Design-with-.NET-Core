namespace Marketplace.Users.Auth
{
    public static class Contracts
    {
        public class Login
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}