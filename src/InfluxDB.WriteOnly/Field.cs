﻿using System;
using System.Globalization;

namespace InfluxDB.WriteOnly
{
    public class Field : IEquatable<Field>
    {
        public string Key { get; }
        public object Value { get; }
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public Field(string key, float value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, double value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, int value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, bool value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, long value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, ulong value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, uint value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, short value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Field(string key, ushort value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            if (Value is float || Value is double)
            {
                return string.Format(Culture, "{0}={1}", PointFormatter.Escape(Key), Value);
            }

            if (Value is int || Value is long || Value is ulong || Value is uint || Value is short || Value is ushort)
            {
                return string.Format(Culture, "{0}={1}i", PointFormatter.Escape(Key), Value);
            }

            if (Value is string)
            {
                return string.Format(Culture, "{0}=\"{1}\"", PointFormatter.Escape(Key),
                    PointFormatter.Escape(Value.ToString()));
            }

            return string.Format(Culture, "{0}={1}", PointFormatter.Escape(Key), (bool) Value ? "t" : "f");
        }

        public bool Equals(Field other)
        {
            return string.Equals(Key, other.Key) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Field) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key?.GetHashCode() ?? 0) * 397) ^ (Value?.GetHashCode() ?? 0);
            }
        }
    }
}