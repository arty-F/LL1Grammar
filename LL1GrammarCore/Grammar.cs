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
        private List<GrammarRule> rules { get; set; } = new List<GrammarRule>();  //Список правил грамматики
        private SpecialSymbols specialSymbols;

        public Grammar(string grammar, SpecialSymbols specialSymbols)
        {
            this.specialSymbols = specialSymbols;
            var splittedGrammar = grammar.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in splittedGrammar)
                rules.Add(new GrammarRule(line, this.specialSymbols));

            foreach (var rule in rules)
                rule.BuildRightPart(rules);
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (var rule in rules)
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
