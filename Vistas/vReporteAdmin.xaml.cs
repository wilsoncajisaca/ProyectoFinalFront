using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ProyectoFinal.Models;

namespace ProyectoFinal.Vistas;

public partial class vReporteAdmin : ContentPage
{

    private static readonly HttpClient client = new HttpClient();

    public vReporteAdmin()
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
                lblMensaje.Text = "Token no encontrado. Por favor, inicia sesión nuevamente.";
                // Navegar a la página de inicio de sesión u otra acción
                return;
            }

            string url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/sinister/getAllSinister";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Siniestro> siniestros = JsonConvert.DeserializeObject<List<Siniestro>>(responseBody);

                if (siniestros == null || siniestros.Count == 0)
                {
                    // Maneja el caso de lista vacía
                    lblMensaje.Text = "No se encontraron siniestros.";
                }
                else
                {
                    grdcabecera.IsVisible = true;
                    lblMensaje.Text = string.Empty; // Limpiar cualquier mensaje previo
                    listSiniestro.ItemsSource = siniestros;
                }
            }
            else
            {
                // Maneja el error de respuesta no exitosa
                lblMensaje.Text = $"No se pudo obtener la información: {response.StatusCode}";
            }
        }
        catch (HttpRequestException e)
        {
            lblMensaje.Text = $"Se produjo un error en la solicitud HTTP: {e.Message}";
        }
        catch (Exception e)
        {
            lblMensaje.Text = $"Se produjo un error: {e.Message}";
        }
    }

    private void listSiniestro_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedSiniestro = e.SelectedItem as Siniestro;
        // Manejar el elemento seleccionado
        DisplayAlert("Siniestro seleccionado", $"ID: {selectedSiniestro.SiniestroId}", "OK");

        // Desmarcar el elemento seleccionado
        ((ListView)sender).SelectedItem = null;
    }

}