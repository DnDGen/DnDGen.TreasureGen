﻿using DnDGen.EventGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;

namespace DnDGen.TreasureGen.Generators.Items.Mundane
{
    internal class MundaneItemGeneratorEventDecorator : MundaneItemGenerator
    {
        private readonly GenEventQueue eventQueue;
        private readonly MundaneItemGenerator innerGenerator;

        public MundaneItemGeneratorEventDecorator(MundaneItemGenerator innerGenerator, GenEventQueue eventQueue)
        {
            this.eventQueue = eventQueue;
            this.innerGenerator = innerGenerator;
        }

        public Item GenerateFrom(Item template, bool allowRandomDecoration = false)
        {
            eventQueue.Enqueue("TreasureGen", $"Beginning mundane item generation from template: {template.ItemType} {template.Name}");
            var item = innerGenerator.GenerateFrom(template, allowRandomDecoration);
            eventQueue.Enqueue("TreasureGen", $"Completed generation of {item.ItemType} {item.Name}");

            return item;
        }

        public Item Generate()
        {
            eventQueue.Enqueue("TreasureGen", "Beginning mundane item generation");
            var item = innerGenerator.Generate();
            eventQueue.Enqueue("TreasureGen", $"Completed generation of {item.ItemType} {item.Name}");

            return item;
        }

        public Item Generate(string itemName)
        {
            eventQueue.Enqueue("TreasureGen", $"Beginning mundane item generation ({itemName})");
            var item = innerGenerator.Generate(itemName);
            eventQueue.Enqueue("TreasureGen", $"Completed generation of {item.ItemType} {item.Name}");

            return item;
        }
    }
}
