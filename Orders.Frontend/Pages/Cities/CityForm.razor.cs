using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Orders.Shared.Entites;

namespace Orders.Frontend.Pages.Cities
{
    public partial class CityForm
    {
        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private EditContext editContext = null!;

        protected override void OnInitialized()
        {
            editContext = new(City);
        }

        [EditorRequired]
        [Parameter]
        public City City { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired]
        [Parameter]
        public EventCallback ReturnAction { get; set; }

        public bool FormPostedSuccessfully { get; set; }

        private async Task OnBeforeInternalNavigationAsync(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();

            if (!formWasEdited)
            {
                return;
            }

            if (FormPostedSuccessfully)
            {
                return;
            }

            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = !string.IsNullOrEmpty(result.Value);

            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}