using System.Collections.Generic;

namespace RazorReport.Example {
    public class Example {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dictionary<object, object> Values { get; set; }
        public byte[] Image { get; set; }
    }
}