using System;
using System.IO;
using System.Reflection;
using Gtk;

namespace MarcusW.SharpUtils.Gtk
{
    public static class Resources
    {
        public static CssProvider LoadStyle(string resourceName, Assembly sourceAssembly = null)
        {
            if (resourceName == null)
                throw new ArgumentNullException(nameof(resourceName));

            Assembly startAssembly = sourceAssembly ?? Assembly.GetEntryAssembly();
            if (startAssembly == null)
                throw new ArgumentException("Start assembly cannot be automatically retrieved and therefore needs to be passed as an argument manually.", nameof(sourceAssembly));

            var cssProvider = new CssProvider();
            using (Stream resourceStream = startAssembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException("Resource not found"))
            using (var streamReader = new StreamReader(resourceStream))
            {
                cssProvider.LoadFromData(streamReader.ReadToEnd());
            }

            return cssProvider;
        }
    }
}
