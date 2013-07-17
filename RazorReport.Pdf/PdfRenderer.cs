using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;

namespace RazorReport.Pdf {
    public class PdfRenderer : ICustomRenderer {
        public byte[] RenderFromHtml (string html) {
            var output = new MemoryStream ();
            using (var reader = new StringReader (html)) {
                var document = new Document ();
                var worker = new HTMLWorker (document);

                document.Open ();
                worker.StartDocument ();
                worker.Parse (reader);

                worker.EndDocument ();
                worker.Close ();
                document.Close ();

                return output.ToArray ();
            }
        }
    }
}
