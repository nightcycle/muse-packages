using System;
using System.Collections.Generic;
namespace DataServiceProvider
{
	public readonly struct DataSlice(
		string key,
		int minValue,
		int maxValue
	)
	{
		public readonly string Key = key;
		public readonly int MinValue = minValue;
		public readonly int MaxValue = maxValue;
	}

	public class DataService
	{
		private DataSlice[] Slices = [];
		private Dictionary<string, int> InitialData = [];

		private static int GetBitCount(DataSlice slice)
		{
			int range = slice.MaxValue - slice.MinValue + 1;
			return (int)Math.Ceiling(Math.Log2((float)range));
		}

		public long Encode(Dictionary<string, int> data)
		{
			long value = 0;

			int cursor = 0;
			for (int i = 0; i < Slices.Length; i++)
			{
				DataSlice slice = Slices[i];
				int size = GetBitCount(slice);

				if (data[slice.Key] > slice.MaxValue){
					throw new Exception($"value of {data[slice.Key]} at {slice.Key} is too large, max is {slice.MaxValue}");
				}
				if (data[slice.Key] < slice.MinValue){
					throw new Exception($"value of {data[slice.Key]} at {slice.Key} is too small, min is {slice.MinValue}");
				}
				int v = data[slice.Key] - slice.MinValue;

				// Ensure v fits within the size bits
				int mask = (1 << size) - 1;
				v &= mask;

				// Shift v left by cursor position and add to the current value
				value |= (long)v << cursor;

				cursor += size;
			}

			return value;
		}

		public Dictionary<string, int> Decode(long value)
		{
			Dictionary<string, int> decodedData = new(InitialData);
			int cursor = 0;

			foreach (var slice in Slices)
			{
				int size = GetBitCount(slice);
				int mask = (1 << size) - 1;

				// Extract the relevant bits from value using mask and cursor
				int extractedValue = (int)((value >> cursor) & mask);

				// Calculate the actual value using the minValue
				extractedValue += slice.MinValue;

				// Store the result in the dictionary
				decodedData[slice.Key] = extractedValue;

				cursor += size;
			}

			return decodedData;
		}

		public void Start(
			Dictionary<string, int> initialData,
			DataSlice[] slices
		)
		{
			InitialData = initialData;
			Slices = slices;
		}

		// set up as singleton
		private static DataService instance;
		public static DataService Instance
		{
			get
			{
				instance ??= new DataService();
				return instance;
			}
		}
		private DataService() { }
	}
}