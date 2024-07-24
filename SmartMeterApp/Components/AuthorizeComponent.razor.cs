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

                // 1. Send the login request to the API
                var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    // 2. Check if OTP was sent: if yes the API returns otpSent property
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);

                    if (responseObject.TryGetValue("otpSent", out var otpSentValue) && otpSentValue.ValueKind == JsonValueKind.True)
                    {
                        // 3. Handle OTP sent scenario
                        string userEmail = responseObject["email"].ToString();
                        ToastService.AddToast($"OTP was sent to {userEmail}", ToastType.Info);

                        // 4. show otp input modal
                        verifyOtpModel.Email = userEmail;
                        await JS.InvokeVoidAsync("showModal", "modal");
                    }
                    else
                    {
                        // OTP not sent scenario
                        Navigation.NavigateTo("/login");
                        ToastService.AddToast($"Error while sending otp. Please login again.", ToastType.Error);
                    }
                }
                else
                {
                    // Handle login failure
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ToastService.AddToast($"Login failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
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

                // 1. Send the entered otp to the API for verification
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/auth/verifyotp", verifyOtpModel);

                if (response.IsSuccessStatusCode)
                {
                    // 2. Handle result from API verification process -> store JWT Token
                    AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (result == null)
                    {
                        throw new Exception("OTP verification failed.");
                    }

                    await StoreToken(result.Token);

                    // 3. Extract payload data from JWT Token
                    TokenData? tokenData = TokenHelper.GetTokenData(result.Token);

                    if (tokenData == null)
                    {
                        throw new InvalidTokenException("Invalid Token.");
                    }

                    HideOtpModal();
                    // 4. check role
                    if (tokenData.Role == "User")
                    {
                        //navigate to usage data page
                        Navigation.NavigateTo("/overview");
                    }
                    else if (tokenData.Role == "Operator")
                    {
                        //navigate to user page
                        Navigation.NavigateTo("/users");
                    }
                    else
                    {
                        //unknown role -> navigate back to login page
                        Navigation.NavigateTo("/login");
                    }
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ToastService.AddToast($"OTP verification failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
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
                isVerifying = true;
                // 1. Validate the repeated password to prevent user from typo
                if (registerModel.Password != repeatedPassword)
                {
                    showRepeatedPasswordError = true;
                    return;
                }

                showRepeatedPasswordError = false;

                // 2. Send the register request to the API
                var response = await HttpClient.PostAsJsonAsync("api/auth/register", registerModel);

                // 3. Check API response
                // success: user can now login
                if (response.IsSuccessStatusCode)
                {
                    userAction = UserActions.Login;
                    ToastService.AddToast($"Registration successfull.", ToastType.Info);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    // Handle registration failure
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ToastService.AddToast($"Registration failed: {errorMessage}", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
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
