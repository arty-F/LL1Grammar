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
        List<string> structNames = new List<string>();
        Dictionary<string, Type> variableTypes = new Dictionary<string, Type>();
        Type currentType = null;
        bool currentTypeIsNullable = false;
        StringBuilder currentTypeArrayModifier = new StringBuilder();
        Dictionary<string, Type> availableTypes;
        List<string> reserverdKeyWords;

        public ActionsContainer()
        {
            Actions.Add(("<A1>", "Добавить символ или группу символов в буфер.", ToBuffer));
            Actions.Add(("<A2>", "Очистить буфер.", ClearBuffer));
            Actions.Add(("<A3>", "Объявить новую переменную, имя которой находится в буфере.", NewValueBuffer));
            Actions.Add(("<A4>", "Объявить новую структуру, имя которой находится в буфере.", NewStructBuffer));
            Actions.Add(("<A5>", "Считать тип переменной. Все последующие объявленные переменные будут данного типа.", SetVariableType));
            
            availableTypes = new Dictionary<string, Type> { 
                {"bool", typeof(int) }, { "Boolean", typeof(int) }, { "System.Boolean", typeof(int) },
                {"byte", typeof(byte) }, { "Byte", typeof(byte) }, { "System.Byte", typeof(byte) },
                {"sbyte", typeof(sbyte) }, { "SByte", typeof(sbyte) }, { "System.SByte", typeof(sbyte) },
                {"char", typeof(char) }, { "Char", typeof(char) }, { "System.Char", typeof(char) },
                {"decimal", typeof(decimal) }, { "Decimal", typeof(decimal) }, { "System.Decimal", typeof(decimal) },
                {"double", typeof(double) }, { "Double", typeof(double) }, { "System.Double", typeof(double) },
                {"float", typeof(float) }, { "Single", typeof(float) }, { "System.Single", typeof(float) },
                {"int", typeof(int) }, { "Int32", typeof(int) }, { "System.Int32", typeof(int) },
                {"uint", typeof(uint) }, { "UInt32", typeof(uint) }, { "System.UInt32", typeof(uint) },
                {"long", typeof(long) }, { "Int64", typeof(long) }, { "System.Int64", typeof(long) },
                {"ulong", typeof(ulong) }, { "UInt64", typeof(ulong) }, { "System.UInt64", typeof(ulong) },
                {"short", typeof(short) }, { "Int16", typeof(short) }, { "System.Int16", typeof(short) },
                {"ushort", typeof(ushort) }, { "UInt16", typeof(ushort) }, { "System.UInt16", typeof(ushort) },
                {"object", typeof(object) }, { "Object", typeof(object) }, { "System.Object", typeof(object) },
                {"string", typeof(string) }, { "String", typeof(string) }, { "System.String", typeof(object) },
                {"dynamic", typeof(object) }
            };

            reserverdKeyWords = new List<string> {
                "abstract", "break", "continue", "do", "event", "finnaly", "foreach", "in", "is",
                "new", "out", "protected", "return", "sizeof", "struct", "true", "using", "while",
                "as", "checked", "explicit", "fixed", "goto", "lock", "null", "override", "public",
                "stackalloc", "switch", "try", "unchecked", "virtual", "base", "case", "class", "default",
                "else", "extern", "if", "interface", "params", "readonly", "sealed", "static", "this",
                "typeof", "unsafe", "void", "catch", "const", "delegate", "enum", "false", "for",
                "implicit", "internal", "namespace", "operator", "private", "ref", "throw", "volatile"
            };
        }

        /// <summary>
        /// Очистить буферы и таблицы.
        /// </summary>
        public void Clear()
        {
            buffer.Clear();
            variableTypes = new Dictionary<string, Type>();
            structNames = new List<string>();
            currentType = null;
            currentTypeIsNullable = false;
            currentTypeArrayModifier.Clear();
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

            var newVarName = buffer.ToString();

            if (variableTypes.Where(t => t.Key == newVarName).Any())
                throw new Exception($"Попытка повторного объявления переменной {newVarName}.");

            if (currentType == null)
                throw new Exception($"Тип переменной {newVarName} не объявлен.");

            if (availableTypes.ContainsKey(newVarName))
                throw new Exception($"Имя переменной {newVarName} недопустимо.");

            if (newVarName[0] != '@')
                if (reserverdKeyWords.Contains(newVarName))
                    throw new Exception($"Данное имя {newVarName} не может быть использовано.");
            else
                if (newVarName.Length == 1)
                    throw new Exception("Имя переменной не может состоять из одного символа @.");

            variableTypes.Add(buffer.ToString(), currentType);
        }

        /// <summary>
        /// Устанавливает тип определяемых переменных.
        /// </summary>
        /// <param name="param">Имя типа.</param>
        internal void SetVariableType(object param)
        {
            string type = param as string;

            if (availableTypes.ContainsKey(type))
                currentType = availableTypes[type];
            else
                throw new Exception($"Неизвестный тип {type}.");

            currentTypeIsNullable = false;
            currentTypeArrayModifier.Clear();
        }

        internal void SetVariableTypeNullable(object param)
        {
            if (currentType == null)
                throw new Exception($"Тип не объявлен.");

            if (currentType.IsClass)
                throw new Exception("Синтаксис \"Type?\" допустим только для типов-значений.");

            currentTypeIsNullable = true;
        }

        internal void SetVariableTypeArray(object param)
        {

        }

        /// <summary>
        /// Объявить новую структуру, имя которой находится в буфере.
        /// </summary>
        internal void NewStructBuffer(object param)
        {
            if (buffer.Length == 0)
                throw new Exception("Буфер пуст.");

            var newStruct = buffer.ToString();

            if (structNames.Where(t => t == newStruct).Any())
                throw new Exception($"Попытка повторного объявления структуры {newStruct}.");

            structNames.Add(buffer.ToString());
        }
        #endregion
    }
}
