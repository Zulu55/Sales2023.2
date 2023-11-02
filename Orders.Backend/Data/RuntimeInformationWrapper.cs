using System.Runtime.InteropServices;

namespace Orders.Backend.Data
{
    public class RuntimeInformationWrapper : IRuntimeInformationWrapper
    {
        public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}