namespace Game.Tools.TextElementParser
{
	public class ParsedElement
	{
		public string FullText { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public int Index { get; set; }

		public bool Contains(int index) => index >= Index;

		public override string ToString() => $"Index = {Index}, Name = {Name}, Value = {Value}";
	}
}
