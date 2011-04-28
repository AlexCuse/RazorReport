using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RazorReport {
    public interface IReportBuilder<T> {
        IReportBuilder<T> WithTemplate (string templateString);
        IReportBuilder<T> WithMasterTemplate (string templateString);
        IReportBuilder<T> WithTemplateFromFileSystem (string templateFile);
        IReportBuilder<T> WithMasterTemplateFromFileSystem (string templateFile);
        IReportBuilder<T> WithTemplateFromResource (string templateFile, Assembly assembly);
        IReportBuilder<T> WithMasterTemplateFromResource (string templateFile, Assembly assembly);

        IReportBuilder<T> WithTitle (Expression<Func<T, string>> titleGenerator);

        string BuildHtml (T model);
    }
}
