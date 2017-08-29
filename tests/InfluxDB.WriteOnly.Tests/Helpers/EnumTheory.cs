using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace InfluxDB.WriteOnly.Tests.Helpers
{
    public class EnumTheory : DataAttribute
    {
        private readonly Type enumType;

        public EnumTheory(Type enumType)
        {
            this.enumType = enumType;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            foreach (var value in Enum.GetValues(enumType))
            {
                yield return new[] { value };
            }
        }
    }
}