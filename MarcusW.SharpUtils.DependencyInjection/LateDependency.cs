using System;
using Microsoft.Extensions.DependencyInjection;

namespace MarcusW.SharpUtils.DependencyInjection
{
    public class LateDependency<T> : Lazy<T>, ILateDependency<T>
    {
        public LateDependency(IServiceProvider serviceProvider) : base(() => (T)serviceProvider.GetRequiredService(typeof(T)))
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
        }
    }
}
