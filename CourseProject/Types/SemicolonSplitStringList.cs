namespace CourseProject.Types {
    public class SemicolonSplitStringList : List<string> {
        public SemicolonSplitStringList() : base() { }

        public SemicolonSplitStringList(int capacity) : base(capacity) { }

        public SemicolonSplitStringList(IEnumerable<string> collection) : base(collection) { }

        public static explicit operator string(SemicolonSplitStringList list) {
            return string.Join(';', list);
        }
    }
    public static class SemicolonSplitStringListExtensions {
        public static string ToSemicolonSplitString(this SemicolonSplitStringList list) {
            return (string)list;
        }
        public static SemicolonSplitStringList ToSemicolonSplitStringList(this string str, StringSplitOptions stringSplitOptions = StringSplitOptions.None) {
            return new SemicolonSplitStringList(str.Split(';', stringSplitOptions));
        }
    }
}
