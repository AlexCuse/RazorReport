using NUnit.Framework;

namespace RazorReport.Tests {
    [TestFixture]
    public class ReportBuilderTest {
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
    }
}
