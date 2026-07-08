using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.Services
{
    public class IISServiceService
    {
        public static List<IISServiceModel> GetIISServices()
        {
            List<IISServiceModel> iIsServiceList = new();

            var serverManager = new Microsoft.Web.Administration.ServerManager();

            foreach (var site in serverManager.Sites.Where(s => s.Name.StartsWith("LinxDMS_", StringComparison.OrdinalIgnoreCase)))
            {
                string serviceName = site.Name;
                string servicePort = string.Empty;

                foreach (var binding in site.Bindings)
                {
                    servicePort = binding.BindingInformation.Substring(2, 2);
                }

                iIsServiceList.Add(new IISServiceModel(serviceName, servicePort));
            }

            return iIsServiceList;
        }
    }
}
