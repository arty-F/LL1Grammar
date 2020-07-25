using System.Collections.Generic;
using System.Linq;

namespace LL1GrammarCore
{
    /// <summary>
    /// Одна из возможных частей правой части правила. Является единственной, или разделена символом "или".
    /// </summary>
    internal class GrammarRulePart
    {
        /// <summary>
        /// Список элементов даного подправила.
        /// </summary>
        internal List<GrammarElement> Elements { get; set; }

        /// <summary>
        /// Создать новый экземпляр подправила грамматики.
        /// </summary>
        /// <param name="part">Часть правила.</param>
        /// <param name="specialSymbols">Специальные символы.</param>
        /// <param name="rules">Список правил, необходим для поиска нетерминалов.</param>
        internal GrammarRulePart(string part, SpecialSymbols specialSymbols, List<GrammarRule> rules)
        {
            Elements = new Tokenizator(part, rules, specialSymbols).GetTokens();
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
