using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LL1GrammarCore
{
    /// <summary>
    /// Класс, отвечающий за выполнение действий в грамматике.
    /// </summary>
    public class ActionsContainer
    {
        /// <summary>
        /// Список доступных действий.
        /// </summary>
        public List<(string Key, string Description, Action<object> Value)> Actions { get; set; } = new List<(string, string, Action<object>)>();

        private StringBuilder buffer = new StringBuilder();
        private StringBuilder value = new StringBuilder();
        List<(string name, string value)> varTable = new List<(string name, string value)>();
        List<string> structTable = new List<string>();

        public ActionsContainer()
        {
            Actions.Add(("<A1>", "Добавить символ или группу символов в буфер.", ToBuffer));
            Actions.Add(("<A2>", "Очистить буфер.", ClearBuffer));
            Actions.Add(("<A3>", "Объявить новую переменную, имя которой находится в буфере.", NewValueBuffer));
            Actions.Add(("<A4>", "Объявить новую структуру, имя которой находится в буфере.", NewStructBuffer));
            Actions.Add(("<A5>", "Считать символ или группу символов как значение в виде строки.", ReadValue));
            Actions.Add(("<A6>", "Очистить считанное значение.", ClearValue));
            Actions.Add(("<A7>", "Присвоить значение переменной, имя которой находятся в буфере.", SetValueBuffer));
        }

        /// <summary>
        /// Очистить буферы и таблицы.
        /// </summary>
        public void Clear()
        {
            buffer.Clear();
            value.Clear();
            varTable = new List<(string name, string value)>();
            structTable = new List<string>();
        }

        /// <summary>
        /// Заменяет идентификатор действия новым значением.
        /// </summary>
        public void Change(string oldValue, string newValue)
        {
            var i = Actions.IndexOf(Actions.Where(a => a.Key == oldValue).FirstOrDefault());
            if (i != -1)
                Actions[i] = (newValue, Actions[i].Description, Actions[i].Value);
        }

        #region ДЕЙСТВИЯ
        /// <summary>
        /// Добавить символ или группу символов в буфер.
        /// </summary>
        internal void ToBuffer(object param)
        {
            if (param == null)
                throw new Exception($"Действие {Actions.Where(a => a.Value == ToBuffer).First().Key} задано для недопустимого элемента.");

            buffer.Append(param as string);
        }

        /// <summary>
        /// Очистить буфер.
        /// </summary>
        internal void ClearBuffer(object param)
        {
            buffer.Clear();
        }

        /// <summary>
        /// Объявить новую переменную, имя которой находится в буфере.
        /// </summary>
        internal void NewValueBuffer(object param)
        {
            if (buffer.Length == 0)
                throw new Exception("Буфер пуст.");

            var newVar = buffer.ToString();

            if (varTable.Where(t => t.name == newVar).Any())
                throw new Exception($"Попытка повторного объявления переменной {newVar}.");

            varTable.Add((buffer.ToString(), ""));
        }

        /// <summary>
        /// Объявить новую структуру, имя которой находится в буфере.
        /// </summary>
        internal void NewStructBuffer(object param)
        {
            if (buffer.Length == 0)
                throw new Exception("Буфер пуст.");

            var newStruct = buffer.ToString();

            if (structTable.Where(t => t == newStruct).Any())
                throw new Exception($"Попытка повторного объявления переменной {newStruct}.");

            structTable.Add(buffer.ToString());
        }

        /// <summary>
        /// Считать символ или группу символов как значение в виде строки.
        /// </summary>
        internal void ReadValue(object param)
        {
            if (param == null)
                throw new Exception($"Действие {Actions.Where(a => a.Value == ReadValue).First().Key} задано для недопустимого элемента.");

            value.Append(param as string);
        }

        /// <summary>
        /// Очистить считанное значение.
        /// </summary>
        internal void ClearValue(object param)
        {
            value.Clear();
        }

        /// <summary>
        /// Присвоить считанное значение переменной, имя которой находятся в буфере.
        /// </summary>
        internal void SetValueBuffer(object param)
        {
            var v = varTable.Where(t => t.name == buffer.ToString());
            if (v.Any())
            {
                varTable.Remove(v.First());
                varTable.Add((buffer.ToString(), value.ToString()));
            }
        }
        #endregion
    }
}
