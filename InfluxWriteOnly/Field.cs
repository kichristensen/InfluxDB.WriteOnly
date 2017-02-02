using System.Globalization;

namespace InfluxWriteOnly {
    public class Field {
        public string Key { get; }
        public object Value { get; }
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public Field(string key, float value) {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, int value) {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, bool value) {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString() {
            if (Value is float) {
                return string.Format(Culture, "{0}={1}", PointFormatter.Escape(Key), Value);
            }

            if (Value is int) {
                return string.Format(Culture, "{0}={1}i", PointFormatter.Escape(Key), Value);
            }

            if (Value is string) {
                return string.Format(Culture, "{0}={1}", PointFormatter.Escape(Key), PointFormatter.Escape(Value.ToString()));
            }

            return string.Format(Culture, "{0}={1}", PointFormatter.Escape(Key), (bool)Value ? "t" : "f");
        }
    }
}