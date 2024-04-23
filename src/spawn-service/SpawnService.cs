using System;
using System.Collections.Generic;
using MuseDotNet.Framework;
namespace UpgradeSystem
{

	public class SpawnService
	{
		private readonly Dictionary<string, Queue<Actor>> SpawnableRegistry = [];
		private readonly List<Actor> SpawnPoints = [];

		public void EnqueueSpawnable(string tag, Actor spawnable)
		{
			if (SpawnableRegistry.ContainsKey(tag) == false)
			{
				SpawnableRegistry[tag] = [];
			}
			spawnable.SetOnTimerCallback(() =>
			{
				// Debug.Log(LogLevel.Display, $"delayed deactivate = {tag}");
				if (SpawnableRegistry.ContainsKey(tag) == true)
				{
					if (SpawnableRegistry[tag].Contains(spawnable))
					{
						spawnable.SetActive(false);
					}
				}
			}, 1, 1, false);

			SpawnableRegistry[tag].Enqueue(spawnable);
		}

		public bool RegisterSpawnPoint(Actor spawnPoint){
			if (SpawnPoints.Contains(spawnPoint)){
				SpawnPoints.Add(spawnPoint);
				return true;
			}
			return false;
		}

		public Actor GetRandomSpawnPoint()
		{
			Actor[] spawnPoints = SpawnPoints.ToArray();
			Random rng = new();

			int index = rng.Next(0, spawnPoints.Length - 1);
			Actor spawnPoint = spawnPoints[index];
			return spawnPoint;
		}

		public bool Spawn(string tag, Actor spawnPoint, Action<Actor> onSpawnCallback)
		{
			if (SpawnableRegistry.ContainsKey(tag) == true)
			{

				Queue<Actor> spawnableQueue = SpawnableRegistry[tag];

				if (spawnableQueue.Count > 0)
				{
					Actor spawned = spawnableQueue.Dequeue();
					spawned.SetActive(true);
					spawned.TeleportTo(spawnPoint);
					onSpawnCallback.Invoke(spawned);

					return true;
				}
				else
				{
					Debug.Log(LogLevel.Warning, $"out of spawnable actors with tag {tag}");
				}
			}
			return false;
		}

		// set up as singleton
		private static readonly SpawnService instance = new();
		public static SpawnService Instance
		{
			get
			{
				return instance;
			}
		}

		private SpawnService() { }
	}
}