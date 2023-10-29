using System.Net;
using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entites;

namespace Orders.Frontend.Pages.States
{
    [Authorize(Roles = "Admin")]
    public partial class StateEdit
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private State? state;
        private StateForm? stateForm;

        [CascadingParameter]
        private BlazoredModalInstance BlazoredModal { get; set; } = default!;

        [Parameter]
        public int StateId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var response = await repository.GetAsync<State>($"/api/states/{StateId}");
            if (response.Error)
            {
                if (response.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    Return();
                }
                var message = await response.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            state = response.Response;
        }

        private async Task SaveAsync()
        {
            var response = await repository.PutAsync($"/api/states", state);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await BlazoredModal.CloseAsync(ModalResult.Ok());
            Return();

            var toast = sweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Cambios guardados con éxito.");
        }

        private void Return()
        {
            stateForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/countries/details/{state!.CountryId}");
        }
    }
}