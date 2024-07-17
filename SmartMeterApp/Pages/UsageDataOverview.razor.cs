using Microsoft.JSInterop;
using SmartMeterApp.Models;
using SmartMeterApp.Utility;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

namespace SmartMeterApp.Pages
{
    public partial class UsageDataOverview
    {
        private bool isVerifying = false;
        private UsageDataModel usageDataModel = new();
        private string modalText;

        private List<UsageDataModel> usageData;
        private string loggedInUserId;

        protected override async Task OnInitializedAsync()
        {
            await LoadUsageData();
        }

        private async Task LoadUsageData()
        {
            try
            {
                string token = await JS.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token not found. User may not be authenticated.");
                    ToastService.AddToast("Token not found. User may not be authenticated.", ToastType.Error);
                    //TODO: redirrect to login page
                    return;
                }

                // use stored token for api authentication
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // get token data
                TokenData tokenData = TokenHelper.GetTokenData(token);

                if (tokenData == null || string.IsNullOrEmpty(tokenData.UserId))
                {
                    Console.WriteLine("UserId not found. User may not be authenticated.");
                    return;
                }

                loggedInUserId = tokenData.UserId;

                // send http request with current user ID
                usageData = await HttpClient.GetFromJsonAsync<List<UsageDataModel>>($"api/consumption/{loggedInUserId}");
                ToastService.AddToast($"Usage data successfully loaded.", ToastType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading usage data: {ex.Message}");
                ToastService.AddToast($"Error while loading usage data: {ex.Message}", ToastType.Error);
            }
        }

        private async Task HandleUpdateUsageData()
        {
            try
            {
                var token = await JS.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token not found. User may not be authenticated.");
                    ToastService.AddToast("Token not found. User may not be authenticated.", ToastType.Error);
                    //TODO: redirrect to login page
                    return;
                }

                // use stored token for api authentication
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = "api/consumption/add";
                var response = await HttpClient.PostAsJsonAsync(apiUrl, usageDataModel);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data successfully updated.");
                    ToastService.AddToast("Data successfully updated.", ToastType.Info);
                    EventAggregator.TriggerAction(usageDataModel.Value);
                    HideEditDataModal();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Handle Unauthorized (401) error
                    Console.WriteLine("Unauthorized access: Token expired or invalid.");
                    ToastService.AddToast("Unauthorized access: Token expired or invalid.", ToastType.Error);
                    //TODO: redirrect to login page
                }
                else
                {
                    Console.WriteLine($"Error while updating usage data: {response.StatusCode}");
                    ToastService.AddToast($"Error while updating usage data: {response.StatusCode}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ToastService.AddToast($"Exception: {ex.Message}", ToastType.Error);
            }
        }

        private async Task ShowUsageDataEditModal(UsageDataModel usageData)
        {
            usageDataModel = usageData;
            modalText = $"Edit Usage Data for {usageDataModel.Month} {usageDataModel.Year}";
            await JS.InvokeVoidAsync("showModal", "modal");
        }

        private void HideEditDataModal()
        {
            JS.InvokeVoidAsync("hideModal", "modal");
        }
    }
}
