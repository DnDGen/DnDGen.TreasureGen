﻿using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Selectors.Interfaces;
using EquipmentGen.Tables.Interfaces;

namespace EquipmentGen.Generators.Items.Magical
{
    public class MagicalItemTraitsGenerator : IMagicalItemTraitsGenerator
    {
        private IPercentileSelector percentileSelector;

        public MagicalItemTraitsGenerator(IPercentileSelector percentileSelector)
        {
            this.percentileSelector = percentileSelector;
        }

        public IEnumerable<String> GenerateFor(String itemType, IEnumerable<String> attributes)
        {
            var tableName = GetTableName(itemType, attributes);
            var result = percentileSelector.SelectFrom(tableName);

            if (String.IsNullOrEmpty(result))
                return Enumerable.Empty<String>();

            return result.Split(',');
        }

        private String GetTableName(String itemType, IEnumerable<String> attributes)
        {
            if (attributes.Contains(AttributeConstants.Melee))
                return String.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, AttributeConstants.Melee);

            if (attributes.Contains(AttributeConstants.Ranged))
                return String.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, AttributeConstants.Ranged);

            return String.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, itemType);
        }
    }
}