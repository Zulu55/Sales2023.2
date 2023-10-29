using Microsoft.AspNetCore.Components;
using Orders.Frontend.Services;

namespace Orders.Frontend.Pages.Auth
{
    public partial class Logout
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private ILoginService loginService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await loginService.LogoutAsync();
            navigationManager.NavigateTo("/");
        }
    }
}