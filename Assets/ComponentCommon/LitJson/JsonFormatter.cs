using System.Text;
using System.Collections.Generic;

public class JsonFormatter
{
    #region class members
    const string Space = " ";
    const int DefaultIndent = 0;
    const string Indent = Space + Space + Space + Space;
    static readonly string NewLine = "\n";
    #endregion
	
    static bool inDoubleString = false;
    static bool inSingleString = false;
    static bool inVariableAssignment = false;
    static char prevChar = '\0';

    static Stack<JsonContextType> context = new Stack<JsonContextType>();
	
    private enum JsonContextType
    {
        Object, Array
    }

    static void BuildIndents(int indents, StringBuilder output)
    {
        indents += DefaultIndent;
        for (; indents > 0; indents--)
            output.Append(Indent);
    }

    static bool InString()
    {
        return inDoubleString || inSingleString;
    }

    public static string PrettyPrint(string input)
    {
		// Clear all states
	    inDoubleString = false;
	    inSingleString = false;
	    inVariableAssignment = false;
	    prevChar = '\0';
	    context.Clear();
		
        var output = new StringBuilder(input.Length * 2);
        char c;

        for (int i = 0; i < input.Length; i++)
        {
            c = input[i];

            switch (c)
            {
                case '[':
                case '{':
                    if (!InString())
                    {
                        if (inVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array))
                        {
                            output.Append(NewLine);
                            BuildIndents(context.Count, output);
                        }
                        output.Append(c);
                        context.Push(JsonContextType.Object);
                        output.Append(NewLine);
                        BuildIndents(context.Count, output);
                    }
                    else
                        output.Append(c);

                    break;

                case ']':
                case '}':
                    if (!InString())
                    {
                        output.Append(NewLine);
                        context.Pop();
                        BuildIndents(context.Count, output);
                        output.Append(c);
                    }
                    else
                        output.Append(c);

                    break;
                case '=':
                    output.Append(c);
                    break;

                case ',':
                    output.Append(c);

                    if (!InString())
                    {
                        BuildIndents(context.Count, output);
                        output.Append(NewLine);
                        BuildIndents(context.Count, output);
                        inVariableAssignment = false;
                    }

                    break;

                case '\'':
                    if (!inDoubleString && prevChar != '\\')
                        inSingleString = !inSingleString;

                    output.Append(c);
                    break;

                case ':':
                    if (!InString())
                    {
                        inVariableAssignment = true;
                        output.Append(Space);
                        output.Append(c);
                        output.Append(Space);
                    }
                    else
                        output.Append(c);

                    break;

                case '"':
                    if (!inSingleString && prevChar != '\\')
                        inDoubleString = !inDoubleString;

                    output.Append(c);
                    break;
                case ' ':
                    if (InString())
                        output.Append(c);
                    break;

                default:
                    output.Append(c);
                    break;
            }
            prevChar = c;
        }

        return output.ToString();
    }
}