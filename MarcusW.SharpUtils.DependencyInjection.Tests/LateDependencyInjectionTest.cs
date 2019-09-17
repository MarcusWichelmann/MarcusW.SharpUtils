using System;
using MarcusW.SharpUtils.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MarcusW.SharpUtils.DependencyInjection.Tests
{
    public class LateDependencyInjectionTest
    {
        [Fact]
        public void InjectsLateDependency()
        {
            bool testDependencyCreated = false;

            // Setup services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLateDependenciesSupport();
            serviceCollection.AddTransient(_ => {
                testDependencyCreated = true;
                return Mock.Of<ITestDependency>();
            });
            serviceCollection.AddTransient<TestClass>();

            // Build service provider
            using (ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Dependency should not have been created yet.
                Assert.False(testDependencyCreated);

                // Make the service provider constructing the TestClass
                var testClass = serviceProvider.GetRequiredService<TestClass>();

                // Dependency should still not have been created.
                Assert.False(testDependencyCreated);

                // Use the test dependency
                testClass.UseTestDependency();

                // Now the dependency should have been created
                Assert.True(testDependencyCreated);
            }
        }

        public class TestClass
        {
            private readonly ILateDependency<ITestDependency> _testDependency;

            public TestClass(ILateDependency<ITestDependency> testDependency)
            {
                _testDependency = testDependency ?? throw new ArgumentNullException(nameof(testDependency));
            }

            public string UseTestDependency()
            {
                ITestDependency dependency = _testDependency.Value;
                return dependency.ToString();
            }
        }

        public interface ITestDependency { }
    }
}
