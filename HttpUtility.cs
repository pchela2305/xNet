namespace xNet
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Security.Cryptography;

    public static class HttpUtility
    {
        public static byte[] GetBytes(this Stream stream, int len)
        {
            lock (_lock)
            {
                byte[] buffer = new byte[len];
                stream.Read(buffer, 0, len);
                return buffer;
            }
        }
        public static int GetLength(this Stream stream)
        {
            lock (_lock)
            {
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                return BitConverter.ToInt32(buffer, 0);
            }
        }

        public static string Iso8859_1(this string txt)
        {
            Encoding iso = Encoding.UTF8;
            byte[] isoBytes = Encoding.ASCII.GetBytes(txt);
            return iso.GetString(isoBytes);
        }
        /// <summary>
        /// Генератор ников
        /// </summary>
        /// <returns></returns>
        public static string Gen_Nick()
        {
            lock (_lock)
            {
                string potentialName;
                do
                {
                    potentialName = "";
                    for (int digraphCount = 0; digraphCount < random.Next(2, 5); digraphCount++)
                        potentialName += digraphs[random.Next(0, digraphs.GetUpperBound(0))];

                } while (invalidNameRegex.IsMatch(potentialName));
                return potentialName.Substring(0, 1).ToUpper() + potentialName.Substring(1);
            }
        }
        private static string[] digraphs = new string[] { "en", "re", "er", "nt", "th", "on", "in", "te", "an", "or", "st", "ed", "ne", "ve", "es", "nd", "to", "se", "at", "ti", "ar", "ee", "rt", "as", "co", "io", "ty", "fo", "fi", "ra", "et", "le", "ou", "ma", "tw", "ea", "is", "si", "de", "hi", "al", "ce", "da", "ec", "rs", "ur", "ni", "ri", "el", "la", "ro", "ta" };
        private static Random random = new Random();
        private static Regex invalidNameRegex = new Regex(@"(?:([aiuy])\1)|(?:(\w\w)\2)|([^aeiouy]{3,})|([aeiouy]{3,})|(\wy\w)|(^nd)|(^nt)|(^rt)|(^rs)|(^ht)|([aiou]$)|(tw$)");
        /// <summary>
        /// Преобразование кирилицы в латиницу и обратно
        /// </summary>
        /// <param name="msg">текст для преобразования</param>
        /// <param name="obratno">метод перевода по умолчанию RU > EN</param>
        /// <returns></returns>
        public static string Translit(this string msg , bool obratno = false)
        {
            lock (_lock)
            {
                if (obratno)
                    return Transliteration.Front(msg);
                else
                    return Transliteration.Back(msg);
            }
        }
        private static class Transliteration
        {
            public enum TransliterationType
            {
                Gost,
                ISO
            }
            private static Dictionary<string, string> gost = new Dictionary<string, string>(); //ГОСТ 16876-71
            private static Dictionary<string, string> iso = new Dictionary<string, string>(); //ISO 9-95
            public static string Front(string text)
            {
                return Front(text, TransliterationType.ISO);
            }
            public static string Front(string text, TransliterationType type)
            {
                string output = text;
                Dictionary<string, string> tdict = GetDictionaryByType(type);

                foreach (KeyValuePair<string, string> key in tdict)
                {
                    output = output.Replace(key.Key, key.Value);
                }
                return output;
            }
            public static string Back(string text)
            {
                return Back(text, TransliterationType.ISO);
            }
            public static string Back(string text, TransliterationType type)
            {
                string output = text;
                Dictionary<string, string> tdict = GetDictionaryByType(type);

                foreach (KeyValuePair<string, string> key in tdict)
                {
                    output = output.Replace(key.Value, key.Key);
                }
                return output;
            }
            private static Dictionary<string, string> GetDictionaryByType(TransliterationType type)
            {
                Dictionary<string, string> tdict = iso;
                if (type == TransliterationType.Gost) tdict = gost;
                return tdict;
            }
            static Transliteration()
            {
                gost.Add("Є", "EH");
                gost.Add("І", "I");
                gost.Add("і", "i");
                gost.Add("№", "#");
                gost.Add("є", "eh");
                gost.Add("А", "A");
                gost.Add("Б", "B");
                gost.Add("В", "V");
                gost.Add("Г", "G");
                gost.Add("Д", "D");
                gost.Add("Е", "E");
                gost.Add("Ё", "JO");
                gost.Add("Ж", "ZH");
                gost.Add("З", "Z");
                gost.Add("И", "I");
                gost.Add("Й", "JJ");
                gost.Add("К", "K");
                gost.Add("Л", "L");
                gost.Add("М", "M");
                gost.Add("Н", "N");
                gost.Add("О", "O");
                gost.Add("П", "P");
                gost.Add("Р", "R");
                gost.Add("С", "S");
                gost.Add("Т", "T");
                gost.Add("У", "U");
                gost.Add("Ф", "F");
                gost.Add("Х", "KH");
                gost.Add("Ц", "C");
                gost.Add("Ч", "CH");
                gost.Add("Ш", "SH");
                gost.Add("Щ", "SHH");
                gost.Add("Ъ", "'");
                gost.Add("Ы", "Y");
                gost.Add("Ь", "");
                gost.Add("Э", "EH");
                gost.Add("Ю", "YU");
                gost.Add("Я", "YA");
                gost.Add("а", "a");
                gost.Add("б", "b");
                gost.Add("в", "v");
                gost.Add("г", "g");
                gost.Add("д", "d");
                gost.Add("е", "e");
                gost.Add("ё", "jo");
                gost.Add("ж", "zh");
                gost.Add("з", "z");
                gost.Add("и", "i");
                gost.Add("й", "jj");
                gost.Add("к", "k");
                gost.Add("л", "l");
                gost.Add("м", "m");
                gost.Add("н", "n");
                gost.Add("о", "o");
                gost.Add("п", "p");
                gost.Add("р", "r");
                gost.Add("с", "s");
                gost.Add("т", "t");
                gost.Add("у", "u");

                gost.Add("ф", "f");
                gost.Add("х", "kh");
                gost.Add("ц", "c");
                gost.Add("ч", "ch");
                gost.Add("ш", "sh");
                gost.Add("щ", "shh");
                gost.Add("ъ", "");
                gost.Add("ы", "y");
                gost.Add("ь", "");
                gost.Add("э", "eh");
                gost.Add("ю", "yu");
                gost.Add("я", "ya");
                gost.Add("«", "");
                gost.Add("»", "");
                gost.Add("—", "-");

                iso.Add("Є", "YE");
                iso.Add("І", "I");
                iso.Add("Ѓ", "G");
                iso.Add("і", "i");
                iso.Add("№", "#");
                iso.Add("є", "ye");
                iso.Add("ѓ", "g");
                iso.Add("А", "A");
                iso.Add("Б", "B");
                iso.Add("В", "V");
                iso.Add("Г", "G");
                iso.Add("Д", "D");
                iso.Add("Е", "E");
                iso.Add("Ё", "YO");
                iso.Add("Ж", "ZH");
                iso.Add("З", "Z");
                iso.Add("И", "I");
                iso.Add("Й", "J");
                iso.Add("К", "K");
                iso.Add("Л", "L");
                iso.Add("М", "M");
                iso.Add("Н", "N");
                iso.Add("О", "O");
                iso.Add("П", "P");
                iso.Add("Р", "R");
                iso.Add("С", "S");
                iso.Add("Т", "T");
                iso.Add("У", "U");
                iso.Add("Ф", "F");
                iso.Add("Х", "X");
                iso.Add("Ц", "C");
                iso.Add("Ч", "CH");
                iso.Add("Ш", "SH");
                iso.Add("Щ", "SHH");
                iso.Add("Ъ", "'");
                iso.Add("Ы", "Y");
                iso.Add("Ь", "");
                iso.Add("Э", "E");
                iso.Add("Ю", "YU");
                iso.Add("Я", "YA");
                iso.Add("а", "a");
                iso.Add("б", "b");
                iso.Add("в", "v");
                iso.Add("г", "g");
                iso.Add("д", "d");
                iso.Add("е", "e");
                iso.Add("ё", "yo");
                iso.Add("ж", "zh");
                iso.Add("з", "z");
                iso.Add("и", "i");
                iso.Add("й", "j");
                iso.Add("к", "k");
                iso.Add("л", "l");
                iso.Add("м", "m");
                iso.Add("н", "n");
                iso.Add("о", "o");
                iso.Add("п", "p");
                iso.Add("р", "r");
                iso.Add("с", "s");
                iso.Add("т", "t");
                iso.Add("у", "u");
                iso.Add("ф", "f");
                iso.Add("х", "x");
                iso.Add("ц", "c");
                iso.Add("ч", "ch");
                iso.Add("ш", "sh");
                iso.Add("щ", "shh");
                iso.Add("ъ", "");
                iso.Add("ы", "y");
                iso.Add("ь", "");
                iso.Add("э", "e");
                iso.Add("ю", "yu");
                iso.Add("я", "ya");
                iso.Add("«", "");
                iso.Add("»", "");
                iso.Add("—", "-");
            }
        }
        /// <summary>
        /// Из потока в массив byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToByteArray(this Stream stream)
        {
            lock (_lock)
            {
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
                    totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
                return buffer;
            }
        }
        /// <summary>
        /// Вернуть строку в виде MD5 хеша 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Md5Crypt(this string t)
        {
            lock (_lock)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(t));
                return BitConverter.ToString(checkSum).Replace("-", String.Empty).ToLower();
            }
        }
        /// <summary>
        /// Generate Random Mac
        /// </summary>
        /// <returns></returns>
        public static string GetRandomMacAddress()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }
        public static object _lock = new object();
        /// <summary>
        /// Преобразование из HTTP UNICODE в текст
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string HttpUnicodeDecoder(this string t)
        {
            lock (_lock)
            {
                var rx = new Regex(@"\\u([0-9A-Z]{4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return rx.Replace(t, p => new string((char)int.Parse(p.Groups[1].Value, NumberStyles.HexNumber), 1));
            }
        }
        /// <summary>
        /// Преобразовать строку в JSON обект
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object Json(this string t)
        {
            lock(_lock)
            return xNet.Json.JsonDecode(t);
        }

        /// <summary>
        /// Из HEX в строку
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static string HexStrToStr(this string hexStr)
        {
            lock (_lock)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hexStr.Length; i += 2)
                {
                    int n = Convert.ToInt32(hexStr.Substring(i, 2), 16);
                    sb.Append(Convert.ToChar(n));
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// Строку в HEX
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrToHexStr(this string str)
        {
            lock (_lock)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    int v = Convert.ToInt32(str[i]);
                    sb.Append(string.Format("{0:X2}", v));
                }
                return sb.ToString();
            }
        }
        //-----   Для инициализацыи ананимного типа 
        public static IEnumerable<T> Ananymous<T>(this T item)
       {
          yield break;
       }
        /// <summary>
        /// Сгенерировать случайный пароль
        /// </summary>
        /// <param name="x">длина пароля по умолчанию 15</param>
        /// <returns></returns>
        public static string GenPass(int x = 15)
        {
            lock (_lock)
            {
                string pass = "";
                var r = new Random();
                while (pass.Length < x)
                {
                    Char c = (char)r.Next(33, 125);
                    if (Char.IsLetterOrDigit(c))
                        pass += c;
                }
                return pass;
            }
        }
        private static char[] char_0 = new char[] { ';', '&' };
        /// <summary>
        ///Получить текущее время в формате Unix 
        /// </summary>
        /// <returns></returns>
        public static long GetUnitTime(int day = 0)
        {
            lock (_lock)
            {
                Int64 retval = 0;
                var st = new DateTime(1970, 1, 1);
                var days = DateTime.Now.AddDays(day) - DateTime.Now ;
                TimeSpan t = (DateTime.Now.ToUniversalTime() - days - st);
                retval = (Int64)(t.TotalMilliseconds + 0.5);
                return retval;
            }
        }




        /// <summary>
        /// Декодирование Html
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string s)
        {
            lock (_lock)
            {
                if (s == null)
                {
                    return null;
                }
                if (s.IndexOf('&') < 0)
                {
                    return s;
                }
                StringBuilder sb = new StringBuilder();
                StringWriter output = new StringWriter(sb);
                HtmlDecode(s, output);
                return sb.ToString();
            }
        }
        private static void HtmlDecode(string s, TextWriter output)
        {
            lock (_lock)
            {
                if (s == null)
                {
                    return;
                }
                if (s.IndexOf('&') < 0)
                {
                    output.Write(s);
                    return;
                }
                int length = s.Length;
                int num2 = 0;
            Label_0022:
                if (num2 >= length)
                {
                    return;
                }
                char ch = s[num2];
                if (ch != '&')
                {
                    goto Label_0109;
                }
                int num3 = s.IndexOfAny(char_0, num2 + 1);
                if ((num3 <= 0) || (s[num3] != ';'))
                {
                    goto Label_0109;
                }
                string str = s.Substring(num2 + 1, (num3 - num2) - 1);
                if ((str.Length > 1) && (str[0] == '#'))
                {
                    try
                    {
                        if ((str[1] != 'x') && (str[1] != 'X'))
                        {
                            ch = (char)int.Parse(str.Substring(1));
                        }
                        else
                        {
                            ch = (char)int.Parse(str.Substring(2), NumberStyles.AllowHexSpecifier);
                        }
                        num2 = num3;
                    }
                    catch (FormatException)
                    {
                        num2++;
                    }
                    catch (ArgumentException)
                    {
                        num2++;
                    }
                    goto Label_0109;
                }
                num2 = num3;
                char ch2 = Class24.smethod_0(str);
                if (ch2 != '\0')
                {
                    ch = ch2;
                    goto Label_0109;
                }
                output.Write('&');
                output.Write(str);
                output.Write(';');
            Label_0100:
                num2++;
                goto Label_0022;
            Label_0109:
                output.Write(ch);
                goto Label_0100;
            }
        }
        /// <summary>
        /// Кодирование HTML
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string s)
        {
            lock (_lock)
            {
                if (s == null)
                {
                    return null;
                }
                int num = smethod_13(s, 0);
                if (num == -1)
                {
                    return s;
                }
                StringBuilder builder = new StringBuilder(s.Length + 5);
                int length = s.Length;
                int num3 = 0;
                goto Label_00F5;
            Label_00CE:
                num3 = num + 1;
                if (num3 >= length)
                {
                    goto Label_010A;
                }
                num = smethod_13(s, num3);
                if (num == -1)
                {
                    builder.Append(s, num3, length - num3);
                    goto Label_010A;
                }
            Label_00F5:
                if (num > num3)
                {
                    builder.Append(s, num3, num - num3);
                }
                char ch = s[num];
                if (ch > '>')
                {
                    builder.Append("&#");
                    builder.Append(((int)ch).ToString(NumberFormatInfo.InvariantInfo));
                    builder.Append(';');
                }
                else
                {
                    switch (ch)
                    {
                        case '<':
                            builder.Append("&lt;");
                            goto Label_00CE;

                        case '=':
                            goto Label_00CE;

                        case '>':
                            builder.Append("&gt;");
                            goto Label_00CE;

                        case '&':
                            builder.Append("&amp;");
                            goto Label_00CE;

                        case '"':
                            builder.Append("&quot;");
                            goto Label_00CE;
                    }
                }
                goto Label_00CE;
            Label_010A:
                return builder.ToString();
            }
        }

        private static byte[] smethod_0(byte[] byte_0, int int_0, int int_1, bool bool_0)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < int_1; i++)
            {
                char ch = (char) byte_0[int_0 + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!smethod_9(ch))
                {
                    num2++;
                }
            }
            if ((!bool_0 && (num == 0)) && (num2 == 0))
            {
                return byte_0;
            }
            byte[] buffer = new byte[int_1 + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < int_1; j++)
            {
                byte num6 = byte_0[int_0 + j];
                char ch2 = (char) num6;
                if (smethod_9(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte) smethod_7((num6 >> 4) & 15);
                    buffer[num4++] = (byte) smethod_7(num6 & 15);
                }
            }
            return buffer;
        }
        private static byte[] smethod_1(byte[] byte_0, int int_0, int int_1, bool bool_0)
        {
            int num = 0;
            for (int i = 0; i < int_1; i++)
            {
                if (smethod_8(byte_0[int_0 + i]))
                {
                    num++;
                }
            }
            if (!bool_0 && (num == 0))
            {
                return byte_0;
            }
            byte[] buffer = new byte[int_1 + (num * 2)];
            int num3 = 0;
            for (int j = 0; j < int_1; j++)
            {
                byte num5 = byte_0[int_0 + j];
                if (smethod_8(num5))
                {
                    buffer[num3++] = 0x25;
                    buffer[num3++] = (byte) smethod_7((num5 >> 4) & 15);
                    buffer[num3++] = (byte) smethod_7(num5 & 15);
                }
                else
                {
                    buffer[num3++] = num5;
                }
            }
            return buffer;
        }
        private static byte[] smethod_10(byte[] byte_0)
        {
            if (byte_0 == null)
            {
                return null;
            }
            return smethod_12(byte_0, 0, byte_0.Length);
        }
        private static byte[] smethod_11(string string_0, Encoding encoding_0)
        {
            if (string_0 == null)
            {
                return null;
            }
            byte[] bytes = encoding_0.GetBytes(string_0);
            return smethod_0(bytes, 0, bytes.Length, false);
        }
        private static byte[] smethod_12(byte[] byte_0, int int_0, int int_1)
        {
            if ((byte_0 == null) && (int_1 == 0))
            {
                return null;
            }
            if (byte_0 == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((int_0 < 0) || (int_0 > byte_0.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((int_1 < 0) || ((int_0 + int_1) > byte_0.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return smethod_0(byte_0, int_0, int_1, true);
        }
        private static int smethod_13(string string_0, int int_0)
        {
            int num = string_0.Length - int_0;
            int num2 = int_0;
            while (num > 0)
            {
                char ch = string_0[num2];
                if (ch <= '>')
                {
                    switch (ch)
                    {
                        case '<':
                        case '>':
                        case '&':
                        case '"':
                            return (string_0.Length - num);

                        case '=':
                            goto Label_004D;
                    }
                }
                else if ((ch >= '\x00a0') && (ch < 'Ā'))
                {
                    return (string_0.Length - num);
                }
            Label_004D:
                num2++;
                num--;
            }
            return -1;
        }
        private static string smethod_2(string string_0, Encoding encoding_0)
        {
            if (string.IsNullOrEmpty(string_0))
            {
                return string_0;
            }
            if (encoding_0 == null)
            {
                encoding_0 = Encoding.UTF8;
            }
            byte[] bytes = encoding_0.GetBytes(string_0);
            bytes = smethod_1(bytes, 0, bytes.Length, false);
            return Encoding.ASCII.GetString(bytes);
        }
        private static string smethod_3(string string_0)
        {
            if ((string_0 != null) && (string_0.IndexOf(' ') >= 0))
            {
                string_0 = string_0.Replace(" ", "%20");
            }
            return string_0;
        }
        private static string smethod_4(byte[] byte_0, int int_0, int int_1, Encoding encoding_0)
        {
            Class23 class2 = new Class23(int_1, encoding_0);
            for (int i = 0; i < int_1; i++)
            {
                int index = int_0 + i;
                byte num3 = byte_0[index];
                if (num3 == 0x2b)
                {
                    num3 = 0x20;
                }
                else if ((num3 == 0x25) && (i < (int_1 - 2)))
                {
                    if ((byte_0[index + 1] == 0x75) && (i < (int_1 - 5)))
                    {
                        int num4 = smethod_6((char) byte_0[index + 2]);
                        int num5 = smethod_6((char) byte_0[index + 3]);
                        int num6 = smethod_6((char) byte_0[index + 4]);
                        int num7 = smethod_6((char) byte_0[index + 5]);
                        if (((num4 < 0) || (num5 < 0)) || ((num6 < 0) || (num7 < 0)))
                        {
                            goto Label_00DA;
                        }
                        char ch = (char) ((((num4 << 12) | (num5 << 8)) | (num6 << 4)) | num7);
                        i += 5;
                        class2.method_1(ch);
                        continue;
                    }
                    int num8 = smethod_6((char) byte_0[index + 1]);
                    int num9 = smethod_6((char) byte_0[index + 2]);
                    if ((num8 >= 0) && (num9 >= 0))
                    {
                        num3 = (byte) ((num8 << 4) | num9);
                        i += 2;
                    }
                }
            Label_00DA:
                class2.method_0(num3);
            }
            return class2.method_3();
        }
        private static string smethod_5(string string_0, Encoding encoding_0)
        {
            int length = string_0.Length;
            Class23 class2 = new Class23(length, encoding_0);
            for (int i = 0; i < length; i++)
            {
                char ch = string_0[i];
                if (ch == '+')
                {
                    ch = ' ';
                }
                else if ((ch == '%') && (i < (length - 2)))
                {
                    if ((string_0[i + 1] == 'u') && (i < (length - 5)))
                    {
                        int num3 = smethod_6(string_0[i + 2]);
                        int num4 = smethod_6(string_0[i + 3]);
                        int num5 = smethod_6(string_0[i + 4]);
                        int num6 = smethod_6(string_0[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                        {
                            goto Label_0106;
                        }
                        ch = (char) ((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        class2.method_1(ch);
                        continue;
                    }
                    int num7 = smethod_6(string_0[i + 1]);
                    int num8 = smethod_6(string_0[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0))
                    {
                        byte num9 = (byte) ((num7 << 4) | num8);
                        i += 2;
                        class2.method_0(num9);
                        continue;
                    }
                }
            Label_0106:
                if ((ch & 0xff80) == 0)
                {
                    class2.method_0((byte) ch);
                }
                else
                {
                    class2.method_1(ch);
                }
            }
            return class2.method_3();
        }
        private static int smethod_6(char char_1)
        {
            if ((char_1 >= '0') && (char_1 <= '9'))
            {
                return (char_1 - '0');
            }
            if ((char_1 >= 'a') && (char_1 <= 'f'))
            {
                return ((char_1 - 'a') + 10);
            }
            if ((char_1 >= 'A') && (char_1 <= 'F'))
            {
                return ((char_1 - 'A') + 10);
            }
            return -1;
        }
        private static char smethod_7(int int_0)
        {
            if (int_0 <= 9)
            {
                return (char) (int_0 + 0x30);
            }
            return (char) ((int_0 - 10) + 0x61);
        }
        private static bool smethod_8(byte byte_0)
        {
            if (byte_0 < 0x7f)
            {
                return (byte_0 < 0x20);
            }
            return true;
        }
        private static bool smethod_9(char char_1)
        {
            if ((((char_1 >= 'a') && (char_1 <= 'z')) || ((char_1 >= 'A') && (char_1 <= 'Z'))) || ((char_1 >= '0') && (char_1 <= '9')))
            {
                return true;
            }
            switch (char_1)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }
        private static string StripTags(string source, bool ReplaceWithSpaces = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            char[] chArray = new char[source.Length];
            int index = 0;
            bool flag = false;
            for (int i = 0; i < source.Length; i++)
            {
                char ch = source[i];
                switch (ch)
                {
                    case '<':
                        flag = true;
                        break;

                    case '>':
                        flag = false;
                        chArray[index] = ' ';
                        index++;
                        break;

                    default:
                        if (!flag)
                        {
                            chArray[index] = ch;
                            index++;
                        }
                        break;
                }
            }
            return new string(chArray, 0, index);
        }
        public static string UrlDecode(this string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlDecode(str, Encoding.UTF8);
        }
        public static string UrlDecode(byte[] bytes, Encoding e)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlDecode(bytes, 0, bytes.Length, e);
        }
        public static string UrlDecode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return smethod_5(str, e);
        }
        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
        {
            if ((bytes == null) && (count == 0))
            {
                return null;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return smethod_4(bytes, offset, count, e);
        }
        public static string UrlEncode(this string str)
        {
            lock (_lock)
            {
                if (str == null)
                {
                    return null;
                }
                return UrlEncode(str, Encoding.UTF8);
            }
        }
        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(smethod_10(bytes));
        }
        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(smethod_11(str, e));
        }
        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(smethod_12(bytes, offset, count));
        }
        public static string UrlPathEncode(string str)
        {
            lock (_lock)
            {
                if (str == null)
                {
                    return null;
                }
                int index = str.IndexOf('?');
                if (index >= 0)
                {
                    return (UrlPathEncode(str.Substring(0, index)) + str.Substring(index));
                }
                return smethod_3(smethod_2(str, Encoding.UTF8));
            }
        }
        private class Class23
        {
            private byte[] byte_0;
            private char[] char_0;
            private Encoding encoding_0;
            private int int_0;
            private int int_1;
            private int int_2;

            internal Class23(int int_3, Encoding encoding_1)
            {
                this.int_0 = int_3;
                this.encoding_0 = encoding_1;
                this.char_0 = new char[int_3];
            }

            internal void method_0(byte byte_1)
            {
                if (this.byte_0 == null)
                {
                    this.byte_0 = new byte[this.int_0];
                }
                this.byte_0[this.int_1++] = byte_1;
            }

            internal void method_1(char char_1)
            {
                if (this.int_1 > 0)
                {
                    this.method_2();
                }
                this.char_0[this.int_2++] = char_1;
            }

            private void method_2()
            {
                if (this.int_1 > 0)
                {
                    this.int_2 += this.encoding_0.GetChars(this.byte_0, 0, this.int_1, this.char_0, this.int_2);
                    this.int_1 = 0;
                }
            }

            internal string method_3()
            {
                if (this.int_1 > 0)
                {
                    this.method_2();
                }
                if (this.int_2 > 0)
                {
                    return new string(this.char_0, 0, this.int_2);
                }
                return string.Empty;
            }
        }
        private static class Class24
        {
            private static Hashtable hashtable_0;
            private static object object_0 = new object();
            private static string[] string_0 = new string[] { 
                "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo", 
                "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo", 
                "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml", 
                "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc", 
                "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml", 
                "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc", 
                "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma", "Δ-Delta", 
                "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau", "Υ-Upsilon", 
                "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda", "μ-mu", 
                "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih", "ϖ-piv", 
                " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo", "†-dagger", 
                "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade", "ℵ-alefsym", 
                "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty", "∇-nabla", 
                "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup", "∫-int", 
                "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes", "⊥-perp", 
                "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
             };

            internal static char smethod_0(string string_1)
            {
                if (hashtable_0 == null)
                {
                    lock (object_0)
                    {
                        if (hashtable_0 == null)
                        {
                            Hashtable hashtable = new Hashtable();
                            foreach (string str in string_0)
                            {
                                hashtable[str.Substring(2)] = str[0];
                            }
                            hashtable_0 = hashtable;
                        }
                    }
                }
                object obj2 = hashtable_0[string_1];
                if (obj2 != null)
                {
                    return (char) obj2;
                }
                return '\0';
            }
        }
    }
}

