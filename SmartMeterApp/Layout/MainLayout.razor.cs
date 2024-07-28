using Microsoft.JSInterop;
using SmartMeterApp.Models;
using SmartMeterApp.Utility;
using System.Collections.Specialized;

namespace SmartMeterApp.Layout
{
    public partial class MainLayout
    {
        //Navigate the user to the specific page depinding on its role in stored JWT Token (if exists)
        protected override async Task OnInitializedAsync()
        {
            ToastService.ToastObjects.CollectionChanged += ToastObjects_CollectionChanged;

            try
            {

                // 1. Check if JWT Token exists in local storage
                string token = await JS.InvokeAsync<string>("localStorage.getItem", "authToken");

                // no token stored, redirect to login page
                if (string.IsNullOrEmpty(token))
                {
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    // 2. Extract payload data from JWT Token
                    TokenData? tokenData = TokenHelper.GetTokenData(token);

                    if (tokenData == null)
                    {
                        throw new InvalidTokenException("Invalid Token.");
                    }

                    // 3. Check expiration date
                    if (tokenData.ExpirationTime < DateTime.UtcNow)
                    {
                        // Token expired: remove stored token and redirect to login page
                        await JS.InvokeVoidAsync("localStorage.removeItem", "authToken");
                        ToastService.AddToast("Token expired.", ToastType.Error);
                        Navigation.NavigateTo("/login");
                    }
                    // Token is still valid
                    else
                    {
                        /*Sicherheitsprinzip:
                            - Zugriffskontrolle (RBAC): Überprüfen der Benutzerrolle und Weiterleitung basierend auf der Rolle
                            -> Dies stellt sicher, dass nur berechtigte Benutzer Zugriff auf bestimmte Seiten und Funktionen haben*/

                        // 5. Check role
                        if (tokenData.Role == "User")
                        {
                            Navigation.NavigateTo($"/overview/{tokenData.UserId}");
                        }
                        else if (tokenData.Role == "Operator")
                        {
                            Navigation.NavigateTo("/users");
                        }
                        else
                        {
                            // unknown role, redirect to login page
                            await JS.InvokeVoidAsync("localStorage.removeItem", "authToken");
                            ToastService.AddToast("Token invalid.", ToastType.Error);
                            Navigation.NavigateTo("/login");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToastService.AddToast(ex.Message, ToastType.Error);
            }
        }

        private void ToastObjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            ToastService.ToastObjects.CollectionChanged -= ToastObjects_CollectionChanged;
        }
    }
}
