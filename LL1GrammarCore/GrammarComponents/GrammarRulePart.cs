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
        internal List<GrammarElement> Elements { get; set; } = new List<GrammarElement>();


        internal GrammarRulePart(string part, SpecialSymbols specialSymbols, List<GrammarRule> rules)
        {
            List<(int index, GrammarElement element)> elementsByIndex = new List<(int index, GrammarElement element)>();
            StringBuilder sb = new StringBuilder(part);

            //Находим нетерминалы
            foreach (var rule in rules)
            {
                var index = sb.ToString().IndexOf(rule.Left);
                if (index != -1)
                {
                    int length = rule.Left.Length;
                    sb.Replace
                    elementsByIndex.Add((index, new GrammarElement(rule, GetActions(sb, index + length, specialSymbols.Or))));
                }
            }


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
                        sb.Remove(findStartedIndex, a.Key.Length);
                        findStartedIndex += a.Key.Length;
                    }
                }
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }





        ///// <summary>
        ///// Коллекция элементов, содержащихся в данной части правила.
        ///// </summary>
        //public List<GrammarElement> Elements { get; set; } = new List<GrammarElement>();

        ///// <summary>
        ///// Создать экземпляр одной из возможных частей правой части правила.
        ///// </summary>
        ///// <param name="grammarPart">Строковое представление.</param>
        ///// <param name="rules">Список правил с известными нетерминалами.</param>
        ///// <param name="specialSymbols">Специальные символы.</param>
        //public GrammarRulePart(string grammarPart, IEnumerable<GrammarRule> rules, SpecialSymbols specialSymbols)
        //{
        //    /*Алгоритм разметки элементов будет сводиться к:
        //       - Нахождению подстрок, отвечающих некоторым требованиям.
        //       - Добавлению найденной подстроки в коллекцию найденных элементов.
        //       - Удалением подстроки из начальной строки.

        //    Т.к. необходимо сохранить порядок элементов, количество элементов в коллекции найденных элементов должно быть равно количеству символов
        //    в анализируемой строке. Найденный элемент будет копироваться в коллекцию по индексу, соответствующему идексу в строке этого элемента.
        //    В случае, когда найденные элементы не будут представлять собой один символ, в коллекции будут возникать пустые элементы, что конечно не
        //    является проблемой. Для сохранения порядка элементов, при удалении элементов из анализируемой строки необходимо за место каждого удаленного 
        //    символова добавлять безопасный символ, который в дальнейшем не будет проанализирован как часть грамматики, а просто будет занимать позицию в строке.*/

        //    //Пустая цепочка
        //    if (grammarPart.Contains(specialSymbols.ChainEmpty))
        //    {
        //        if (grammarPart.Length > 1)
        //            throw new System.Exception($"Пустая цепочкая не может содержать каких-либо других символов." +
        //                                       $"{Environment.NewLine}Исключение вызвало: {grammarPart + Environment.NewLine}");

        //        Elements.Add(new GrammarElement());
        //    }
        //    else //Цепочка не пуста
        //    {
        //        /*Содержит строку для анализа. Найденные элементы будут заменены на <SpecialSymbols.Or>, т.к. данный символ(строка) является единственным 
        //        гарантированно безопасным символом в данной ситуации, который можно использовать для обозначения элементов, которые уже были проверены.*/
        //        StringBuilder partBuilder = new StringBuilder(grammarPart);

        //        //Массив(коллекция) размеченных элементов
        //        GrammarElement[] markuped = new GrammarElement[grammarPart.Length];

        //        //Находим все терминалы-диапазоны (a-z, 0-9 и т.д.)
        //        int index;
        //        while ((index = partBuilder.ToString().IndexOf(specialSymbols.Range)) != -1)
        //        {
        //            if (index == 0 || index == (grammarPart.Length - 1))
        //                throw new System.Exception($"Символ диапазона не может стоять в начале или в конце правила или части правила." +
        //                                           $"{Environment.NewLine}Исключение вызвало: {grammarPart + Environment.NewLine}");

        //            //Добавляем найденный диапазон в коллекцию
        //            markuped[index] = new GrammarElement(grammarPart.ElementAt(index - 1), grammarPart.ElementAt(index + 1));

        //            //В строке для анализа заменяем найденные элементы на спец символы
        //            partBuilder.Remove(index - 1, 3);
        //            partBuilder.Insert(index - 1, string.Join(string.Empty, Enumerable.Repeat(specialSymbols.Or, 3)));
        //        }

        //        //Находим нетерминалы
        //        foreach (var rule in rules)
        //        {
        //            while ((index = partBuilder.ToString().IndexOf(rule.Left)) != -1)
        //            {
        //                markuped[index] = new GrammarElement(rule);

        //                //В строке для анализа заменяем найденные элементы на спец символы
        //                partBuilder.Remove(index, rule.Left.Length);
        //                partBuilder.Insert(index, string.Join(string.Empty, Enumerable.Repeat(specialSymbols.Or, rule.Left.Length)));
        //            }
        //        }

        //        //Добавляем терминалы
        //        StringBuilder buffer = new StringBuilder();
        //        //Все что осталось в строке, является обычными терминалами, проходим строку посимвольно
        //        for (int i = 0; i < partBuilder.Length; i++)
        //        {
        //            if (partBuilder[i] == specialSymbols.Or)
        //            {
        //                if (buffer.Length > 0)
        //                {
        //                    markuped[i - 1] = new GrammarElement(buffer.ToString());
        //                    buffer.Clear();
        //                }
        //            }
        //            else
        //                buffer.Append(partBuilder[i]);
        //        }

        //        //Если что-то осталось в буфере (последним элементов строке был терминал) закидываем в размечнную коллекцию
        //        if (buffer.Length != 0)
        //            markuped[markuped.Count() - 1] = new GrammarElement(buffer.ToString());

        //        //Перемещаем все найденные элементы в коллекцию элементов данного правила, игнорируя пустые
        //        foreach (var elem in markuped)
        //        {
        //            if (elem != null)
        //                Elements.Add(elem);
        //        }
        //    }
        //}

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
