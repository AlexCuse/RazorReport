using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace RazorReport.Tests {
    [TestFixture]
    public class ReportBuilderTest {
        [Test]
        public void Throws_Exception_If_Template_Undefined () {
            var ex = Assert.Throws (typeof (InvalidOperationException),
                          () => ReportBuilder<object>.Create ("named").BuildHtml (null));

            Assert.AreEqual ("ReportBuilder must have Template configured before use.", ex.Message);
        }

        [Test]
        public void Correctly_Merges_Stylesheet_Into_Template () {
            var template = "some text and @@STYLES and some more";
            var css = "THIS IS THE STYLES";

            var builder = ReportBuilder<object>.Create ("testMergesTemplate")
                .WithCss (css)
                .WithTemplate (template);

            Assert.AreEqual ("some text and <style type='text/css'>" + Environment.NewLine + "THIS IS THE STYLES" + Environment.NewLine + "</style> and some more", builder.BuildHtml (null));
        }

        [Test]
        public void Ignores_Stylesheet_If_Not_Provided () {
            var template = "@@STYLES THIS IS THE BODY";

            var builder = ReportBuilder<object>.Create ("testIgnoresMissingMaster")
                .WithTemplate (template);

            Assert.AreEqual (" THIS IS THE BODY", builder.BuildHtml (null));
        }

        [Test]
        public void Can_Include_At_Signs_In_Templates_By_Escaping () {
            var template = "@@";

            var builder = ReportBuilder<object>.Create ("test@Escape")
                .WithTemplate (template);

            Assert.AreEqual ("@", builder.BuildHtml (null));
        }

        [Test]
        public void Recompiles_If_Template_Changed () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "recompileIfChange";
            var template = "template";
            var newTemplate = "template2";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);
                engine.Compile (newTemplate, templateName);

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }

            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template);

                builder.BuildHtml (model);

                builder = builder.WithTemplate (newTemplate);

                builder.BuildHtml (model);
            }
        }

        [Test]
        public void Recompiles_If_Stylesheet_Changed () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "recompileIfChange";
            var template = "template";
            var css = "STYLES";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);
                LastCall.Repeat.Twice ();

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }

            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template);

                builder.BuildHtml (model);

                builder = builder.WithCss (css);

                builder.BuildHtml (model);
            }
        }

        [Test]
        public void Doesnt_Recompile_If_No_Changes_To_Builder () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "doesntRecompileIfNoChange";
            var template = "template";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);//just once

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }

            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template);

                builder.BuildHtml (model);

                model.Name = "changed";

                builder.BuildHtml (model);
            }
        }

        [Test]
        public void Doesnt_Recompile_If_Template_Or_Stylesheet_Set_To_Same_Thing () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "doesntRecompileIfNoChange";
            var template = "template";
            var css = "css";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);//just once

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }

            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template)
                    .WithCss (css);

                builder.BuildHtml (model);

                builder.WithTemplate (template)
                    .WithCss (css);

                builder.BuildHtml (model);
            }
        }
    }
}
