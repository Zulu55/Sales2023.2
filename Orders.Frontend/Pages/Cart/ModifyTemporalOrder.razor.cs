using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;

namespace Orders.Frontend.Pages.Cart
{
    public partial class ModifyTemporalOrder
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private List<string>? categories;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private TemporalOrderDTO? temporalOrderDTO;

        [Parameter]
        public int TemporalOrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadTemporalOrderAsync();
        }

        private async Task LoadTemporalOrderAsync()
        {
            loading = true;
            var httpResponse = await repository.GetAsync<TemporalOrder>($"/api/temporalOrders/{TemporalOrderId}");

            if (httpResponse.Error)
            {
                loading = false;
                var message = await httpResponse.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            var temporalOrder = httpResponse.Response!;
            temporalOrderDTO = new TemporalOrderDTO
            {
                Id = temporalOrder.Id,
                ProductId = temporalOrder.ProductId,
                Remarks = temporalOrder.Remarks!,
                Quantity = temporalOrder.Quantity
            };
            product = temporalOrder.Product;
            categories = product!.ProductCategories!.Select(x => x.Category.Name).ToList();
            images = product.ProductImages!.Select(x => x.Image).ToList();
            loading = false;
        }

        public async Task UpdateCartAsync()
        {
            var httpResponse = await repository.PutAsync("/api/temporalOrders/full", temporalOrderDTO);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            var toast2 = sweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast2.FireAsync(icon: SweetAlertIcon.Success, message: "Producto modificado en el de compras.");
            navigationManager.NavigateTo("/");
        }
    }
}