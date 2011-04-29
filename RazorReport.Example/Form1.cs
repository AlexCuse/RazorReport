using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace RazorReport.Example {
    public partial class Form1 : Form {
        public Form1 () {
            InitializeComponent ();
        }

        string Test () {
            var model = new Example { Name = "Alex", Email = "test@example.com", Values = new Dictionary<object, object> { { "Dogs", "Jasmine, Daisy" }, { "Cats", "Rocky, FuFu" } } };

            var assembly = Assembly.GetExecutingAssembly ();

            return ReportBuilder<Example>.Create ("modelReport")
                //.WithCssFromResource ("RazorReport.Example.MasterTemplate.htm", assembly)
                .WithTemplateFromResource ("RazorReport.Example.ExampleTemplate.htm", assembly)
                .BuildHtml (model);
        }

        private void button1_Click (object sender, EventArgs e) {
            webBrowser1.DocumentText = Test ();
        }
    }
}
