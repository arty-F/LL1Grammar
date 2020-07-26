using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1GrammarCore
{
    public class ActionsContainer
    {
        public List<(string Key, string Description, Action<object> Value)> Actions { get; set; } = new List<(string, string, Action<object>)>();

        

        public ActionsContainer()
        {
            Actions.Add(("<A1>","asd",Action1));
            Actions.Add(("<A2>", "asd", Action2));
            Actions.Add(("<A3>", "asd", Action3));
            Actions.Add(("<A4>", "asd", Action3));
        }

        public void Change(string oldValue, string newValue)
        {
            var i = Actions.IndexOf(Actions.Where(a => a.Key == oldValue).FirstOrDefault());
            if (i != -1)
            {
                Actions[i] = (newValue, Actions[i].Description, Actions[i].Value);
            }
        }

        internal void Action1(object param)
        {
            
        }

        internal void Action2(object param)
        {

        }

        internal void Action3(object param)
        {

        }

        internal void Action4(object param)
        {

        }
    }
}
