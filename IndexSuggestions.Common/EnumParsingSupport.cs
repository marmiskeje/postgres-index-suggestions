using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IndexSuggestions.Common
{
    public static class EnumParsingSupport
    {
        public static bool TryConvertUsingAttribute<TEnum, TAttribute, TAttributeValue>(TAttributeValue val, Func<TAttribute, TAttributeValue> attrPropertySelector, out TEnum result)
            where TEnum : struct
            where TAttribute : Attribute
        {
            // todo - cache
            result = default(TEnum);
            if (!EqualityComparer<TAttributeValue>.Default.Equals(val, default(TAttributeValue)))
            {
                foreach (var n in Enum.GetNames(typeof(TEnum)))
                {
                    FieldInfo field = typeof(TEnum).GetField(n);
                    var attr = field.GetCustomAttribute<TAttribute>();
                    if (attr != null && attrPropertySelector(attr).Equals(val))
                    {
                        result = (TEnum)Enum.Parse(typeof(TEnum), n);
                        return true;
                    }
                }
            }
            return false;
        }

        public static TEnum ConvertUsingAttributeOrDefault<TEnum, TAttribute, TAttributeValue>(TAttributeValue val, Func<TAttribute, TAttributeValue> attrPropertySelector)
            where TEnum : struct
            where TAttribute : Attribute
        {
            TEnum result = default(TEnum);
            TryConvertUsingAttribute(val, attrPropertySelector, out result);
            return result;
        }
        public static TEnum ConvertFromNumericOrDefault<TEnum>(int n) where TEnum : struct
        {
            if (Enum.IsDefined(typeof(TEnum), n))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), n);
            }
            return default(TEnum);
        }
    }
}
