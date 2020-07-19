namespace LL1GrammarCore.Interfaces
{
    /// <summary>
    /// Обертка для специальных символов грамматики.
    /// </summary>
    public interface ISpecialSymbols
    {
        /// <summary>
        /// Строка-разделитель, между левой и правой частью грамматики.
        /// </summary>
        string Splitter { get; set; }
        /// <summary>
        /// Символ пустой цепочки.
        /// </summary>
        char ChainEmpty { get; set; }
        /// <summary>
        /// Символ знака "или" для правой части правила.
        /// </summary>
        char Or { get; set; }
        /// <summary>
        /// Символ диапазона значений.
        /// </summary>
        char Range { get; set; }
    }
}
