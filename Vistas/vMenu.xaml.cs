namespace ProyectoFinal.Vistas;

public partial class vMenu : ContentPage
{
	public vMenu()
	{
		InitializeComponent();
	}

    private void btn_historial_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vReporteCliente());
    }

    private void btn_reportar_Clicked(object sender, EventArgs e)
    {

    }
}