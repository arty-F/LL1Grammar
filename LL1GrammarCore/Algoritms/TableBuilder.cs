using System.Collections.Generic;
using System.Linq;
using System;

namespace LL1GrammarCore
{
    /// <summary>
    /// Строитель таблицы разбора.
    /// </summary>
    internal class TableBuilder
    {
        /// <summary>
        /// Построить таблицу разбора по стартовому элементу грамматики.
        /// </summary>
        /// <param name="startedElement">Стартовый элемент грамматики.</param>
        internal Dictionary<GrammarElement, Dictionary<string, GrammarRulePart>> Build(GrammarElement startedElement)
        {
            Dictionary<GrammarElement, Dictionary<string, GrammarRulePart>> table = new Dictionary<GrammarElement, Dictionary<string, GrammarRulePart>>();

            List<GrammarElement> nonterminals = new List<GrammarElement> { startedElement };
            FindNonterminal(startedElement, nonterminals);

            foreach (var nonterm in nonterminals)
            {
                Dictionary<string, GrammarRulePart> map = FindUnfoldedWays(nonterm);
                table.Add(nonterm, map);
            }

            return table;
        }

        /// <summary>
        /// Рекурсивный метод, находящий коллекцию нетерминалов грамматики. Все найденные результаты добавляются в коллекцию nonterminals.
        /// </summary>
        /// <param name="element">Текущий рассматриваемый элемент.</param>
        /// <param name="nonterminals">Коллекция нетерминалов.</param>
        private void FindNonterminal(GrammarElement element, List<GrammarElement> nonterminals)
        {
            foreach (var rulePart in element.Rule.Right)
                foreach (var elem in rulePart.Elements)
                    if (elem.Type == ElementType.NonTerminal && !nonterminals.Contains(elem))
                    {
                        nonterminals.Add(elem);
                        FindNonterminal(elem, nonterminals);
                    }
        }

        /// <summary>
        /// Найти все варианты раскрытия переданного параметром нетерминала. Результат возвращается в
        /// виде словаря где TKey - направляющие символы, TValue - часть правила грамматики в которою
        /// раскроется нетерминал по направляющим символам TKey.
        /// </summary>
        private Dictionary<string, GrammarRulePart> FindUnfoldedWays(GrammarElement nonterm)
        {
            Dictionary<string, GrammarRulePart> result = new Dictionary<string, GrammarRulePart>();

            foreach (var rulePart in nonterm.Rule.Right)
            {
                List<string> guideChars = new List<string>();
                GetGuideChars(rulePart.Elements.First(), guideChars);

                foreach (var str in guideChars)
                {
                    if (result.ContainsKey(str))
                        throw new Exception("Ne LL1");

                    result.Add(str, rulePart);
                }
            }
            return result;
        }

        /// <summary>
        /// Рекурсивно определяет все направляющие символы, которые может генерировать переданный элемент грамматики.
        /// Найденные результаты сохраняются в коллекцию guideChars.
        /// </summary>
        private void GetGuideChars(GrammarElement element, List<string> guideChars)
        {
            switch (element.Type)
            {
                case ElementType.NonTerminal:
                    foreach (var rulePart in element.Rule.Right)
                        GetGuideChars(rulePart.Elements.First(), guideChars);
                    break;

                case ElementType.Terminal:
                    if (!guideChars.Contains(element.Characters))
                        guideChars.Add(element.Characters);
                    break;

                case ElementType.Range:
                    foreach (var c in element.Characters)
                        if (!guideChars.Contains(c.ToString()))
                            guideChars.Add(c.ToString());
                    break;

                case ElementType.Empty:
                    if (!guideChars.Contains(""))
                        guideChars.Add("");
                    break;

                default:
                    break;
            }
        }
    }
}
