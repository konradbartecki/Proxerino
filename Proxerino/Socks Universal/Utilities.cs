using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;

namespace Proxerino
{
    public static class Utilities
    {
        public static HostName CurrentHostName()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp != null && icp.NetworkAdapter != null)
            {
                var hostname =
                    NetworkInformation.GetHostNames()
                        .SingleOrDefault(
                            hn =>
                                hn.IPInformation != null && hn.IPInformation.NetworkAdapter != null
                                && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                                == icp.NetworkAdapter.NetworkAdapterId);

                if (hostname != null)
                {
                    // the ip address
                    return new HostName(hostname.CanonicalName);
                }
            }
            return new HostName(string.Empty);
        }

        public static NetworkAdapter GetWanNetworkAdapter()
        {
            var icp = NetworkInformation.GetConnectionProfiles();

            if (icp != null)
            {
                ConnectionProfile wwanConnectionProfile = icp.SingleOrDefault(na => na.IsWwanConnectionProfile);
                return wwanConnectionProfile.NetworkAdapter;
            }
            return null;
        }

        //public async static void CellulaNetworkAdapter(out NetworkAdapter networkAdapter)
        //{
        //    ConnectionProfileFilter filter = new ConnectionProfileFilter();
        //    filter.IsWwanConnectionProfile = true;
        //    IReadOnlyList<ConnectionProfile> connectionProfiles = await NetworkInformation.FindConnectionProfilesAsync(filter);
        //    foreach (ConnectionProfile cp in connectionProfiles)
        //    {
        //        if (cp.IsWwanConnectionProfile)
        //        {
        //            networkAdapter = cp.NetworkAdapter;
        //            return;
        //        }                 
        //    }
        //}
    }
}
