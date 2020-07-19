using LL1GrammarCore.GrammarComponents;
using LL1GrammarCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Обертка для работы с грамматикой. Содержит список правил, специальные символы грамматики и метод для валидации строки согласно хранящимся правилам.
    /// </summary>
    public class Grammar : IGrammar
    {
        private List<IGrammarRule> rules { get; set; } = new List<IGrammarRule>();  //Список правил грамматики
        private ISpecialSymbols specialSymbols;

        private bool isNameReading = false;
        private List<StringBuilder> names = new List<StringBuilder> { new StringBuilder() };

        /// <summary>
        /// Создать новый экземпляр грамматики. Необходимо описание грамматики в виде строки, а также
        /// класс реализующий <see cref="ISpecialSymbols"/>, содержащий специальные символы грамматики.
        /// </summary>
        /// <param name="grammar">Строковое описание грамматики.</param>
        /// <param name="specialSymbols">Специальные символы грамматики.</param>
        public Grammar(string grammar, ISpecialSymbols specialSymbols)
        {
            this.specialSymbols = specialSymbols;

            try
            {
                //Разбиваем грамматику по строкам и добавляем в коллекцию правил экземпляры правил
                foreach (var line in grammar.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    rules.Add(new GrammarRule(line, specialSymbols));

                //Теперь, когда нетерминалы найдены, заполним правые части грамматики
                foreach (var rule in rules)
                    rule.BuildRightPart(rules, specialSymbols);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.ToString() + Environment.NewLine + Environment.NewLine}" +
                                    $"Состояние грамматики:{Environment.NewLine + this.ToString()}");
            }
        }

        /// <summary>
        /// Проверка входной строки на соответствие правилам грамматики.
        /// </summary>
        /// <param name="data">Строка с данными для проверки.</param>
        public bool ValidateData(string data)
        {
            var remainingData = new StringBuilder(data);
            try
            {
                DoValidate(remainingData, rules.First());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + $"{Environment.NewLine + Environment.NewLine}");
            }

            if (remainingData.Length > 0)
                throw new Exception($"Разбор завершен, при этом часть строки осталась неразобранна:{Environment.NewLine + remainingData.ToString()}");

            return true;
        }

        //Рекурсивная валидация строки согласно заданной грамматики
        private void DoValidate(StringBuilder remainingData, IGrammarRule rule)
        {
            var grammarRulePart = rule.GetNext(remainingData.ToString());   //Находим следующее подправило

            if (grammarRulePart == null)
                throw new Exception($"Данная строка не является частью грамматики. Из правила:{Environment.NewLine + rule.ToString() + Environment.NewLine}" +
                                    $"Невозможно осуществить разбор оставшейся части грамматики: {Environment.NewLine + remainingData.ToString() + Environment.NewLine}");

            foreach (var element in grammarRulePart.Elements)   //Проходим по каждому элементу подправила
            {
                switch (element.Type)
                {
                    case ElementType.NonTerminal:
                        DoValidate(remainingData, element.Rule);    //Если нетерминал, рекурсивно проходим все его подъэлементы
                        break;

                    case ElementType.Terminal:                      //Если терминал, удаляем найденный терминал из анализируемой строки
                        RemoveFromData(remainingData, element.Characters.Length, element, rule);
                        break;

                    case ElementType.Range:                         //Если диапазон, то необходимый символ гарантированно находится на первой позиции
                        WriteName(remainingData.ToString().First());    //Записываем первый символ в таблицу имен
                        RemoveFromData(remainingData, 1, element, rule);
                        break;

                    case ElementType.Empty:                         //Если пустая цепочка ничего не делаем, переходим к следующему элементу/правилу
                        if (isNameReading)                          //Если велась запись в талицу имен, то закончить запись текущего имени
                            NextName();
                        break;

                    default:
                        break;
                }
            }
        }

        //Удаление терминальной части из анализируемой строки с проверкой
        private void RemoveFromData(StringBuilder remainingData, int lenght, IGrammarElement element, IGrammarRule rule)
        {
            if (remainingData.Length < lenght)
                throw new Exception($"В буфере остались элементы грамматки, при этом разбор строки завершен.{Environment.NewLine}Оставшаяся строка: " +
                                    $"{Environment.NewLine + remainingData + Environment.NewLine}Разбор закончился на элементе: {element}, правила: {rule + Environment.NewLine}");
            //Проверка перед удалением для диапазона
            else if (element.Type == ElementType.Range && !element.Characters.Contains(remainingData.ToString().First()))
                throw new Exception($"Процесс разбора строки не может быть завершен с помощью данной грамматики.{ Environment.NewLine }Оставшаяся строка: " +
                                    $"{Environment.NewLine + remainingData + Environment.NewLine}Разбор закончился на элементе: {element}, правила: {rule + Environment.NewLine}");
            //Проверка перед удалением для терминала
            else if (element.Type == ElementType.Terminal && remainingData.ToString().Substring(0, element.Characters.Length) != element.Characters)
                throw new Exception($"Процесс разбора строки не может быть завершен с помощью данной грамматики.{ Environment.NewLine }Оставшаяся строка: " +
                                    $"{Environment.NewLine + remainingData + Environment.NewLine}Разбор закончился на элементе: {element}, правила: {rule + Environment.NewLine}");
            else
                remainingData.Remove(0, lenght);
        }

        //Запись считыаемого из диапазона сивола в последний элемент коллекции имен
        private void WriteName(char c)
        {
            if (!isNameReading)
                isNameReading = true;

            names.Last().Append(c);
        }

        //Проверка имен на совпадение и добавление в коллекцию нового элемента
        private void NextName()
        {
            isNameReading = false;

            for (int i = 0; i < names.Count() - 1; i++)
            {
                if (names.Last().ToString() == names[i].ToString())
                    throw new Exception($"Конфликт имен. Имя <{names.Last()}> уже было использовано ранее.{Environment.NewLine}");
            }

            names.Add(new StringBuilder());
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (var rule in rules)
            {
                buffer.Append(rule.Left + specialSymbols.Splitter);

                bool isFirstPart = true;
                foreach (var rulePart in rule.Right)
                {
                    if (isFirstPart)
                        isFirstPart = false;
                    else
                        buffer.Append(specialSymbols.Or);

                    foreach (var elem in rulePart.Elements)
                    {
                        if (elem.Type == ElementType.Empty)
                            buffer.Append(specialSymbols.ChainEmpty);
                        else if (elem.Type == ElementType.Terminal)
                            buffer.Append(elem.Characters);
                        else if (elem.Type == ElementType.NonTerminal)
                            buffer.Append(elem.Rule.Left);
                        else if (elem.Type == ElementType.Range)
                            buffer.Append(elem.Characters.First().ToString() + specialSymbols.Range.ToString() + elem.Characters.Last().ToString());
                    }
                }
                buffer.Append(Environment.NewLine);
            }

            return buffer.ToString();
        }
    }
}
