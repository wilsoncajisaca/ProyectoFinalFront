using Newtonsoft.Json;
using ProyectoFinal.Models;
using System.Collections.ObjectModel;

namespace ProyectoFinal.Vistas;

public partial class vReporteCliente : ContentPage
{

    private const string url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/sinister/getAllSinisterByPartner"; //cambiar por la del back
    private readonly HttpClient cliente = new HttpClient();
    private ObservableCollection<Siniestro> est;


    public vReporteCliente()
	{
		InitializeComponent();
        ObtenerDatos();
    }

    public async void ObtenerDatos()
    {
        var content = await cliente.GetStringAsync(url);
        List<Siniestro> mostrar = JsonConvert.DeserializeObject<List<Siniestro>>(content);
        est = new ObservableCollection<Siniestro>(mostrar);
        listSiniestro.ItemsSource = est;
    }

    private void listSiniestro_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            var objSiniestro = (Siniestro)e.SelectedItem;
            // Desmarcar el elemento seleccionado
            ((ListView)sender).SelectedItem = null;

            // Navegar a la página de detalles del siniestro
            //await Navigation.PushAsync(new vDetalleReporte(objSiniestro));
        }

    }
}