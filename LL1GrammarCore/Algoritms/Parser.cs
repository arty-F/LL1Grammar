using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Данный класс выполняет парсинг входной строки в соответствии с заданной грамматикой.
    /// </summary>
    internal class Parser
    {
        private string data;
        private Stack<GrammarElement> stack;

        private int lineCounter = 1;
        private int charCounter = 1;

        /// <summary>
        /// Создать новый экземпляр парсера в основе работы которого лежит метод рекурсивного спуска.
        /// </summary>
        /// <param name="data">Строка принадлежность к граматике которой, необходимо определить.</param>
        /// <param name="startedRule">Стартовое правило грамматики.</param>
        internal Parser(string data, GrammarRule startedRule)
        {
            this.data = data;
            stack = new Stack<GrammarElement>();
            stack.Push(new GrammarElement(startedRule));
        }

        /// <summary>
        /// Разбор входной строки, используя стартовое правило грамматики.
        /// </summary>
        internal bool Parse()
        {
            StringBuilder remainingData = new StringBuilder(data);

            while (stack.Count > 0)
                NextElementCheck(stack.Pop(), remainingData);  
            
            if (remainingData.Length > 0)
                throw new Exception($"Данная строка не принадлежит граматике. Строка №:{lineCounter}, символ №:{charCounter}." +
                    $"Стек пуст, оставшаяся часть строки:{Environment.NewLine + remainingData.ToString()}");

            return true;
        }

        /// <summary>
        /// Проверка следующего элемента на стеке. Если элемент терминал, то выполняется проверка на символьное
        /// совпадение с входной строкой. Если элемент нетерминал, то выполняется распаковка (открытие) элемента.
        /// </summary>
        private void NextElementCheck(GrammarElement element, StringBuilder compareData)
        {
            switch (element.Type)
            {
                case ElementType.NonTerminal:
                    ExecuteActions(element, compareData.ToString(), null);
                    UnpackNonTerminal(element, compareData.ToString());
                    break;

                case ElementType.Terminal:
                    if (element.Characters.Length <= compareData.Length && element.Characters == compareData.ToString().Substring(0, element.Characters.Length))
                    {
                        var data = compareData.ToString().Substring(0, element.Characters.Length);
                        RefreshCounters(data);
                        ExecuteActions(element, compareData.ToString(), data);
                        compareData.Remove(0, element.Characters.Length);
                    }
                    else
                        throw new Exception(GetExceptionMsg("Данная строка не принадлежит граматике.", element, compareData.ToString()));
                    break;

                case ElementType.Range:
                    if (compareData.Length > 0 && element.Characters.Contains(compareData.ToString().Substring(0, 1)))
                    {
                        var data = compareData.ToString().Substring(0, 1);
                        RefreshCounters(data);
                        ExecuteActions(element, compareData.ToString(), data);
                        compareData.Remove(0, 1);
                    }
                    else
                        throw new Exception(GetExceptionMsg("Данная строка не принадлежит граматике.", element, compareData.ToString()));
                    break;

                case ElementType.Empty:
                    ExecuteActions(element, compareData.ToString(), null);
                    break;

                default:
                    throw new Exception("Неизвестный тип элемента.");
            }
        }

        /// <summary>
        /// Вызов прикрепленных к элементу грамматики действий.
        /// </summary>
        /// <param name="element">Элемент.</param>
        /// <param name="compareData">Оставшаяся строка.</param>
        /// <param name="parameters">Параметры действия.</param>
        private void ExecuteActions(GrammarElement element, string compareData, object parameters)
        {
            try
            {
                element.Actions.ForEach(a => a.Invoke(parameters));
            }
            catch (Exception ex)
            {
                throw new Exception(GetExceptionMsg(ex.Message, element, compareData.ToString()));
            }
        }

        /// <summary>
        /// Открывает нетерминал, проверяя возможные символьные совпадения, использую рекурсивный спуск.
        /// </summary>
        private void UnpackNonTerminal(GrammarElement element, string compareData)
        {
            GrammarRulePart comparePath = null;
            List<GrammarRulePart> emptyPathes = new List<GrammarRulePart>();

            //Проверка каждого первого элемент подправил на совпадение и возможность генерации пустых цепочек
            foreach (var rulePart in element.Rule.Right)
            {
                if (CanBeEmpty(rulePart.Elements.First()))
                        emptyPathes.Add(rulePart);

                if (HasMatch(rulePart.Elements.First(), compareData))
                    if (comparePath == null)
                        comparePath = rulePart;
                    else
                        throw new Exception(GetExceptionMsg("Грамматика не является LL(1). Неопределенность выбора между правилами.", element, compareData));
            }

            //Приоритет 1: Если есть путь с символьным совпадением
            if (comparePath != null)
                ToStack(comparePath.Elements);
            //Приоритет 2: Если дальнейший разбор возможен только по единственной пустой цепочке
            else if (emptyPathes.Count == 1)
                ToStack(emptyPathes.First().Elements);
            //Приоритет 3: Если имеются несколько пустых цепочек
            else if (emptyPathes.Count > 1)
                throw new Exception(GetExceptionMsg("Грамматика не является LL(1). Неопределенность выбора между пустыми цепочками.", element, compareData));
            //Приоритет 4: Если не найдено путей, то дальнейший разбор невозможен
            else
                throw new Exception(GetExceptionMsg("Дальнейший разбор невозможен.", element, compareData));
        }

        /// <summary>
        /// Добавляет все элементы подправила на стек.
        /// </summary>
        private void ToStack(List<GrammarElement> elements)
        {
            var elems = new List<GrammarElement>(elements);
            elems.Reverse(); 
            elems.ForEach(e => stack.Push(e));
        }

        /// <summary>
        /// Определяет, с помощью рекурсивного спуска, может ли данный элемент генирировать пустую цепочку.
        /// </summary>
        private bool CanBeEmpty(GrammarElement element)
        {
            bool result = false;

            if (element.Type == ElementType.Empty)
                return true;
            else if (element.Type == ElementType.NonTerminal)
                foreach (var rulePart in element.Rule.Right)
                    result = result | CanBeEmpty(rulePart.Elements.First());

            return result;
        }

        /// <summary>
        /// Определяет, с помощью рекурсивного спуска, имеет ли данный элемент посимвольное совпадение с входной строкой.
        /// </summary>
        private bool HasMatch(GrammarElement element, string compareData)
        {
            bool result = false;

            if (element.Type == ElementType.Terminal && element.Characters.Length <= compareData.Length && element.Characters == compareData.Substring(0, element.Characters.Length))
                return true;
            else if (element.Type == ElementType.Range && compareData.Length > 0 && element.Characters.Contains(compareData.Substring(0, 1)))
                return true;
            else if (element.Type == ElementType.NonTerminal)
                foreach (var rulePart in element.Rule.Right)
                    result = result | HasMatch(rulePart.Elements.First(), compareData);

            return result;
        }

        /// <summary>
        /// Изменяет значения счетчиков линий и символов, в зависимости от анализируемой части строки.
        /// </summary>
        private void RefreshCounters(string data)
        {
            if (data == Environment.NewLine)
            {
                ++lineCounter;
                charCounter = 1;
            }
            else
                charCounter += data.Length;
        }

        /// <summary>
        /// Дополняет сообщение об ошибке данными о номере строки и символа, в котором возникла ошибка. Также состоянием стека
        /// и оставшейся строкой разбора.
        /// </summary>
        /// <param name="mainMsg">Описание причины возникновения ошибки.</param>
        /// <param name="element">Елемент, на котором закончился разбор.</param>
        /// <param name="compareData">Оставшаяся, неразобранная строка.</param>
        private string GetExceptionMsg(string mainMsg, GrammarElement element, string compareData)
        {
            return $"{mainMsg + Environment.NewLine + Environment.NewLine}Исключение возникло при считывании символа или лексемы в " +
                   $"строке №:{lineCounter}, символ №:{charCounter}.{Environment.NewLine + Environment.NewLine}Оставшаяся строка: " +
                    $"{Environment.NewLine + compareData + Environment.NewLine + Environment.NewLine}Считан элемент со стека: " +
                    $"{element + Environment.NewLine}Осталось на стеке: {string.Join(Environment.NewLine, stack.ToList())}";
        }
    }
}
