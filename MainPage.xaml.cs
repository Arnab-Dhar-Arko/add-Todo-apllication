using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace MauiApp6
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> _todoItems;

        public MainPage()
        {
            InitializeComponent(); // Ensures components from XAML are initialized
            _todoItems = new ObservableCollection<string>();
            TodoListView.ItemsSource = _todoItems; // Bind the ListView to the ObservableCollection
            UpdateTaskCount(); // Initialize task count
        }

        private async void OnWeatherButtonClicked(object sender, EventArgs e)
        {
            WeatherActivityIndicator.IsVisible = true; // Show activity indicator
            WeatherActivityIndicator.IsRunning = true;
            WeatherLabel.Text = "Fetching weather data...";

            try
            {
                string weatherData = await GetWeatherData();
                WeatherLabel.Text = weatherData; // Display the weather data
            }
            catch (Exception ex)
            {
                WeatherLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                WeatherActivityIndicator.IsVisible = false; // Hide activity indicator
                WeatherActivityIndicator.IsRunning = false;
            }
        }

        private async Task<string> GetWeatherData()
        {
            string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current_weather=true";

            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(content);

                var currentWeather = json["current_weather"];
                if (currentWeather == null)
                {
                    return "Error: Unable to fetch weather data.";
                }

                // Extract necessary details
                double temperature = (double)(currentWeather["temperature"] ?? 0);
                double windSpeed = (double)(currentWeather["windspeed"] ?? 0);

                // Return formatted weather data
                return $"Current Weather:\n" +
                       $"Temperature: {temperature}°C\n" +
                       $"Wind Speed: {windSpeed} m/s";
            }
            catch (Exception ex)
            {
                return $"Unable to retrieve weather data: {ex.Message}";
            }
        }

        private void OnAddTaskButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewTaskEntry.Text))
            {
                _todoItems.Add(NewTaskEntry.Text);
                NewTaskEntry.Text = string.Empty; // Clear the Entry field
                UpdateTaskCount(); // Update task count
            }
        }

        private void OnDeleteTaskButtonClicked(object sender, EventArgs e)
        {
            if (TodoListView.SelectedItem != null)
            {
                _todoItems.Remove((string)TodoListView.SelectedItem);
                UpdateTaskCount(); // Update task count
            }
        }

        private void OnMarkAsCompletedButtonClicked(object sender, EventArgs e)
        {
            if (TodoListView.SelectedItem != null)
            {
                string task = (string)TodoListView.SelectedItem;
                _todoItems.Remove(task);
                _todoItems.Add($"[Completed] {task}"); // Mark as completed
                UpdateTaskCount(); // Update task count
            }
        }

        private void UpdateTaskCount()
        {
            TaskCountLabel.Text = $"{_todoItems.Count} tasks"; // Update task count
        }
    }
}