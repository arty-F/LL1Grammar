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
                      "Array->[~ArrInner~]~Array|$" + Environment.NewLine +
                      "ArrInner->,~Arrinner|$" + Environment.NewLine +
                      "RepeatFName->,~FName~<A3><A2>RepeatFName|$" + Environment.NewLine +
                      "DataType->Alias|System.FullTypeName|FullTypeName" + Environment.NewLine +
                      "Alias->bool<A5>|byte<A5>|sbyte<A5>|char<A5>|decimal<A5>|double<A5>|float<A5>|int<A5>|uint<A5>|long<A5>|ulong<A5>|short<A5>|ushort<A5>|object<A5>|string<A5>|dynamic<A5>" + Environment.NewLine +
                      "FullTypeName->Boolean<A5>|Byte<A5>|SByte<A5>|Char<A5>|Decimal<A5>|Double<A5>|Single<A5>|Int32<A5>|UInt32<A5>|Int64<A5>|UInt64<A5>|Int16<A5>|UInt16<A5>|Object<A5>|String<A5>" + Environment.NewLine +
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
