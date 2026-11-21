# TreasureGen

Generate random Dungeons & Dragons treasure, equipment, and items

[![Build Status](https://github.com/DnDGen/DnDGen.TreasureGen/actions/workflows/release.yml/badge.svg?branch=main)](https://github.com/DnDGen/DnDGen.TreasureGen/actions/workflows/release.yml?query=branch%3Amain)

### Use

To use TreasureGen, simply generate from the generator you wish to use.  Levels are from 1 to 20, and powers can be Mundane, Minor, Medium, and Major.

```C#
var treasure = treasureGenerator.GenerateAtLevel(6);
var mundaneArmor = mundaneArmorGenerator.Generate();
var magicalWeapon = magicalWeaponGenerator.GenerateAtPower(PowerConstants.Major);
```

### Getting the Generators

You can obtain generators via the Ninject module. Because the generators are very complex and are decorated in various ways, there is not a (recommended) way to build these generator manually. **Note:** This will also load the dependencies for TreasureGen, including RollGen and Infrastructure

```C#
var kernel = new StandardKernel();
var treasureGenModuleLoader = new TreasureGenModuleLoader();

treasureGenModuleLoader.LoadModules(kernel);
```

Your particular syntax for how the Ninject injection should work will depend on your project (class library, web site, etc.)

### Installing TreasureGen

The project is on [Nuget](https://www.nuget.org/packages/DnDGen.TreasureGen). Install via the NuGet Package Manager.

    PM > Install-Package DnDGen.TreasureGen
