using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Orders.Backend.Data
{
    [ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
    public class RuntimeInformationWrapper : IRuntimeInformationWrapper
    {
        public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}