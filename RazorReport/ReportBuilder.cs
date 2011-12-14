using System;
using System.Reflection;

namespace RazorReport {
    public class ReportBuilder<T> : IReportBuilder<T> {
        string name;
        string mainTemplate;
        string styleSheet;
        bool precompile;
        bool needsCompilation = true;
        IEngine<T> engine;
        IPdfRenderer pdfRenderer;
        bool stripStylesForPdfRendering;

        private ReportBuilder () { }

        public static IReportBuilder<T> Create (string name) {
            return CreateWithEngineInstance (name, new Engine<T> ());
        }

        public static IReportBuilder<T> CreateWithEngineInstance (string name, IEngine<T> engine) {
            return new ReportBuilder<T> { name = name, engine = engine };
        }

        public IReportBuilder<T> WithTemplate (string template) {
            needsCompilation = mainTemplate != template;
            mainTemplate = template;
            return this;
        }

        public IReportBuilder<T> WithTemplateFromFileSystem (string templatePath) {
            return WithTemplate (TemplateFinder.GetTemplateFromFileSystem (templatePath));
        }

        public IReportBuilder<T> WithTemplateFromResource (string resourceName, Assembly assembly) {
            return WithTemplate (TemplateFinder.GetTemplateFromResource (resourceName, assembly));
        }

        public IReportBuilder<T> WithCss (string css) {
            needsCompilation = styleSheet != css;
            styleSheet = css;
            return this;
        }

        public IReportBuilder<T> WithCssFromFileSystem (string cssPath) {
            return WithCss (TemplateFinder.GetTemplateFromFileSystem (cssPath));
        }

        public IReportBuilder<T> WithCssFromResource (string resourceName, Assembly assembly) {
            return WithCss (TemplateFinder.GetTemplateFromResource (resourceName, assembly));
        }

        public IReportBuilder<T> WithPrecompilation () {
            precompile = true;
            return this;
        }

        public IReportBuilder<T> WithPdfRenderer (IPdfRenderer renderer, bool stripStyles = true) {
            stripStylesForPdfRendering = stripStyles;
            pdfRenderer = renderer;
            return this;
        }

        public string BuildReport (T model) {
            return precompile ? CompiledReport (model) : Report (model);
        }

        public byte[] BuildPdf (T model) {
            if (pdfRenderer == null) {
                throw new InvalidOperationException ("No PDF Renderer has been configured.");
            }
            var html = BuildReport (model);
            if (stripStylesForPdfRendering) html = html.Replace (this.styleSheet, "");
            return pdfRenderer.RenderFromHtml (html);
        }

        string CompiledReport (T model) {
            if (needsCompilation) {
                engine.Compile (PrepareTemplate (), name);
                needsCompilation = false;
            }
            return engine.Run (model, name);
        }

        string Report (T model) {
            return engine.Parse (PrepareTemplate (), model);
        }

        string PrepareTemplate () {
            if (string.IsNullOrEmpty (mainTemplate))
                throw new InvalidOperationException ("ReportBuilder must have Template configured before use.");
            return mainTemplate.Replace ("@@STYLES", PrepareStylesheet ());
        }

        string PrepareStylesheet () {
            return string.IsNullOrEmpty (styleSheet) ? string.Empty : string.Format ("<style type='text/css'>{1}{0}{1}</style>", styleSheet, Environment.NewLine);
        }
    }
}