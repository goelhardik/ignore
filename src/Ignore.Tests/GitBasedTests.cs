namespace Ignore.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using PrQuantifier.Common.Tests;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class GitBasedTests : IClassFixture<GitRepoFixture>, IDisposable
    {
        private readonly GitRepoFixture gitFixture;

        public GitBasedTests(GitRepoFixture gitFixture)
        {
            this.gitFixture = gitFixture;
            gitFixture.CreateRepo();
        }

        [Fact]
        public void EmptyLines() => GitBasedTest(
            @"""
# empty lines, nothing is ignored


""",
            new[] { "foo", "bar" });

        [Fact]
        public void TrailingWhitespaces() => GitBasedTest(
            @"""
# trailing whitespaces handled
foo   
""",
            new[] { "foo", "bar" });

        [Fact]
        public void SimpleIgnore() => GitBasedTest(
            @"""
# exclude foo
foo
""",
            new[] { "foo", "bar/foo", "foob/bar", "src/foo/bar", "src/foo/", "fooc" });

        [Fact]
        public void SimpleIgnore_WithSubdirs() => GitBasedTest(
            @"""
# exclude foo/bar
foo/bar
""",
            new[] { "src/foo/bar", "foo/bar/", "foo/bar/char", "src/bar/char", "a/foo/bar/char" });

        [Fact]
        public void SimpleIgnore_Dir() => GitBasedTest(
            @"""
foo/
""",
            new[] { "foo/bar", "bar/foo", "foo/har", "tar/foo/bar", "tar/bar/foo" });

        [Fact]
        public void SimpleIgnore_Dotfiles() => GitBasedTest(
            @"""
.foo/
""",
            new[] { ".foo/bar", ".bar/foo", ".foo/har", "tar/.foo/bar", "tar/bar/.foo" });

        [Fact]
        public void SimpleIgnore_Dotfiles_WithStar() => GitBasedTest(
            @"""
.foo/*
""",
            new[] { ".foo/bar", ".foo/.foo/bar", ".foo/har" });

        [Fact]
        public void SimpleIgnore_Dotfiles_WithStar2() => GitBasedTest(
            @"""
*.mm.*
""",
            new[] { "file.mm", "commonFile.txt" });

        [Fact]
        public void StartsWithStar() => GitBasedTest(
            @"""
*.cs
""",
            new[] { "foo.cs", "foo/bar/foo.cs", "foo/bar/bar.csproj" });

        [Fact]
        public void StartsWithStar_Negated() => GitBasedTest(
            @"""
!*.cs
""",
            new[] { "foo.cs", "foo/bar/foo.cs", "foo/bar/bar.csproj" });

        [Fact]
        public void StartsWithStar_LeadingSlash() => GitBasedTest(
            @"""
/*.cs
""",
            new[] { "foo.cs", "foo/bar/foo.cs", "foo/bar/bar.csproj" });

        [Fact]
        public void SubdirStartsWithStar() => GitBasedTest(
            @"""
foo/*.cs
""",
            new[] { "foo.cs", "foo/bar/foo.cs", "foo/foo.cs", "foo/bar/bar.csproj" });

        [Fact]
        public void TrailingStar() => GitBasedTest(
            @"""
foo*
""",
            new[] { "fooc", "foo/bar/foo", "foo/foob.cs", "foo/bar/bar.csproj", "bar/foo" });

        [Fact]
        public void EscapedBang() => GitBasedTest(
            @"""
\!.foo/*
""",
            new[] { "!.foo/bar", ".foo/.foo/bar", ".foo/har" });

        [Fact]
        public void EscapedHash() => GitBasedTest(
            @"""
\#.foo/*
""",
            new[] { "!#foo/bar", ".foo/.foo/bar", "#.foo/har" });

        [Fact]
        public void SingleStar() => GitBasedTest(
            @"""
# * ignores everything
*
""",
            new[] { "foo", "bar" });

        [Fact]
        public void MiddleStar() => GitBasedTest(
            @"""
# intermediate *
fo*b
""",
            new[] { "foobar", "bar", "foob" });

        [Fact]
        public void LeadingSlash() => GitBasedTest(
            @"""
# leading slash
/fo*b
/bar
""",
            new[] { "foobar", "bar", "foob" });

        [Fact]
        public void EscapedSpaces() => GitBasedTest(
            @"""
# escaped spaces
/fo\ b
""",
            new[] { "foobar", "bar", "fo b", "spacebar" });

        [Fact]
        public void QuestionMark() => GitBasedTest(
            @"""
# ?
foo?
""",
            new[] { "foob", "foo" });

        [Fact]
        public void LeadingDoubleStar() => GitBasedTest(
            @"""
# leading **
**/foo
""",
            new[] { "src/foo", "foo/bar", "src/bar/foo" });

        [Fact]
        public void LeadingDoubleStar2() => GitBasedTest(
            @"""
# leading **
**foo.txt
""",
            new[] { "src/foo.txt", "foo/bar/foo.txt", "foo.txt", "foo.bar" });

        [Fact]
        public void MiddleDoubleStar() => GitBasedTest(
            @"""
# middle **
foo/**/
""",
            new[] { "src/foo/bar/char", "src/foo/tar", "src/foo/har/char/tar/har" });

        [Fact]
        public void MiddleDoubleStar_2() => GitBasedTest(
            @"""
# middle **
foo/**/bar
""",
            new[] { "foo/bar", "foo/tar/bar", "foo/har/tar/bar", "src/foo/tar/bar", "src/foo/har/char/tar/har/bar" });

        [Fact]
        public void MiddleDoubleStar_Complex() => GitBasedTest(
            @"""
# middle **
foo/**/**/bar
""",
            new[] { "foo/bar", "src/foo/tar/bar", "foo/har/char/tar/bar", "foo/tar/bar", "foobar" });

        [Fact]
        public void MiddleDoubleStar_Complex2() => GitBasedTest(
            @"""
# middle **
**/test/**/*.json
""",
            new[] { "foo/test/unit/bar/car.json", "foo/test/tar.json", "src/foo/tar/car.json" });

        [Fact]
        public void TrailingDoubleStar() => GitBasedTest(
            @"""
# trailing **
foo/**
""",
            new[] { "foo/bar", "src/foo/tar/bar", "foo/har/char/tar/bar", "foo/tar/bar", "foobar" });

        [Fact]
        public void TrailingDoubleStar_2() => GitBasedTest(
            @"""
# trailing **
src/foo/**
""",
            new[] { "src/foo/bar", "src/foo/tar/bar", "foo/har/char/tar/bar", "foo/tar/bar", "srcfoo", "src/bar/foo" });

        [Fact]
        public void MiddleSlash() => GitBasedTest(
            @"""
# trailing **
src/foo
""",
            new[] { "src/foo/bar", "foo/src/foo" });

        [Fact]
        public void TrailingSlash() => GitBasedTest(
            @"""
# trailing **
src/foo/
""",
            new[] { "src/foo" });

        [Fact]
        public void TrailingSlash_2() => GitBasedTest(
            @"""
# trailing **
src/foo/
""",
            new[] { "src/foo/" });

        [Fact]
        public void NoopNegate() => GitBasedTest(
            @"""
# negate
!foo
""",
            new[] { "foo", "bar", "src/foo/tar", "har/foo", "src/bar/foo", "har/bar/foo/tar" });

        [Fact]
        public void SimpleNegate() => GitBasedTest(
            @"""
# negate
foo
!foo
""",
            new[] { "foo", "bar" });

        [Fact]
        public void SimpleNegate_2() => GitBasedTest(
            @"""
# negate
foo
!foo
""",
            new[] { "foo", "bar", "src/foo", "src/bar/foo" });

        [Fact]
        public void ComplexNegate() => GitBasedTest(
            @"""
# negate
/*
!/foo
/foo/*
!/foo/bar
""",
            new[] { "foo/bar", "bar", "src/foo", "src/bar/foo/bar" });

        [Fact]
        public void Range() => GitBasedTest(
            @"""
# range regex
*.py[cod]
""",
            new[] { "foo.py", "bar.p", "foo.pyc", "foo.pyco", "foo.pyd" });

        public void Dispose()
        {
            gitFixture.DeleteRepoDirectory();
        }

        private void GitBasedTest(string gitignore, string[] files)
        {
            // setup gitignore
            gitFixture.AddTrackedFileToRepo(".gitignore", gitignore);

            // add untracked files to filesystem
            files.ToList().ForEach(file =>
            {
                if (file.EndsWith("/"))
                {
                    // create directory
                    gitFixture.AddUntrackedDirToRepo(file);
                }
                else
                {
                    gitFixture.AddUntrackedFileToRepo(file);
                }
            });

            // get results of git
            var gitUntrackedFiles = gitFixture.GetUntrackedFiles().Select(se => se.FilePath).ToList();

            // setup ignore
            var ignore = new Ignore();
            ignore.Add(gitignore.Split("\n"));

            // get results of ignore
            var ignoreFilteredFiles = ignore.Filter(files);

            Assert.Equal(gitUntrackedFiles.OrderBy(a => a), ignoreFilteredFiles.OrderBy(a => a));
        }
    }
}
