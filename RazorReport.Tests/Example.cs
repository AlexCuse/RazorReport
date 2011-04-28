using System.Collections.Generic;

namespace RazorReport.Tests {
    public class Example {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dictionary<object, object> Values { get; set; }
        public string NameAndEmail () {
            return Name + " " + Email;
        }
    }
}