using System.IO;
using System.Reflection;

namespace RazorReport {
    public interface IReportBuilder<T> {
        IReportBuilder<T> WithTemplate (string template);
        IReportBuilder<T> WithCss (string css);
        IReportBuilder<T> WithTemplateFromFileSystem (string templatePath);
        IReportBuilder<T> WithCssFromFileSystem (string cssPath);
        IReportBuilder<T> WithTemplateFromResource (string resourceName, Assembly assembly);
        IReportBuilder<T> WithCssFromResource (string resourceName, Assembly assembly);
        IReportBuilder<T> WithPrecompilation ();
        IReportBuilder<T> WithPdfRenderer (IPdfRenderer renderer, bool stripStyles = true);

        string BuildReport (T model);
        byte[] BuildPdf (T model);
    }
}
