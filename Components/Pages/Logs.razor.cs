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
        private List<Log> logs = new();
        private PaginationModel pagination = new();
        private bool loading;
        private LogType? logtypeFilter = null;
        protected override async Task OnInitializedAsync()
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            loading = true;
            try
            {
                var (items, totalCount) = await LogService.GetLogsAsync(pagination.PageNumber, pagination.PageSize,null);
                logs = items;
                pagination.TotalCount = totalCount;
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

    

        private async Task OnSearch(LogType logType)
        {
            logtypeFilter = logType;
            await LoadLogsAsync();
        }
    }
}