// a unit testing solution
using System;
using System.Collections.Generic;
using OptionProvider;
using MuseDotNet.Framework;
namespace TestServiceProvider
{

	public class TestService
	{
		private readonly Dictionary<string, UnitTest> TestRegistry = [];

		private enum TestResult
		{
			MissingDependency,
			NotRunYet,
			Blocked,
			Fail,
			Success
		}

		private class UnitTest(
				string key,
				Func<int, bool> test,
				string[] dependencies,
				int testCount
			)
		{
			public readonly string Key = key;
			public readonly string[] Dependencies = dependencies;
			public readonly int TestCount = testCount;
			public readonly Func<int, bool> Test = test;
			private readonly Option<TestResult> Result = new();

			public override string ToString()
			{
				return $"\"{Key}\" -> {GetResult()}";
			}

			public TestResult GetResult()
			{
				if (Result.GetIfSafe())
				{
					return Result.Get();
				}
				else
				{
					return TestResult.NotRunYet;
				}
			}

			public TestResult Run()
			{
				if (Result.GetIfSafe())
				{
					return GetResult();
				}
				else
				{
					foreach (string testKey in Dependencies)
					{
						if (Instance.TestRegistry.TryGetValue(testKey, out UnitTest test))
						{
							TestResult result = test.GetResult();
							if (result == TestResult.NotRunYet)
							{
								result = test.Run();
							}
							if (result == TestResult.Blocked || result == TestResult.Fail || result == TestResult.MissingDependency)
							{
								Result.Set(TestResult.Blocked);
								return TestResult.Blocked;
							}
						}
						else
						{
							return TestResult.MissingDependency;
						}
					}
					for (int i = 0; i < TestCount; i++)
					{
						bool isSuccess;
						try
						{
							isSuccess = Test.Invoke(i);
						}
						catch
						{
							isSuccess = false;
						}

						if (!isSuccess)
						{
							Result.Set(TestResult.Fail);
							return TestResult.Fail;
						}
					}

					Result.Set(TestResult.Success);
					return TestResult.Success;
				}
			}
		}

		public void CreateUnitTest(
			string key,
			Func<int, bool> test,
			string[] dependencies,
			int testCount = 1
		)
		{
			if (TestRegistry.TryGetValue(key, out UnitTest unitTest))
			{
				throw new Exception($"key \"{key}\" already assigned to");
			}
			else
			{
				TestRegistry[key] = new(key, test, dependencies, testCount);
			}
		}

		public Queue<string> GetTestQueue()
		{
			List<string> keys = [];
			void runKeyPass(int attempt = 0)
			{
				bool isDone = true;
				foreach (KeyValuePair<string, UnitTest> item in TestRegistry)
				{
					if (!keys.Contains(item.Key))
					{
						bool isAllAdded = true;
						foreach (string dep in item.Value.Dependencies)
						{
							if (!keys.Contains(dep))
							{
								isAllAdded = false;
							}
						}
						if (isAllAdded)
						{
							keys.Add(item.Key);
						}
						else
						{
							isDone = false;
						}
					}
				}
				if (!isDone)
				{
					if (attempt < 10)
					{
						runKeyPass(attempt + 1);
					}
					else
					{
						throw new Exception($"recursion passed 10 layers={attempt}");
					}
				}
			}
			runKeyPass();


			return new(keys.ToArray());
		}

		public bool RunTest(
			string key,
			bool isLogDebugEnabled = true
		)
		{
			if (TestRegistry.TryGetValue(key, out UnitTest test))
			{
				TestResult result = test.Run();
				if (isLogDebugEnabled)
				{
					if (result == TestResult.Success)
					{
						Debug.Log(LogLevel.Display, $"{test}");
					}
					else if (result == TestResult.Fail || result == TestResult.MissingDependency)
					{
						Debug.Log(LogLevel.Error, $"{test}");
					}
					else
					{
						Debug.Log(LogLevel.Warning, $"{test}");
					}
				}
				return result == TestResult.Success;
			}
			else if (isLogDebugEnabled)
			{
				Debug.Log(LogLevel.Error, $"no test found with key \"{key}\"");
			}
			return false;
		}

		// set up as singleton
		private static TestService instance;
		public static TestService Instance
		{
			get
			{
				instance ??= new TestService();
				return instance;
			}
		}
		private TestService() { }
	}
}