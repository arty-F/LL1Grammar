using System;
using System.Collections.Generic;

namespace LL1GrammarCore
{
    public class ActionsContainer
    {
        public Dictionary<string, Action<object>> Actions { get; set; } = new Dictionary<string, Action<object>>();

        
    }
}
