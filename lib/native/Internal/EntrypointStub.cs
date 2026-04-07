using System.Reflection;
using System.Runtime.CompilerServices;

namespace tosu.pp.Internal;

internal static class EntrypointStub
{
#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Init()
    {
        // Fix for entry detection in o!f
        Assembly.SetEntryAssembly(typeof(EntrypointStub).Assembly);
    }
}