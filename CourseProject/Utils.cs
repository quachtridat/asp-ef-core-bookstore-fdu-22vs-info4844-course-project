namespace CourseProject {
    public static class Utils {
        /// <summary>
        /// Return a string containing the first <paramref name="n"/> words of <paramref name="str"/>.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="n">The number of words.</param>
        /// <param name="delimiter">The character that separates words.</param>
        /// <param name="endingEllipsis">Whether to append ending <paramref name="ellipsis"/> at the end when the resulting string is shorter than the initial string <paramref name="str"/>.</param>
        /// <param name="ellipsis">The ellipsis string.</param>
        /// <param name="ellipsisPrefix">The string that is placed before the <paramref name="ellipsis"/> in case the <paramref name="ellipsis"/> gets inserted.</param>
        /// <returns></returns>
        public static string? TakeWords(this string? str, int n, char delimiter = ' ', bool endingEllipsis = false, string ellipsis = "...", string ellipsisPrefix = "") {
            if (str == null) return null;

            if (n == 0) return string.Empty;
            List<char> chars = new List<char>();

            bool wasDelimiter = false;
            int nWords = 0;

            foreach (char c in str.ToCharArray()) {
                if (c == delimiter) {
                    if (!wasDelimiter) {
                        nWords += 1;
                        if (nWords >= n) {
                            break;
                        }
                    }
                    wasDelimiter = true;
                } else {
                    wasDelimiter = false;
                }
                chars.Add(c);
            }

            string result = string.Join("", chars);

            if (result.Length < str.Length && endingEllipsis) {
                return result + ellipsisPrefix + ellipsis;
            }

            return result;
        }
    }
}
