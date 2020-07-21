using System;

namespace LL1GrammarCore
{
    /// <summary>
    /// Обертка для специальных символов грамматики.
    /// </summary>
    public class SpecialSymbols
    {
        /// <summary>
        /// Строка-разделитель, между левой и правой частью грамматики.
        /// </summary>
        public string Splitter { get; set; }
        /// <summary>
        /// Символ пустой цепочки.
        /// </summary>
        public char ChainEmpty { get; set; }
        /// <summary>
        /// Символ знака "или" для правой части правила.
        /// </summary>
        public char Or { get; set; }
        /// <summary>
        /// Символ диапазона значений.
        /// </summary>
        public char Range { get; set; }

        /// <summary>
        /// Создать новый экземпляр класса, представляющего набор специальных символов грамматики.
        /// </summary>
        /// <param name="splitter">Символ-разделитель.</param>
        /// <param name="chainEmpty">Символ пустой цепочки.</param>
        /// <param name="or">Символ альтернативного варианта правила.</param>
        /// <param name="range">Символ диапазона значений.</param>
        public SpecialSymbols(string splitter = "->", char chainEmpty = '$', char or = '|', char range = '-')
        {
            if (splitter == chainEmpty.ToString() || splitter == or.ToString() || splitter == range.ToString() || chainEmpty == or || chainEmpty == range || or == range)
                throw new Exception("Специальные символы не должны повторяться.");

            Splitter = splitter;
            ChainEmpty = chainEmpty;
            Or = or;
            Range = range;
        }
    }
}
