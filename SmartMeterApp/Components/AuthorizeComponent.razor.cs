using Microsoft.JSInterop;
using SmartMeterApi.Models;
using SmartMeterApp.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartMeterApp.Components
{
    public partial class AuthorizeComponent
    {
        private UserActions userAction { get; set; }

        private LoginModel loginModel = new LoginModel();
        private RegisterModel registerModel = new RegisterModel();

        private string repeatedPassword;
        private bool showRepeatedPasswordError = false;

        private bool showOtpInputDialog = false;

        private async Task HandleValidSubmitLogin()
        {
            try
            {
                // Send the login request to the API
                var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    // Check if OTP was sent (assuming the API returns OTPSent property)
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);


                    if (responseObject.TryGetValue("otpSent", out var otpSentValue) && otpSentValue.ValueKind == JsonValueKind.True)
                    {
                        // Handle OTP sent scenario
                        var userEmail = responseObject["email"].ToString();
                        Console.WriteLine($"OTP was sent to {userEmail}");

                        // Proceed with OTP validation logic or UI update
                        // For example, show an OTP input field or message in the UI
                        showOtpInputDialog = true;
                        await JS.InvokeVoidAsync("ModalInterop.show", "otpModal");
                    }
                    else
                    {
                        // OTP not sent scenario
                        Navigation.NavigateTo("/");
                    }
                }
                else
                {
                    // Handle login failure (e.g., show an error message)
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions (e.g., network errors, etc.)
                Console.WriteLine($"Failed to login: {ex.Message}");
            }
        }

        private async Task HandleOTPVerification(string otpCode)
        {
            VerifyOtpModel verifyOtpModel = new VerifyOtpModel() {Email = loginModel.Email, OTPCode = otpCode };
            // Hier wird die API-Anfrage zur OTP-Code-Verifikation gemacht
            var response = await HttpClient.PostAsJsonAsync("api/auth/", verifyOtpModel);

            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (true)
                {
                    // Erfolgreiche Authentifizierung, erhalte und speichere den JWT-Token
                    var token = result.Token;
                    StoreToken(token);
                    Navigation.NavigateTo("/");
                }
                else
                {
                    // Fehler: OTP-Code ungültig
                    Console.WriteLine("Ungültiger OTP-Code. Bitte versuche es erneut.");
                }
            }
            else
            {
                // Fehler: API-Anfrage fehlgeschlagen
                Console.WriteLine("Serverfehler. Bitte versuche es später erneut.");
            }
        }



        private async Task HandleValidSubmitRegister()
        {
            // Validate the repeated password
            if (registerModel.Password != repeatedPassword)
            {
                showRepeatedPasswordError = true;
                return;
            }

            showRepeatedPasswordError = false;
            // Send the register request to the API
            var response = await HttpClient.PostAsJsonAsync("api/auth/register", registerModel);

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
