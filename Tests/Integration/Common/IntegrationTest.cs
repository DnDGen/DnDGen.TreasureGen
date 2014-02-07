﻿using System;
using System.Collections.Generic;
using EquipmentGen.Bootstrap;
using EquipmentGen.Core.Data.Items;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Common
{
    [TestFixture]
    public abstract class IntegrationTest
    {
        [Inject]
        public Random Random { get; set; }

        private IKernel kernel;

        public IntegrationTest()
        {
            kernel = new StandardKernel();

            var equipmentGenModuleLoader = new EquipmentGenModuleLoader();
            equipmentGenModuleLoader.LoadModules(kernel);

            kernel.Inject(this);
        }

        protected Int32 GetNewLevel()
        {
            return Random.Next(1, 21);
        }

        protected String GetNewPower(Boolean allowMundane)
        {
            var limit = allowMundane ? 4 : 3;

            switch (Random.Next(limit))
            {
                case 0: return PowerConstants.Minor;
                case 1: return PowerConstants.Medium;
                case 2: return PowerConstants.Major;
                case 3: return PowerConstants.Mundane;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected IEnumerable<String> GetNewTypes(Boolean allowNoMaterial)
        {
            var types = new List<String>();

            switch (Random.Next(2))
            {
                case 0: types.Add(ItemTypeConstants.Armor); break;
                case 1: types.Add(ItemTypeConstants.Weapon); break;
            }

            if (types.Contains(ItemTypeConstants.Weapon))
            {
                switch (Random.Next(3))
                {
                    case 0: types.Add(TypeConstants.Melee); break;
                    case 1: types.Add(TypeConstants.Ranged); break;
                    case 2:
                        types.Add(TypeConstants.Melee);
                        types.Add(TypeConstants.Ranged);
                        break;
                }

                switch (Random.Next(4))
                {
                    case 0: types.Add(TypeConstants.Metal); break;
                    case 1: types.Add(TypeConstants.Wood); break;
                    case 2:
                        types.Add(TypeConstants.Metal);
                        types.Add(TypeConstants.Wood);
                        break;
                    case 3:
                        if (allowNoMaterial)
                            break;
                        types.Add(TypeConstants.Metal);
                        break;
                }
            }

            if (types.Contains(ItemTypeConstants.Armor))
            {
                switch (Random.Next(2))
                {
                    case 0: types.Add(TypeConstants.Shield); break;
                    case 1: break;
                }

                switch (Random.Next(3))
                {
                    case 0: types.Add(TypeConstants.Metal); break;
                    case 1: types.Add(TypeConstants.Wood); break;
                    case 2: break;
                }
            }

            return types;
        }
    }
}