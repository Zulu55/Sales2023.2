using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;

namespace Orders.Frontend.Pages.Auth
{
    public partial class ConfirmEmail
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private string? message;

        [CascadingParameter]
        private IModalService Modal { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string UserId { get; set; } = "";

        [Parameter]
        [SupplyParameterFromQuery]
        public string Token { get; set; } = "";

        protected async Task ConfirmAccountAsync()
        {
            var responseHttp = await repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                navigationManager.NavigateTo("/");
            }
            else
            {
                await sweetAlertService.FireAsync("Confirmación", "Gracias por confirmar su email, ahora puedes ingresar al sistema.", SweetAlertIcon.Info);
                Modal.Show<Login>();
            }
        }
    }
}