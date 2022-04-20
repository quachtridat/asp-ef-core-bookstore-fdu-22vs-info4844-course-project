using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Web;

namespace CourseProject {
    public static class Helpers {
        public static HtmlString Attr(this IHtmlHelper helper, string name, int value, Func<bool>? condition = null) {
            if (helper is null) {
                throw new ArgumentNullException(nameof(helper));
            }

            var render = condition != null ? condition.Invoke() : true;

            return render ?
                new HtmlString(string.Format("{0}={1}", name, value)) :
                HtmlString.Empty;
        }
        public static HtmlString Attr(this IHtmlHelper helper, string name, string value, Func<bool>? condition = null) {
            if (helper is null) {
                throw new ArgumentNullException(nameof(helper));
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) {
                return HtmlString.Empty;
            }

            var render = condition != null ? condition.Invoke() : true;

            return render ?
                new HtmlString(string.Format("{0}=\"{1}\"", name, HttpUtility.HtmlAttributeEncode(value))) :
                HtmlString.Empty;
        }
        public static HtmlString Attr(this IHtmlHelper helper, string name, Func<bool> condition, int valueIfTrue, int valueIfFalse) {
            if (helper is null) {
                throw new ArgumentNullException(nameof(helper));
            }

            if (string.IsNullOrEmpty(name)) {
                return HtmlString.Empty;
            }

            var render = condition();

            return render ?
                    new HtmlString(string.Format("{0}={1}", name, valueIfTrue)) :
                    new HtmlString(string.Format("{0}={1}", name, valueIfFalse));
        }
        public static HtmlString Attr(this IHtmlHelper helper, string name, Func<bool> condition, string valueIfTrue, string valueIfFalse) {
            if (helper is null) {
                throw new ArgumentNullException(nameof(helper));
            }

            if (string.IsNullOrEmpty(name)) {
                return HtmlString.Empty;
            }

            var render = condition();

            return render ?
                string.IsNullOrEmpty(valueIfTrue) ?
                    HtmlString.Empty :
                    new HtmlString(string.Format("{0}=\"{1}\"", name, HttpUtility.HtmlAttributeEncode(valueIfTrue))) :
                string.IsNullOrEmpty(valueIfFalse) ?
                    HtmlString.Empty :
                    new HtmlString(string.Format("{0}=\"{1}\"", name, HttpUtility.HtmlAttributeEncode(valueIfFalse)));
        }
    }
}
