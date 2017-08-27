using System;

namespace InfluxDB.WriteOnly
{
    public class LoginInformation
    {
        public string Username { get; }
        public string Password { get; }

        public LoginInformation(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(username));
            }

            Username = username;
            Password = password;
        }
    }
}