﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace xNet
{
    /// <summary>
    /// Представляет генератор псевдослучайных чисел и строк.
    /// </summary>
    /// <remarks>Данный класс является потокобезопасным.</remarks>
    public static class Rand
    {
        private static readonly Random _rand = new Random();


        #region Статические методы (открытые)

        /// <summary>
        /// Возвращает неотрицательное случайное число.
        /// </summary>
        /// <returns>Неотрицательное случайное число.</returns>
        public static int Next()
        {
            lock (_rand)
            {
                return _rand.Next();
            }
        }

        public static long LongRandom(long min, long max)
        {
            lock (_rand)
            {
                long result = _rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
                result = (result << 32);
                result = result | (long)_rand.Next((Int32)min, (Int32)max);
                return result;
            }
        }

        public static string GenPass(int x = 15)
        {
            string pass = "";
            while (pass.Length < x)
            {
                Char c;
                lock (_rand)
                    c = (char)_rand.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }
        //******** генератор имен
        private static string[] digraphs = new string[] { "en", "re", "er", "nt", "th", "on", "in", "te", "an", "or", "st", "ed", "ne", "ve", "es", "nd", "to", "se", "at", "ti", "ar", "ee", "rt", "as", "co", "io", "ty", "fo", "fi", "ra", "et", "le", "ou", "ma", "tw", "ea", "is", "si", "de", "hi", "al", "ce", "da", "ec", "rs", "ur", "ni", "ri", "el", "la", "ro", "ta" };
        public static string GenName()
        {
            string potentialName;
            do
            {
                potentialName = "";
                lock(_rand)
                for (int digraphCount = 0; digraphCount < _rand.Next(2, 5); digraphCount++)
                    potentialName += digraphs[_rand.Next(0, digraphs.GetUpperBound(0))];
            }
            while (invalidNameRegex.IsMatch(potentialName));
            return potentialName.Substring(0, 1).ToUpper() + potentialName.Substring(1);
        }
        private static Regex invalidNameRegex = new Regex(@"(?:([aiuy])\1)|(?:(\w\w)\2)|([^aeiouy]{3,})|([aeiouy]{3,})|(\wy\w)|(^nd)|(^nt)|(^rt)|(^rs)|(^ht)|([aiou]$)|(tw$)");
        //*************************

        /// <summary>
        /// Возвращает неотрицательное случайное число, которое меньше заданого максимального значения.
        /// </summary>
        /// <param name="maxValue">Исключенный верхний предел возвращаемого случайного числа. Значение параметра должно быть больше, либо равно нулю.</param>
        /// <returns>Неотрицательное случайное число.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра <paramref name="maxValue"/> меньше 0.</exception>
        public static int Next(int maxValue)
        {
            lock (_rand)
            {
                return _rand.Next(maxValue);
            }
        }

        /// <summary>
        /// Возвращает случайное число в указанном диапазоне.
        /// </summary>
        /// <param name="minValue">Включенный нижний предел возвращаемого случайного числа. </param>
        /// <param name="maxValue">Исключенный верхний предел возвращаемого случайного числа. Значение параметра должно быть больше, либо равно значению параметра <paramref name="minValue"/>.</param>
        /// <returns>Неотрицательное случайное число.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра <paramref name="minValue"/> больше значения параметра <paramref name="maxValue"/>.</exception>
        public static int Next(int minValue, int maxValue)
        {
            lock (_rand)
            {
                return _rand.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Заполняет элементы указанного массива байтов случайными числами.
        /// </summary>
        /// <param name="buffer">Массив байтов, который будет заполнен случайными числами.</param>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="buffer"/> равно <see langword="null"/>.</exception>
        public static void NextBytes(byte[] buffer)
        {
            #region Проверка параметров

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            #endregion

            lock (_rand)
            {
                _rand.NextBytes(buffer);
            }
        }

        /// <summary>
        /// Возвращает случайное число в диапазоне от 0,0 до 1,0.
        /// </summary>
        /// <returns>Число двойной точности с плавающей запятой, которое больше или равно 0,0, и меньше 1,0.</returns>
        public static double NextDouble()
        {
            lock (_rand)
            {
                return _rand.NextDouble();
            }
        }



        /// <summary>
        /// Возвращает случайную строку, которая формируется с помощью специальных конструкций.
        /// </summary>
        /// <param name="sourceStr">Исходная строка, которая содержит специальные конструкции.</param>
        /// <returns>Случайная строка, сформированная с помощью специальных конструкций.</returns>
        /// <remarks>Специальная конструкция имеет вид - {значение1|значение2|значение3|..n}, где значения разделяются с помощью символа '|'. Когда встречается подобная конструкция, то из неё выбирается случайное значение и подставляется за место конструкции.</remarks>
        public static string NextString(string sourceStr)
        {
            if (string.IsNullOrEmpty(sourceStr))
            {
                return string.Empty;
            }

            int begPosition = -1;
            var separatorsPos = new List<int>();
            var strBuilder = new StringBuilder(sourceStr);

            // Регулярные выражения? Не, не слышал.
            for (int i = 0; i < strBuilder.Length; ++i)
            {
                if (strBuilder[i] == '{')
                {
                    begPosition = i;
                }
                else if (strBuilder[i] == '}' && begPosition != -1)
                {
                    for (int j = begPosition + 1; j < i; ++j)
                    {
                        if (strBuilder[j] == '|')
                        {
                            separatorsPos.Add(j);
                        }
                    }

                    int offset = strBuilder.Length;
                    int randValueIndex = Next(0, separatorsPos.Count + 1);

                    if (separatorsPos.Count == 0)
                    {
                        // Удаляем скобку после x: {x} = {x
                        strBuilder.Remove(i, 1);
                        // Удаляем скобку перед x: {x = x
                        strBuilder.Remove(begPosition, 1);
                    }
                    else if (randValueIndex == 0)
                    {
                        // Удаляем всё, что идёт после x: {x|1|2|..n} = {x
                        strBuilder.Remove(separatorsPos[0], i - separatorsPos[0] + 1);
                        // Удаляем скобку перед x: {x = x
                        strBuilder.Remove(begPosition, 1);
                    }
                    else if (randValueIndex == separatorsPos.Count)
                    {
                        // Удаляем скобку после x: {1|2|..n|x} = {1|2|..n|x
                        strBuilder.Remove(i, 1);
                        // Удаляем всё, что идёт перед x: {1|2|..n|x = x
                        strBuilder.Remove(begPosition, separatorsPos[randValueIndex - 1] - begPosition + 1);
                    }
                    else
                    {
                        // Удаляем всё, что идёт после x: {..n|x|..n} = {..n|x
                        strBuilder.Remove(separatorsPos[randValueIndex], i - separatorsPos[randValueIndex] + 1);
                        // Удаляем всё, что идёт перед x: {..n|x = x
                        strBuilder.Remove(begPosition, separatorsPos[randValueIndex - 1] - begPosition + 1);
                    }

                    begPosition = -1;
                    separatorsPos.Clear();

                    offset -= strBuilder.Length;
                    i -= offset;
                }
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Возвращает случайный элемент из последовательности. Если последовательность пуста, то <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TSource">Тип элементов последовательности <paramref name="source"/>.</typeparam>
        /// <param name="source">Последовательность, из которой будет возвращён случайный элемент.</param>
        /// <returns>Случайный элемент из последовательности.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="source"/> равно <see langword="null"/>.</exception>
        public static TSource NextElement<TSource>(IEnumerable<TSource> source)
        {
            #region Проверка параметров

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            #endregion

            int count = source.Count();

            if (count == 0)
            {
                return default(TSource);
            }

            lock (_rand)
            {
                return source.ElementAt(_rand.Next(count));
            }
        }

        /// <summary>
        /// Возвращает <see langword="true"/>, либо <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/>, либо <see langword="false"/>.</returns>
        public static bool IsTrue()
        {
            lock (_rand)
            {
                return (_rand.NextDouble() < 0.5);
            }
        }

        #endregion
    }
}