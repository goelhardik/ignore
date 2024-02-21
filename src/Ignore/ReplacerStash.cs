namespace Ignore
{
    using System;
    using System.Text.RegularExpressions;

    public static partial class ReplacerStash
    {
        public static readonly Replacer TrailingSpaces = new(
            name: nameof(TrailingSpaces),
            regex: ReplaceExpressions.TrailingSpaces(),
#if NET8_0_OR_GREATER
            replacer: match => match.Value.StartsWith('\\') ? " " : string.Empty);
#else
            replacer: match => match.Value.StartsWith("\\", StringComparison.Ordinal) ? " " : string.Empty);
#endif

        public static readonly Replacer EscapedSpaces = new(
            name: nameof(EscapedSpaces),
            regex: ReplaceExpressions.EscapedSpaces(),
            replacer: match => " ");

        public static readonly Replacer LiteralPlus = new(
            name: nameof(LiteralPlus),
            regex: ReplaceExpressions.LiteralPlus(),
            replacer: match => @"\+");

        // a ? matches any character other than a /
        public static readonly Replacer QuestionMark = new(
            name: nameof(QuestionMark),
            regex: ReplaceExpressions.QuestionMark(),
            replacer: match => "[^/]");

        // a leading / matches the beginning of the path
        // eg. /fake.c only matches fake.c, not src/fake.c
        public static readonly Replacer LeadingSlash = new(
            name: nameof(LeadingSlash),
            regex: ReplaceExpressions.LeadingSlash(),
            replacer: match => "^");

        public static readonly Replacer MetacharacterSlashAfterLeadingSlash = new(
            name: nameof(MetacharacterSlashAfterLeadingSlash),
            regex: ReplaceExpressions.MetacharacterSlashAfterLeadingSlash(),
            replacer: match => "\\/");

        /// <summary>
        /// From gitignore:
        /// A leading "**" followed by a slash means match in all directories.
        /// For example, "**/foo" matches file or directory "foo" anywhere, the same as pattern "foo".
        /// "**/foo/bar" matches file or directory "bar" anywhere that is directly under directory "foo".
        /// </summary>
        public static readonly Replacer LeadingDoubleStar = new(
            name: nameof(LeadingDoubleStar),
            regex: ReplaceExpressions.LeadingDoubleStar(),
            replacer: match => @".*");

        /// <summary>
        /// From gitignore:
        /// A slash followed by two consecutive asterisks then a slash matches zero or more directories.
        /// For example, "a/**/b" matches "a/b", "a/x/b", "a/x/y/b" and so on.
        /// </summary>
        public static readonly Replacer MiddleDoubleStar = new(
            name: nameof(MiddleDoubleStar),
            regex: ReplaceExpressions.MiddleDoubleStar(),
            replacer: match => @"(.*/)?");

        /// <summary>
        /// From gitignore:
        /// A trailing "/**" matches everything inside. For example,
        /// "abc/**" matches all files inside directory "abc",
        /// relative to the location of the .gitignore file, with infinite depth.
        /// </summary>
        public static readonly Replacer TrailingDoubleStar = new(
            name: nameof(TrailingDoubleStar),
            regex: ReplaceExpressions.TrailingDoubleStar(),
            replacer: match => @".*$");

        /// <summary>
        /// Undocumented cases like foo/**.ps
        /// Treat ** as match any character other than /.
        /// </summary>
        public static readonly Replacer OtherDoubleStar = new(
            name: nameof(TrailingDoubleStar),
            regex: ReplaceExpressions.OtherDoubleStar(),
            replacer: match => @"[^/]*");

        /// <summary>
        /// If there is a separator at the beginning or middle (or both) of the pattern,
        /// then the pattern is relative to the directory level of the particular .gitignore file itself.
        /// Otherwise the pattern may also match at any level below the .gitignore level.
        /// The leading slash should be gone after using <see cref="LeadingSlash"/>.
        /// So put a ^ in the beginning of the match.
        /// </summary>
        public static readonly Replacer MiddleSlash = new(
            name: nameof(MiddleSlash),
            regex: ReplaceExpressions.MiddleSlash(),
            replacer: match => $"^{match.Groups[1]}");

        /// <summary>
        /// From gitignore:
        /// If there is a separator at the end of the pattern then the pattern will only match directories,
        /// otherwise the pattern can match both files and directories.
        ///
        /// This regex handles the paths with trailing slash present.
        /// </summary>
        public static readonly Replacer TrailingSlash = new(
            name: nameof(TrailingSlash),
            regex: ReplaceExpressions.TrailingSlash(),
            replacer: match => $@"(/|^){match.Groups[1]}/");

        /// <summary>
        /// From gitignore:
        /// If there is a separator at the end of the pattern then the pattern will only match directories,
        /// otherwise the pattern can match both files and directories.
        ///
        /// This pattern handles the paths with no trailing slash.
        /// </summary>
        public static readonly Replacer NoTrailingSlash = new(
            name: nameof(NoTrailingSlash),
            regex: ReplaceExpressions.NoTrailingSlash(),
            replacer: match => $@"{match.Groups[1]}(/.*)?$");

        /// <summary>
        /// From gitignore:
        /// An asterisk "*" matches anything except a slash.
        ///
        /// Replaces single * with anything other than a /.
        /// Unless the star is in the beginning of the pattern.
        /// </summary>
        public static readonly Replacer NonLeadingSingleStar = new(
            name: nameof(NonLeadingSingleStar),
            regex: ReplaceExpressions.NonLeadingSingleStar(),
            replacer: match => @"[^/]*");

        public static readonly Replacer LeadingSingleStar = new(
            name: nameof(LeadingSingleStar),
            regex: ReplaceExpressions.LeadingSingleStar(),
            replacer: match => @".*");

        public static readonly Replacer Ending = new(
            name: nameof(Ending),
            regex: ReplaceExpressions.Ending(),
            replacer: match => $"{match.Groups[1]}$");

        public static readonly Replacer LiteralDot = new(
            name: nameof(LiteralDot),
            regex: ReplaceExpressions.LiteralDot(),
            replacer: match => @"\.");

        public static readonly Replacer NoSlash = new(
            name: nameof(NoSlash),
            regex: ReplaceExpressions.NoSlash(),
            replacer: match => $"(^|/){match.Groups[1]}");

        private static partial class ReplaceExpressions
        {
#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\\?\s+$")]
            public static partial Regex TrailingSpaces();
#else
            public static Regex TrailingSpaces() => new Regex(@"\\?\s+$", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\\\s")]
            public static partial Regex EscapedSpaces();
#else
            public static Regex EscapedSpaces() => new Regex(@"\\\s", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\+")]
            public static partial Regex LiteralPlus();
#else
            public static Regex LiteralPlus() => new Regex(@"\+", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"(?!\\)\?")]
            public static partial Regex QuestionMark();
#else
            public static Regex QuestionMark() => new Regex(@"(?!\\)\?", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"^\/")]
            public static partial Regex LeadingSlash();
#else
            public static Regex LeadingSlash() => new Regex(@"^\/", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\/")]
            public static partial Regex MetacharacterSlashAfterLeadingSlash();
#else
            public static Regex MetacharacterSlashAfterLeadingSlash() => new Regex(@"\/", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"^(\*\*/|\*\*)")]
            public static partial Regex LeadingDoubleStar();
#else
            public static Regex LeadingDoubleStar() => new Regex(@"^(\*\*/|\*\*)", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"(?<=/)\*\*/")]
            public static partial Regex MiddleDoubleStar();
#else
            public static Regex MiddleDoubleStar() => new Regex(@"(?<=/)\*\*/", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\*\*$")]
            public static partial Regex TrailingDoubleStar();
#else
            public static Regex TrailingDoubleStar() => new Regex(@"\*\*$", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\*\*")]
            public static partial Regex OtherDoubleStar();
#else
            public static Regex OtherDoubleStar() => new Regex(@"\*\*", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"^([^/\^]+/[^/]+)")]
            public static partial Regex MiddleSlash();
#else
            public static Regex MiddleSlash() => new Regex(@"^([^/\^]+/[^/]+)");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"^([^/]+)/$")]
            public static partial Regex TrailingSlash();
#else
            public static Regex TrailingSlash() => new Regex(@"^([^/]+)/$");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"([^/$]+)$")]
            public static partial Regex NoTrailingSlash();
#else
            public static Regex NoTrailingSlash() => new Regex(@"([^/$]+)$");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"(?<!^)(?<!\*)\*(?!\*)")]
            public static partial Regex NonLeadingSingleStar();
#else
            public static Regex NonLeadingSingleStar() => new Regex(@"(?<!^)(?<!\*)\*(?!\*)");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"^(?<!\*)\*(?!\*)")]
            public static partial Regex LeadingSingleStar();
#else
            public static Regex LeadingSingleStar() => new Regex(@"^(?<!\*)\*(?!\*)");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"([^/$]+)$")]
            public static partial Regex Ending();
#else
            public static Regex Ending() => new Regex(@"([^/$]+)$");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"\.")]
            public static partial Regex LiteralDot();
#else
            public static Regex LiteralDot() => new Regex(@"\.");
#endif

#if NET8_0_OR_GREATER
            [GeneratedRegex(@"(^[^/]*$)")]
            public static partial Regex NoSlash();
#else
            public static Regex NoSlash() => new Regex(@"(^[^/]*$)");
#endif
        }
    }
}
