using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class PersonalityTraitsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.PersonalityTraits; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase("Distinctive groove", 1)]
        [TestCase("Chipped", 2)]
        [TestCase("Small part broken off", 3)]
        [TestCase("Offensive (in communication or appearance)", 4)]
        [TestCase("Strong, unpleasant, unwavering odor", 5)]
        [TestCase("Pleasant smelling (perfumed)", 6)]
        [TestCase("Slippery", 7)]
        [TestCase("Vibrates gently when wielded", 8)]
        [TestCase("Unusually-colored markings", 9)]
        [TestCase("Mumbles", 10)]
        [TestCase("Drones on", 11)]
        [TestCase("Particularly low voice", 12)]
        [TestCase("Particularly high voice", 13)]
        [TestCase("Slurs words", 14)]
        [TestCase("Lisps", 15)]
        [TestCase("Stutters", 16)]
        [TestCase("Enunciates very clearly", 17)]
        [TestCase("Speaks loudly", 18)]
        [TestCase("Whispers", 19)]
        [TestCase("Hard of hearing", 20)]
        [TestCase("Marked by previous owner", 21)]
        [TestCase("Marked by creator", 22)]
        [TestCase("Unusual color", 23)]
        [TestCase("Shiny", 24)]
        [TestCase("Dull", 25)]
        [TestCase("", 26)]
        [TestCase("Tarnish or wear", 27)]
        [TestCase("Complains", 28)]
        [TestCase("Distinctive jewels", 29)]
        [TestCase("Flamboyant or outlandish decoration", 30)]
        [TestCase("Bare", 31)]
        [TestCase("Ornate", 32)]
        [TestCase("Reflective", 33)]
        [TestCase("Nervous", 34)]
        [TestCase("Whistles a lot", 35)]
        [TestCase("Sings a lot", 36)]
        [TestCase("Indifferent", 37)]
        [TestCase("Prideful", 38)]
        [TestCase("Curved, bent, or warped", 39)]
        [TestCase("Long", 40)]
        [TestCase("Short", 41)]
        [TestCase("Thin", 42)]
        [TestCase("Thick", 43)]
        [TestCase("Visible wear and tear", 44)]
        [TestCase("Short attention span", 45)]
        [TestCase("Gets lost in thought", 46)]
        [TestCase("Frequently ponders out loud", 47)]
        [TestCase("Dirty and ill-kept", 48)]
        [TestCase("Clean", 49)]
        [TestCase("Distinctive feel", 50)]
        [TestCase("Selfish", 51)]
        [TestCase("Obsequious", 52)]
        [TestCase("Drowsy", 53)]
        [TestCase("Bookish", 54)]
        [TestCase("Observant", 55)]
        [TestCase("Not very observant", 56)]
        [TestCase("Overly critical", 57)]
        [TestCase("Passionate art lover", 58)]
        [TestCase("Passionate hobbyist (riddles, reading, hunting, etc.)", 59)]
        [TestCase("Storyteller", 60)]
        [TestCase("Stingy", 61)]
        [TestCase("Squanderer", 62)]
        [TestCase("Pessimist", 63)]
        [TestCase("Optimist", 64)]
        [TestCase("Gets drunk off of victory", 65)]
        [TestCase("Offended by drunkenness", 66)]
        [TestCase("Well-mannered", 67)]
        [TestCase("Rude", 68)]
        [TestCase("Easily startled", 69)]
        [TestCase("Excessively concerned with appearance", 70)]
        [TestCase("Overbearing", 71)]
        [TestCase("Aloof", 72)]
        [TestCase("Proud", 73)]
        [TestCase("Individualist", 74)]
        [TestCase("Conformist", 75)]
        [TestCase("Hot tempered", 76)]
        [TestCase("Even tempered", 77)]
        [TestCase("Neurotic", 78)]
        [TestCase("Jealous", 79)]
        [TestCase("Brave", 80)]
        [TestCase("Cowardly", 81)]
        [TestCase("Careless", 82)]
        [TestCase("Curious", 83)]
        [TestCase("Truthful", 84)]
        [TestCase("Liar", 85)]
        [TestCase("Lazy", 86)]
        [TestCase("Energetic", 87)]
        [TestCase("Reverent or pious", 88)]
        [TestCase("Irreverent or irreligious", 89)]
        [TestCase("Strong opinions on politics or morals", 90)]
        [TestCase("Moody", 91)]
        [TestCase("Cruel", 92)]
        [TestCase("Uses flowery speech or long words", 93)]
        [TestCase("Uses the same phrase over and over", 94)]
        [TestCase("Sexist, racist, or otherwise prejudiced", 95)]
        [TestCase("Fascinated by magic", 96)]
        [TestCase("Distrustful of magic", 97)]
        [TestCase("Prefers members of one class over all others", 98)]
        [TestCase("Jokester", 99)]
        [TestCase("No sense of humor", 100)]
        public void PersonalityTraitsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}