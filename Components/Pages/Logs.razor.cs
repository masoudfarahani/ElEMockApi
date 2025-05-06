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

        private string _buttonText = "All Logs";

        private async Task SetButtonText(string text)
        {
            if (_buttonText == text)
                return;

            _buttonText = text;

            switch (text)
            {
                case "All Logs":
                    logtypeFilter = null;
                    break;
                case "App Logs":
                    logtypeFilter = LogType.AppLog;
                    break;
                case "User Logs":
                    logtypeFilter = LogType.UserLog;
                    break;
            }


          
            await LoadLogsAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            loading = true;
            try
            {
                var (items, totalCount) = await LogService.GetLogsAsync(pagination.PageNumber, pagination.PageSize,logtypeFilter);
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
    
    }
}