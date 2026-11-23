using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Generators;
using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.IoC;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DnDGen.TreasureGen.Tests.Integration
{
    [TestFixture]
    public partial class ReadmeTests : IntegrationTests
    {
        private const string ThemeSongBadge = "[![Theme Song](https://img.shields.io/badge/theme_song-Suno-ff69b4)]";
        private const string RecordUrl = "https://github.com/DnDGen/DnDGen.TreasureGen/blob/main/docs/theme-song/README.md";
        private const BindingFlags DeclaredPublicMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private static readonly Type[] PublicTypes = typeof(Treasure).Assembly.GetExportedTypes();
        private static readonly Dictionary<Type, string> TypeKeywords = new()
        {
            [typeof(bool)] = "bool",
            [typeof(char)] = "char",
            [typeof(double)] = "double",
            [typeof(int)] = "int",
            [typeof(object)] = "object",
            [typeof(string)] = "string",
        };

        private string[] _readme;

        [OneTimeSetUp]
        public void ReadReadme()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "README.md");
            _readme = File.ReadAllLines(path);
        }

        [Test]
        public void TreasureAtLevel()
        {
            var treasureGenerator = GetNewInstanceOf<ITreasureGenerator>();
            var treasure = treasureGenerator.GenerateAtLevel(6);

            Assert.That(treasure.Coin, Is.Not.Null);
            Assert.That(treasure.Goods, Is.Not.Null);
            Assert.That(treasure.Items, Is.Not.Null);
        }

        [Test]
        public void TreasureAtLevel_Async()
        {
            var treasureGenerator = GetNewInstanceOf<ITreasureGenerator>();
            var treasure = treasureGenerator.GenerateAtLevelAsync(6).Result;

            Assert.That(treasure.Coin, Is.Not.Null);
            Assert.That(treasure.Goods, Is.Not.Null);
            Assert.That(treasure.Items, Is.Not.Null);
        }

        [Test]
        public void CoinGoodsAndItemsAtLevel()
        {
            var coin = GetNewInstanceOf<ICoinGenerator>().GenerateAtLevel(6);
            var goods = GetNewInstanceOf<IGoodsGenerator>().GenerateAtLevel(6);
            var items = GetNewInstanceOf<IItemsGenerator>().GenerateRandomAtLevel(6);

            Assert.That(coin, Is.Not.Null);
            Assert.That(goods, Is.Not.Null);
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public void MundaneArmor()
        {
            var mundaneArmorGenerator = GetNewInstanceOf<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var mundaneArmor = mundaneArmorGenerator.GenerateRandom();

            Assert.That(mundaneArmor, Is.InstanceOf<Armor>());
            Assert.That(mundaneArmor.IsMagical, Is.False);
        }

        [Test]
        public void MagicalWeapon()
        {
            var magicalWeaponGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Weapon);
            var magicalWeapon = magicalWeaponGenerator.GenerateRandom(PowerConstants.Major);

            Assert.That(magicalWeapon, Is.InstanceOf<Weapon>());
            Assert.That(magicalWeapon.IsMagical, Is.True);
        }

        [Test]
        public void SpecificMasterworkWeapon()
        {
            var mundaneWeaponGenerator = GetNewInstanceOf<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var sword = mundaneWeaponGenerator.Generate(WeaponConstants.Longsword, TraitConstants.Masterwork);

            Assert.That(sword.Name, Is.EqualTo(WeaponConstants.Longsword));
            Assert.That(sword.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GettingTheGenerators()
        {
            var kernel = new StandardKernel();
            TreasureGenModuleLoader.LoadModules(kernel);

            Assert.That(kernel.Get<ITreasureGenerator>(), Is.Not.Null);
            Assert.That(kernel.Get<MundaneItemGenerator>(ItemTypeConstants.Armor), Is.Not.Null);
            Assert.That(kernel.Get<MagicalItemGenerator>(ItemTypeConstants.Weapon), Is.Not.Null);
        }

        [Test]
        public void ThemeSong_HasBadgeBesideTheBuildBadge()
        {
            var buildBadgeLine = _readme.FirstOrDefault(line => line.StartsWith("[![Build Status]")) ?? string.Empty;
            Assert.That(buildBadgeLine, Does.Contain($" {ThemeSongBadge}(https://suno.com/s/"));
        }

        [Test]
        public void ThemeSong_HasSectionAtTheEnd()
        {
            var headings = GetLinesOutsideCodeBlocks(_readme).Where(line => line.StartsWith("## "));
            Assert.That(headings.LastOrDefault(), Is.EqualTo("## Theme Song"));
        }

        [Test]
        public void ThemeSong_LinksToTheRecord()
        {
            var links = GetLinks(_readme);
            Assert.That(links, Does.Contain(RecordUrl));

            var recordPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "docs", "theme-song", "README.md");
            Assert.That(recordPath, Does.Exist);
        }

        [Test]
        public void ThemeSong_LinksToTheSameSongAsTheRecord()
        {
            var recordPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "docs", "theme-song", "README.md");
            var record = File.ReadAllLines(recordPath);
            string[] recordSongs = [.. GetLinks(record).Where(IsSunoLink).Distinct()];
            string[] readmeSongs = [.. GetLinks(_readme).Where(IsSunoLink).Distinct()];

            Assert.That(recordSongs, Has.Exactly(1).Items, "The record needs to link to one song");
            Assert.That(readmeSongs, Is.EqualTo(recordSongs));
        }

        [Test]
        public void DocumentLinks_AreAbsolute()
        {
            var relativeLinks = GetLinks(_readme).Where(link => !link.StartsWith("http://") && !link.StartsWith("https://"));
            Assert.That(relativeLinks, Is.Empty, "Relative links don't work on NuGet");
        }

        [Test]
        public void Constructors_AreAllDocumented()
        {
            var constructors = PublicTypes
                .Where(type => !IsConstantsClass(type))
                .SelectMany(type => type.GetConstructors().Select(constructor => $"{WriteTypeName(type)}.{WriteMember(constructor)}"));

            Assert.That(GetDocumented("Constructor"), Is.EquivalentTo(constructors));
        }

        [Test]
        public void Methods_AreAllDocumented()
        {
            var methods = PublicTypes
                .SelectMany(type => type.GetMethods(DeclaredPublicMembers)
                    .Where(method => !method.IsSpecialName && !OverridesObject(method))
                    .Select(method => $"{WriteTypeName(type)}.{WriteMember(method)}"));

            Assert.That(GetDocumented("Method"), Is.EquivalentTo(methods));
        }

        [Test]
        public void Properties_AreAllDocumented()
        {
            var members = PublicTypes.SelectMany(type =>
            {
                var fields = IsConstantsClass(type) ? [] : type.GetFields(DeclaredPublicMembers).Select(field => field.Name);
                var properties = type.GetProperties(DeclaredPublicMembers).Select(property => property.Name);

                return fields.Concat(properties).Select(member => $"{WriteTypeName(type)}.{member}");
            });

            Assert.That(GetDocumented("Property"), Is.EquivalentTo(members));
        }

        [Test]
        public void ConstantsClasses_AreAllDocumented()
        {
            var constantsClasses = PublicTypes.Where(IsConstantsClass).Select(WriteTypeName);
            Assert.That(GetTableRows("Constants").SelectMany(GetCodeSpans), Is.EquivalentTo(constantsClasses));
        }

        private static bool IsConstantsClass(Type type) => type.IsAbstract
            && type.IsSealed
            && (type.Name.EndsWith("Constants") || (type.DeclaringType != null && IsConstantsClass(type.DeclaringType)));

        private static bool OverridesObject(MethodInfo method) => method.GetBaseDefinition().DeclaringType == typeof(object);

        private static string WriteTypeName(Type type) => type.DeclaringType == null ? type.Name : $"{WriteTypeName(type.DeclaringType)}.{type.Name}";

        private string[] GetDocumented(string kind) => [.. GetTableRows(kind)
            .SelectMany(row => CodeSpanPartialRegex().Matches(row.Cells[0]).Select(match => $"{row.Heading}.{match.Groups[1].Value}"))];

        private static string[] GetCodeSpans(TableRow row) => [.. CodeSpanPartialRegex().Matches(row.Cells[0]).Select(match => match.Groups[1].Value)];

        private IEnumerable<TableRow> GetTableRows(string kind)
        {
            var heading = string.Empty;
            string[] header = [];

            foreach (var line in GetLinesOutsideCodeBlocks(_readme))
            {
                if (line.StartsWith('#'))
                    heading = line.TrimStart('#').Trim().Trim('`');

                if (!line.StartsWith('|'))
                {
                    header = [];
                    continue;
                }

                string[] cells = [.. line.Trim().Trim('|').Split('|').Select(cell => cell.Trim())];

                if (header.Length == 0)
                    header = cells;
                else if (header[0] == kind && !cells[0].StartsWith("---"))
                    yield return new TableRow(heading, cells);
            }
        }

        private static string WriteMember(MethodBase member)
        {
            var name = member is ConstructorInfo ? member.DeclaringType.Name : member.Name;

            if (member.IsGenericMethod)
                name += $"<{string.Join(", ", member.GetGenericArguments().Select(WriteType))}>";

            var parameters = member.GetParameters().Select(WriteParameter);

            return $"{name}({string.Join(", ", parameters)})";
        }

        private static string WriteParameter(ParameterInfo parameter)
        {
            var written = $"{WriteType(parameter.ParameterType)} {parameter.Name}";

            if (parameter.IsDefined(typeof(ParamArrayAttribute)))
                written = $"params {written}";

            if (parameter.HasDefaultValue)
                written += $" = {WriteValue(parameter.DefaultValue)}";

            return written;
        }

        private static string WriteType(Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
                return $"{WriteType(underlyingType)}?";

            if (type.IsArray)
                return $"{WriteType(type.GetElementType())}[]";

            if (type.IsGenericType)
                return $"{type.Name[..type.Name.IndexOf('`')]}<{string.Join(", ", type.GetGenericArguments().Select(WriteType))}>";

            return TypeKeywords.GetValueOrDefault(type, type.Name);
        }

        private static string WriteValue(object value) => value switch
        {
            null => "null",
            bool boolean => boolean ? "true" : "false",
            char character => $"'{character.ToString().Replace("\\", "\\\\")}'",
            string text => $"\"{text}\"",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture),
        };

        private static bool IsSunoLink(string link) => link.StartsWith("https://suno.com/");

        private static string[] GetLinks(IEnumerable<string> lines)
        {
            var links = GetLinesOutsideCodeBlocks(lines)
                .SelectMany(line => MarkdownLinkPartialRegex().Matches(line).Concat(HtmlLinkPartialRegex().Matches(line)))
                .Select(match => match.Groups[1].Value);

            return [.. links];
        }

        private static IEnumerable<string> GetLinesOutsideCodeBlocks(IEnumerable<string> lines)
        {
            var inCodeBlock = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("```"))
                    inCodeBlock = !inCodeBlock;
                else if (!inCodeBlock)
                    yield return line;
            }
        }

        private record TableRow(string Heading, string[] Cells);

        [GeneratedRegex("`([^`]+)`")]
        private static partial Regex CodeSpanPartialRegex();

        [GeneratedRegex(@"\]\(([^)\s]+)\)")]
        private static partial Regex MarkdownLinkPartialRegex();

        [GeneratedRegex(@"(?:href|src)=""([^""]+)""")]
        private static partial Regex HtmlLinkPartialRegex();
    }
}
