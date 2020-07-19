using System.Collections.Generic;

namespace LL1GrammarCore.Interfaces
{
    /// <summary>
    /// Одно правило грамматики.
    /// </summary>
    internal interface IGrammarRule
    {
        /// <summary>
        /// Левая часть правила.
        /// </summary>
        string Left { get; set; }
        /// <summary>
        /// Правая часть правила. Представляет собой коллекцию элементов реализующих интерфейс <see cref="IGrammarRulePart"/>.
        /// </summary>
        List<IGrammarRulePart> Right { get; set; }

        /// <summary>
        /// Заполнение правой части правила. Для этого необходим список правил с известными нетерминалами и специальные символы грамматики.
        /// </summary>
        /// <param name="rules">Список правил с известными нетерминалами.</param>
        /// <param name="specialSymbols">Специальные символы грамматики.</param>
        void BuildRightPart(IEnumerable<IGrammarRule> rules, ISpecialSymbols specialSymbols);

        /// <summary>
        /// Возврщает правило или альтернативу правила из которого можно вывести начальную часть передаваемой строки.
        /// </summary>
        /// <param name="remainingData">Оставшаяся часть анализируемой строки.</param>
        IGrammarRulePart GetNext(string remainingData);
    }
}
