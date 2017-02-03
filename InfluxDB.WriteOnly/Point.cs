using System;
using System.Collections.Generic;

namespace InfluxDB.WriteOnly {
    public class Point {
        public string Measurement { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Field> Fields { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}