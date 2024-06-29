using Microsoft.AspNetCore.Components;
using SmartMeterApi.Models;

namespace SmartMeterApp.Components
{
    public partial class AuthorizeComponent
    {
        [Parameter] public UserActions UserAction { get; set; }
        [Parameter] public EventCallback OnUserActionChanged { get; set; }

        private LoginModel loginModel = new LoginModel();
        private RegisterModel registerModel = new RegisterModel();

        private void HandleValidSubmitRegister()
        {

        }

        private void HandleValidSubmitLogin()
        {

        }

        private async Task UserActionButtonOnClick()
        {
            await OnUserActionChanged.InvokeAsync();
            StateHasChanged();
        }
    }
}
