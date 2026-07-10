using Microsoft.Web.Administration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TrocaBaseGUI.Models;


namespace TrocaBaseGUI.Services
{
    public class IISServiceService
    {
        public static List<IISServiceModel> GetIISDMSSite()
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

        public static async Task StopIISService(string name)
        {
            string cfgName = name.Substring(8);

            var serverManager = new Microsoft.Web.Administration.ServerManager();

            Site dmsSite = serverManager.Sites.FirstOrDefault(s => s.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase));

            var dmsPool = serverManager.ApplicationPools.Where(p => p.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase)).ToList();

            while (dmsSite.State == ObjectState.Started || dmsPool.Any(p => p.State == ObjectState.Started || p.State == ObjectState.Stopping))
            {
                serverManager = new Microsoft.Web.Administration.ServerManager();
                dmsSite = serverManager.Sites.FirstOrDefault(s => s.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase));
                dmsPool = serverManager.ApplicationPools.Where(p => p.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var item in dmsPool)
                {
                    while (item.State == ObjectState.Started)
                    {
                        if(item.State == ObjectState.Started)
                            item.Stop();
                    }
                }
                if (dmsSite.State == ObjectState.Started)
                    dmsSite.Stop();

                await Task.Delay(250);
            }
        }

        public static async Task StartIISService(string name)
        {
            string cfgName = name.Substring(8);

            var serverManager = new Microsoft.Web.Administration.ServerManager();

            Site dmsSite = serverManager.Sites.FirstOrDefault(s => s.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase));

            var dmsPool = serverManager.ApplicationPools.Where(p => p.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase)).ToList();

            while (dmsPool.Any(p => p.State == ObjectState.Stopped))
            {
                serverManager = new Microsoft.Web.Administration.ServerManager();
                dmsSite = serverManager.Sites.FirstOrDefault(s => s.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase));
                dmsPool = serverManager.ApplicationPools.Where(p => p.Name.EndsWith(cfgName, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var item in dmsPool)
                {
                    while (item.State == ObjectState.Stopped || item.State == ObjectState.Stopping)
                    {
                        if(item.State == ObjectState.Stopped)
                            item.Start();
                    }
                }
                if(dmsSite.State == ObjectState.Stopped)
                    dmsSite.Start();

            }
        }
    }
}
