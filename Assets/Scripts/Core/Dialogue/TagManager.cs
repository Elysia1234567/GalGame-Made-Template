using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagManager
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>
    {
        { "<mainChar>",()=>"Avira"},
        {"<time>",()=> DateTime.Now.ToString("hh:mm tt")},
        {"<playerLevel>", () => "15" },
        {"<input>",() => InputPanel.instance.lastInput },
        {"<tempVal1>",() => "42"}
    };
    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text)
    {
        if(tagRegex.IsMatch(text))
        {
            foreach(Match match in tagRegex.Matches(text))
            {
                if(tags.TryGetValue(match.Value,out var tarValueRequest))
                {
                    text = text.Replace(match.Value, tarValueRequest());
                }
            }
        }
        return text;
    }
}
