using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1GrammarCore
{
    /// <summary>
    /// Одно правило грамматики. Состоит из правой и левой части.
    /// </summary>
    internal class GrammarRule
    {
        /// <summary>
        /// Левая часть правила.
        /// </summary>
        internal string Left { get; set; }
        /// <summary>
        /// Правая часть правила. Представляет собой коллекцию элементов реализующих интерфейс <see cref="IGrammarRulePart"/>.
        /// </summary>
        internal List<GrammarRulePart> Right { get; set; } = new List<GrammarRulePart>();

        private List<string> tmpRight = new List<string>();     //необработанная превая часть
        private SpecialSymbols specialSymbols;

        internal GrammarRule(string line, SpecialSymbols specialSymbols)
        {
            this.specialSymbols = specialSymbols;

            var splitted = line.Split(new string[] { specialSymbols.Splitter }, StringSplitOptions.None);
            if (splitted.Length != 2)
                throw new Exception($"Правило {line} не соответствует синтаксису правила грамматики.");

            Left = splitted.First();

            foreach (var part in splitted.Last().Split(new string[] { specialSymbols.Splitter }, StringSplitOptions.None))
                tmpRight.Add(part);
        }

        internal void BuildRightPart(List<GrammarRule> rules)
        {
            foreach (var part in tmpRight)
                Right.Add(new GrammarRulePart(part, specialSymbols, rules));
        }



        //private string tmpRight; //Правая часть грамматики в необработанном виде

        ///// <summary>
        ///// Создать экземпляр одного правила грамматики. Необходима строка с описанием грамматики, а также
        ///// класс реализующий <see cref="ISpecialSymbols"/>, содержащий специальные символы грамматики. При этом
        ///// в каждом правиле заполняется только левая часть правила (нетерминалы). После того как будет известен
        ///// полный список нетерминалов необходимо использовать метод BuldRightPart, для заполнения правых частей.
        ///// </summary>
        ///// <param name="line">Строка с описанием одного правила грамматики.</param>
        ///// <param name="specialSymbols">Специальные символы грамматики.</param>
        //public GrammarRule(string line, SpecialSymbols specialSymbols)
        //{
        //    if (line == null || line == "")
        //        throw new Exception($"Неверный синтаксис грамматики. Пустая строка.{Environment.NewLine}");

        //    //Разбиваем грамматику на правую и левую часть
        //    var splitted = line.Split(new string[] { specialSymbols.Splitter }, StringSplitOptions.None);

        //    if (splitted.Count() != 2)
        //        throw new Exception($"Каждая строка-правило должна содержать один символ-разделитель." +
        //                            $"{Environment.NewLine}Исключение вызвало: {line + Environment.NewLine}");

        //    Left = splitted.First();
        //    tmpRight = splitted.Last();
        //}

        ///// <summary>
        ///// Заполнение правой части правила. Для этого необходим список правил с известными нетерминалами и специальные символы грамматики.
        ///// </summary>
        ///// <param name="rules">Список правил с известными нетерминалами.</param>
        ///// <param name="specialSymbols">Специальные символы грамматики.</param>
        //public void BuildRightPart(IEnumerable<GrammarRule> rules, SpecialSymbols specialSymbols)
        //{
        //    //Разбиваем правую часть по символу "или"
        //    var splitedByOr = tmpRight.Split(specialSymbols.Or);

        //    foreach (var part in splitedByOr)
        //        Right.Add(new GrammarRulePart(part, rules, specialSymbols));
        //}

        ////Попытка записи результата, в случае если результат уже найден генерируется исключение
        //private void SetResult(ref GrammarRulePart result, GrammarRulePart rulePart)
        //{
        //    if (result != null)
        //        throw new Exception($"Грамматика не является LL(1) грамматикой. Неопределенность между правилами:{Environment.NewLine}" +
        //                            $"< {result.ToString()} > и < {rulePart.ToString()} >{Environment.NewLine}");
        //    else
        //        result = rulePart;
        //}

        ///// <summary>
        ///// Возврщает правило или альтернативу правила из которого можно вывести начальную часть передаваемой строки.
        ///// </summary>
        ///// <param name="remainingData">Оставшаяся часть анализируемой строки.</param>
        //public GrammarRulePart GetNext(string remainingData)
        //{
        //    GrammarRulePart result = null;
        //    GrammarRulePart empty = null;

        //    //Проверка первых элементов каждого подправила на совпадение с remainingData
        //    foreach (var rulePart in Right)
        //    {
        //        var firstElem = rulePart.Elements.First();

        //        switch (firstElem.Type)
        //        {
        //            //Если терминал, то рекурсивно просматриваем все подъэлементы на совпадаение
        //            case ElementType.NonTerminal:
        //                var findedInNonTerm = firstElem.Rule.GetNext(remainingData);
        //                if (findedInNonTerm != null)
        //                    if (findedInNonTerm.Elements.First().Type != ElementType.Empty)
        //                        SetResult(ref result, rulePart);
        //                    else
        //                        SetResult(ref empty, rulePart);
        //                break;

        //            //Если терминал то проверяем на полное совпадение
        //            case ElementType.Terminal:
        //                if (firstElem.Characters.Length <= remainingData.Length && firstElem.Characters == remainingData.Substring(0, firstElem.Characters.Length))
        //                    SetResult(ref result, rulePart);
        //                break;

        //            //Если диапазон, то проверяем первый символ на совпадение с символами диапазона
        //            case ElementType.Range:
        //                if (remainingData.Length > 0 && firstElem.Characters.Contains(remainingData.First()))
        //                    SetResult(ref result, rulePart);
        //                break;

        //            //Если пустая цепочка то сохраняем отдельно в empty
        //            case ElementType.Empty:
        //                SetResult(ref empty, rulePart);
        //                break;

        //            default:
        //                break;
        //        }
        //    }

        //    if (result == null & empty != null) //Если не нашлось совпадений, идем по пустой цепочке
        //        result = empty;

        //    return result;
        //}

        public override string ToString()
        {
            return Left;
        }

        public override bool Equals(object obj)
        {
            GrammarRule rule = (GrammarRule)obj;
            return Left == rule.Left;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Left.GetHashCode();
            if (Right != null)
                hash = hash * 23 + Right.Count();

            return hash;
        }
    }
}
