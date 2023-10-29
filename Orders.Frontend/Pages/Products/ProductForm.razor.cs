﻿using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Orders.Frontend.Helpers;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;

namespace Orders.Frontend.Pages.Products
{
    public partial class ProductForm
    {
        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private EditContext editContext = null!;
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();
        private string? imageUrl;

        [Parameter]
        public bool IsEdit { get; set; } = false;

        [EditorRequired]
        [Parameter]
        public ProductDTO ProductDTO { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired]
        [Parameter]
        public EventCallback ReturnAction { get; set; }

        [Parameter]
        public EventCallback AddImageAction { get; set; }

        [Parameter]
        public EventCallback RemoveImageAction { get; set; }

        [Parameter]
        public List<Category> SelectedCategories { get; set; } = new();

        [Parameter]
        [EditorRequired]
        public List<Category> NonSelectedCategories { get; set; } = new();

        public bool FormPostedSuccessfully { get; set; } = false;

        protected override void OnInitialized()
        {
            editContext = new(ProductDTO);

            selected = SelectedCategories.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            nonSelected = NonSelectedCategories.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
        }

        private void ImageSelected(string imagenBase64)
        {
            if (ProductDTO.ProductImages is null)
            {
                ProductDTO.ProductImages = new List<string>();
            }

            ProductDTO.ProductImages!.Add(imagenBase64);
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            ProductDTO.ProductCategoryIds = selected.Select(x => int.Parse(x.Key)).ToList();
            await OnValidSubmit.InvokeAsync();
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
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
                Icon = SweetAlertIcon.Warning,
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