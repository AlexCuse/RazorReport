using RazorEngine;

namespace RazorReport.Example {
    public static class RazorConfiguration {
        public static void Configure () {
            Razor.SetTemplateBase (typeof (RazorTemplateBase<>));
        }
    }
}
