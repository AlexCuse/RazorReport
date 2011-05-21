using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace RazorReport.Example {
    public partial class Form1 : Form {
        IReportBuilder<Example> builder;

        public Form1 () {
            InitializeComponent ();

            var assembly = Assembly.GetExecutingAssembly ();

            builder = ReportBuilder<Example>.Create ("modelReport")
                .WithCssFromResource ("RazorReport.Example.Style.css", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly);
        }



        string RunCompiled () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "Yes" }, { "Worked", "Yes" } } };

            return builder.CompiledReport (model);
        }

        string Run () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "No" }, { "Worked", "Yes" } } };

            return builder.Report (model);
        }

        private void runCompiled_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = RunCompiled ();
        }

        private void run_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = Run ();
        }
    }
}
