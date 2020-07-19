using System.Collections.Generic;

namespace LL1GrammarCore.Interfaces
{
    /// <summary>
    /// Одна из возможных частей правой части правила. Является единственной, или разделена символом "или".
    /// </summary>
    internal interface IGrammarRulePart
    {
        /// <summary>
        /// Коллекция элементов, содержащихся в данной части правила.
        /// </summary>
        List<IGrammarElement> Elements { get; set; }
    }
}
