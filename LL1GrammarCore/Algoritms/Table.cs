using System;
using System.Collections.Generic;

namespace LL1GrammarCore
{
    /// <summary>
    /// Таблица разбора грамматики.
    /// </summary>
    internal class Table
    {
        Dictionary<GrammarElement, Dictionary<string, GrammarRulePart>> table;

        /// <summary>
        /// Создать новый экземпляр таблицы разбора. Для создания необходим словарь где ключем являются
        /// элементы грамматики, а значением словарь из направляющих символов и часть правила грамматики.
        /// Таким образом имеется конструкция Dictionary<TMainKey, Dictionary<TSecondaryKey, TValue>>, где:
        /// TMainKey - элемент грамматики из которого осуществляется переход (элемент на котором 
        /// находится читывающая головка).
        /// TSecondaryKey - строка, соответствующая направляющим символам.
        /// TValue - часть правила грамматики, в которую раскроется TMainKey, по TSecondaryKey символам.
        /// </summary>
        public Table(Dictionary<GrammarElement, Dictionary<string, GrammarRulePart>> table)
        {
            this.table = table;
        }
        
        /// <summary>
        /// Развернуть элемент грамматики по направлющим символам.
        /// </summary>
        /// <param name="startingRule">Элемент грамматики, подлежащий развертыванию.</param>
        /// <param name="guideChars">Направляющие символы.</param>
        internal GrammarRulePart Unfold(GrammarElement startingRule, string guideChars)
        {
            if (!table.ContainsKey(startingRule))
                throw new Exception("Строка не пренадлежит грамматике.");

            GrammarRulePart result = null;

            foreach (var key in table[startingRule].Keys)
            {
                if (key == "" && result == null)
                    result = table[startingRule][key];
                else if (key.Length <= guideChars.Length && key == guideChars.Substring(0, key.Length))
                {
                    result = table[startingRule][key];
                    break;
                }
            }

            if (result == null)
                throw new Exception("Строка не пренадлежит грамматике.");

            return result;
        }
    }
}
