using System;
using RazorEngine.Templating;

namespace RazorReport.Example {
    public class RazorTemplateBase<T> : TemplateBase<T> {
        public String DocType {
            get { return @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">"; }
        }
    }
}
