namespace Merchello.Tests.UnitTests.Collections
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using NUnit.Framework;

    [TestFixture]
    public class CollectionCombinationTests
    {
        private readonly IEnumerable<string> collection = new[] { "F1", "F2", "F3" };

        [Test]
        public void AllCombinationsOfExtension()
        {
            //// Arrange
            var totalCount = 7;
            var maxLevels = 3;
            var level1GroupCount = 3;
            var level2GroupCount = 3;
            var level3GroupCount = 1;


            //// Act
            var combos = collection.AllCombinationsOf<string>();

            //// [
            ////    [1, [V1]], [1, [V2]], [1, [V3]],
            ////    [2, [V1, V2]], [2, [V1, V3]], [2, [V2, V3]]
            ////    [3, [V1, V2, V3]]
            //// ]

            //// Assert
            Assert.AreEqual(totalCount, combos.Count());
            Assert.AreEqual(maxLevels, combos.Max(x => x.Item1));
            Assert.AreEqual(level1GroupCount, combos.Count(x => x.Item1 == 1));
            Assert.AreEqual(level2GroupCount, combos.Count(x => x.Item1 == 2));
            Assert.AreEqual(level3GroupCount, combos.Count(x => x.Item1 == 3));

            Assert.AreEqual(3, combos.First(x => x.Item1 == 3).Item2.Count());

            var combo3 = combos.First(x => x.Item1 == 3).Item2;
            Assert.AreEqual(combo3.Count(), collection.Count());
            Assert.IsTrue(combo3.All(x => collection.Contains(x)));

        }
    }
}