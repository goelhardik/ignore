namespace Ignore.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class IgnoreTests
    {
        [Theory]
        [InlineData("/foo")]
        [InlineData(@"C:/foo")]
        [InlineData("C:foo")]
        [InlineData("C:\\foo")]
        public void RootedPath_ThrowsArgumentException(string path)
        {
            var ignore = new Ignore();

            Assert.Throws<ArgumentException>(() => ignore.IsIgnored(path));
        }
    }
}