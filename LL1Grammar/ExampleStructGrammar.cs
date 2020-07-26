using System;

namespace LL1GrammarUI
{
    /// <summary>
    /// Представление структуры в виде LL(1) грамматики в языке С#
    /// </summary>
    public class ExampleStructGrammar
    {
        private string grammar;
        private string data;

        public ExampleStructGrammar()
        {
            //grammar = "Struct->_Title_Body_" + Environment.NewLine +
            //          "_-> _|\\t_|\\n_|$" + Environment.NewLine +
            //          "Title->Mod _struct _Name|_struct _Name" + Environment.NewLine +
            //          "Mod->private|public|internal" + Environment.NewLine +
            //          "Name->A-zN1" + Environment.NewLine +
            //          "N1->A-zN1|0-9N1|$" + Environment.NewLine +
            //          "Body->{_Field_}" + Environment.NewLine +
            //          "Field->Mod _Type _Name_;_Field|Type _Name_;_Field|$" + Environment.NewLine +
            //          "Type->int|double|char|float";

            //data = "private struct myStruct" + Environment.NewLine +
            //       "{" + Environment.NewLine +
            //       "\tpublic int field1;" + Environment.NewLine +
            //       "\tdouble field2;" + Environment.NewLine +
            //       "}";

            grammar = "S->AB" + Environment.NewLine +
                      "A->aAa|$" + Environment.NewLine +
                      "B->bBb|$" + Environment.NewLine;

            data = "aabb";
        }

        public string GetGrammar()
        {
            return grammar;
        }

        public string GetData()
        {
            return data;
        }
    }
}
