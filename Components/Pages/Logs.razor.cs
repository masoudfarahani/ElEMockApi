using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using static MudBlazor.Colors;
using System;
using ELE.MockApi.Components.Forms;
using MudBlazor;

namespace ELE.MockApi.Components.Pages
{
    public partial class Logs
    {
        private List<ApiCallLog> logs = new();
        private PaginationModel pagination = new();
        private bool loading;
        private string filterUrl = "";
        protected override async Task OnInitializedAsync()
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            loading = true;
            try
            {
                var (items, totalCount) = await CallLogService.GetApiCallLogsAsync(pagination.PageNumber, pagination.PageSize,filterUrl);
                logs = items;
                pagination.TotalCount = totalCount;
                //StateHasChanged();
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

        private async Task ShowDetails(string response, string body, string headers)
        {
            var parameters = new DialogParameters
            {
                ["Response"] = response,
                ["Body"] = body,
                ["Header"] = headers
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            await DialogService.ShowAsync<LogDetailDialog>("Log Details", parameters, options);
        }


        private async Task OnSearch(string text)
        {
            filterUrl = text.ToString();
            await LoadLogsAsync();
        }
    }
}