using System.Globalization;

namespace InfluxWriteOnly {
    public class Field {
        private readonly string key;
        private readonly object value;
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public Field(string key, float value) {
            this.key = key;
            this.value = value;
        }

        public Field(string key, int value) {
            this.key = key;
            this.value = value;
        }

        public Field(string key, bool value) {
            this.key = key;
            this.value = value;
        }

        public Field(string key, string value) {
            this.key = key;
            this.value = value;
        }

        public override string ToString() {
            if (value is float) {
                return string.Format(Culture, "{0}={1}", PointFormatter.Escape(key), value);
            }

            if (value is int) {
                return string.Format(Culture, "{0}={1}i", PointFormatter.Escape(key), value);
            }

            if (value is string) {
                return string.Format(Culture, "{0}={1}", PointFormatter.Escape(key), PointFormatter.Escape(value.ToString()));
            }

            return string.Format(Culture, "{0}={1}", PointFormatter.Escape(key), (bool)value ? "t" : "f");
        }
    }
}