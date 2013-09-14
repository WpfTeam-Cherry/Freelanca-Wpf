using FreeLancerWpf.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FreeLancerWpf.Client.Data
{
    public class DataPersister
    {
        private const int TokenLength = 50;
        private const string TokenChars = "qwertyuiopasdfghjklmnbvcxzQWERTYUIOPLKJHGFDSAZXCVBNM";
        private const int MinUsernameLength = 6;
        private const int MaxUsernameLength = 30;
        private const int AuthenticationCodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.@";

        protected static string AccessToken { get; set; }
        //TODO change BaseServiceUrl
        private const string BaseServicesUrl = "http://localhost:16183/api/";

        internal static void RegisterUser(string username, string displayName, string email, 
            string phone, string location, string authenticationCode)
            
        {
            ValidateUsername(username);
            ValidateEmail(email);
            ValidateAuthCode(authenticationCode);

            var userModel = new UserModel()
            {
                Username = username,
                DisplayName = displayName,
                Email = email,
                Phone = phone,
                Location = location,
                AuthCode = authenticationCode
            };
            HttpRequester.Post(BaseServicesUrl + "users/register",
                userModel);
        }

        internal static string LoginUser(string username, string authenticationCode)
        {
            ValidateUsername(username);
            ValidateAuthCode(authenticationCode);

            var userModel = new UserModel()
            {
                Username = username,
                AuthCode = authenticationCode
            };
            var loginResponse = HttpRequester.Post<LoginResponseModel>(BaseServicesUrl + "auth/token",
                userModel);
            AccessToken = loginResponse.AccessToken;
            return loginResponse.Username;
        }

        internal static bool LogoutUser()
        {
            var headers = new Dictionary<string, string>();
            headers["X-accessToken"] = AccessToken;
            var isLogoutSuccessful = HttpRequester.Put(BaseServicesUrl + "users/logout", headers);
            return isLogoutSuccessful;
        }

        internal static void ChangeState(int todoId)
        {
            var headers = new Dictionary<string, string>();
            headers["X-accessToken"] = AccessToken;

            HttpRequester.Put(BaseServicesUrl + "todos/" + todoId, headers);
        }

        private static void ValidateEmail(string email)
        {
            try
            {
                new MailAddress(email);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Email is invalid", ex);
            }
        }

        private static void ValidateUser(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new FormatException("Username and/or password are invalid");
            }
            ValidateUsername(userModel.Username);
            ValidateAuthCode(userModel.AuthCode);
        }

        private static void ValidateAuthCode(string authCode)
        {
            if (string.IsNullOrEmpty(authCode) || authCode.Length != AuthenticationCodeLength)
            {
                throw new FormatException("Password is invalid");
            }
        }

        public static void ValidateUsername(string username)
        {
            if (username.Length < MinUsernameLength || MaxUsernameLength < username.Length)
            {
                throw new FormatException(
                    string.Format("Username must be between {0} and {1} characters",
                        MinUsernameLength,
                        MaxUsernameLength));
            }
            if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new FormatException("Username contains invalid characters");
            }
        }
    }
}