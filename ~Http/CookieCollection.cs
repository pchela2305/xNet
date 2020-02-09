using System;
using System.Collections.Generic;
using System.Text;

namespace xNet
{
    /// <summary>
    /// Представляет коллекцию HTTP-куки.
    /// </summary>
    public class CookieCollection : Dictionary<string, string>
    {
        /// <summary>
        /// Возвращает или задает значение, указывающие, закрыты ли куки для редактирования
        /// </summary>
        /// <value>Значение по умолчанию — <see langword="false"/>.</value>
        public bool IsLocked { get; set; }


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CookieCollection"/>.
        /// </summary>
        /// <param name="isLocked">Указывает, закрыты ли куки для редактирования.</param>
        public CookieCollection(bool isLocked = false) : base(StringComparer.OrdinalIgnoreCase)
        {
            IsLocked = isLocked;
        }


        /// <summary>
        /// Возвращает строку, состоящую из имён и значений куки.
        /// </summary>
        /// <returns>Строка, состоящая из имён и значений куки.</returns>
        override public string ToString()
        {
            var strBuilder = new StringBuilder();        

            foreach (var cookie in this)
            {
                strBuilder.AppendFormat("{0}={1}; ", cookie.Key, cookie.Value);
            }

            if (strBuilder.Length > 0)
            {
                strBuilder.Remove(strBuilder.Length - 2, 2);
            }

            return strBuilder.ToString();
        }
    }
}