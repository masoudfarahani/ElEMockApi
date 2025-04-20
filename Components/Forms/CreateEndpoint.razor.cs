using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace ELE.MockApi.Components.Forms
{
    public partial class CreateEndpoint
    {

        private MudForm form;
        private bool success;
        private string[] errors;
        private CreateOrUpdateEndpointModel model = new();
        private bool IsUpdate
        {
            get
            {
                if (model != null && model.Id != null) return true;
                return false;
            }
        }

        protected override void OnInitialized()
        {
            AppEvents.OnEndpointSelectd += GetSelectedModel;
        }

        public void Dispose()
        {
            AppEvents.OnEndpointSelectd -= GetSelectedModel;
        }
        public async Task GetSelectedModel(MockEndpoint _model)
        {
            model = new CreateOrUpdateEndpointModel
            {
                Id = _model.Id,
                BaseUrl = _model.BaseUrl,
                HttpMethod = _model.HttpMethod,
                Rule = _model.Rule,
                Responses = _model.Responses.Select(c => new CreateOrUpdateResponseModel { Id = c.Id, Body = c.Body, Status = c.Status }).ToList()

            };
            await InvokeAsync(StateHasChanged);
        }
        private void AddResponse()
        {
            model.Responses.Add(new CreateOrUpdateResponseModel());
        }

        private void RemoveResponse(CreateOrUpdateResponseModel response)
        {
            model.Responses.Remove(response);
        }

        private async Task CreateOrUpdate()
        {
            await form.Validate();
            if (form.IsValid)
            {
                try
                {
                    if (IsUpdate)
                    {
                        await EndpointService.UpdateEndpointAsync(model);
                        model = new CreateOrUpdateEndpointModel();
                        await DialogService.ShowMessageBox("Success", "Endpoint Updated successfully!");
                        await AppEvents.NotifyNewEndpoint();
                    }
                    else
                    {
                        await EndpointService.CreateEndpointAsync(model);
                        model = new CreateOrUpdateEndpointModel();
                        await DialogService.ShowMessageBox("Success", "Endpoint created successfully!");
                        await AppEvents.NotifyNewEndpoint();
                    }
                }
                catch (Exception ex)
                {
                    await DialogService.ShowMessageBox("Error", ex.Message);
                }
            }
        }
    }
}