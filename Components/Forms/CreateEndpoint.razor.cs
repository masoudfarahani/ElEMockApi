using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;
using ELE.MockApi.Migrations;
using Jsbeautifier;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System.Text.RegularExpressions;

namespace ELE.MockApi.Components.Forms
{
    public partial class CreateEndpoint
    {
        private Beautifier beautifier = new Beautifier();
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

        public void BeautifyRule()
        {
            string prettyJs = beautifier.Beautify(model.Rule);
            this.model.Rule = prettyJs;
            StateHasChanged();
        }
        public void BeautifyBodySchema()
        {
            string prettyJs = beautifier.Beautify(model.RequestBodySchema);
            this.model.RequestBodySchema = prettyJs;
            StateHasChanged();
        }


        public void BeautifyResponseBody(Guid rsponseId)
        {
            var resp = this.model.Responses.FirstOrDefault(c => c.Id == rsponseId);
            string prettyJs = beautifier.Beautify(resp.Body);
            resp.Body = prettyJs;
            StateHasChanged();
        }

        private string ValidateBaseUrl(string url)
        {
            string pattern = @"^((https?:\/\/)?([\w-]+\.)+[\w-]+)?(\/[\w\-.\/?%&=]*)?$";
            return Regex.IsMatch(model.BaseUrl, pattern, RegexOptions.IgnoreCase) ? "" : "Invalid url";
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
                RequestBodySchema = _model.RequestBodySchema,
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