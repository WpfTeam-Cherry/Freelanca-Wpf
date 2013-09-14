using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FreeLancerWpf.Client.Data;
using FreeLancerWpf.Client.Helpers;
using FreeLancerWpf.Client.Behavior;

namespace FreeLancerWpf.Client.ViewModels
{
    public class LoginRegisterFormViewModel: ViewModelBase, IPageViewModel
    {
        private string message;
        private ICommand loginCommand;
        private ICommand registerCommand;

        public string Name
        {
            get
            {
                return "Login Form";
            }
        }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }

        public string Email { get; set; }

        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
                this.OnPropertyChanged("Message");
            }
        }

        public ICommand Login
        {
            get
            {
                if (this.loginCommand == null)
                {
                    this.loginCommand = new RelayCommand(this.HandleLoginCommand);
                }
                return this.loginCommand;
            }
        }

        public ICommand Register
        {
            get
            {
                if (this.registerCommand == null)
                {
                    this.registerCommand = new RelayCommand(this.HandleRegisterCommand);
                }
                return this.registerCommand;
            }
        }

        public event EventHandler<LoginSuccessArgs> LoginSuccess;

        public LoginRegisterFormViewModel()
        {
            this.Username = "DonchoMinkov";
            this.Email = "Minkov@doncho.com";
        }
        
        private void HandleRegisterCommand(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            //SHA1 sha = new SHA1CryptoServiceProvider();
            //var passwordBytes = Encoding.Default.GetBytes(password);
            //passwordBytes = Encoding.Convert(Encoding.Default,Encoding.UTF8,passwordBytes);
            //var authenticationCodeBytes = sha.ComputeHash(passwordBytes);
            //var authenticationCode = Encoding.UTF8.GetString(authenticationCodeBytes);

            var authenticationCode = this.GetSHA1HashData(password);

            DataPersister.RegisterUser(this.Username, this.DisplayName, this.Email,this.Phone, this.Location, authenticationCode);
            this.HandleLoginCommand(parameter);
        }

        private void HandleLoginCommand(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            var authenticationCode = this.GetSHA1HashData(password);

            var username = DataPersister.LoginUser(this.Username, authenticationCode);

            if (!string.IsNullOrEmpty(username))
            {
                this.RaiseLoginSuccess(username);
            }
        }

        private string GetSHA1HashData(string data)
        {
            return "0123456789012345678901234567890123456789";
        }


        protected void RaiseLoginSuccess(string username)
        {
            if (this.LoginSuccess!= null)
            {
                this.LoginSuccess(this, new LoginSuccessArgs(username));                
            }
        }
    }
}
   
