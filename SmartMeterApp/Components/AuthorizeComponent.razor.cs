using SmartMeterApi.Models;
using System.Net.Http.Json;

namespace SmartMeterApp.Components
{
    public partial class AuthorizeComponent
    {
        private UserActions userAction { get; set; }

        private LoginModel loginModel = new LoginModel();
        private RegisterModel registerModel = new RegisterModel();

        private async Task HandleValidSubmitLogin()
        {
            // Send the login request to the API
            var response = await Http.PostAsJsonAsync("api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                // Store the token (e.g., in local storage or a global state)
                // Here we just use a placeholder for the storage logic
                StoreToken(result.Token);

                // Redirect to another page (e.g., the main dashboard)
                Navigation.NavigateTo("/");
            }
            else
            {
                // Handle login failure (e.g., show an error message)
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed: {errorMessage}");
            }
        }

        private async Task HandleValidSubmitRegister()
        {
            // Send the register request to the API
            var response = await Http.PostAsJsonAsync("api/auth/register", registerModel);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to the login page after successful registration
                userAction = UserActions.Login;
            }
            else
            {
                // Handle registration failure (e.g., show an error message)
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registration failed: {errorMessage}");
            }
        }

        private void StoreToken(string token)
        {
            // Implement token storage logic
            // For example, using local storage:
            // await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            Console.WriteLine($"Token: {token}");
        }

        public class AuthResponse
        {
            public string Token { get; set; }
        }

        private void ToggleUserActionState()
        {
            if (userAction == UserActions.Login)
            {
                userAction = UserActions.Register;
            }
            else
            {
                userAction = UserActions.Login;
            }
        }
    }
}
