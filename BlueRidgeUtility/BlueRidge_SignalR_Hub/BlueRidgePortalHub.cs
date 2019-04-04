using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BlueRidgeUtility.BlueRidge_SignalR_Hub
{
    public class BlueRidgePortalHub : Hub
    {
        public static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BlueRidgePortalHub>();

        public void Hello()
        {
            Clients.All.hello();
        }
    }
}