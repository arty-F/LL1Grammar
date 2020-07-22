using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1GrammarCore
{
    /// <summary>
    /// Одно правило грамматики. Состоит из правой и левой части.
    /// </summary>
    internal class GrammarRule
    {
        /// <summary>
        /// Левая часть правила.
        /// </summary>
        internal string Left { get; set; }
        /// <summary>
        /// Правая часть правила. Представляет собой коллекцию элементов реализующих интерфейс <see cref="IGrammarRulePart"/>.
        /// </summary>
        internal List<GrammarRulePart> Right { get; set; } = new List<GrammarRulePart>();

        private List<string> tmpRight = new List<string>();     //необработанная превая часть
        private SpecialSymbols specialSymbols;

        internal GrammarRule(string line, SpecialSymbols specialSymbols)
        {
            this.specialSymbols = specialSymbols;

            var splitted = line.Split(new string[] { specialSymbols.Splitter }, StringSplitOptions.None);
            if (splitted.Length != 2)
                throw new Exception($"Правило {line} не соответствует синтаксису правила грамматики.");

            Left = splitted.First();

            foreach (var part in splitted.Last().Split(new char[] { specialSymbols.Or }, StringSplitOptions.None))
                tmpRight.Add(part);
        }

        internal void BuildRightPart(List<GrammarRule> rules)
        {
            foreach (var part in tmpRight)
                Right.Add(new GrammarRulePart(part, specialSymbols, rules));
        }

        public override string ToString()
        {
            return Left;
        }

        public override bool Equals(object obj)
        {
            GrammarRule rule = (GrammarRule)obj;
            return Left == rule.Left;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Left.GetHashCode();
            if (Right != null)
                hash = hash * 23 + Right.Count();

            return hash;
        }
    }
}
