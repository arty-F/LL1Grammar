using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Обертка для работы с грамматикой. Содержит список правил, специальные символы грамматики и метод для валидации строки согласно хранящимся правилам.
    /// </summary>
    public class Grammar
    {
        /// <summary>
        /// Список правил грамматики.
        /// </summary>
        internal List<GrammarRule> Rules { get; set; } = new List<GrammarRule>();
        private SpecialSymbols specialSymbols;

        /// <summary>
        /// Создать новый экземпляр грамматики.
        /// </summary>
        /// <param name="grammar">Строка с описанием грамматики.</param>
        /// <param name="specialSymbols">Специальные символы грамматики.</param>
        public Grammar(string grammar, SpecialSymbols specialSymbols, ActionsContainer actions)
        {
            this.specialSymbols = specialSymbols;

            foreach (var line in grammar.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                Rules.Add(new GrammarRule(line, this.specialSymbols));

            foreach (var rule in Rules)
                rule.BuildRightPart(Rules, actions);
        }

        /// <summary>
        /// Проверка входной строки на принадлежность к данной грамматике.
        /// </summary>
        public bool Validate(string str)
        {
            return new Parser(str, Rules.First()).Parse();
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (var rule in Rules)
            {
                buffer.Append(rule.Left + specialSymbols.Splitter);

                bool isFirstPart = true;
                foreach (var rulePart in rule.Right)
                {
                    if (isFirstPart)
                        isFirstPart = false;
                    else
                        buffer.Append(specialSymbols.Or);

                    foreach (var elem in rulePart.Elements)
                    {
                        if (elem.Type == ElementType.Empty)
                            buffer.Append(specialSymbols.ChainEmpty);
                        else if (elem.Type == ElementType.Terminal)
                            buffer.Append(elem.Characters);
                        else if (elem.Type == ElementType.NonTerminal)
                            buffer.Append(elem.Rule.Left);
                        else if (elem.Type == ElementType.Range)
                            buffer.Append(elem.Characters.First().ToString() + specialSymbols.Range.ToString() + elem.Characters.Last().ToString());
                    }
                }
                buffer.Append(Environment.NewLine);
            }

            return buffer.ToString();
        }
    }
}
