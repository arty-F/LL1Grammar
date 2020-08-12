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
                      "Body->{~Elements~}~SplitterStruct" + Environment.NewLine +
                      "SplitterStruct->;|$" + Environment.NewLine +
                      "Elements->Field~Elements|InnerStruct~Elements|$" + Environment.NewLine +
                      "InnerStruct->struct ~StructName~Body~" + Environment.NewLine +
                      "Field->Mod ~DataType~Nullable~Array~FName~<A3><A2>RepeatFName;|DataType~Nullable~Array~FName~<A3><A2>RepeatFName;" + Environment.NewLine +
                      "Nullable->?|$" + Environment.NewLine +
                      "Array->[]|$" + Environment.NewLine +
                      "RepeatFName->,~FName~<A3><A2>RepeatFName|$" + Environment.NewLine +
                      "DataType->Alias|System.FullTypeName|FullTypeName" + Environment.NewLine +
                      "Alias->bool|byte|sbyte|char|decimal|double|float|int|uint|long|ulong|short|ushort|object|string|dynamic" + Environment.NewLine +
                      "FullTypeName->Boolean|Byte|SByte|Char|Decimal|Double|Single|Int32|UInt32|Int64|UInt64|Int16|UInt16|Object|String" + Environment.NewLine +
                      "FName->a-z<A1>NxtName|A-Z<A1>NxtName|0-9<A1>NxtName|_<A1>NxtName";

            data = "public struct exStruct" + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "\tpublic int? field1, field2 ,field3;" + Environment.NewLine + Environment.NewLine +
                   "\tstruct inStruct" + Environment.NewLine +
                   "\t{" + Environment.NewLine +
                   "\t\tprivate bool[] innerArrayfield1;" + Environment.NewLine +
                   "\t\tprivate System.UInt64 innerField2;" + Environment.NewLine +
                   "\t\tprivate SByte innerField3;" + Environment.NewLine +
                   "\t};" + Environment.NewLine + Environment.NewLine +
                   "\tdouble afterField;" + Environment.NewLine +
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
