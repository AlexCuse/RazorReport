using System.Reflection;
using RazorEngine;

namespace RazorReport {
    public class ReportBuilder<T> : IReportBuilder<T> {
        string name;
        string template;
        string masterTemplate;
        bool needsCompilation = true;

        private ReportBuilder () { }

        public static IReportBuilder<T> Create (string name) {
            return new ReportBuilder<T> { name = name };
        }

        public IReportBuilder<T> WithTemplate (string templateString) {
            template = templateString;
            needsCompilation = true;
            return this;
        }

        public IReportBuilder<T> WithMasterTemplate (string templateString) {
            masterTemplate = templateString;
            needsCompilation = true;
            return this;
        }

        public IReportBuilder<T> WithTemplateFromFileSystem (string templateFile) {
            return WithTemplate (TemplateFinder.GetTemplateFromFileSystem (templateFile));
        }

        public IReportBuilder<T> WithMasterTemplateFromFileSystem (string templateFile) {
            return WithMasterTemplate (TemplateFinder.GetTemplateFromFileSystem (templateFile));
        }

        public IReportBuilder<T> WithTemplateFromResource (string templateFile, Assembly assembly) {
            return WithTemplate (TemplateFinder.GetTemplateFromResource (templateFile, assembly));
        }

        public IReportBuilder<T> WithMasterTemplateFromResource (string templateFile, Assembly assembly) {
            return WithMasterTemplate (TemplateFinder.GetTemplateFromResource (templateFile, assembly));
        }

        public string BuildHtml (T model) {
            if (needsCompilation) {
                var templateToUse = string.IsNullOrEmpty (masterTemplate) ? template : masterTemplate.Replace ("@@BODY", template);
                Razor.Compile (templateToUse, typeof (T), name);
                needsCompilation = false;
            }

            return Razor.Run<T> (model, name);
        }
    }
}