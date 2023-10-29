using System.Net;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Enums;

namespace Orders.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class OrderDetails
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private Order? order;

        [Parameter]
        public int OrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var responseHppt = await repository.GetAsync<Order>($"api/orders/{OrderId}");
            if (responseHppt.Error)
            {
                if (responseHppt.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/orders");
                    return;
                }
                var messageError = await responseHppt.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", messageError, SweetAlertIcon.Error);
                return;
            }
            order = responseHppt.Response;
        }

        private async Task CancelOrderAsync()
        {
            await ModifyTemporalOrder("cancelar", OrderStatus.Cancelled);
        }

        private async Task DispatchOrderAsync()
        {
            await ModifyTemporalOrder("despachar", OrderStatus.Dispatched);
        }

        private async Task SendOrderAsync()
        {
            await ModifyTemporalOrder("enviar", OrderStatus.Sent);
        }

        private async Task ConfirmOrderAsync()
        {
            await ModifyTemporalOrder("confirmar", OrderStatus.Confirmed);
        }

        private async Task ModifyTemporalOrder(string message, OrderStatus status)
        {
            var result = await sweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Esta seguro que quieres {message} el pedido?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var saleDTO = new OrderDTO
            {
                Id = OrderId,
                OrderStatus = status
            };

            var responseHTTP = await repository.PutAsync("api/orders", saleDTO);
            if (responseHTTP.Error)
            {
                var mensajeError = await responseHTTP.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                return;
            }

            navigationManager.NavigateTo("/orders");
        }
    }
}