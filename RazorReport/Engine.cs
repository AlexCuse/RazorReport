using RazorEngine;

namespace RazorReport {
    public class Engine<T> : IEngine<T> {
        public void Compile (string preparedTemplate, string name) {
            Razor.Compile (preparedTemplate, typeof (T), name);
        }

        public string Run (T model, string name) {
            return Razor.Run<T> (model, name);
        }

        public string Parse (string template, T model) {
            return Razor.Parse<T> (template, model);
        }
    }
}
