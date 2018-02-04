using SimpleInjector;
using System;
using Xunit;

namespace Sextant.Host.Tests
{
    public class BootstrapperTests
    {
        [Fact]
        public static void Bootstrapper_Should_RegisterSucessfully()
        {
            var container = new Container();
            Bootstrapper b = new Bootstrapper();

            b.Bootstrap(Environment.CurrentDirectory, container);
        }
    }
}
