// written by CJ_Oyer (@nightcycle)
// can't handle tabs or multi-line strings
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace TextUtil
{

	public class TextDimensionSolver
	{
		public int CharacterHeight = 15;
		public int LineSpacing = 10;
		public int MinimumSpacerLength = 10;
		public Dictionary<char, int> CharacterWidths = new Dictionary<char, int>{
			[' '] = 5,
			['!'] = 5,
			['"'] = 8,
			['#'] = 13,
			['$'] = 11,
			['%'] = 17,
			['&'] = 15,
			['\''] = 4,
			['('] = 6,
			[')'] = 6,
			['*'] = 11,
			['+'] = 11,
			[','] = 5,
			['-'] = 6,
			['.'] = 5,
			['/'] = 7,
			['0'] = 11,
			['1'] = 11,
			['2'] = 11,
			['3'] = 11,
			['4'] = 11,
			['5'] = 11,
			['6'] = 11,
			['7'] = 11,
			['8'] = 11,
			['9'] = 11,
			[':'] = 5,
			[';'] = 5,
			['<'] = 11,
			['='] = 11,
			['>'] = 11,
			['?'] = 9,
			['@'] = 18,
			['A'] = 13,
			['B'] = 13,
			['C'] = 13,
			['D'] = 15,
			['E'] = 11,
			['F'] = 10,
			['G'] = 15,
			['H'] = 15,
			['I'] = 6,
			['J'] = 5,
			['K'] = 12,
			['L'] = 10,
			['M'] = 18,
			['N'] = 15,
			['O'] = 16,
			['P'] = 12,
			['Q'] = 16,
			['R'] = 12,
			['S'] = 11,
			['T'] = 11,
			['U'] = 15,
			['V'] = 12,
			['W'] = 18,
			['X'] = 12,
			['Y'] = 11,
			['Z'] = 11,
			['['] = 7,
			['\\'] = 7,
			[']'] = 7,
			['^'] = 11,
			['_'] = 9,
			['`'] = 6,
			['a'] = 11,
			['b'] = 12,
			['c'] = 10,
			['d'] = 12,
			['e'] = 11,
			['f'] = 7,
			['g'] = 11,
			['h'] = 12,
			['i'] = 5,
			['j'] = 5,
			['k'] = 11,
			['l'] = 5,
			['m'] = 18,
			['n'] = 12,
			['o'] = 12,
			['p'] = 12,
			['q'] = 12,
			['r'] = 8,
			['s'] = 10,
			['t'] = 7,
			['u'] = 12,
			['v'] = 10,
			['w'] = 16,
			['x'] = 10,
			['y'] = 10,
			['z'] = 9,
			['{'] = 8,
			['|'] = 11,
			['}'] = 8,
			['~'] = 11,
		};

		public TextDimensionSolver(){}

		public string GetEdgeSpacer(int length, bool isRightSide){
			int spaceWidth = this.GetStringLength(" ");
			if (length < spaceWidth) {
				length = spaceWidth;
			}
			int nonSpaceCount = length % spaceWidth;
			int spaceCount = (length - nonSpaceCount) / spaceWidth;
			string nonSpaceCharacter = "";
			if (nonSpaceCount == 1){
				if (nonSpaceCount <= 2){
					if (isRightSide == false){
						nonSpaceCharacter = "(";
					}else{
						nonSpaceCharacter = ")";
					}
				}else{
					nonSpaceCharacter = "|";
					spaceCount -= 1;
				}
			}else if (nonSpaceCount == 2){
				if (isRightSide == false){
					nonSpaceCharacter = "[";
				}else{
					nonSpaceCharacter = "]";
				}
			}else if (nonSpaceCount == 3){
				if (isRightSide == false){
					nonSpaceCharacter = "{";
				}else{
					nonSpaceCharacter = "}";
				}
			}else if (nonSpaceCount == 4){
				if (isRightSide == false){
					nonSpaceCharacter = "_";
				}else{
					nonSpaceCharacter = "_";
				}
			}
			if (isRightSide == false){
				return $"{string.Concat(Enumerable.Repeat(nonSpaceCharacter, nonSpaceCount))}{string.Concat(Enumerable.Repeat(" ", spaceCount))}";
			}else{
				return $"{string.Concat(Enumerable.Repeat(" ", spaceCount))}{string.Concat(Enumerable.Repeat(nonSpaceCharacter, nonSpaceCount))}";
			}
		}

		// this ended up not working at all
		public string AddStringPadding(string value, int maxValueWidth){

			int spacedMaxWidth = maxValueWidth + this.MinimumSpacerLength*2;

			int netSpacerLength = spacedMaxWidth - this.GetStringLength(value);

			// if (netSpacerLength % 2 == 0) {
			// 	netSpacerLength += 10;
			// }else{
			// 	netSpacerLength += 5;
			// }			
			// int leftSpacerLength = netSpacerLength / 2;
			// int rightSpacerLength = netSpacerLength / 2;
			int spaceWidth = this.GetStringLength(" ");
			int spaceCount = (netSpacerLength-(netSpacerLength%spaceWidth))/spaceWidth;
			string spacing = string.Concat(Enumerable.Repeat(" ", spaceCount));

			return $"{value}{spacing}"; //$"{this.GetEdgeSpacer(leftSpacerLength, false)}{value}{this.GetEdgeSpacer(rightSpacerLength, true)}";
		}

		public int GetStringLength(string value){
			int length = 0;
			foreach (char character in value)
			{
				int charWidth = this.CharacterWidths[character];
				length += charWidth;
			}
			return length;
		}

		private string DebugPrint(){
			StringBuilder output = new StringBuilder($"");

			foreach (KeyValuePair<char, int> item in this.CharacterWidths)
			{
				string key = item.Key.ToString();
				int value = item.Value;
				// repeats the character value number of times
				output.Append($"\n[{string.Concat(Enumerable.Repeat(key, value))}]");
			}

			return output.ToString();
		}
	}
}