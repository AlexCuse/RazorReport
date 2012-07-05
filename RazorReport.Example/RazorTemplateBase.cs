using System;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace RazorReport.Example {
    public class RazorTemplateBase<T> : TemplateBase<T> {
        public IEncodedString DocType {
            get { return Raw(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">"); }
        }

        public IEncodedString DataImage(byte[] image, string altText = "image") {
            return
                Raw(string.Format(
                    @"<img src=""data:image/png;base64,{0}"" alt=""{1}"" />", Convert.ToBase64String (image, Base64FormattingOptions.None), altText));
        }
    }
}
