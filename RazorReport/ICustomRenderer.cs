namespace RazorReport {
    public interface ICustomRenderer {
        byte[] RenderFromHtml (string html);
    }
}
