using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;

namespace Orders.Frontend.Pages.Auth
{
    public partial class ResetPassword
    {
        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private ResetPasswordDTO resetPasswordDTO = new();
        private bool loading;

        [CascadingParameter]
        private IModalService Modal { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Token { get; set; } = "";

        private async Task ChangePasswordAsync()
        {
            loading = true;
            resetPasswordDTO.Token = Token;
            var responseHttp = await repository.PostAsync("/api/accounts/ResetPassword", resetPasswordDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                loading = false;
                return;
            }

            loading = false;
            await sweetAlertService.FireAsync("Confirmación", "Contraseña cambiada con éxito, ahora puede ingresar con su nueva contraseña.", SweetAlertIcon.Info);
            Modal.Show<Login>();
        }
    }
}