namespace GECManager.Mobile;

public partial class MainPage : ContentPage
{
	{
    HttpClient _client = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5000") }; // localhost no emulador

    protected override async void OnAppearing()
    {
        var tasks = await _client.GetFromJsonAsync<List<ProjectTask>>("api/tasks");
        MyListView.ItemsSource = tasks;
    }
	}
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
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

