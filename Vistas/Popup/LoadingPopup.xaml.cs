namespace ProyectoFinal.Vistas.Popup;

public partial class LoadingPopup : ContentPage
{
	public LoadingPopup()
	{
		InitializeComponent();
	}
    public static async Task ShowLoading()
    {
        var loadingPage = new Vistas.Popup.LoadingPopup();
        await Application.Current.MainPage.Navigation.PushModalAsync(loadingPage);
    }

    public static async Task HideLoading()
    {
        await Application.Current.MainPage.Navigation.PopModalAsync();
    }
}