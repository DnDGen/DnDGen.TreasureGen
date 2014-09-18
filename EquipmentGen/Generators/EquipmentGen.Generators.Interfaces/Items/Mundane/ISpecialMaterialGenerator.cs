﻿using System;
using System.Collections.Generic;

namespace EquipmentGen.Generators.Interfaces.Items.Mundane
{
    public interface ISpecialMaterialGenerator
    {
        Boolean CanHaveSpecialMaterial(String itemType, IEnumerable<String> attributes, IEnumerable<String> traits);
        String GenerateFor(String itemType, IEnumerable<String> attributes, IEnumerable<String> traits); 
    }
}