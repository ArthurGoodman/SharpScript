using System;
using System.Collections;
using System.Reflection;

namespace SharpScript {
    public static class ObjectDumper {
        public static string Inspect(this object obj) {
            string str = "{ ";

            MemberInfo[] members = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);

            bool first = true;

            foreach (MemberInfo member in members) {
                FieldInfo field = member as FieldInfo;
                PropertyInfo property = member as PropertyInfo;

                if (field != null || property != null) {
                    if (!first)
                        str += ", ";
                    else
                        first = false;

                    str += member.Name + " = \"";

                    Type type = field != null ? field.FieldType : property.PropertyType;

                    if (type.IsValueType || type == typeof(string))
                        WriteValue(field != null ? field.GetValue(obj) : property.GetValue(obj, null), ref str);
                    else
                        if (typeof(IEnumerable).IsAssignableFrom(type))
                            str += "...";
                        else
                            str += "{ }";

                    str += "\"";
                }
            }

            return str + " }";
        }

        private static void WriteValue(object value, ref string str) {
            if (value == null)
                str += "null";
            else if (value is DateTime)
                str += ((DateTime)value).ToShortDateString();
            else if (value is ValueType || value is string)
                str += value.ToString();
            else if (value is IEnumerable)
                str += "...";
            else
                str += "{ }";
        }
    }
}
