namespace ImproveMauiToolbarItems;

public partial class App : Application
{
	public static App Instance => (App)Application.Current!;
	public event EventHandler? AppResumed;
	public event EventHandler? AppPutToForeground;

	public App()
	{
		InitializeComponent();
	}

	protected override void OnSleep()
	{
		AppPutToForeground?.Invoke(this, EventArgs.Empty);
		base.OnSleep();
	}

	protected override void OnResume()
	{
		AppResumed?.Invoke(this, EventArgs.Empty);
		base.OnResume();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}