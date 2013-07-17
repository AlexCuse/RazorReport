using System;
using System.Reflection;

namespace RazorReport {
    public class ReportBuilder<T> : IReportBuilder<T> {
        string name;
        string mainTemplate;
        string styleSheet;
        string helpers;
        bool precompile;
        bool needsCompilation = true;
        IEngine<T> engine;
        ICustomRenderer _customRenderer;
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
            return WithTemplate (ContentFinder.GetFromFileSystem (templatePath));
        }

        public IReportBuilder<T> WithTemplateFromResource (string resourceName, Assembly assembly) {
            return WithTemplate (ContentFinder.GetFromResource (resourceName, assembly));
        }

        public IReportBuilder<T> WithCss (string css) {
            needsCompilation = styleSheet != css;
            styleSheet = css;
            return this;
        }

        public IReportBuilder<T> WithCssFromFileSystem (string cssPath) {
            return WithCss (ContentFinder.GetFromFileSystem (cssPath));
        }

        public IReportBuilder<T> WithCssFromResource (string resourceName, Assembly assembly) {
            return WithCss (ContentFinder.GetFromResource (resourceName, assembly));
        }

        public IReportBuilder<T> WithHelpers(string razorHelpers) {
            needsCompilation = helpers != razorHelpers;
            helpers = razorHelpers;
            return this;
        }

        public IReportBuilder<T> WithHelpersFromFileSystem (string helperFilePath) {
            return WithHelpers (ContentFinder.GetFromFileSystem (helperFilePath));
        }

        public IReportBuilder<T> WithHelpersFromResource (string resourceName, Assembly assembly) {
            return WithHelpers (ContentFinder.GetFromResource (resourceName, assembly));
        }

        public IReportBuilder<T> WithPrecompilation () {
            precompile = true;
            return this;
        }

        public IReportBuilder<T> WithCustomRenderer (ICustomRenderer renderer, bool stripStyles = true) {
            stripStylesForPdfRendering = stripStyles;
            _customRenderer = renderer;
            return this;
        }

        public string BuildReport (T model) {
            return precompile ? CompiledReport (model) : Report (model);
        }

        public byte[] BuildCustomRendering (T model) {
            if (_customRenderer == null) {
                throw new InvalidOperationException ("No Custom Renderer has been configured.");
            }
            var html = BuildReport (model);
            if (stripStylesForPdfRendering) html = html.Replace (this.styleSheet, "");
            return _customRenderer.RenderFromHtml (html);
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
            return mainTemplate.Replace ("@@STYLES", PrepareStylesheet ())
                .Replace("@@HELPERS", helpers ?? string.Empty);
        }

        string PrepareStylesheet () {
            return string.IsNullOrEmpty (styleSheet) ? string.Empty : string.Format ("<style type='text/css'>{1}{0}{1}</style>", styleSheet, Environment.NewLine);
        }
    }
}