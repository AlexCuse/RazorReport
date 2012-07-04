using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace RazorReport.Tests {
    [TestFixture]
    public class ReportBuilderTest {
        [Test]
        public void Throws_Exception_If_Template_Undefined() {
            var ex = Assert.Throws(typeof(InvalidOperationException),
                          () => ReportBuilder<object>.Create("named").BuildReport(null));

            Assert.AreEqual("ReportBuilder must have Template configured before use.", ex.Message);
        }

        [Test]
        public void Correctly_Merges_Stylesheet_Into_Template() {
            var template = "some text and @@STYLES and some more";
            var css = "THIS IS THE STYLES";

            var builder = ReportBuilder<object>.Create("testMergesTemplate")
                .WithCss(css)
                .WithTemplate(template);

            Assert.AreEqual("some text and <style type='text/css'>" + Environment.NewLine + "THIS IS THE STYLES" + Environment.NewLine + "</style> and some more", builder.BuildReport(null));
        }

        [Test]
        public void Ignores_Stylesheet_If_Not_Provided() {
            var template = "@@STYLES THIS IS THE BODY";

            var builder = ReportBuilder<object>.Create("testIgnoresMissingMaster")
                .WithTemplate(template);

            Assert.AreEqual(" THIS IS THE BODY", builder.BuildReport(null));
        }

        [Test]
        public void Can_Include_At_Signs_In_Templates_By_Escaping() {
            var template = "@@";

            var builder = ReportBuilder<object>.Create("test@Escape")
                .WithTemplate(template);

            Assert.AreEqual("@", builder.BuildReport(null));
        }

        [Test]
        public void Recompiles_If_Template_Changed() {
            var mockery = new MockRepository();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "recompileIfChange";
            var template = "template";
            var newTemplate = "template2";
            var model = new Example();

            using (mockery.Record()) {
                engine.Compile(template, templateName);
                engine.Compile(newTemplate, templateName);

                Expect.Call(engine.Run(model, templateName)).Repeat.Twice().Return("return");
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithPrecompilation();

                builder.BuildReport(model);

                builder = builder.WithTemplate(newTemplate);

                builder.BuildReport(model);
            }
        }

        [Test]
        public void Recompiles_If_Stylesheet_Changed() {
            var mockery = new MockRepository();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "recompileIfChange";
            var template = "template";
            var css = "STYLES";
            var model = new Example();

            using (mockery.Record()) {
                engine.Compile(template, templateName);
                LastCall.Repeat.Twice();

                Expect.Call(engine.Run(model, templateName)).Repeat.Twice().Return("return");
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithPrecompilation();

                builder.BuildReport(model);

                builder = builder.WithCss(css);

                builder.BuildReport(model);
            }
        }

        [Test]
        public void Doesnt_Recompile_If_No_Changes_To_Builder() {
            var mockery = new MockRepository();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "doesntRecompileIfNoChange";
            var template = "template";
            var model = new Example();

            using (mockery.Record()) {
                engine.Compile(template, templateName);//just once

                Expect.Call(engine.Run(model, templateName)).Repeat.Twice().Return("return");
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithPrecompilation();

                builder.BuildReport(model);

                model.Name = "changed";

                builder.BuildReport(model);
            }
        }

        [Test]
        public void Doesnt_Recompile_If_Template_Or_Stylesheet_Set_To_Same_Thing() {
            var mockery = new MockRepository();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "doesntRecompileIfNoChange";
            var template = "template";
            var css = "css";
            var model = new Example();

            using (mockery.Record()) {
                engine.Compile(template, templateName);//just once

                Expect.Call(engine.Run(model, templateName)).Repeat.Twice().Return("return");
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithCss(css)
                    .WithPrecompilation();

                builder.BuildReport(model);

                builder.WithTemplate(template)
                    .WithCss(css);

                builder.BuildReport(model);
            }
        }

        [Test]
        public void WithPdfRenderer() {
            var mockery = new MockRepository();
            var renderer = mockery.StrictMock<IPdfRenderer>();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "templateName";
            var template = "template";
            var css = "css";
            var model = new Example();
            var rendered = "cssrendered";
            var strippedRendered = "rendered";
            var pdf = new byte[0];

            using (mockery.Record()) {
                engine.Compile(template, templateName);
                SetupResult.For(engine.Run(model, templateName)).Return(rendered);

                Expect.Call(renderer.RenderFromHtml(strippedRendered)).Return(pdf);
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithCss(css)
                    .WithPdfRenderer(renderer)
                    .WithPrecompilation();

                var output = builder.BuildPdf(model);
                Assert.AreEqual(output, pdf);
            }
        }

        [Test]
        public void WithPdfRenderer_KeepStyles() {
            var mockery = new MockRepository();
            var renderer = mockery.StrictMock<IPdfRenderer>();
            var engine = mockery.StrictMock<IEngine<Example>>();

            var templateName = "templateName";
            var template = "template";
            var css = "css";
            var model = new Example();
            var rendered = "cssrendered";
            var pdf = new byte[0];

            using (mockery.Record()) {
                engine.Compile(template, templateName);
                SetupResult.For(engine.Run(model, templateName)).Return(rendered);

                Expect.Call(renderer.RenderFromHtml(rendered)).Return(pdf);
            }

            using (mockery.Playback()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance(templateName, engine)
                    .WithTemplate(template)
                    .WithCss(css)
                    .WithPdfRenderer(renderer, false)
                    .WithPrecompilation();

                var output = builder.BuildPdf(model);
                Assert.AreEqual(output, pdf);
            }
        }

        [Test]
        public void Exception_If_No_PdfRenderer() {
            var builder = ReportBuilder<Example>.Create("asdf");

            var ex = Assert.Throws(typeof(InvalidOperationException), () => builder.BuildPdf(new Example()));
            Assert.AreEqual("No PDF Renderer has been configured.", ex.Message);
        }

        [Test]
        public void Can_Use_Helper_Method_To_Include_Image_As_Base64 () {
            var fakeImage = new byte[1000];
            new Random(1024).NextBytes(fakeImage);

            var model = new Example { Image = fakeImage, Name="test" };
            string helpers =
                @"@helper DataImage(byte[] image, string altText) {
    <img src=""data:image/png;base64,@System.Convert.ToBase64String(image, Base64FormattingOptions.None)"" alt=""@altText"" />
}";
            string template =
@"@@HELPERS@DataImage(Model.Image, Model.Name)";

            var report = ReportBuilder<Example>
                .Create("imageTest")
                .WithTemplate(template)
                .WithHelpers(helpers)
                .BuildReport(model);

            Func<byte[], string> makeImageSrcData = b => "data:image/png;base64," +
                                                                  Convert.ToBase64String(b, Base64FormattingOptions.None);

            var expected = "<img src=\"" + makeImageSrcData(fakeImage) + "\" alt=\"test\" />";
            Assert.AreEqual(expected, report.Trim().Replace(Environment.NewLine, string.Empty));
        }

        [Test]
        public void Recompiles_If_Helpers_Changed () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "recompileIfChange";
            var template = "template";
            var helpers = "HELPERS";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);
                LastCall.Repeat.Twice ();

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }
                
            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template)
                    .WithPrecompilation ();

                builder.BuildReport (model);

                builder = builder.WithHelpers (helpers);

                builder.BuildReport (model);
            }
        }
    }
}
