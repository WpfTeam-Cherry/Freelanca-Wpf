using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeLancerWpf.Client.Data
{
    public class DataPersister
    {
        protected static string AccessToken { get; set; }
        //TODO change BaseServiceUrl
        private const string BaseServicesUrl = "http://localhost:16183/api/";

        internal static void RegisterUser(string username, string displayName,string email,string phone, string location, string authenticationCode)
            
        {
            //Validation!!!!!
            //validate username
            //validate email
            //validate authentication code
            //use validation from WebAPI
            var userModel = new UserModel()
            {
                Username = username,
                DisplayName=displayName,
                Email = email,
                Phone=phone,
                Location=location,
                AuthCode = authenticationCode
            };
            HttpRequester.Post(BaseServicesUrl + "users/register",
                userModel);
        }

        internal static string LoginUser(string username, string authenticationCode)
        {
            //Validation!!!!!
            //validate username
            //validate email
            //validate authentication code
            //use validation from WebAPI
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

        internal static void CreateNewTodosList(string title, IEnumerable<TodoViewModel> todos)
        {
            var listModel = new TodolistModel()
            {
                Title = title,
                Todos = todos.Select(t => new TodoModel()
                {
                    Text = t.Text
                })
            };

            var headers = new Dictionary<string, string>();
            headers["X-accessToken"] = AccessToken;

            var response =
                HttpRequester.Post<ListCreatedModel>(BaseServicesUrl + "lists", listModel, headers);
        }

        internal static IEnumerable<TodoListViewModel> GetTodoLists()
        {
            var headers = new Dictionary<string, string>();
            headers["X-accessToken"] = AccessToken;

            var todoListsModels =
                HttpRequester.Get<IEnumerable<TodolistModel>>(BaseServicesUrl + "lists", headers);
            return todoListsModels.
                AsQueryable().
                 Select(model => new TodoListViewModel()
                 {
                     Id = model.Id,
                     Title = model.Title,
                     Todos = model.Todos.AsQueryable().Select(todo => new TodoViewModel()
                     {
                         Id = todo.Id,
                         Text = todo.Text,
                         IsDone = todo.IsDone
                     })
                 });
        }

        internal static void ChangeState(int todoId)
        {
            var headers = new Dictionary<string, string>();
            headers["X-accessToken"] = AccessToken;

            HttpRequester.Put(BaseServicesUrl + "todos/" + todoId, headers);
        }
    }
}