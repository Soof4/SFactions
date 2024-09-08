using System.Text;

namespace SFactions;

public static class Extensions
{
    public static string ToTitleCase(this string str)
    {
        if (str.Trim().Length <= 1) return str.Trim().ToUpper();

        str = str.Trim();

        StringBuilder builder = new();
        builder.Append(str[0]);

        char preC = str[0];
        char c = str[1];

        for (int i = 1; i < str.Length; i++, preC = c)
        {
            c = str[i];
            if (char.IsWhiteSpace(preC) && char.IsWhiteSpace(c))
            {
                continue;
            }

            if (char.IsDigit(c))
            {
                if (!char.IsDigit(preC) && !char.IsWhiteSpace(preC)) builder.Append(' ');

                builder.Append(c);

            }
            else if (char.IsUpper(c))
            {
                if (!char.IsWhiteSpace(preC)) builder.Append(' ');

                builder.Append(c);
            }
            else    // If c is lowercase
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    public static string GetFirstWord(this string str)
    {
        if (str.Trim().Length <= 1) return str.Trim().ToUpper();

        str = str.Trim();

        string res = str[0].ToString().ToLower();

        for (int i = 1; i < str.Length; i++)
        {
            char c = str[i];

            if (char.IsUpper(c) || char.IsDigit(c))
            {
                break;
            }

            res += c;
        }

        return res;
    }
}