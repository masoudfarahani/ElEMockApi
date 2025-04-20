using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;

namespace ELE.MockApi.Shared
{
    public class AppEvents
    {
        public event Func<Task> OnNewEndpointAdded;
        public event Func<MockEndpoint,Task> OnEndpointSelectd;
        

        public async Task NotifyNewEndpoint()
        {
            if (OnNewEndpointAdded != null)
                await OnNewEndpointAdded.Invoke();
        }

        public async Task NotifyEndpointSelected(MockEndpoint endpointModel)
        {
            if (OnEndpointSelectd != null)
                await OnEndpointSelectd.Invoke(endpointModel);
        }

    }
}
