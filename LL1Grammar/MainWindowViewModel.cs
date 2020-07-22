using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LL1GrammarCore;

namespace LL1Grammar
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public string Grammar { get; set; }
        public string InputData { get; set; }
        public string Result { get; set; } = "Результат анализа";

        public string Splitter { get; set; } = "->";
        public string Or { get; set; } = "|";
        public string Range { get; set; } = "-";
        public string Empty { get; set; } = "$";

        public MainWindowViewModel()
        {
            var initializer = new ExampleStructGrammar();
            Grammar = initializer.GetGrammar();
            InputData = initializer.GetData();
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
            Grammar gram = new Grammar(Grammar, new SpecialSymbols(Splitter, Empty.First(), Or.First(), Range.First()));
            //bool result = false;
            //if (Splitter == "" || Or == "" || Range == "" || Empty == "")
            //    MessageBox.Show("Заполните все поля специальных символов.");
            //else
            //{
            //    try
            //    {
            //        Grammar gram = new Grammar(Grammar, new SpecialSymbols(Splitter, Empty.First(), Or.First(), Range.First()));
            //        //result = gram.ValidateData(InputData);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }

            //    if (result)
            //        Result = "Разбор завершен успешно.";
            //    else
            //        Result = "Во время разбора возникла ошибка.";

            //    OnPropertyChanged(nameof(Result));
            //}
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
