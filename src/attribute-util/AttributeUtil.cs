using System;
using System.Text;
using MuseDotNet.Framework;
namespace AttributeUtil
{
	public interface IAttributeTest{
		// private static void TestByte(Actor actor)
		// {
		// 	string key = "TEST_BYTE";
		// 	Random rng = new();
		// 	for (int j = 0; j < 20; j++)
		// 	{
		// 		byte[] bytes = new byte[12];
		// 		rng.NextBytes(bytes);
		// 		for (int i = 0; i < bytes.Length; i++)
		// 		{
		// 			byte input = bytes[i];
		// 			IAttribute.SetByte(actor, key, i, input);
		// 			byte output = IAttribute.GetByte(actor, key, i);
		// 			string message = $"test_byte/[input={input},output={output}]";
		// 			if (output == input)
		// 			{
		// 				Debug.Log(LogLevel.Display, message);
		// 			}
		// 			else
		// 			{
		// 				throw new Exception(message);
		// 			}
		// 		}
		// 	}
		// }
		private static void TestInt(Actor actor)
		{
			string key = "TEST_INT_KEY";
			Random rng = new();
			for (int i = 0; i < 128; i++)
			{
				int input = rng.Next();
				IAttribute.SetIntAttribute(actor, key, input);
				int output = IAttribute.GetIntAttribute(actor, key);
				IAttribute.SetIntAttribute(actor, key, 0);
				if (input != output)
				{
					throw new Exception($"TestInt failed / input={input}, output={output}");
				}
				else
				{
					Debug.Log(LogLevel.Display, $"TestInt success / input={input}, output={output}");
				}
			}
		}

		private static void TestString(Actor actor)
		{
			string key = "TEST_STRING";
			Random rng = new();

			for (int j = 0; j < 20; j++)
			{
				int size = rng.Next(6, 23);
				byte[] bytes = new byte[size];

				string input = "hello world! this is a test or something"; //Encoding.ASCII.GetString(bytes);
				IAttribute.SetStringAttribute(actor, key, input);

				string output = IAttribute.GetStringAttribute(actor, key);
				string message = $"test_string/[input=\"{input}\",output=\"{output}\"]";
				if (output == input)
				{
					Debug.Log(LogLevel.Display, message);
				}
				else
				{
					throw new Exception(message);
				}

			}
		}

		public static void RunUnitTests(Actor actor)
		{
			// TestByte(actor);
			TestInt(actor);
			TestString(actor);
		}
	}

	public interface IPlayerAttribute
	{
		private static string GetBitTag(string key, int index)
		{
			return $"{key}_ATTR_BIT_TAG_INDEX_{index}";
		}

		private static void SetBit(Player player, string key, int index, bool value)
		{
			if (value == true)
			{
				player.AddTag(GetBitTag(key, index));
			}
			else
			{
				player.RemoveTag(GetBitTag(key, index));
			}
		}

		private static bool GetBit(Player player, string key, int index)
		{
			return player.HasTag(GetBitTag(key, index));
		}

		private static void SetByte(Player player, string key, int index, byte value)
		{
			int start = index * 8;
			for (int i = 0; i < 8; i++)
			{
				SetBit(player, key, i + start, (value & (1 << (7 - i))) != 0);
			}
		}

		private static byte GetByte(Player player, string key, int index)
		{
			int start = index * 8;
			byte result = 0;

			for (int i = 0; i < 8; i++)
			{
				if (GetBit(player, key, start + i))
				{
					result |= (byte)(1 << (7 - i));  // Set the bit at position i (0 is the most significant bit)
				}
			}

			return result;
		}

		public static void SetBytesAttribute(Player player, string key, byte[] bytes, int size)
		{
			if (size < bytes.Length)
			{
				throw new Exception($"size is smaller than bytes, size={size}, byte count = {bytes.Length}");
			}

			for (int i = 0; i < size; i++)
			{
				if (i < bytes.Length)
				{
					SetByte(player, key, i, bytes[i]);
				}
				else
				{
					SetByte(player, key, i, 0);
				}
			}
		}

		public static byte[] GetBytesAttribute(Player player, string key, int size)
		{
			byte[] bytes = new byte[size];
			for (int i = 0; i < size; i++)
			{
				bytes[i] = GetByte(player, key, i);
			}
			return bytes;
		}

		public static void SetIntAttribute(Player player, string key, int value, int bits = 32)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			int size = (int)Math.Ceiling(((float)bits) / 8.0f);

			SetBytesAttribute(player, key, bytes, size);
		}

		public static int GetIntAttribute(Player player, string key, int bits = 32)
		{
			int size = (int)Math.Ceiling(((float)bits) / 8.0f);
			byte[] bytes = GetBytesAttribute(player, key, size);

			return BitConverter.ToInt32(bytes);
		}


		// ascii only
		public static void SetStringAttribute(
			Player player,
			string key,
			string value,
			int characters = 32
		)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(value);
			SetBytesAttribute(player, key, bytes, characters + 8);
		}

		public static string GetStringAttribute(Player player, string key, int characters = 32, bool trimNull = true)
		{
			byte[] bytes = GetBytesAttribute(player, key, characters + 8);

			string result = Encoding.ASCII.GetString(bytes);
			if (trimNull)
			{
				return result.Replace("\0", string.Empty);
			}
			else
			{
				return result;
			}

		}

	}

	// encodes / decodes binary data via the tags onto an actor
	public interface IAttribute
	{
		private static string GetBitTag(string key, int index)
		{
			return $"{key}_ATTR_BIT_TAG_INDEX_{index}";
		}

		private static void SetBit(Actor actor, string key, int index, bool value)
		{
			if (value == true)
			{
				actor.AddTag(GetBitTag(key, index));
			}
			else
			{
				actor.RemoveTag(GetBitTag(key, index));
			}
		}

		private static bool GetBit(Actor actor, string key, int index)
		{
			return actor.HasTag(GetBitTag(key, index));
		}

		private static void SetByte(Actor actor, string key, int index, byte value)
		{
			int start = index * 8;
			for (int i = 0; i < 8; i++)
			{
				SetBit(actor, key, i + start, (value & (1 << (7 - i))) != 0);
			}
		}

		private static byte GetByte(Actor actor, string key, int index)
		{
			int start = index * 8;
			byte result = 0;

			for (int i = 0; i < 8; i++)
			{
				if (GetBit(actor, key, start + i))
				{
					result |= (byte)(1 << (7 - i));  // Set the bit at position i (0 is the most significant bit)
				}
			}

			return result;
		}

		public static void SetBytesAttribute(Actor actor, string key, byte[] bytes, int size)
		{
			if (size < bytes.Length)
			{
				throw new Exception($"size is smaller than bytes, size={size}, byte count = {bytes.Length}");
			}

			for (int i = 0; i < size; i++)
			{
				if (i < bytes.Length)
				{
					SetByte(actor, key, i, bytes[i]);
				}
				else
				{
					SetByte(actor, key, i, 0);
				}
			}
		}

		public static byte[] GetBytesAttribute(Actor actor, string key, int size)
		{
			byte[] bytes = new byte[size];
			for (int i = 0; i < size; i++)
			{
				bytes[i] = GetByte(actor, key, i);
			}
			return bytes;
		}

		public static void SetIntAttribute(Actor actor, string key, int value, int bits = 32)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			int size = (int)Math.Ceiling(((float)bits) / 8.0f);

			SetBytesAttribute(actor, key, bytes, size);
		}

		public static int GetIntAttribute(Actor actor, string key, int bits = 32)
		{
			int size = (int)Math.Ceiling(((float)bits) / 8.0f);
			byte[] bytes = GetBytesAttribute(actor, key, size);

			return BitConverter.ToInt32(bytes);
		}

		
		// ascii only
		public static void SetStringAttribute(
			Actor actor,
			string key,
			string value,
			int characters = 32
		)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(value);
			SetBytesAttribute(actor, key, bytes, characters + 8);
		}

		public static string GetStringAttribute(Actor actor, string key, int characters = 32, bool trimNull=true)
		{
			byte[] bytes = GetBytesAttribute(actor, key, characters + 8);

			string result = Encoding.ASCII.GetString(bytes);
			if (trimNull){
				return result.Replace("\0", string.Empty);
			}else{
				return result;
			}
			
		}

	}

}
