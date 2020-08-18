using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LL1GrammarCore;

namespace LL1GrammarUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public string Grammar { get; set; }
        public string InputData { get; set; }
        public string Result { get; set; } = "Результат анализа";

        public List<ActionContainerWrapper> ContainerWrapper { get; set; } = new List<ActionContainerWrapper>();
        private ActionsContainer actionsContainer = new ActionsContainer();

        public string Splitter { get; set; } = "->";
        public string Or { get; set; } = "|";
        public string Range { get; set; } = "-";
        public string Empty { get; set; } = "$";

        public MainWindowViewModel()
        {
            var initializer = new ExampleStructGrammar();
            Grammar = initializer.GetGrammar();
            InputData = initializer.GetData();

            foreach (var item in actionsContainer.Actions)
            {
                ContainerWrapper.Add(new ActionContainerWrapper(actionsContainer, item.Key));
            }
        }

        private ICommand analyzeCmd;
        public ICommand AnalyzeCmd
        {
            get
            {
                return analyzeCmd ?? (analyzeCmd = new CommandHandler(Analyze, () => true));
            }
        }
        private void Analyze()
        {
            //var a = typeof(int[,,][,,,]).ToString();
            MessageBox.Show(typeof(int?[]).ToString());
            MessageBox.Show(typeof(int?[]).AssemblyQualifiedName);
            //MessageBox.Show(Type.GetType("System.Nullable'1[System.Int32][,,,][,,]").AssemblyQualifiedName);


            bool result = false;

            if (Splitter == "" || Or == "" || Range == "" || Empty == "")
            {
                MessageBox.Show("Заполните все поля специальных символов.");
                return;
            }

            if (ContainerWrapper.Where(c => c.Name == "").Any())
            {
                MessageBox.Show("Заполните все идентификаторы действий.");
                return;
            }

            actionsContainer.Clear();

            try
            {
                Grammar gram = new Grammar(Grammar, new SpecialSymbols(Splitter, Empty.First(), Or.First(), Range.First()), actionsContainer);
                result = gram.Validate(InputData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result)
                Result = "Разбор завершен успешно.";
            else
                Result = "Во время разбора возникла ошибка.";

            OnPropertyChanged(nameof(Result));
        }

        private ICommand clearCmd;
        public ICommand ClearCmd
        {
            get
            {
                return clearCmd ?? (clearCmd = new CommandHandler(Clear, () => true));
            }
        }
        private void Clear()
        {
            actionsContainer = new ActionsContainer();
            ContainerWrapper = new List<ActionContainerWrapper>();
            foreach (var item in actionsContainer.Actions)
            {
                ContainerWrapper.Add(new ActionContainerWrapper(actionsContainer, item.Key));
            }

            Grammar = "";
            InputData = "";
            Result = "";
            Splitter = "";
            Or = "";
            Range = "";
            Empty = "";

            OnPropertyChanged(nameof(Grammar));
            OnPropertyChanged(nameof(InputData));
            OnPropertyChanged(nameof(Result));
            OnPropertyChanged(nameof(Splitter));
            OnPropertyChanged(nameof(Or));
            OnPropertyChanged(nameof(Range));
            OnPropertyChanged(nameof(Empty));
            OnPropertyChanged(nameof(ContainerWrapper));
        }

        #region Property changing
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }
        #endregion
    }
}
