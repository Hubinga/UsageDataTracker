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
        private VerifyOtpModel verifyOtpModel = new VerifyOtpModel();

        private string repeatedPassword;
        private bool showRepeatedPasswordError = false;

        private bool isVerifying = false;

        private async Task HandleValidSubmitLogin()
        {
            try
            {
                isVerifying = true;

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

                        // show otp input modal
                        await JS.InvokeVoidAsync("showModal", "otpModal");
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
                Console.WriteLine($"Failed to login: {ex.Message}");
            }
            finally
            {
                isVerifying = false;
            }
        }

        private async Task HandleOTPVerification()
        {
            try
            {
                isVerifying = true;
                verifyOtpModel.Email = loginModel.Email;
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/auth/verifyotp", verifyOtpModel);

                if (response.IsSuccessStatusCode)
                {
                    AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    StoreToken(result.Token);
                    HideOtpModal();
                    Navigation.NavigateTo("/");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OTP verification failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to verify otp: {ex.Message}");
            }
            finally
            {
                isVerifying = false;
            }
        }

        private async Task HandleValidSubmitRegister()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register account: {ex.Message}");
            }
            finally
            {
                isVerifying = false; 
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

        private void HideOtpModal()
        {
            JS.InvokeVoidAsync("hideModal", "otpModal");
        }
    }
}
