using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ProyectoFinal.Models;

namespace ProyectoFinal.Vistas;

public partial class vReporteCliente : ContentPage
{

    private static readonly HttpClient client = new HttpClient();

    public  vReporteCliente()
    {
        InitializeComponent();
        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadSiniestrosAsync();
    }


    private async Task LoadSiniestrosAsync()
    {
        try
        {
            // Recuperar el token de las preferencias
            string token = Preferences.Get("auth_token", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                lblMessage.Text = "Token no encontrado. Por favor, inicia sesi�n nuevamente.";
                // Navegar a la p�gina de inicio de sesi�n u otra acci�n
                return;
            }

            string url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/sinister/getAllSinisterByPartner";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Siniestro> siniestros = JsonConvert.DeserializeObject<List<Siniestro>>(responseBody);

                if (siniestros == null || siniestros.Count == 0)
                {
                    // Maneja el caso de lista vac�a
                    lblMessage.Text = "No se encontraron siniestros.";
                }
                else
                {
                    grdcabecera.IsVisible = true;
                    lblMessage.Text = string.Empty; // Limpiar cualquier mensaje previo
                    listSiniestro.ItemsSource = siniestros;
                }
            }
            else
            {
                // Maneja el error de respuesta no exitosa
                lblMessage.Text = $"No se pudo obtener la informaci�n: {response.StatusCode}";
            }
        }
        catch (HttpRequestException e)
        {
            lblMessage.Text = $"Se produjo un error en la solicitud HTTP: {e.Message}";
        }
        catch (Exception e)
        {
            lblMessage.Text = $"Se produjo un error: {e.Message}";
        }
    }

    private void listSiniestro_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedSiniestro = e.SelectedItem as Siniestro;
        // Manejar el elemento seleccionado
        Navigation.PushAsync(new vDetalleReporte());

        // Desmarcar el elemento seleccionado
        ((ListView)sender).SelectedItem = null;
    }

}