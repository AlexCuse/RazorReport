namespace RazorReport {
    public interface IPdfRenderer {
        byte[] RenderFromHtml (string html);
    }
}
