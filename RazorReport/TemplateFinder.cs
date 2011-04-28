using System.IO;
using System.Reflection;

namespace RazorReport {
    class TemplateFinder {

        public static string GetTemplateFromResource (string resourceName, Assembly assembly) {
            using (var stream = assembly.GetManifestResourceStream (resourceName))
            using (TextReader reader = new StreamReader (stream)) {
                return reader.ReadToEnd ();
            }
        }

        public static string GetTemplateFromFileSystem (string templatePath) {
            return File.ReadAllText (templatePath);
        }
    }
}
