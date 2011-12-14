using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RazorReport.Pdf;

namespace RazorReport.Example {
    public partial class Form1 : Form {
        IReportBuilder<Example> builder;
        IReportBuilder<Example> precompilingBuilder;

        public Form1 () {
            InitializeComponent ();

            var assembly = Assembly.GetExecutingAssembly ();

            builder = ReportBuilder<Example>.Create ("modelReport")
                .WithCssFromResource ("RazorReport.Example.Style.css", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly)
                .WithPdfRenderer (new PdfRenderer ());

            precompilingBuilder = ReportBuilder<Example>.Create ("modelReport")
                .WithCssFromResource ("RazorReport.Example.Style.css", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly)
                .WithPrecompilation ();
        }



        string RunCompiled () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "Yes" }, { "Worked", "Yes" } } };

            return precompilingBuilder.BuildReport (model);
        }

        string Run () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "No" }, { "Worked", "Yes" } } };

            return builder.BuildReport (model);
        }

        byte[] RunPdf () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Compiled", "No" }, { "Worked", "Yes" } } };

            return builder.BuildPdf (model);
        }

        private void runCompiled_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = RunCompiled ();
        }

        private void run_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = Run ();
        }

        private void runPdf_Click (object sender, EventArgs e) {
            using (var sfd = new SaveFileDialog ()) {
                var dialogResult = sfd.ShowDialog ();
                if (dialogResult == DialogResult.OK) {
                    var file = sfd.FileName;

                    using (var fileStream = File.OpenWrite (file)) {
                        var content = RunPdf ();
                        fileStream.Write (content, 0, content.Length);
                    }
                    MessageBox.Show (string.Format ("PDF Saved to: {0}", file));
                }
            }
        }
    }
}
