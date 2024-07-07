using Microsoft.JSInterop;
using SmartMeterApi.Models;
using SmartMeterApp.Models;
using SmartMeterApp.Utility;
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
                        ToastService.AddToast($"OTP was sent to {userEmail}", ToastType.Info);

                        // show otp input modal
                        await JS.InvokeVoidAsync("showModal", "modal");
                    }
                    else
                    {
                        // OTP not sent scenario
                        Navigation.NavigateTo("/login");
                        Console.WriteLine($"Error while sending otp. Please login again.");
                        ToastService.AddToast($"Error while sending otp. Please login again.", ToastType.Error);
                    }
                }
                else
                {
                    // Handle login failure (e.g., show an error message)
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {errorMessage}");
                    ToastService.AddToast($"Login failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to login: {ex.Message}");
                ToastService.AddToast($"Failed to login: {ex.Message}", ToastType.Error);
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
                    await StoreToken(result.Token);

                    TokenData tokenData = TokenHelper.GetTokenData(result.Token);

                    HideOtpModal();
                    //check role
                    if(tokenData.Role == "User")
                    {
                        //navigate to usage data page
                        Navigation.NavigateTo("/overview");
                    }
                    else if(tokenData.Role == "Operator")
                    {
                        //navigate to user page
                        Navigation.NavigateTo("/users");
                    }
                    else
                    {
                        //unknown role
                        Navigation.NavigateTo("/login");
                    }
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OTP verification failed: {errorMessage}");
                    ToastService.AddToast($"OTP verification failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to verify otp: {ex.Message}");
                ToastService.AddToast($"Failed to verify otp: {ex.Message}", ToastType.Error);
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
                    userAction = UserActions.Login;
                    ToastService.AddToast($"Registration successfull.", ToastType.Info);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    // Handle registration failure (e.g., show an error message)
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed: {errorMessage}");
                    ToastService.AddToast($"Registration failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register account: {ex.Message}");
                ToastService.AddToast($"Failed to register account: {ex.Message}", ToastType.Error);
            }
            finally
            {
                isVerifying = false; 
            }
        }

        private async Task StoreToken(string token)
        {
            // use local storage
            await JS.InvokeVoidAsync("localStorage.setItem", "authToken", token);
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
            JS.InvokeVoidAsync("hideModal", "modal");
        }
    }
}
