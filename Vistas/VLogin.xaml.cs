using Newtonsoft.Json;
using ProyectoFinal.Models;
using ProyectoFinal.Objetos;
using System.Text;

namespace ProyectoFinal.Vistas;

public partial class VLogin : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    public VLogin()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        ignoreLogin();
    }

    private void ignoreLogin()
    {
        if (Preferences.ContainsKey("auth_token"))
        {
            Navigation.PushAsync(new vMenu());
            Navigation.RemovePage(this);
        }
    }

    // Método estático para mostrar el diálogo de carga
    public static async Task ShowLoading()
    {
        var loadingPage = new Vistas.Popup.LoadingPopup();
        await Application.Current.MainPage.Navigation.PushModalAsync(loadingPage);
    }

    // Método estático para ocultar el diálogo de carga
    public static async Task HideLoading()
    {
        await Application.Current.MainPage.Navigation.PopModalAsync();
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        btnLogin.IsVisible = false;
        activityIndicator.IsVisible = true;
        activityIndicator.IsRunning = true;
        var username = txtUser.Text;
        var password = txtPass.Text;

        var result = await LoginAsync(username.Trim(), password);
        activityIndicator.IsVisible = false;
        activityIndicator.IsRunning = false;
        btnLogin.IsVisible = true;
        if ((bool)result["IsSuccess"])
        {
            var loginData = (LoginResponse)result["Data"];

            Preferences.Set("auth_token", loginData.Token);
            Preferences.Set("auth_user", loginData.Usuario);
            Preferences.Set("auth_rol", loginData.Rol);

            await Navigation.PushAsync(new vMenu());
        }
        else
        {
            var errorMessage = (string)result["ErrorMessage"];
            await DisplayAlert("Error", errorMessage, "OK");
        }
    }

    private async Task<Dictionary<string, object>> LoginAsync(string username, string password)
    {
        var url = $"{Common.BaseUrl}/appMovilesFinal/api/auth/login";
        var loginData = new { username = username, password = password };
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var result = new Dictionary<string, object>();

        try
        {
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                result["IsSuccess"] = true;
                result["Data"] = loginResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                result["IsSuccess"] = false;
                result["ErrorMessage"] = errorResponse.Message;
            }
        }
        catch (HttpRequestException e)
        {
            result["IsSuccess"] = false;
            result["ErrorMessage"] = "Ocurrió un error al intentar conectarse al servicio.";
        }

        return result;

        
    }

    private void btnRegister_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vRegistro());
    }

    private void btnInfo_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Alerta", "Lamentamos que tengas problemas para iniciar sección, para poder ayudarte ponte en contacto con el administrador \nadmin@proyectofinal.com", "Cerrar");
    }
}