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
                // 1. Check if JWT Token exists in local storage
                string token = await JS.InvokeAsync<string>("localStorage.getItem", "authToken");

                // no token stored, redirect to login page
                if (string.IsNullOrEmpty(token))
                {
                    ToastService.AddToast("Token not found. User may not be authenticated.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }

                // 2. Use stored token for API authentication
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 3. Extract payload data from JWT Token
                TokenData? tokenData = TokenHelper.GetTokenData(token);

                if (tokenData == null)
                {
                    throw new InvalidTokenException("Invalid Token.");
                }

                // Only allowed for role "Operator": redirect to login page
                if (tokenData.Role != "Operator")
                {
                    ToastService.AddToast("Access denied for non-operators.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }

                // 4. Send HTTP request to get all users
                userData = await HttpClient.GetFromJsonAsync<List<UserDataModel>>("api/user");

                ToastService.AddToast("User data successfully loaded.", ToastType.Info);
            }
            catch (Exception ex)
            {
                ToastService.AddToast($"Error while loading user data: {ex.Message}", ToastType.Error);
            }
        }

        private void LoadUsageDataForUser()
        {

        }
    }
}
