using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Одна из возможных частей правой части правила. Является единственной, или разделена символом "или".
    /// </summary>
    internal class GrammarRulePart
    {
        internal List<GrammarElement> Elements { get; set; }


        internal GrammarRulePart(string part, SpecialSymbols specialSymbols, List<GrammarRule> rules)
        {
            List<(int index, GrammarElement element)> elementsByIndex = new List<(int index, GrammarElement element)>();
            StringBuilder sb = new StringBuilder(part);

            //Находим нетерминалы
            foreach (var rule in rules)
            {
                var index = sb.ToString().IndexOf(rule.Left);
                while (index != -1)
                {
                    int length = rule.Left.Length;
                    HoldPlaces(sb, index, length, specialSymbols.Or);
                    elementsByIndex.Add((index, new GrammarElement(rule, GetActions(sb, index + length, specialSymbols.Or))));
                    index = sb.ToString().IndexOf(rule.Left);
                }
            }

            //Находим пустые цепочки
            while (sb.ToString().IndexOf(specialSymbols.ChainEmpty) != -1)
            {
                int i = sb.ToString().IndexOf(specialSymbols.ChainEmpty);
                sb[i] = specialSymbols.Or;
                elementsByIndex.Add((i, new GrammarElement(GetActions(sb, i + 1, specialSymbols.Or))));
            }

            //Находим терминалы - диапазоны
            while (sb.ToString().IndexOf(specialSymbols.Range) != -1)
            {
                int i = sb.ToString().IndexOf(specialSymbols.Range);
                if (i == 0 || i == sb.Length || sb[i - 1] == specialSymbols.Or || sb[i + 1] == specialSymbols.Or)
                    throw new Exception($"Диапазон значений в правиле {part} задан неверно.");

                char startChar = sb[i - 1];
                char endChar = sb[i + 1];
                HoldPlaces(sb, i - 1, i + 1, specialSymbols.Or);

                elementsByIndex.Add((i, new GrammarElement(startChar, endChar, GetActions(sb, i + 2, specialSymbols.Or))));
            }

            //Находим терминалы
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] != specialSymbols.Or)
                {
                    buffer.Append(sb[i]);
                    var a = GetActions(sb, i + 1, specialSymbols.Or);
                    if (a != null)
                    {
                        elementsByIndex.Add((i, new GrammarElement(buffer.ToString(), a)));
                        buffer.Clear();
                    }
                }
                else
                {
                    if (buffer.Length > 0)
                    {
                        elementsByIndex.Add((i, new GrammarElement(buffer.ToString())));
                        buffer.Clear();
                    }
                }
            }
            if (buffer.Length > 0)
                elementsByIndex.Add((sb.Length, new GrammarElement(buffer.ToString())));

            //Извлекаем полученную коллекцию
            Elements = new List<GrammarElement>(elementsByIndex.OrderBy(e => e.index).Select(e => e.element));
        }

        /// <summary>
        /// Заменяет указанные позиции символов в StringBuilder'e на placeHolder'ы.
        /// </summary>
        private void HoldPlaces(StringBuilder sb, int startIndex, int length, char placeHolder)
        {
            for (int i = startIndex; i < startIndex + length; i++)
                sb[i] = placeHolder;
        }

        /// <summary>
        /// Возвращает список действий прикрепленных к элементу.
        /// </summary>
        private List<Action<object>> GetActions(StringBuilder sb, int findStartedIndex, char placeHolder)
        {
            bool anyFinded = true;
            List<Action<object>> result = new List<Action<object>>();

            while (anyFinded)
            {
                anyFinded = false;

                foreach (var a in ActionsContainer.Actions)
                {
                    if (sb.Length >= findStartedIndex + a.Key.Length && sb.ToString().Substring(findStartedIndex, a.Key.Length) == a.Key)
                    {
                        anyFinded = true;
                        result.Add(a.Value);
                        HoldPlaces(sb, findStartedIndex, a.Key.Length, placeHolder);
                        findStartedIndex += a.Key.Length;
                    }
                }
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }

        public override string ToString()
        {
            return string.Concat(Elements.Select(e => e.ToString()));
        }

        public override int GetHashCode()
        {
            int hash = 17;

            foreach (var element in Elements)
                hash = hash * 23 + element.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            List<GrammarElement> elem = (List<GrammarElement>)obj;
            return elem == Elements;
        }
    }
}
