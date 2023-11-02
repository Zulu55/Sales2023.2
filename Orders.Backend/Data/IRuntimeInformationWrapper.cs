using System.Runtime.InteropServices;

namespace Orders.Backend.Data
{
    public interface IRuntimeInformationWrapper
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}