namespace Ignore
{
    using System.Collections.Generic;
    using System.Linq;

    public class Ignore
    {
        private readonly List<IgnoreRule> rules;

        public Ignore()
        {
            rules = new List<IgnoreRule>();
            OriginalPatterns = new List<string>();
        }

        public List<string> OriginalPatterns { get; }

        /// <summary>
        /// Adds the given pattern to this <see cref="Ignore"/> instance.
        /// </summary>
        /// <param name="pattern">Gitignore style pattern string.</param>
        /// <returns>Current instance of <see cref="Ignore"/>.</returns>
        public Ignore Add(string pattern)
        {
            OriginalPatterns.Add(pattern);
            rules.Add(new IgnoreRule(pattern));
            return this;
        }

        /// <summary>
        /// Adds the given pattern list to this <see cref="Ignore"/> instance.
        /// </summary>
        /// <param name="patterns">List of gitignore style pattern strings.</param>
        /// <returns>Current instance of <see cref="Ignore"/>.</returns>
        public Ignore Add(IEnumerable<string> patterns)
        {
            var patternList = patterns.ToList();
            OriginalPatterns.AddRange(patternList);
            patternList.ForEach(pattern => rules.Add(new IgnoreRule(pattern)));
            return this;
        }

        public IEnumerable<string> Filter(IEnumerable<string> paths)
        {
            var filteredPaths = new List<string>();
            foreach (var path in paths)
            {
                var ignore = false;
                foreach (var rule in rules)
                {
                    if (rule.IsIgnored(path))
                    {
                        ignore = true;
                    }
                }

                if (ignore == false)
                {
                    filteredPaths.Add(path);
                }
            }

            return filteredPaths;
        }
    }
}
