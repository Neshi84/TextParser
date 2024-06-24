using WebApplication1.Controllers;
using WebApplication1.Models;
using NUnit.Framework.Legacy;

namespace TestProject1
{
    [TestFixture]
    public class ParseControllerTests
    {
        private ParseController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new ParseController();
        }

        [Test]
        public void ParseInput_WithSimpleText_ShouldReturnCorrectUsers()
        {
            string input = "John Doe, Jane Smith, Alice Johnson";
            List<User> expected =
            [
                new User { FirstName = "John", LastName = "Doe" },
                new User { FirstName = "Jane", LastName = "Smith" },
                new User { FirstName = "Alice", LastName = "Johnson" }
            ];

            List<User> actual = ParseController.ParseInput(input);

            ClassicAssert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                ClassicAssert.AreEqual(expected[i].FirstName, actual[i].FirstName);
                ClassicAssert.AreEqual(expected[i].LastName, actual[i].LastName);
            }
        }

        [Test]
        public void ParseInput_WithHTMLTags_ShouldIgnoreSpecifiedTags()
        {
            string input = @"<title>This title should be ignored</title>
                            <p>John Doe, Jane Smith, Alice Johnson</p>
                            <ul>
                                <li>Lisa Yellow</li>
                                <li>Michael Grey</li>
                            </ul>";

            List<User> expected =
            [
                new User { FirstName = "John", LastName = "Doe" },
                new User { FirstName = "Jane", LastName = "Smith" },
                new User { FirstName = "Alice", LastName = "Johnson" },
                new User { FirstName = "Lisa", LastName = "Yellow" },
                new User { FirstName = "Michael", LastName = "Grey" }
            ];

            List<User> actual = ParseController.ParseInput(input);

            ClassicAssert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                ClassicAssert.AreEqual(expected[i].FirstName, actual[i].FirstName);
                ClassicAssert.AreEqual(expected[i].LastName, actual[i].LastName);
            }
        }

        [Test]
        public void ParseInput_WithDifferentDelimiters_ShouldReturnCorrectUsers()
        {
            string input = "John Doe, Jane Smith; Alice Johnson\nBob Brown\r\nCharlie Davis";
            List<User> expected =
            [
                new User { FirstName = "John", LastName = "Doe" },
                new User { FirstName = "Jane", LastName = "Smith" },
                new User { FirstName = "Alice", LastName = "Johnson" },
                new User { FirstName = "Bob", LastName = "Brown" },
                new User { FirstName = "Charlie", LastName = "Davis" }
            ];

            List<User> actual = ParseController.ParseInput(input);

            ClassicAssert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                ClassicAssert.AreEqual(expected[i].FirstName, actual[i].FirstName);
                ClassicAssert.AreEqual(expected[i].LastName, actual[i].LastName);
            }
        }

        [Test]
        public void ParseInput_WithExtraSpaces_ShouldTrimSpaces()
        {
            string input = "  John   Doe ,  Jane    Smith  , Alice  Johnson ";
            List<User> expected =
            [
                new User { FirstName = "John", LastName = "Doe" },
                new User { FirstName = "Jane", LastName = "Smith" },
                new User { FirstName = "Alice", LastName = "Johnson" }
            ];

            List<User> actual = ParseController.ParseInput(input);

            ClassicAssert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                ClassicAssert.AreEqual(expected[i].FirstName, actual[i].FirstName);
                ClassicAssert.AreEqual(expected[i].LastName, actual[i].LastName);
            }
        }
    }
}