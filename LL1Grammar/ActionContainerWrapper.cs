using LL1GrammarCore;
using System;
using System.Linq;
using System.Windows;

namespace LL1GrammarUI
{
    /// <summary>
    /// Обертка для отображения и работы с действиями в пользовательском интерфейсе
    /// </summary>
    public class ActionContainerWrapper
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (container.Actions.Where(a => a.Key == value).Any())
                {
                    MessageBox.Show("Выбранное имя действия уже занято.");
                }
                else
                {
                    container.Change(name, value);
                    name = value;
                }
            }
        }

        public string Description { get; }

        ActionsContainer container;

        public ActionContainerWrapper(ActionsContainer container, string Key)
        {
            this.container = container;
            this.name = Key;
            Description = container.Actions.Where(a => a.Key == Key).Select(a => a.Description).First();
        }
    }
}
