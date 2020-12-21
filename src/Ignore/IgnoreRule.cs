namespace Ignore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class IgnoreRule
    {
#pragma warning disable 414
        private readonly bool negate;
#pragma warning restore 414

        private readonly Regex parsedRegex = null;

        private readonly List<Replacer> replacers = new List<Replacer>
        {
            ReplacerStash.TrailingSpaces
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreRule"/> class.
        /// Parses the given pattern as per .gitignore spec.
        /// https://git-scm.com/docs/gitignore#_pattern_format
        /// </summary>
        /// <param name="pattern">Pattern to parse.</param>
        public IgnoreRule(string pattern)
        {
            // A blank line matches no files, so it can serve as a separator for readability.
            if (string.IsNullOrEmpty(pattern.Trim()))
            {
                return;
            }

            // A line starting with # serves as a comment. Put a backslash ("\") in front of the first hash for patterns that begin with a hash.
            if (pattern.StartsWith('#'))
            {
                return;
            }

            // account for escaped # and !
            // remove the leading backslash
            if (pattern.StartsWith("\\!") || pattern.StartsWith("\\#"))
            {
                pattern = pattern.Substring(1);
            }

            // is negate rule
            if (pattern.StartsWith('!'))
            {
                negate = true;
                pattern = pattern.Substring(1);
            }

            foreach (var replacer in replacers)
            {
                pattern = replacer.Invoke(pattern);
            }

            parsedRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool IsIgnored(string pattern)
        {
            return parsedRegex != null && parsedRegex.IsMatch(pattern);
        }
    }
}
