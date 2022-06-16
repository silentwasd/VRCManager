using System.Diagnostics;
using System.Net;
using System.Reactive;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using WorldManager.Models;
using WorldManager.Services;

namespace WorldManager.ViewModels;

public class AuthViewModel : ViewModelBase
{
    private bool _authorized;

    private CurrentUser _currentUser;

    private Configuration _apiConfig;
    
    private string _username;

    private string _password;

    private string _errorString;

    public AuthViewModel()
    {
        var canEnter = this.WhenAnyValue(x => x.Username,
            y => y.Password,
            (x, y) => x?.Length > 0 && y?.Length > 0);

        Enter = ReactiveCommand.Create(() => {
            try
            {
                ApiConfig = new Configuration
                {
                    Username = Username,
                    Password = Password,
                    ApiKeyPrefix =
                    {
                        ["auth"] = AppCompose.Config.AuthCookie
                    }
                };

                var authApi = new AuthenticationApi(ApiConfig);
                var response = authApi.GetCurrentUserWithHttpInfo();

                if (AppCompose.Config.AuthCookie == null)
                {
                    AppCompose.Config.AuthCookie = response.Cookies.Find(x => x.Name == "auth")?.Value;
                    AppCompose.Config.Save();
                }

                Authorized = true;
                CurrentUser = response.Data;
            }
            catch (ApiException e)
            {
                var error = ApiError.Parse(e.ErrorContent);
                ErrorString = "API Error: " + error.Message + " (" + error.StatusCode + ")";
            }
        }, canEnter);

        if (AppCompose.Config.AuthCookie != null)
        {
            Username = "****";
            Password = "****";
        }
    }

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public bool Authorized
    {
        get => _authorized;
        set => this.RaiseAndSetIfChanged(ref _authorized, value);
    }

    public string ErrorString
    {
        get => _errorString;
        set => this.RaiseAndSetIfChanged(ref _errorString, value);
    }
    
    public CurrentUser CurrentUser
    {
        get => _currentUser;
        set => this.RaiseAndSetIfChanged(ref _currentUser, value);
    }

    public Configuration ApiConfig
    {
        get => _apiConfig;
        set => this.RaiseAndSetIfChanged(ref _apiConfig, value);
    }

    public ReactiveCommand<Unit, Unit> Enter { get; }

    public void Test()
    {
        var web = new WebClient();
        web.Headers.Add("User-Agent", "VRCManager/1.0.0");
        var bytes = web.DownloadData("https://api.vrchat.cloud/api/1/image/file_c2b7ad18-5276-4e9b-aef7-8e18555e5030/7/256");
        Debug.WriteLine(bytes.Length);
    }
}