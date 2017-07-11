using System.Text.RegularExpressions;
using System.Collections.Generic;

public class EditorRegex
{
    private List<Regex> m_regexs = new List<Regex>();

    public bool IsMatch(string path)
    {
        for (int i = 0; i < m_regexs.Count; ++i)
        {
            if (m_regexs[i].IsMatch(path))
                return true;
        }
        return false;
    }

    public static EditorRegex Create(string str)
    {
        EditorRegex ret = null;
        if (ms_dict.TryGetValue(str, out ret))
        {
            return ret;
        }
        ret = new EditorRegex();

        string pattern = BuildPatternString(str);
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        ret.m_regexs.Add(regex);

        ms_dict.Add(str, ret);
        return ret;
    }

    public static string BuildPatternString(string match)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < match.Length; ++i)
        {
            if (match[i] == '*')
            {
                sb.Append('.');
            }
            else if (IsMatchRegexChar(match[i]))
            {
                sb.Append('\\');
            }
            sb.Append(match[i]);
        }
        return sb.ToString();
    }

    public static bool IsMatchRegexChar(char ch)
    {
        return ch == '\\' || ch == '^' || ch == '$' || ch == '*' ||
            ch == '+' || ch == '?' || ch == '.';
    }

    private static Dictionary<string, EditorRegex> ms_dict = new Dictionary<string, EditorRegex>();
}