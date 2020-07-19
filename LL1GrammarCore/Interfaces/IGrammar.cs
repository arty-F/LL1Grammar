namespace LL1GrammarCore.Interfaces
{
    /// <summary>
    /// Грамматика. Содержит список правил.
    /// </summary>
    public interface IGrammar
    {
        /// <summary>
        /// Проверка входной строки на соответствие правилам грамматики.
        /// </summary>
        /// <param name="data">Строка с данными для проверки.</param>
        bool ValidateData(string data);
    }
}
