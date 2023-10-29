using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Services;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;

namespace Orders.Frontend.Pages.Auth
{
    public partial class EditUser
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        [Inject] private ILoginService loginService { get; set; } = null!;

        private User? user;
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private string? imageUrl;
        private bool loading;

        [CascadingParameter]
        private IModalService Modal { get; set; } = default!;

        private void ShowModal()
        {
            Modal.Show<ChangePassword>();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadUserAsyc();
            await LoadCountriesAsync();
            await LoadStatesAsyn(user!.City!.State!.Country!.Id);
            await LoadCitiesAsyn(user!.City!.State!.Id);

            if (!string.IsNullOrEmpty(user!.Photo))
            {
                imageUrl = user.Photo;
                user.Photo = null;
            }
        }

        private async Task LoadUserAsyc()
        {
            var responseHTTP = await repository.GetAsync<User>($"/api/accounts");
            if (responseHTTP.Error)
            {
                if (responseHTTP.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/");
                    return;
                }
                var messageError = await responseHTTP.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", messageError, SweetAlertIcon.Error);
                return;
            }
            user = responseHTTP.Response;
        }

        private void ImageSelected(string imagenBase64)
        {
            user!.Photo = imagenBase64;
            imageUrl = null;
        }

        private async Task CountryChangedAsync(ChangeEventArgs e)
        {
            var selectedCountry = Convert.ToInt32(e.Value!);
            states = null;
            cities = null;
            user!.CityId = 0;
            await LoadStatesAsyn(selectedCountry);
        }

        private async Task StateChangedAsync(ChangeEventArgs e)
        {
            var selectedState = Convert.ToInt32(e.Value!);
            cities = null;
            user!.CityId = 0;
            await LoadCitiesAsyn(selectedState);
        }

        private async Task LoadCountriesAsync()
        {
            var responseHttp = await repository.GetAsync<List<Country>>("/api/countries/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            countries = responseHttp.Response;
        }

        private async Task LoadStatesAsyn(int countryId)
        {
            var responseHttp = await repository.GetAsync<List<State>>($"/api/states/combo/{countryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            states = responseHttp.Response;
        }

        private async Task LoadCitiesAsyn(int stateId)
        {
            var responseHttp = await repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            cities = responseHttp.Response;
        }

        private async Task SaveUserAsync()
        {
            loading = true;
            var responseHttp = await repository.PutAsync<User, TokenDTO>("/api/accounts", user!);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await loginService.LoginAsync(responseHttp.Response!.Token);
            navigationManager.NavigateTo("/");
        }
    }
}