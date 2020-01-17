using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.MobCAT
{
    public static class Guard
    {
        public static object Null(object value, [CallerMemberName]string propertyName = null)
        {
            if (value == null)
                throw new ArgumentException($"Parameter {propertyName} cannot be null");

            return value;
        }

        public static T Null<T>(T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
                throw new ArgumentException($"Parameter {propertyName} cannot be null or default for type {typeof(T)}");

            return value;
        }

        public static string NullOrWhitespace(string value, [CallerMemberName]string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Parameter {propertyName} cannot be null or whitespace");

            return value;
        }
    }
}