using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class ItemAlignmentRequirementsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.ItemAlignmentRequirements; }
        }

        [TestCase(ArmorConstants.CelestialArmor, AlignmentConstants.Good)]
        [TestCase(ArmorConstants.DemonArmor, AlignmentConstants.Evil)]
        [TestCase(WeaponConstants.AssassinsDagger, AlignmentConstants.Evil)]
        [TestCase(WeaponConstants.NineLivesStealer, AlignmentConstants.Evil)]
        [TestCase(WeaponConstants.SunBlade, AlignmentConstants.Good)]
        [TestCase(RodConstants.Python, AlignmentConstants.Good)]
        [TestCase(RodConstants.Viper, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.ChaosDiamond, AlignmentConstants.Chaotic)]
        [TestCase(WondrousItemConstants.Darkskull, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, AlignmentConstants.Evil)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction, AlignmentConstants.Evil)]
        [TestCase(ArmorConstants.ArmorOfRage, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.BootsOfDancing, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.RobeOfVermin, AlignmentConstants.Evil)]
        [TestCase(RingConstants.Clumsiness, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, AlignmentConstants.Evil)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, AlignmentConstants.Evil)]
        [TestCase(WondrousItemConstants.VacousGrimoire, AlignmentConstants.Evil)]
        [TestCase(WeaponConstants.CursedMinus2Sword, AlignmentConstants.Evil)]
        public void Collection(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [Test]
        public void HolyAvengerAlignmentRequirement()
        {
            var attributes = new[] { AlignmentConstants.LawfulGood };
            base.AssertCollection(WeaponConstants.HolyAvenger, attributes);
        }

        [Test]
        public void MaceOfBloodAlignmentRequirement()
        {
            var attributes = new[] { AlignmentConstants.ChaoticEvil };
            base.AssertCollection(WeaponConstants.MaceOfBlood, attributes);
        }

        [Test]
        public void ItemsWithAlignmentRequirements()
        {
            var items = new[]
            {
                ArmorConstants.CelestialArmor,
                ArmorConstants.DemonArmor,
                WeaponConstants.AssassinsDagger,
                WeaponConstants.HolyAvenger,
                WeaponConstants.NineLivesStealer,
                WeaponConstants.SunBlade,
                RodConstants.Python,
                RodConstants.Viper,
                WondrousItemConstants.ChaosDiamond,
                WondrousItemConstants.Darkskull,
                WondrousItemConstants.AmuletOfInescapableLocation,
                ArmorConstants.ArmorOfArrowAttraction,
                ArmorConstants.ArmorOfRage,
                WondrousItemConstants.BootsOfDancing,
                WondrousItemConstants.BracersOfDefenselessness,
                WondrousItemConstants.BroomOfAnimatedAttack,
                WondrousItemConstants.CloakOfPoisonousness,
                WondrousItemConstants.CrystalBall_Hypnosis,
                WondrousItemConstants.GauntletsOfFumbling,
                WeaponConstants.MaceOfBlood,
                WondrousItemConstants.MedallionOfThoughtProjection,
                WondrousItemConstants.NecklaceOfStrangulation,
                WondrousItemConstants.PeriaptOfFoulRotting,
                WondrousItemConstants.RobeOfPowerlessness,
                WondrousItemConstants.RobeOfVermin,
                RingConstants.Clumsiness,
                WondrousItemConstants.ScarabOfDeath,
                WeaponConstants.CursedBackbiterSpear,
                WondrousItemConstants.StoneOfWeight_Loadstone,
                WondrousItemConstants.VacousGrimoire,
                WeaponConstants.CursedMinus2Sword
            };

            base.AssertCollection("Items", items);
        }
    }
}