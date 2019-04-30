using DiplomaThesis.Common.CommandProcessing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace DiplomaThesis.Collector.Postgres
{
    class LoadDebugTreeToContextCommand : ChainableCommand
    {
        #region private class EscapeCharacterReplacement
        private class EscapeCharacterReplacement
        {
            public string EscapeCharacter { get; }
            public string ProcessingReplacement { get; }
            public string RealValue { get; }
            public EscapeCharacterReplacement(string escapeCharacter, string processingReplacement, string realValue)
            {
                EscapeCharacter = escapeCharacter;
                ProcessingReplacement = processingReplacement;
                RealValue = realValue;
            }
        } 
        #endregion
        private static readonly Regex ATTRIBUTES_WITHOUT_VALUES_REGEX = new Regex(@":[a-z|A-Z]*? :");
        private static readonly List<EscapeCharacterReplacement> COLUMN_NAME_ESCAPE_CHARACTER_REPLACEMENTS = new List<EscapeCharacterReplacement>();

        private readonly Func<string> getInputFunc;
        private readonly Action<JObject> setOutputAction;
        public LoadDebugTreeToContextCommand(Func<string> getInputFunc, Action<JObject> setOutputAction)
        {
            this.getInputFunc = getInputFunc;
            this.setOutputAction = setOutputAction;
            COLUMN_NAME_ESCAPE_CHARACTER_REPLACEMENTS.Add(new EscapeCharacterReplacement(@"\ ", "@WHITE_SPACE", " "));
        }

        private static string ConvertArrayToJsonProperty(string input)
        {
            string result = null;
            var items = new List<string>(input.Replace("(", "").Replace(")", "").Split(" ", StringSplitOptions.RemoveEmptyEntries));
            var formatedItems = new List<string>();
            items.ForEach(x => formatedItems.Add(ConvertToJsonProperty(x)));
            result = String.Join(",", formatedItems);
            return "[" + result + "]";
        }

        private static string ConvertToJsonProperty(string input, bool asArray = false)
        {
            string result = input.Trim();
            if (asArray)
            {
                var items = new List<string>(input.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                var formatedItems = new List<string>();
                items.ForEach(x => formatedItems.Add(ConvertToJsonProperty(x)));
                result = String.Join(",", formatedItems);
            }
            else if (!String.IsNullOrEmpty(result) && !input.StartsWith("\""))
            {
                result = "\"" + input + "\"";
            }
            return result.Replace("\\", "");
        }

        protected override void OnExecute()
        {
            string inputParseTreeStr = getInputFunc();
            if (inputParseTreeStr == null)
            {
                return;
            }
            string parseTreeStr = inputParseTreeStr.Replace("\r\n", " ").Replace("\n", " ").Replace("\t", " ").Trim();
            foreach (var item in COLUMN_NAME_ESCAPE_CHARACTER_REPLACEMENTS)
            {
                parseTreeStr = parseTreeStr.Replace(item.EscapeCharacter, item.ProcessingReplacement);
            }
            while (parseTreeStr.Contains("  "))
            {
                parseTreeStr = parseTreeStr.Replace("  ", " ");
            }
            bool doNextAttributesWithoutValuesIteration = true;
            while (doNextAttributesWithoutValuesIteration)
            {
                doNextAttributesWithoutValuesIteration = false;
                foreach (Match m in ATTRIBUTES_WITHOUT_VALUES_REGEX.Matches(parseTreeStr))
                {
                    string toReplace = m.Value.Substring(0, m.Value.Length - 1).Trim() + " <> :";
                    parseTreeStr = parseTreeStr.Replace(m.Value, toReplace);
                    doNextAttributesWithoutValuesIteration = true; // two attributes without values might be next to each other, we need another iteration
                }
            }
            var split = parseTreeStr.Split(":", StringSplitOptions.RemoveEmptyEntries);
            var inputEntries = new List<string>();
            bool isFirst = true;
            foreach (var s in split)
            {
                var splitValue = ":" + s;
                if (isFirst)
                {
                    splitValue = s;
                    isFirst = false;
                }
                var keyValues = splitValue.Trim().Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
                foreach (var kv in keyValues)
                {
                    string workingString = kv;
                    int index = -1;
                    while ((index = workingString.IndexOf("}")) >= 0)
                    {
                        inputEntries.Add(workingString.Substring(0, index + 1));
                        workingString = workingString.Substring(index + 1);
                    }
                    if (!String.IsNullOrEmpty(workingString))
                    {
                        inputEntries.Add(workingString);
                    }
                }
            }
            for (int i = 0; i < inputEntries.Count; i++)
            {
                string input = inputEntries[i].Trim();
                bool isArray = input.Contains("(") && input.Contains(")");
                string arrayValue = null;
                string replacedArrayValue = null;
                if (isArray)
                {
                    int arrayStart = input.IndexOf("(");
                    int arrayEnd = input.IndexOf(")");
                    arrayValue = input.Substring(arrayStart, arrayEnd - arrayStart + 1);
                    replacedArrayValue = Guid.NewGuid().ToString();
                    input = input.Replace(arrayValue, replacedArrayValue);
                }
                if (input.StartsWith("({"))
                {
                    input = "[ { " + ConvertToJsonProperty(input.Substring(2)) + ": {";
                }
                else if (input.StartsWith("("))
                {
                    string originalInput = input;
                    int length = input.Length - 1;
                    if (originalInput.EndsWith(")"))
                    {
                        length = input.Length - 2;
                    }
                    else if (originalInput.EndsWith(")}"))
                    {
                        length = input.Length - 3;
                    }
                    input = ConvertToJsonProperty(input.Substring(1, length));
                    input = "[ " + input;
                    if (originalInput.EndsWith(")"))
                    {
                        input = input + " ], ";
                    }
                    else if (originalInput.EndsWith(")}"))
                    {
                        input = input + " ] } }, ";
                    }
                    else
                    {
                        input = input + ", ";
                    }
                }
                else if (input.StartsWith("{"))
                {
                    input = "{ " + ConvertToJsonProperty(input.Substring(1)) + ": {";
                }
                else if (input.StartsWith(":"))
                {
                    input = ConvertToJsonProperty(input.Substring(1)) + ": ";
                }
                else if (input.EndsWith("}") || input.EndsWith(")"))
                {
                    string val = input.Replace("}", "").Replace(")", "");
                    if (!String.IsNullOrEmpty(val))
                    {
                        input = input.Replace(val, ConvertToJsonProperty(val));
                    }
                    input = input.Replace(")", "]").Replace("}", "} }") + ", ";
                }
                else
                {
                    input = ConvertToJsonProperty(input);
                    input = input + ",";
                }
                if (replacedArrayValue != null)
                {
                    input = input.Replace("\"" + replacedArrayValue + "\"", ConvertArrayToJsonProperty(arrayValue));
                    input = input.Replace(replacedArrayValue, arrayValue.Replace("\\", ""));
                }
                inputEntries[i] = input;
            }
            string json = String.Join(" ", inputEntries);
            foreach (var item in COLUMN_NAME_ESCAPE_CHARACTER_REPLACEMENTS)
            {
                json = json.Replace(item.ProcessingReplacement, item.RealValue);
            }
            json = json.Substring(0, json.Length - 2); // remove ending ", "
            var result = JObject.Parse(json);
#if DEBUG
            if (Debugger.IsAttached)
            {
                var niceJson = result.ToString(Newtonsoft.Json.Formatting.Indented);
            }
#endif
            setOutputAction(result);
        }
    }
}
