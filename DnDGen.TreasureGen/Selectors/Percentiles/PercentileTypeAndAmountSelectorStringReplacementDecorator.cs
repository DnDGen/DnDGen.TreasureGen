using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Selectors.Percentiles
{
    internal class PercentileTypeAndAmountSelectorStringReplacementDecorator : ITreasurePercentileTypeAndAmountSelector
    {
        private readonly IPercentileTypeAndAmountSelector innerSelector;
        private readonly IReplacementSelector replacementSelector;

        public PercentileTypeAndAmountSelectorStringReplacementDecorator(
            IPercentileTypeAndAmountSelector innerSelector,
            IReplacementSelector replacementSelector)
        {
            this.innerSelector = innerSelector;
            this.replacementSelector = replacementSelector;
        }

        public TypeAndAmountDataSelection SelectFrom(string assemblyName, string tableName)
        {
            var result = innerSelector.SelectFrom(assemblyName, tableName);
            result.Type = replacementSelector.SelectRandom(result.Type);

            return result;
        }

        public IEnumerable<TypeAndAmountDataSelection> SelectAllFrom(string assemblyName, string tableName)
        {
            var results = innerSelector.SelectAllFrom(assemblyName, tableName);
            var allResults = new List<TypeAndAmountDataSelection>();

            foreach (var result in results)
            {
                var replacedResults = replacementSelector
                    .SelectAll(result.Type)
                    .Select(t => new TypeAndAmountDataSelection { Type = t, Roll = result.Roll, AmountAsDouble = result.AmountAsDouble });

                allResults.AddRange(replacedResults);
            }

            return allResults;
        }
    }
}