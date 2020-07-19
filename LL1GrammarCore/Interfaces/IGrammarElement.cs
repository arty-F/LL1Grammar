namespace LL1GrammarCore.Interfaces
{
    /// <summary>
    /// Представляет собой один элемент правой части правила грамматики. Содержит либо ссылку на не терминал, либо строковой терминал.
    /// </summary>
    internal interface IGrammarElement
    {
        /// <summary>
        /// Тип элемента.
        /// </summary>
        ElementType Type { get; }
        /// <summary>
        /// Ссылка на не терминал.
        /// </summary>
        IGrammarRule Rule { get; }
        /// <summary>
        /// Строковой терминал.
        /// </summary>
        string Characters { get; }
    }
}
