using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using ProyectoFinal.Models;
using ProyectoFinal.Objetos;
using System.Text;

namespace ProyectoFinal.Vistas;

public partial class vRegistro : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    public vRegistro()
	{
		InitializeComponent();
	}

    private async void btnRegister_Clicked(object sender, EventArgs e)
    {
        string cedula = txtId.Text;
        string nombre = txtName.Text;
        string apellido = txtLastName.Text;
        string email = txtEmail.Text;
        string contrasenia = txtPass.Text;

        if (string.IsNullOrEmpty(cedula) ||
            string.IsNullOrEmpty(nombre) ||
            string.IsNullOrEmpty(apellido) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(contrasenia))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        await RegisterUserAsync(cedula, nombre, apellido, email, contrasenia);
    }

    private async Task RegisterUserAsync(string cedula, string nombre, string apellido, string email, string contrasenia)
    {
        btnRegister.IsVisible = false;
        activityIndicator.IsVisible = true;
        activityIndicator.IsRunning = true;
        var url = $"{Common.BaseUrl}/appMovilesFinal/api/partner/create";
        var userData = new
        {
            cedula = cedula.Trim(),
            nombre = nombre.Trim(),
            apellido = apellido.Trim(),
            email = email.Trim(),
            contrasenia = contrasenia,
            celular = "1234567890",
            direccion = "Sin direccion",
            genero = 1
        };

        var json = JsonConvert.SerializeObject(userData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(url, content);
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
            btnRegister.IsVisible = true;
            if (response.IsSuccessStatusCode)
            {
                await Navigation.PushAsync(new VLogin());
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                await DisplayAlert("Error", errorResponse.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
}