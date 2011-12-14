using System.IO;

namespace RazorReport {
    public interface IPdfRenderer {
        Stream RenderFromHtml (string html);
    }
}
