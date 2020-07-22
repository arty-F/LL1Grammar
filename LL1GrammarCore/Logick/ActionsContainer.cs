using System;
using System.Collections.Generic;

namespace LL1GrammarCore
{
    public static class ActionsContainer
    {
        public static Dictionary<string, Action<object>> Actions { get; set; } = new Dictionary<string, Action<object>>();
    }
}
