namespace ProyectoFinal.Vistas;

public partial class vMenu : ContentPage
{
	public vMenu()
	{
		InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        string userLogged = Preferences.Get("auth_user", string.Empty);
        Title = $"Bienvenido {userLogged}";
    }

    private void btn_historial_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vReporteCliente());
    }

    private async void btn_reportar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vReporteSiniestro());
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        Preferences.Remove("auth_token");
        Preferences.Remove("auth_user");
        Preferences.Remove("auth_rol");
        Navigation.PushAsync(new VLogin());
    }
}