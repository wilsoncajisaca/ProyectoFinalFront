using Newtonsoft.Json;
using System.Net.Http.Headers;
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
                lblMessage.Text = "Token no encontrado. Por favor, inicia sesión nuevamente.";
                // Navegar a la página de inicio de sesión u otra acción
                return;
            }

            string url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/sinister/getAllSinisterByPartner";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Siniestro> siniestros = JsonConvert.DeserializeObject<List<Siniestro>>(responseBody);
                foreach (var siniestro in siniestros)
                {
                    siniestro.BackgroundColor = ConvertHexToColor(siniestro.ColorFondo);
                }

                if (siniestros == null || siniestros.Count == 0)
                {
                    lblMessage.Text = "No se encontraron siniestros.";
                }
                else
                {
                    lblMessage.Text = string.Empty;
                    collectionViewSiniestro.ItemsSource = siniestros;
                }
            }
            else
            {
                // Maneja el error de respuesta no exitosa
                lblMessage.Text = $"No se pudo obtener la información: {response.StatusCode}";
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

    private Color ConvertHexToColor(string hex)
    {
        if (!string.IsNullOrEmpty(hex))
        {
            return Color.FromArgb(hex);
        }
        return Colors.Transparent;
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

    private async void collectionViewSiniestro_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedSiniestro = e.CurrentSelection.FirstOrDefault() as Siniestro;
        if (selectedSiniestro != null)
        {
            await DisplayAlert("Seleccionado", $"Has seleccionado: {selectedSiniestro.TipoSiniestro}", "OK");
        }
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var siniestro = frame.BindingContext as Siniestro;
        if (siniestro != null)
        {
            await DisplayAlert("Seleccionado", $"Has seleccionado: {siniestro.TipoSiniestro}", "OK");
        }
    }
}