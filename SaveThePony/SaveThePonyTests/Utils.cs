using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SaveThePonyTests
{
    public class Utils
    {
        public static Stream GetEmbeddedResourceFileStream(string resourceId)
        {
            var assembly = Assembly.GetCallingAssembly();
            if (assembly.GetManifestResourceNames().Contains(resourceId))
            {
                return assembly.GetManifestResourceStream(resourceId);
            }
            return null;
        }
    }
}
