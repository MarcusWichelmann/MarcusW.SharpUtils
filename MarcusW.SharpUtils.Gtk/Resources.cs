using System;
using System.IO;
using System.Reflection;
using Gtk;

namespace MarcusW.SharpUtils.Gtk
{
    public static class Resources
    {
        public static CssProvider LoadStyle(string resourceName)
        {
            if (resourceName == null)
                throw new ArgumentNullException(nameof(resourceName));

            var cssProvider = new CssProvider();
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ?? throw new ArgumentException("Resource not found"))
            using (var streamReader = new StreamReader(resourceStream))
            {
                cssProvider.LoadFromData(streamReader.ReadToEnd());
            }

            return cssProvider;
        }
    }
}
