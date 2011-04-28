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
        public void Correctly_Merges_Template_Into_Master () {
            var master = "some text and @@BODY and some more";
            var template = "THIS IS THE BODY";

            var builder = ReportBuilder<object>.Create ("testMergesTemplate")
                .WithMasterTemplate (master)
                .WithTemplate (template);

            Assert.AreEqual ("some text and THIS IS THE BODY and some more", builder.BuildHtml (null));
        }

        [Test]
        public void Ignores_Master_Template_If_Not_Provided () {
            var template = "THIS IS THE BODY";

            var builder = ReportBuilder<object>.Create ("testIgnoresMissingMaster")
                .WithTemplate (template);

            Assert.AreEqual ("THIS IS THE BODY", builder.BuildHtml (null));
        }

        [Test]
        public void Can_Replace_Tags_In_Master () {
            var master = "sometimes you just want to report on a @Model @@BODY";
            var template = "sometimes you don't";

            var builder = ReportBuilder<string>.Create ("testMasterTagReplacement")
                .WithMasterTemplate (master)
                .WithTemplate (template);

            Assert.AreEqual ("sometimes you just want to report on a RDLC sometimes you don't", builder.BuildHtml ("RDLC"));
        }

        [Test]
        public void Can_Add_Title_Manually () {
            //only single member expressions for now (need to compile into a razor tag)
            var model = new Example { Name = "Alex", Email = "test@example.com" };
            var master = "<title>@@TITLE</title>@@BODY";
            var template = "some other stuff";

            var builder = ReportBuilder<Example>.Create ("testManualTitleAdditionInMaster")
                .WithMasterTemplate (master)
                .WithTemplate (template)
                .WithTitle (x => x.Name);

            Assert.AreEqual ("<title>Alex</title>some other stuff", builder.BuildHtml (model));
        }

        [Test]
        public void Title_Can_Be_Parameterless_Method () {
            var model = new Example { Name = "Alex", Email = "test@example.com" };
            var master = "<title>@@TITLE</title>@@BODY";
            var template = "some other stuff";

            var builder = ReportBuilder<Example>.Create ("testInvalidTitleAdditionInMaster")
                .WithMasterTemplate (master)
                .WithTemplate (template)
                .WithTitle (x => x.NameAndEmail ());

            Assert.AreEqual ("<title>Alex test@example.com</title>some other stuff", builder.BuildHtml (model));
        }

        [Test]
        public void Can_Include_At_Signs_In_Templates_By_Escaping () {
            var master = "@@ @@BODY";
            var template = "@@";

            var builder = ReportBuilder<object>.Create ("test@Escape")
                .WithMasterTemplate (master)
                .WithTemplate (template);

            Assert.AreEqual ("@ @", builder.BuildHtml (null));
        }

        [Test]
        public void Title_Function_Too_Complex () {
            var ex = Assert.Throws (typeof (InvalidOperationException),
                          () => ReportBuilder<object>.Create ("test").WithTitle (x => x.ToString () + x.GetType ().Name));

            Assert.AreEqual ("Invalid expression type passed.", ex.Message);

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
        public void Recompiles_If_MasterTemplate_Changed () {
            var mockery = new MockRepository ();
            var engine = mockery.StrictMock<IEngine<Example>> ();

            var templateName = "recompileIfChange";
            var template = "template";
            var masterTemplate = "master @@BODY";
            var model = new Example ();

            using (mockery.Record ()) {
                engine.Compile (template, templateName);
                engine.Compile (masterTemplate.Replace ("@@BODY", template), templateName);

                Expect.Call (engine.Run (model, templateName)).Repeat.Twice ().Return ("return");
            }

            using (mockery.Playback ()) {
                var builder = ReportBuilder<Example>.CreateWithEngineInstance (templateName, engine)
                    .WithTemplate (template);

                builder.BuildHtml (model);

                builder = builder.WithMasterTemplate (masterTemplate);

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
    }
}
