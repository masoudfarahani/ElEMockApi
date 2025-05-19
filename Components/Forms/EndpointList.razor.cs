using ELE.MockApi.Components.Pages;
using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;
using ELE.MockApi.Shared;
using MudBlazor;

namespace ELE.MockApi.Components.Forms
{
    public partial class EndpointList
    {
        private List<MockEndpoint> endpoints = new();
        private PaginationModel pagination = new();
        private bool loading;
        private string filter = "";

        protected override async Task OnInitializedAsync()
        {
            appEvents.OnNewEndpointAdded += LoadEndpointsAsync;
            await LoadEndpointsAsync();
        }

        public void Dispose()
        {
            appEvents.OnNewEndpointAdded -= LoadEndpointsAsync;
        }

        private async Task LoadEndpointsAsync()
        {
            loading = true;
            try
            {
                var (items, totalCount) = await EndpointService.GetEndpointsAsync(pagination.PageNumber, pagination.PageSize,filter);
                endpoints = items;
                pagination.TotalCount = totalCount;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageBox("Error", ex.Message);
            }
            finally
            {
                loading = false;
            }
        }


        private async Task DeleteEndpoint(Guid endpointId)
        {
            await EndpointService.DeleteEndpointAsync(endpointId);
            await LoadEndpointsAsync();
        }

        private async Task SelectForUpdate(MockEndpoint model)
        {
            await appEvents.NotifyEndpointSelected(model);
        }

        private async Task ShowResponseDetails(AvailableResponse response)
        {
            var parameters = new DialogParameters
            {
                ["Response"] = response
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            await DialogService.ShowAsync<ResponseDetailDialog>("Response Details", parameters, options);
        }


        private async Task ShowRuleAndSchemaDetails(string content, string subject)
        {
            var parameters = new DialogParameters
            {
                ["Content"] = content
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            await DialogService.ShowAsync<RuleAndSchemaDetailDialog>($"{subject} Details", parameters, options);
        }

        private async Task OnSearch(string text)
        {
            filter = text.ToString();
            await LoadEndpointsAsync();
        }
    }
}