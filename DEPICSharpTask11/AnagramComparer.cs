using System.Diagnostics.CodeAnalysis;

namespace DEPICSharpTask11
{
    // Custom comparer for GroupBy (same idea as MyEqualityComparer in the session):
    // two words are "equal" when they consist of the same characters, e.g. "from" and "form".
    internal class AnagramComparer : IEqualityComparer<string>
    {
        // Sorts the characters of the word: "from" => "fmor", "form" => "fmor"
        private static string SortCharacters(string word)
        {
            char[] chars = word.ToCharArray();
            Array.Sort(chars);
            return new string(chars);
        }

        public bool Equals(string? x, string? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return SortCharacters(x) == SortCharacters(y);
        }

        // Equal words MUST return the same hash code, so the hash is built from the sorted characters
        public int GetHashCode([DisallowNull] string obj)
        {
            return SortCharacters(obj).GetHashCode();
        }
    }
}
