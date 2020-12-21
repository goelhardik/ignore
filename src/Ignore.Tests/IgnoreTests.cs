namespace Ignore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class IgnoreTests
    {
        [Fact]
        public void Whitespace() => IgnoreTest(
            patterns: new[] { @"\r\n", "  " },
            inputs: new[] { "a" },
            expectedOutputs: new[] { "a" });

        [Fact]
        public void SimpleMatch() => IgnoreTest(
            patterns: new[] { @"a" },
            inputs: new[] { "a" },
            expectedOutputs: new string[] { });

        [Fact]
        public void EscapedTrailingWhitespaces_NotIgnored() => IgnoreTest(
            patterns: new[] { @"a\ ", @"a \ ", @"a \ b" },
            inputs: new[] { "a", "a ", "a  ", @"a  b", "ab" },
            expectedOutputs: new[] { "a", "ab" });

        private void IgnoreTest(
            string[] patterns,
            string[] inputs,
            string[] expectedOutputs)
        {
            // Arrange
            var ignore = new Ignore();
            ignore.Add(patterns);

            // Act
            var filteredPaths = ignore.Filter(inputs);

            // Assert
            Assert.Equal(expectedOutputs, filteredPaths);
        }
    }
}
