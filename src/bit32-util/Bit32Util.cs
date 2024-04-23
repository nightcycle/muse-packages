using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Bit32Util
{
	public interface IBit32
	{

		// returns the number in 1's and 0's format for debugging
		public static string ToDebugString(int value)
		{
			if (value < 0)
			{
				throw new System.Exception($"you can't decode a negative integer: {value}");
			}

			if (value > 0)
			{
				StringBuilder binaryRepresentation = new StringBuilder();
				while (value > 0)
				{
					int remainder = value % 2;
					binaryRepresentation.Insert(0, remainder);  // Prepend the remainder to the string
					value = value / 2;
				}
				List<char> outputArray = string.Concat(binaryRepresentation.ToString()).ToCharArray().ToList();
				while (outputArray.Count < 32)
				{
					outputArray.Insert(0, '0');
				}
				return new string(outputArray.ToArray());
			}
			else
			{
				return string.Concat(Enumerable.Repeat("0", 32));
			}
		}

		public static int Band(int a, int b)
		{
			return a & b;
		}

		public static int Bor(int a, int b)
		{
			return a | b;
		}

		public static int LShift(int value, int amount)
		{
			return value << amount;
		}

		public static int RShift(int value, int amount)
		{
			return value >> amount;
		}
	}
}