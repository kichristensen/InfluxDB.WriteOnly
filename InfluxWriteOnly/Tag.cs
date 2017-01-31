namespace InfluxWriteOnly {
    public class Tag {
        private readonly string key;
        private readonly string value;

        public Tag(string key, string value) {
            this.key = key;
            this.value = value;
        }

        public override string ToString() {
            return $"{PointFormatter.Escape(key)}={PointFormatter.Escape(value)}";
        }
    }
}