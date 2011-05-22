using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace RazorReport.Example {
    public partial class Form1 : Form {
        IReportBuilder<Example> builder;
        IReportBuilder<Example> precompilingBuilder;

        public Form1 () {
            InitializeComponent ();

            var assembly = Assembly.GetExecutingAssembly ();

            builder = ReportBuilder<Example>.Create ("modelReport")
                .WithCssFromResource ("RazorReport.Example.Style.css", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly);

            precompilingBuilder = ReportBuilder<Example>.Create ("modelReport")
                .WithCssFromResource ("RazorReport.Example.Style.css", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly)
                .WithPrecompilation ();
        }



        string RunCompiled () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "Yes" }, { "Worked", "Yes" } } };

            return builder.BuildReport (model);
        }

        string Run () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "No" }, { "Worked", "Yes" } } };

            return builder.BuildReport (model);
        }

        private void runCompiled_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = RunCompiled ();
        }

        private void run_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = Run ();
        }
    }
}
