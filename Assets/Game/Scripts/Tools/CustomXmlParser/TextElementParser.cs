using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Game.Tools.TextElementParser
{
	public static class TextElementParser
	{
		public static readonly string TagPattern = @"\[(.*?)\]";

		public static List<ParsedElement> Parse(string text)
		{
			Regex tagRegex = new(TagPattern);
			MatchCollection matches = tagRegex.Matches(text);
			List<ParsedElement> result = new();
			int accumulatedIndex = 0;

			foreach (Match item in matches)
			{
				string[] split = item.Groups[1].Value.Split('=');

				result.Add(new()
				{
					Index = item.Index - accumulatedIndex,
					FullText = item.Value,
					Name = split[0],
					Value = split.Length == 2 ? split[1] : string.Empty
				});
				accumulatedIndex += item.Groups[0].Value.Length;
			}
			return result;
		}

		public static string RemoveElements(string lightText, List<ParsedElement> textEffects)
		{
			string result = lightText;

			foreach (ParsedElement item in textEffects)
				result = result.Replace(item.FullText, "");
			return result;
		}
	}
}
