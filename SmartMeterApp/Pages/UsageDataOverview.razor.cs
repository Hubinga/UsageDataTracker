using Microsoft.JSInterop;
using SmartMeterApp.Models;
using SmartMeterApp.Utility;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Components;

namespace SmartMeterApp.Pages
{
    public partial class UsageDataOverview
    {
        [Parameter]
        public string UserId { get; set; }

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

                if (tokenData == null || string.IsNullOrEmpty(tokenData.UserId))
                {
                    throw new InvalidTokenException("Invalid Token.");
                }

                loggedInUserId = tokenData.UserId;

                /*Sicherheitsprinzipien:
                    - Zugriffskontrolle (RBAC): Überprüfen der Benutzerrolle und Weiterleitung basierend auf der Rolle
                    -> Dies stellt sicher, dass nur berechtigte Benutzer Zugriff auf bestimmte Seiten und Funktionen haben*/

                // 4. Check if user is an operator or is accessing his own data: Only Operator is allowed to see other users data
                if (tokenData.Role != "Operator" && UserId != loggedInUserId)
                {
                    ToastService.AddToast("Access denied.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }

                // 5. Send HTTP request with user ID
                usageData = await HttpClient.GetFromJsonAsync<List<UsageDataModel>>($"api/consumption/{UserId}");

                ToastService.AddToast($"Usage data successfully loaded.", ToastType.Info);
            }
            catch (Exception ex)
            {
                ToastService.AddToast($"Error while loading usage data: {ex.Message}", ToastType.Error);
            }
        }

        private async Task HandleUpdateUsageData()
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

                string apiUrl = "api/consumption/add";
                // 3. Send HTTP request to API to add usage data
                usageDataModel.UserId = UserId;
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync(apiUrl, usageDataModel);

                // 4. Check API response
                // success
                if (response.IsSuccessStatusCode)
                {
                    ToastService.AddToast("Data successfully updated.", ToastType.Info);
                    EventAggregator.TriggerAction(usageDataModel.Value);
                    HideEditDataModal();
                }
                // Handle Unauthorized (401) error
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {            
                    ToastService.AddToast("Unauthorized access: Token expired or invalid.", ToastType.Error);
                    Navigation.NavigateTo("/login");
                    return;
                }
                else
                {
                    ToastService.AddToast($"Error while updating usage data: {response.StatusCode}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
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
