namespace xNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class Json
    {
        protected static void EatWhitespace(char[] json, ref int index)
        {
            while (index < json.Length)
            {
                if (" \t\n\r".IndexOf(json[index]) == -1)
                {
                    return;
                }
                index++;
            }
        }
        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            int num = index;
            while (num < json.Length)
            {
                if ("0123456789+-.eE".IndexOf(json[num]) == -1)
                {
                    break;
                }
                num++;
            }
            return (num - 1);
        }
        protected static bool IsNumeric(object o)
        {
            double num;
            return ((o != null) && double.TryParse(o.ToString(), out num));
        }

        public static object JsonDecode(string json)
        {
            bool success = true;
            object obj2 = JsonDecode(json, ref success);
            if (!success)
            {
                //throw new FormatException();
                return null;
            }
            return obj2;
        }

        public static object JsonDecode(string json, ref bool success)
        {
            success = true;
            if (json != null)
            {
                char[] chArray = json.ToCharArray();
                int index = 0;
                return ParseValue(chArray, ref index, ref success);
            }
            return null;
        }

        public static string JsonEncode(object json)
        {
            StringBuilder builder = new StringBuilder(0x7d0);
            if (!SerializeValue(json, builder))
            {
                return null;
            }
            return builder.ToString();
        }

        public static bool JsonEncodeClass<T>(T anObject)
        {
            StringBuilder builder = new StringBuilder(0x7d0);

            builder.Append("{");

            foreach (var prop in anObject.GetType().GetProperties())
            {
                var value = prop.GetValue(anObject.GetType(), null);
                SerializeString(prop.Name, builder);
                builder.Append(":");
                if (!SerializeValue(value, builder))
                {
                    return false;
                }
                builder.Append(", ");

            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append("}");
            return true;
        }




        protected static int LookAhead(char[] json, int index)
        {
            int num = index;
            return NextToken(json, ref num);
        }

        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            if (index != json.Length)
            {
                char ch = json[index];
                index++;
                switch (ch)
                {
                    case '"':
                    case '\'':
                        return 7;

                    case ',':
                        return 6;

                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return 8;

                    case ':':
                        return 5;

                    case '[':
                        return 3;

                    case ']':
                        return 4;

                    case '{':
                        return 1;

                    case '}':
                        return 2;
                }
                index--;
                int num = json.Length - index;
                if ((((num >= 5) && (json[index] == 'f')) && ((json[index + 1] == 'a') && (json[index + 2] == 'l'))) && ((json[index + 3] == 's') && (json[index + 4] == 'e')))
                {
                    index += 5;
                    return 10;
                }
                if ((((num >= 4) && (json[index] == 't')) && ((json[index + 1] == 'r') && (json[index + 2] == 'u'))) && (json[index + 3] == 'e'))
                {
                    index += 4;
                    return 9;
                }
                if ((((num >= 4) && (json[index] == 'n')) && ((json[index + 1] == 'u') && (json[index + 2] == 'l'))) && (json[index + 3] == 'l'))
                {
                    index += 4;
                    return 11;
                }
            }
            return 0;
        }

        public static object jx = new object();
        public static JsonObject Parse(string json)
        {
            lock (jx)
                return new JsonObject(JsonDecode(json));
        }

        protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
        {
            ArrayList list = new ArrayList();
            NextToken(json, ref index);
            bool flag = false;
            while (!flag)
            {
                switch (LookAhead(json, index))
                {
                    case 0:
                        success = false;
                        return null;

                    case 6:
                    {
                        NextToken(json, ref index);
                        continue;
                    }
                    case 4:
                        NextToken(json, ref index);
                        return list;
                }
                object obj2 = ParseValue(json, ref index, ref success);
                if (!success)
                {
                    return null;
                }
                list.Add(obj2);
            }
            return list;
        }

        protected static double ParseNumber(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
            int length = (lastIndexOfNumber - index) + 1;
            char[] destinationArray = new char[length];
            Array.Copy(json, index, destinationArray, 0, length);
            index = lastIndexOfNumber + 1;
            return double.Parse(new string(destinationArray), CultureInfo.InvariantCulture);
        }

        protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
        {
            Hashtable hashtable = new Hashtable();
            NextToken(json, ref index);
            bool flag = false;
            while (!flag)
            {
                switch (LookAhead(json, index))
                {
                    case 0:
                        success = false;
                        return null;

                    case 6:
                    {
                        NextToken(json, ref index);
                        continue;
                    }
                    case 2:
                        NextToken(json, ref index);
                        return hashtable;
                }
                string str = ParseString(json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }
                if (NextToken(json, ref index) != 5)
                {
                    success = false;
                    return null;
                }
                object obj2 = ParseValue(json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }
                hashtable[str] = obj2;
            }
            return hashtable;
        }

        protected static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder builder = new StringBuilder(0x7d0);
            EatWhitespace(json, ref index);
            char ch2 = json[index++];
            bool flag = false;
            while (!flag)
            {
                if (index == json.Length)
                {
                    break;
                }
                char ch = json[index++];
                if (ch == ch2)
                {
                    flag = true;
                    break;
                }
                if (ch == '\\')
                {
                    if (index == json.Length)
                    {
                        break;
                    }
                    ch = json[index++];
                    switch (ch)
                    {
                        case '"':
                        {
                            builder.Append('"');
                            continue;
                        }
                        case '\\':
                        {
                            builder.Append('\\');
                            continue;
                        }
                        case '/':
                        {
                            builder.Append('/');
                            continue;
                        }
                        case 'b':
                        {
                            builder.Append('\b');
                            continue;
                        }
                        case 'f':
                        {
                            builder.Append('\f');
                            continue;
                        }
                        case 'n':
                        {
                            builder.Append('\n');
                            continue;
                        }
                        case 'r':
                        {
                            builder.Append('\r');
                            continue;
                        }
                        case 't':
                        {
                            builder.Append('\t');
                            continue;
                        }
                        case 'u':
                        {
                            int num = json.Length - index;
                            if (num < 4)
                            {
                                goto Label_0157;
                            }
                            char[] destinationArray = new char[4];
                            Array.Copy(json, index, destinationArray, 0, 4);
                            uint num2 = uint.Parse(new string(destinationArray), NumberStyles.HexNumber);
                            builder.Append(char.ConvertFromUtf32((int) num2));
                            index += 4;
                            break;
                        }
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }
        Label_0157:
            if (!flag)
            {
                success = false;
                return null;
            }
            return builder.ToString();
        }

        protected static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case 1:
                    return ParseObject(json, ref index, ref success);

                case 3:
                    return ParseArray(json, ref index, ref success);

                case 7:
                    return ParseString(json, ref index, ref success);

                case 8:
                    return ParseNumber(json, ref index);

                case 9:
                    NextToken(json, ref index);
                    return bool.Parse("TRUE");

                case 10:
                    NextToken(json, ref index);
                    return bool.Parse("FALSE");

                case 11:
                    NextToken(json, ref index);
                    return null;
            }
            success = false;
            return null;
        }

        protected static bool SerializeArray(ArrayList anArray, StringBuilder builder)
        {
            builder.Append("[");
            bool flag = true;
            for (int i = 0; i < anArray.Count; i++)
            {
                object obj2 = anArray[i];
                if (!flag)
                {
                    builder.Append(", ");
                }
                if (!SerializeValue(obj2, builder))
                {
                    return false;
                }
                flag = false;
            }
            builder.Append("]");
            return true;
        }

        protected static bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
            return true;
        }


        public static bool SerializeObject(Hashtable anObject, StringBuilder builder)
        {
            builder.Append("{");
            IDictionaryEnumerator enumerator = anObject.GetEnumerator();
            for (bool flag = true; enumerator.MoveNext(); flag = false)
            {
                string aString = enumerator.Key.ToString();
                object obj2 = enumerator.Value;
                if (!flag)
                {
                    builder.Append(", ");
                }
                SerializeString(aString, builder);
                builder.Append(":");
                if (!SerializeValue(obj2, builder))
                {
                    return false;
                }
            }
            builder.Append("}");
            return true;
        }

        protected static bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");
            foreach (char ch in aString.ToCharArray())
            {
                switch (ch)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;

                    case '\\':
                        builder.Append(@"\\");
                        break;

                    case '\b':
                        builder.Append(@"\b");
                        break;

                    case '\f':
                        builder.Append(@"\f");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    case '\t':
                        builder.Append(@"\t");
                        break;

                    default:
                    {
                        int num2 = Convert.ToInt32(ch);
                        if ((num2 >= 0x20) && (num2 <= 0x7e))
                        {
                            builder.Append(ch);
                        }
                        else
                        {
                            builder.Append(@"\u" + Convert.ToString(num2, 0x10).PadLeft(4, '0'));
                        }
                        break;
                    }
                }
            }
            builder.Append("\"");
            return true;
        }

        protected static bool SerializeValue(object value, StringBuilder builder)
        {
            bool flag = true;

            if (value is string)
            {
                return SerializeString((string) value, builder);
            }
            if (value is Hashtable)
            {
                return SerializeObject((Hashtable) value, builder);
            }
            if (value is ArrayList)
            {
                return SerializeArray((ArrayList) value, builder);
            }
            if (IsNumeric(value))
            {
                return SerializeNumber(Convert.ToDouble(value), builder);
            }
            if ((value is bool) && ((bool) value))
            {
                builder.Append("true");
                return flag;
            }
            if ((value is bool) && !((bool) value))
            {
                builder.Append("false");
                return flag;
            }

            if (value == null)
            {
                builder.Append("null");
                return flag;
            }
            return false;
        }


        #region

        #endregion





        #region Serialize
        public static string Serialize(object obj, bool includeTypeInfoForDerivedTypes = false, bool prettyPrint = false, bool includePrivateFields = true)
        {
            Json s = new Json(includeTypeInfoForDerivedTypes, prettyPrint, includePrivateFields);
            s.SerializeValue(obj);
            return s.GetJson();
        }

        private Json(bool includeTypeInfoForDerivedTypes, bool prettyPrint, bool includePrivateFields)
        {
            m_builder = new StringBuilder();
            m_includeTypeInfoForDerivedTypes = includeTypeInfoForDerivedTypes;
            m_prettyPrint = prettyPrint;
            m_includePrivateFields = includePrivateFields;
            m_prefix = "";

            m_fieldFlags =
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                (m_includePrivateFields ? System.Reflection.BindingFlags.NonPublic : 0);
        }

        private string GetJson()
        {
            return m_builder.ToString();
        }

        private StringBuilder m_builder;
        private bool m_includeTypeInfoForDerivedTypes;
        private bool m_prettyPrint;
        private bool m_includePrivateFields;
        private System.Reflection.BindingFlags m_fieldFlags;
        private string m_prefix;

        private void Indent()
        {
            if (m_prettyPrint)
            {
                m_prefix = m_prefix + "  ";
            }
        }

        private void Outdent()
        {
            if (m_prettyPrint)
            {
                m_prefix = m_prefix.Substring(2);
            }
        }

        private void AddIndent()
        {
            if (m_prettyPrint)
            {
                m_builder.Append(m_prefix);
            }
        }

        private void AddLine()
        {
            if (m_prettyPrint)
            {
                m_builder.Append("\n");
            }
        }

        private void AddSpace()
        {
            if (m_prettyPrint)
            {
                m_builder.Append(" ");
            }
        }


        public bool IsGenericList(System.Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

        private void SerializeValue(object obj)
        {
            if (obj == null)
            {
                m_builder.Append("null");
                return;
            }
            System.Type type = obj.GetType();

            if (type.IsArray)
            {
                SerializeArray(obj);
            }
            else if (IsGenericList(type))
            { //(type == typeof(List<Object>)) {
                Type elementType = type.GetGenericArguments()[0];
                System.Reflection.MethodInfo castMethod = typeof(System.Linq.Enumerable).GetMethod("Cast").MakeGenericMethod(new System.Type[] { elementType });
                System.Reflection.MethodInfo toArrayMethod = typeof(System.Linq.Enumerable).GetMethod("ToArray").MakeGenericMethod(new System.Type[] { elementType });
                var castedObjectEnum = castMethod.Invoke(null, new object[] { obj });
                var castedObject = toArrayMethod.Invoke(null, new object[] { castedObjectEnum });
                //            object[] oArray = ((List<object>)obj).ToArray();
                //            SerializeArray(oArray);
                SerializeArray(castedObject);
            }
            else if (type.IsEnum)
            {
                SerializeString(obj.ToString());
            }
            else if (type == typeof(string))
            {
                SerializeString(obj as string);
            }
            else if (type == typeof(Char))
            {
                SerializeString(obj.ToString());
            }
            else if (type == typeof(bool))
            {
                m_builder.Append((bool)obj ? "true" : "false");
            }
            else if (type == typeof(Boolean))
            {
                m_builder.Append((Boolean)obj ? "true" : "false");
                //        } else if (type.IsPrimitive) {
                //            m_builder.Append(System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertToInvariantString(obj));

                m_builder.Append(Convert.ChangeType(obj, typeof(string)));
            }
            else if (type == typeof(int))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(Byte))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(SByte))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(Int16))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(UInt16))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(Int32))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(UInt32))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(Int64))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(UInt64))
            {
                m_builder.Append(obj);
            }
            else if (type == typeof(Single))
            {
                m_builder.Append(((Single)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(Double))
            {
                m_builder.Append(((Double)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(float))
            {
                m_builder.Append(((float)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                m_builder.Append(((double)obj).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type.IsValueType)
            {
                SerializeObject(obj);
            }
            else if (type.IsClass)
            {
                SerializeObject(obj);
            }
            else
            {
                throw new System.InvalidOperationException("unsupport type: " + type.Name);
            }
        }

        private void SerializeArray(object obj)
        {
            m_builder.Append("[");
            AddLine();
            Indent();
            Array array = obj as Array;
            bool first = true;
            foreach (object element in array)
            {
                if (!first)
                {
                    m_builder.Append(",");
                    AddLine();
                }
                AddIndent();
                SerializeValue(element);
                first = false;
            }
            AddLine();
            Outdent();
            AddIndent();
            m_builder.Append("]");
        }

        private void SerializeDictionary(IDictionary obj)
        {
            bool first = true;
            foreach (object key in obj.Keys)
            {
                if (!first)
                {
                    m_builder.Append(',');
                    AddLine();
                }

                AddIndent();
                SerializeString(key.ToString());
                m_builder.Append(':');
                AddSpace();

                SerializeValue(obj[key]);

                first = false;
            }
        }

        private void SerializeObject(object obj)
        {
            m_builder.Append("{");
            AddLine();
            Indent();
            bool first = true;
            if (m_includeTypeInfoForDerivedTypes)
            {
                // Only inlcude type info for derived types.
                System.Type type = obj.GetType();
                System.Type baseType = type.BaseType;
                if (baseType != null && baseType != typeof(System.Object))
                {
                    AddIndent();
                    SerializeString("$dotNetType");  // assuming this won't clash with user's properties.
                    m_builder.Append(":");
                    AddSpace();
                    SerializeString(type.AssemblyQualifiedName);
                }
            }

            IDictionary asDict;
            if ((asDict = obj as IDictionary) != null)
            {
                SerializeDictionary(asDict);
            }
            else
            {
                System.Reflection.FieldInfo[] fields = obj.GetType().GetFields(m_fieldFlags);
                foreach (System.Reflection.FieldInfo info in fields)
                {
                    if (info.IsStatic)
                    {
                        continue;
                    }
                    object fieldValue = info.GetValue(obj);
                    //if (fieldValue != null)
                    //{
                        if (!first)
                        {
                            m_builder.Append(",");
                            AddLine();
                        }
                        AddIndent();

                        SerializeString(info.Name
                            .Replace("i__Field", "")
                            .Replace('<', ' ')
                            .Replace('>', ' ')
                            .Trim()
                        );
                        m_builder.Append(":");
                        AddSpace();
                        SerializeValue(fieldValue == null ? null : fieldValue);
                        first = false;
                    //}
                }
            }
            AddLine();
            Outdent();
            AddIndent();
            m_builder.Append("}");
        }

        private void SerializeString(string str)
        {
            m_builder.Append('\"');

            char[] charArray = str.ToCharArray();
            foreach (var c in charArray)
            {
                switch (c)
                {
                    case '"':
                        m_builder.Append("\\\"");
                        break;
                    case '\\':
                        m_builder.Append("\\\\");
                        break;
                    case '\b':
                        m_builder.Append("\\b");
                        break;
                    case '\f':
                        m_builder.Append("\\f");
                        break;
                    case '\n':
                        m_builder.Append("\\n");
                        break;
                    case '\r':
                        m_builder.Append("\\r");
                        break;
                    case '\t':
                        m_builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126))
                        {
                            m_builder.Append(c);
                        }
                        else
                        {
                            m_builder.Append("\\u");
                            m_builder.Append(codepoint.ToString("x4"));
                        }
                        break;
                }
            }

            m_builder.Append('\"');
        }
        #endregion
    }
}

