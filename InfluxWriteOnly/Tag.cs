namespace InfluxWriteOnly {
    public class Tag {
        public string Key { get; }
        public string Value { get; }

        public Tag(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString() {
            return $"{PointFormatter.Escape(Key)}={PointFormatter.Escape(Value)}";
        }
    }
}