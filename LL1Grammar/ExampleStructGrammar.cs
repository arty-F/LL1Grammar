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
            grammar = "StartStruct->~Title~Body~" + Environment.NewLine +
                      "~-> ~|\\t~|\\n~|$" + Environment.NewLine +
                      "Title->Mod ~struct ~StructName|~struct ~StructName" + Environment.NewLine +
                      "Mod->private|public|internal|protected" + Environment.NewLine +
                      "StructName->a-z<A1>NxtName~<A4><A2>|A-Z<A1>NxtName~<A4><A2>|0-9<A1>NxtName~<A4><A2>|_<A1>NxtName~<A4><A2>" + Environment.NewLine +
                      "NxtName->a-z<A1>NxtName|A-Z<A1>NxtName|0-9<A1>NxtName|_<A1>NxtName|$" + Environment.NewLine +
                      "Body->{~Elements~}" + Environment.NewLine +
                      "Elements->Field~Elements|InnerStruct~Elements|$" + Environment.NewLine +
                      "InnerStruct->struct ~StructName~Body~" + Environment.NewLine +
                      "Field->Mod ~Type ~FName~<A3>Value|Type ~FName~<A3>Value" + Environment.NewLine +
                      "Type->int|float|double|bool|string|char" + Environment.NewLine +
                      "FName->a-z<A1>NxtName|A-Z<A1>NxtName|0-9<A1>NxtName|_<A1>NxtName" + Environment.NewLine +
                      "Value->=~NameVal~<A7><A2><A6>;|;" + Environment.NewLine +
                      "NameVal->a-z<A5>NxtVal|A-Z<A5>NxtVal|0-9<A5>NxtVal" + Environment.NewLine +
                      "NxtVal->a-z<A5>NxtVal|A-Z<A5>NxtVal|0-9<A5>NxtVal|_<A5>NxtVal|$";

            data = "public struct exStruct" + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "\tpublic int field1 = 10;" + Environment.NewLine + Environment.NewLine +
                   "\tstruct inStruct" + Environment.NewLine +
                   "\t{" + Environment.NewLine +
                   "\t\tprivate bool innerField = true;" + Environment.NewLine +
                   "\t}" + Environment.NewLine + Environment.NewLine +
                   "\tdouble field2;" + Environment.NewLine +
                   "}";
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
