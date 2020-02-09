namespace xNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class JsonObject
    {
        [CompilerGenerated]
        private static Func<object, JsonObject> func_0;
        [CompilerGenerated]
        private static Func<DictionaryEntry, string> func_1;
        [CompilerGenerated]
        private static Func<DictionaryEntry, JsonObject> func_2;
        [CompilerGenerated]
        private object object_0;

        public JsonObject(object element)
        {
            this.Element = element;
        }

        public static explicit operator bool(JsonObject obj)
        {
            return (bool) obj.Element;
        }

        public static explicit operator double(JsonObject obj)
        {
            return (double) obj.Element;
        }

        public static explicit operator int(JsonObject obj)
        {
            return (int) ((double) obj.Element);
        }

        public static explicit operator string(JsonObject obj)
        {
            if (obj == null) return null;
            return (string) obj.Element;
        }

        [CompilerGenerated]
        private static JsonObject smethod_0(object object_1)
        {
            return new JsonObject(object_1);
        }

        [CompilerGenerated]
        private static string smethod_1(DictionaryEntry dictionaryEntry_0)
        {
            return dictionaryEntry_0.Key.ToString();
        }

        [CompilerGenerated]
        private static JsonObject smethod_2(DictionaryEntry dictionaryEntry_0)
        {
            return new JsonObject(dictionaryEntry_0.Value);
        }

        public JsonObject[] ToArray()
        {
            ArrayList element = (ArrayList) this.Element;
            if (func_0 == null)
            {
                func_0 = new Func<object, JsonObject>(JsonObject.smethod_0);
            }
            return Enumerable.Select<object, JsonObject>(element.Cast<object>(), func_0).ToArray<JsonObject>();
        }

        public Dictionary<string, JsonObject> ToDictionary()
        {
            Hashtable element = (Hashtable) this.Element;
            if (func_1 == null)
            {
                func_1 = new Func<DictionaryEntry, string>(JsonObject.smethod_1);
            }
            if (func_2 == null)
            {
                func_2 = new Func<DictionaryEntry, JsonObject>(JsonObject.smethod_2);
            }
            return Enumerable.ToDictionary<DictionaryEntry, string, JsonObject>(element.Cast<DictionaryEntry>(), func_1, func_2);
        }

        public override string ToString()
        {
            return this.Element.ToString();
        }

        public object Element
        {
            [CompilerGenerated]
            get
            {
                return this.object_0;
            }
            [CompilerGenerated]
            private set
            {
                this.object_0 = value;
            }
        }

        public JsonObject this[string key]
        {
            get
            {
                Hashtable element = (Hashtable) this.Element;
                if (!element.ContainsKey(key))
                {
                    return null;
                }
                return new JsonObject(element[key]);
            }
        }

        public JsonObject this[int key]
        {
            get
            {
                ArrayList element = (ArrayList) this.Element;
                return new JsonObject(element[key]);
            }
        }
    }
}

