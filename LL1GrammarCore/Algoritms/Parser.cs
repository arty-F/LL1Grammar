using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    internal class Parser
    {
        private string data;
        private ActionsContainer actions;
        private Stack<GrammarElement> stack;

        internal Parser(string data, GrammarRule startedRule, ActionsContainer actions)
        {
            this.data = data;
            this.actions = actions;
            stack = new Stack<GrammarElement>();
            stack.Push(new GrammarElement(startedRule));
        }

        /// <summary>
        /// Разбор входной строки, используя стартовое правило грамматики.
        /// </summary>
        internal bool Parse()
        {
            StringBuilder remainingData = new StringBuilder(data);

            while (remainingData.Length > 0)
            {
                if (stack.Count == 0)
                    throw new Exception($"преждевременно");

                NextElementCheck(stack.Pop(), remainingData);
            }

            if (stack.Count > 0)
                throw new Exception($"преждевременно");

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
                    UnpackNonTerminal(element, compareData.ToString());
                    break;

                case ElementType.Terminal:
                    if (element.Characters == compareData.ToString().Substring(0, element.Characters.Length))
                        compareData.Remove(0, element.Characters.Length);
                    else
                        throw new Exception($"невозможно");
                    break;

                case ElementType.Range:
                    if (element.Characters.Contains(compareData.ToString().Substring(0, 1)))
                        compareData.Remove(0, 1);
                    else
                        throw new Exception($"невозможно");
                    break;

                case ElementType.Empty:
                    break;

                default:
                    throw new Exception("неизвестный тип");
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
                        throw new Exception($"не лл1 пустые цпочки конфликт");

                if (HasMatch(rulePart.Elements.First(), compareData))
                    if (comparePath == null)
                        comparePath = rulePart;
                    else
                        throw new Exception($"не лл1 пустые цпочки конфликт");
            }

            //Приоритет 1: Если может быть пустая цепочка, нужно проверить следующий элемент на стеке на совпадение
            if (emptyPath != null && stack.Count > 0 && HasMatch(stack.Peek(), compareData))
                ToStack(emptyPath);
            //Приоритет 2: Если есть путь с символьным совпадением
            else if (comparePath != null)
                ToStack(comparePath);
            //Приоритет 3: Если дальнейший разбор возможен только по пустой цепочке
            else if (emptyPath != null)
                ToStack(emptyPath);
            //Приоритет 4: Если не найдено путей, то дальнейший разбор невозможен
            else
                throw new Exception($"дальнейший разбор невозможен");
        }

        /// <summary>
        /// Добавляет все элементы подправила на стек.
        /// </summary>
        private void ToStack(GrammarRulePart rulePart)
        {
            var elements = rulePart.Elements;
            elements.Reverse();                 //Необходимо перевернуть коллекцию для соблюдения очередности
            elements.ForEach(e => stack.Push(e));
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

            if (element.Type == ElementType.Terminal && element.Characters == compareData.Substring(0, element.Characters.Length))
                return true;
            else if (element.Type == ElementType.Range && element.Characters.Contains(compareData.Substring(0, 1)))
                return true;
            else if (element.Type == ElementType.NonTerminal)
                foreach (var rulePart in element.Rule.Right)
                        result = result | HasMatch(rulePart.Elements.First(), compareData);

            return result;
        }
    }
}
