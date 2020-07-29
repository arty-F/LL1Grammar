using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Данный класс выполняет парсинг входной строки в соответствии с заданной грамматикой.
    /// </summary>
    internal class Parser
    {
        private string data;
        private Stack<GrammarElement> stack;

        /// <summary>
        /// Создать новый экземпляр парсера в основе работы которого лежит метод рекурсивного спуска.
        /// </summary>
        /// <param name="data">Строка принадлежность к граматике которой, необходимо определить.</param>
        /// <param name="startedRule">Стартовое правило грамматики.</param>
        internal Parser(string data, GrammarRule startedRule)
        {
            this.data = data;
            stack = new Stack<GrammarElement>();
            stack.Push(new GrammarElement(startedRule));
        }

        /// <summary>
        /// Разбор входной строки, используя стартовое правило грамматики.
        /// </summary>
        internal bool Parse()
        {
            StringBuilder remainingData = new StringBuilder(data);

            while (stack.Count > 0)
                NextElementCheck(stack.Pop(), remainingData);  
            
            if (remainingData.Length > 0)
                throw new Exception($"Данная строка не принадлежит граматике. Оставшаяся часть строки:{Environment.NewLine + remainingData.ToString()}");

            return true;
        }

        /// <summary>
        /// Проверка следующего элемента на стеке. Если элемент терминал, то выполняется проверка на символьное
        /// совпадение с входной строкой. Если элемент нетерминал, то выполняется распаковка (открытие) элемента.
        /// </summary>
        private void NextElementCheck(GrammarElement element, StringBuilder compareData)
        {
            switch (element.Type)
            {
                case ElementType.NonTerminal:
                    element.Actions.ForEach(a => a.Invoke(null));
                    UnpackNonTerminal(element, compareData.ToString());
                    break;

                case ElementType.Terminal:
                    if (element.Characters.Length <= compareData.Length && element.Characters == compareData.ToString().Substring(0, element.Characters.Length))
                    {
                        element.Actions.ForEach(a => a.Invoke(compareData.ToString().Substring(0, element.Characters.Length)));    
                        compareData.Remove(0, element.Characters.Length);
                    }
                    else
                        throw new Exception($"Данная строка не принадлежит граматике. Оставшаяся строка: {compareData.ToString() + Environment.NewLine}" +
                            $"Считан элемент со стека: {element + Environment.NewLine}Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}");
                    break;

                case ElementType.Range:
                    if (compareData.Length > 0 && element.Characters.Contains(compareData.ToString().Substring(0, 1)))
                    {
                        element.Actions.ForEach(a => a.Invoke(compareData.ToString().Substring(0, 1)));
                        compareData.Remove(0, 1);
                    }
                    else
                        throw new Exception($"Данная строка не принадлежит граматике. Оставшаяся строка: {compareData.ToString() + Environment.NewLine}" +
                            $"Считан элемент со стека: {element + Environment.NewLine}Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}");
                    break;

                case ElementType.Empty:
                    element.Actions.ForEach(a => a.Invoke(null));
                    break;

                default:
                    throw new Exception("Неизвестный тип элемента.");
            }
        }

        /// <summary>
        /// Открывает нетерминал, проверяя возможные символьные совпадения, использую рекурсивный спуск.
        /// </summary>
        private void UnpackNonTerminal(GrammarElement element, string compareData)
        {
            GrammarRulePart comparePath = null;
            GrammarRulePart emptyPath = null;

            //Проверка каждого первого элемент подправил на совпадение и возможность генерации пустых цепочек
            foreach (var rulePart in element.Rule.Right)
            {
                if (CanBeEmpty(rulePart.Elements.First()))
                    if (emptyPath == null)
                        emptyPath = rulePart;
                    else
                        throw new Exception($"Грамматика не является LL(1). Неопределенность выбора между пустыми цепочками.Оставшаяся строка: " +
                            $"{compareData.ToString() + Environment.NewLine}Считан элемент со стека: {element + Environment.NewLine}" +
                            $"Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}");

                if (HasMatch(rulePart.Elements.First(), compareData))
                    if (comparePath == null)
                        comparePath = rulePart;
                    else
                        throw new Exception($"Грамматика не является LL(1). Неопределенность выбора между правилами {comparePath} и {rulePart}." +
                            $"Оставшаяся строка: {compareData.ToString() + Environment.NewLine}Считан элемент со стека: " +
                            $"{element + Environment.NewLine}Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}");
            }

            //Приоритет 1: Если есть путь с символьным совпадением
            if (comparePath != null)
                ToStack(comparePath.Elements);
            //Приоритет 2: Если дальнейший разбор возможен только по пустой цепочке
            else if (emptyPath != null)
                ToStack(emptyPath.Elements);
            //Приоритет 3: Если не найдено путей, то дальнейший разбор невозможен
            else
                throw new Exception($"Дальнейший разбор невозможен. Оставшаяся строка: { compareData.ToString() + Environment.NewLine }Считан элемент со стека: " +
                            $"{element + Environment.NewLine}Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}");
        }

        /// <summary>
        /// Добавляет все элементы подправила на стек.
        /// </summary>
        private void ToStack(List<GrammarElement> elements)
        {
            var elems = new List<GrammarElement>(elements);
            elems.Reverse(); 
            elems.ForEach(e => stack.Push(e));
        }

        /// <summary>
        /// Определяет, с помощью рекурсивного спуска, может ли данный элемент генирировать пустую цепочку.
        /// </summary>
        private bool CanBeEmpty(GrammarElement element)
        {
            bool result = false;

            if (element.Type == ElementType.Empty)
                return true;
            else if (element.Type == ElementType.NonTerminal)
                foreach (var rulePart in element.Rule.Right)
                    result = result | CanBeEmpty(rulePart.Elements.First());

            return result;
        }

        /// <summary>
        /// Определяет, с помощью рекурсивного спуска, имеет ли данный элемент посимвольное совпадение с входной строкой.
        /// </summary>
        private bool HasMatch(GrammarElement element, string compareData)
        {
            bool result = false;

            if (element.Type == ElementType.Terminal && element.Characters.Length <= compareData.Length && element.Characters == compareData.Substring(0, element.Characters.Length))
                return true;
            else if (element.Type == ElementType.Range && compareData.Length > 0 && element.Characters.Contains(compareData.Substring(0, 1)))
                return true;
            else if (element.Type == ElementType.NonTerminal)
                foreach (var rulePart in element.Rule.Right)
                    result = result | HasMatch(rulePart.Elements.First(), compareData);

            return result;
        }
    }
}
