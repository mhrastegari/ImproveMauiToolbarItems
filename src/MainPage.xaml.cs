namespace ImproveMauiToolbarItems;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnPrimaryItemClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Primary Item Clicked", "Add was clicked.", "OK");
	}

	private async void OnSecondaryItemClicked(object sender, EventArgs e)
	{
		var toolbarItem = sender as ToolbarItem;
		await DisplayAlert("Secondary Item Clicked", $"{toolbarItem?.Text} was clicked.", "OK");
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

