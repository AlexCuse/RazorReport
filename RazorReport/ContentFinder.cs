using System.IO;
using System.Reflection;

namespace RazorReport {
    class ContentFinder {

        public static string GetFromResource (string resourceName, Assembly assembly) {
            using (var stream = assembly.GetManifestResourceStream (resourceName))
            using (TextReader reader = new StreamReader (stream)) {
                return reader.ReadToEnd ();
            }
        }

        public static string GetFromFileSystem (string templatePath) {
            return File.ReadAllText (templatePath);
        }
    }
}
