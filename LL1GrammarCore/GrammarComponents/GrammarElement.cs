using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1GrammarCore
{
    /// <summary>
    /// Представляет собой один элемент правой части правила грамматики. Содержит либо ссылку на не терминал, либо строковой терминал.
    /// </summary>
    internal class GrammarElement
    {
        /// <summary>
        /// Тип элемента.
        /// </summary>
        public ElementType Type { get; private set; }
        /// <summary>
        /// Ссылка на не терминал.
        /// </summary>
        public GrammarRule Rule { get; private set; }
        /// <summary>
        /// Строковой терминал.
        /// </summary>
        public string Characters { get; private set; }
        /// <summary>
        /// Действия.
        /// </summary>
        public List<Action<object>> Actions { get; private set; } = new List<Action<object>>();

        /// <summary>
        /// Создать экземпляр элемента правила, который является пустой цепочкой.
        /// </summary>
        public GrammarElement(List<Action<object>> actions = null)
        {
            Type = ElementType.Empty;
            CheckAndReadActions(actions);
        }

        /// <summary>
        /// Создать экземпляр элемента правила, который является ссылкой на другое правило-нетерминал.
        /// </summary>
        public GrammarElement(GrammarRule rule, List<Action<object>> actions = null)
        {
            Type = ElementType.NonTerminal;
            Rule = rule;
            CheckAndReadActions(actions);
        }

        /// <summary>
        /// Создать экземпляр элемента правила, который является терминалом.
        /// </summary>
        public GrammarElement(string characters, List<Action<object>> actions = null)
        {
            Type = ElementType.Terminal;
            
            switch (characters)
            {
                case "\\t":
                    Characters = ((char)9).ToString(); //Символ табуляции
                    break;

                case "\\n":
                    Characters = Environment.NewLine;
                    break;

                default:
                    Characters = characters;
                    break;
            }
            CheckAndReadActions(actions);
        }

        /// <summary>
        /// Создать экземпляр элемента правила, который является диапазоном значений.
        /// </summary>
        /// <param name="startChar">Начальный символ диапазона.</param>
        /// <param name="endChar">Конечный символ диапазона.</param>
        public GrammarElement(char startChar, char endChar, List<Action<object>> actions = null)
        {
            if (startChar > endChar)
                throw new System.Exception($"Неверный порядок символов в диапазоне.{Environment.NewLine}" +
                                           $"Исключение вызвало: {startChar}-{endChar}{Environment.NewLine}");

            Type = ElementType.Range;

            //Получаем диапазон
            Characters = string.Join(string.Empty, Enumerable.Range(startChar, endChar - startChar + 1).Select(c => (char)c));

            CheckAndReadActions(actions);
        }

        private void CheckAndReadActions(List<Action<object>> actions)
        {
            if (actions != null)
                Actions = actions;
        }

        public override bool Equals(object obj)
        {
            GrammarElement elem = (GrammarElement)obj;
            return Type == elem.Type && Characters == elem.Characters && Rule == elem.Rule;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ElementType.Empty:
                    return "Пустая цепочка";

                case ElementType.NonTerminal:
                    return Rule.ToString();

                case ElementType.Range:
                    return $"Диапазон от {Characters.First()} до {Characters.Last()}";

                case ElementType.Terminal:
                    return Characters;

                default:
                    throw new Exception("Неизвестный тип элемента.");
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Type.GetHashCode();
            if (Rule != null)
                hash = hash * 23 + Rule.GetHashCode();
            if (Characters != null)
                hash = hash * 23 + Characters.GetHashCode();

            return hash;
        }
    }
}
