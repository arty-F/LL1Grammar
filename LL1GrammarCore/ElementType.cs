namespace LL1GrammarCore
{
    /// <summary>
    /// Перечисление типов элемента граматики.
    /// </summary>
    internal enum ElementType : byte
    {
        NonTerminal,
        Terminal,
        Range,
        Empty
    }
}
