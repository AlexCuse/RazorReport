using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace RazorReport.Pdf {
    public class PdfRenderer : IPdfRenderer {
        public byte[] RenderFromHtml (string html) {
            var output = new MemoryStream ();
            using (var reader = new StringReader (html)) {
                var document = new Document ();
                var writer = PdfWriter.GetInstance (document, output);
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
