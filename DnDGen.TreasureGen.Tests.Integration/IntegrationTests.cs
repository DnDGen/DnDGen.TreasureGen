using DnDGen.TreasureGen.IoC;
using Ninject;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration
{
    [TestFixture]
    public abstract class IntegrationTests
    {
        protected IKernel kernel;

        public IntegrationTests()
        {
            kernel = new StandardKernel(new NinjectSettings() { InjectNonPublic = true });

            var treasureGenModuleLoader = new TreasureGenModuleLoader();
            TreasureGenModuleLoader.LoadModules(kernel);
        }

        [SetUp]
        public void IntegrationTestsSetup()
        {
            kernel.Inject(this);
        }

        protected T GetNewInstanceOf<T>()
        {
            return kernel.Get<T>();
        }

        protected T GetNewInstanceOf<T>(string name)
        {
            return kernel.Get<T>(name);
        }
    }
}