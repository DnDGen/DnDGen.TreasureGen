using DnDGen.Infrastructure.IoC;
using DnDGen.TreasureGen.IoC.Modules;
using Ninject;
using System.Linq;

namespace DnDGen.TreasureGen.IoC
{
    public class TreasureGenModuleLoader
    {
        public static void LoadModules(IKernel kernel)
        {
            //Dependencies
            InfrastructureModuleLoader.LoadModules(kernel);

            //TreasureGen
            var modules = kernel.GetModules();

            if (!modules.Any(m => m is GeneratorsModule))
                kernel.Load<GeneratorsModule>();

            if (!modules.Any(m => m is SelectorsModule))
                kernel.Load<SelectorsModule>();
        }
    }
}