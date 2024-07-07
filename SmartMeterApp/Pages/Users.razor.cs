using Microsoft.JSInterop;
using SmartMeterApp.Models;
using SmartMeterApp.Utility;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartMeterApp.Pages
{
    public partial class Users
    {
        private List<UserDataModel> userData = new List<UserDataModel>();

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
        }

        private async Task LoadUserData()
        {
            try
            {
                var token = await JS.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token not found. User may not be authenticated.");
                    ToastService.AddToast("Token not found. User may not be authenticated.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }

                // Use stored token for API authentication
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Get token data
                TokenData tokenData = TokenHelper.GetTokenData(token);

                if (tokenData.Role != "Operator")
                {
                    // Only allowed for role "Operator": redirect to login page
                    Console.WriteLine("Access denied for non-operators.");
                    ToastService.AddToast("Access denied for non-operators.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }

                // Send HTTP request
                userData = await HttpClient.GetFromJsonAsync<List<UserDataModel>>("api/user");

                ToastService.AddToast("User data successfully loaded.", ToastType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading user data: {ex.Message}");
                ToastService.AddToast($"Error while loading user data: {ex.Message}", ToastType.Error);
            }
        }

        private void LoadUsageDataForUser()
        {

        }
    }
}
