using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RazorReport.Example {
    public static class RazorConfiguration {
        public static void Configure () {
            var service =
                new TemplateService (new TemplateServiceConfiguration { BaseTemplateType = typeof (RazorTemplateBase<>) });
            Razor.SetTemplateService (service);
        }
    }
}
