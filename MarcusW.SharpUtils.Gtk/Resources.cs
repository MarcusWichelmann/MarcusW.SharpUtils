using System;
using System.IO;
using System.Reflection;
using Gtk;

namespace MarcusW.SharpUtils.Gtk
{
    public static class Resources
    {
        /// <summary>
        /// Creates a CssProvider for a given CSS resource.
        /// </summary>
        /// <param name="resourceName">Name of the CSS resource</param>
        /// <param name="sourceAssembly">The assembly to search the resource in</param>
        /// <returns>A CssProvider</returns>
        public static CssProvider LoadStyle(string resourceName, Assembly sourceAssembly = null)
        {
            if (resourceName == null)
                throw new ArgumentNullException(nameof(resourceName));

            Assembly assembly = sourceAssembly ?? Assembly.GetEntryAssembly();
            if (assembly == null)
                throw new ArgumentException("Start assembly cannot be automatically retrieved and therefore needs to be passed as an argument manually.", nameof(sourceAssembly));

            var cssProvider = new CssProvider();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Resource not found in assembly {assembly.FullName}. Maybe you need to pass another assembly."))
            using (var streamReader = new StreamReader(resourceStream))
            {
                cssProvider.LoadFromData(streamReader.ReadToEnd());
            }

            return cssProvider;
        }
    }
}
