using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Shared.DTOs;
using Orders.Frontend.Repositories;

namespace Orders.Frontend.Pages.Auth
{
    public partial class ChangePassword
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private ChangePasswordDTO changePasswordDTO = new();
        private bool loading;

        [CascadingParameter]
        private BlazoredModalInstance BlazoredModal { get; set; } = default!;

        private async Task ChangePasswordAsync()
        {
            loading = true;
            var responseHttp = await repository.PostAsync("/api/accounts/changePassword", changePasswordDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                loading = false;
                return;
            }

            loading = false;
            await BlazoredModal.CloseAsync(ModalResult.Ok());

            navigationManager.NavigateTo("/editUser");
            var toast = sweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 5000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Contraseña cambiada con éxito.");
        }
    }
}