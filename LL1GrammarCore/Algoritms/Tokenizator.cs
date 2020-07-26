using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{ 
    /// <summary>
    /// Данный класс извлекает элементы грамматики из строки.
    /// </summary>
    internal class Tokenizator
    {
        List<GrammarRule> rules;
        SpecialSymbols specialSymbols;
        List<(int index, GrammarElement element)> elementsByIndex;
        ActionsContainer actions;
        StringBuilder sb;
        string part;

        /// <summary>
        /// Создать новый экземпляр токенизатора.
        /// </summary>
        internal Tokenizator(string part, List<GrammarRule> rules, SpecialSymbols specialSymbols, ActionsContainer actions)
        {
            this.part = part;
            this.rules = rules;
            this.specialSymbols = specialSymbols;
            this.actions = actions;
            elementsByIndex = new List<(int index, GrammarElement element)>();
            sb = new StringBuilder(part);
        }

        /// <summary>
        /// Возвращает список элементов грамматики в том же порядке, в котором они находились в строке.
        /// <para>Алгоритм:</para>
        /// В общем случае поиск элементов сводится к тому, чтобы сопоставить часть строки некому шаблону и
        /// в случае соответствия заменить совпавшую часть строки на специальный символ (place holder), который
        /// впоследствии не будет считыватья, но будет занимать свое место, для того, чтобы не нарушать индексацию
        /// оставшихся символов в строке.
        /// </summary>
        internal List<GrammarElement> GetTokens()
        {
            TakeNoNTerminals();
            TakeEmptyChains();
            TakeRangeTerminals();
            TakeTerminals();

            return new List<GrammarElement>(elementsByIndex.OrderBy(e => e.index).Select(e => e.element));
        }

        /// <summary>
        /// Находит все нетерминалы по общему списку правил.
        /// </summary>
        private void TakeNoNTerminals()
        {
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
        }

        /// <summary>
        /// Находит пустую цепочку.
        /// </summary>
        private void TakeEmptyChains()
        {
            int index = sb.ToString().IndexOf(specialSymbols.ChainEmpty);
            while (index != -1)
            {
                if (sb.Length > specialSymbols.ChainEmpty.ToString().Length)
                    throw new Exception($"В правиле {part} пустая цепочка содержит недопустимые элементы.");
                
                sb[index] = specialSymbols.Or;
                elementsByIndex.Add((index, new GrammarElement(GetActions(sb, index + 1, specialSymbols.Or))));
                index = sb.ToString().IndexOf(specialSymbols.ChainEmpty);
            }
        }

        /// <summary>
        /// Находит терминалы-диапазоны по символу specialSymbols.Range.
        /// </summary>
        private void TakeRangeTerminals()
        {
            int index = sb.ToString().IndexOf(specialSymbols.Range);
            while (index != -1)
            {
                if (index == 0 || index == sb.Length || sb[index - 1] == specialSymbols.Or || sb[index + 1] == specialSymbols.Or)
                    throw new Exception($"Диапазон значений в правиле {part} задан неверно.");

                char startChar = sb[index - 1];
                char endChar = sb[index + 1];
                HoldPlaces(sb, index - 1, index + 1, specialSymbols.Or);

                elementsByIndex.Add((index, new GrammarElement(startChar, endChar, GetActions(sb, index + 2, specialSymbols.Or))));
                index = sb.ToString().IndexOf(specialSymbols.Range);
            }
        }

        /// <summary>
        /// Получает терминалы из оставшейся части правила.
        /// </summary>
        private void TakeTerminals()
        {
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
                        elementsByIndex.Add((i - 1, new GrammarElement(buffer.ToString())));
                        buffer.Clear();
                    }
                }
            }
            if (buffer.Length > 0)
                elementsByIndex.Add((sb.Length, new GrammarElement(buffer.ToString())));
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

                foreach (var a in actions.Actions)
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
    }
}
