using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarcusW.SharpUtils.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLateDependenciesSupport(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddTransient(typeof(ILateDependency<>), typeof(LateDependency<>));
        }
    }
}
