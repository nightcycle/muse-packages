// Allows for spawning of arbitrary actors via tag at arbitrary spawn points
using System;
using System.Collections.Generic;
using MuseDotNet.Framework;
namespace UpgradeSystem
{

	public class SpawnService
	{
		private readonly Dictionary<string, Queue<Actor>> SpawnableRegistry = [];
		public void EnqueueSpawnable(string tag, Actor spawnable)
		{
			if (SpawnableRegistry.ContainsKey(tag) == false)
			{
				SpawnableRegistry[tag] = [];
			}
			spawnable.SetActive(false);
			SpawnableRegistry[tag].Enqueue(spawnable);
		}

		public bool GetIfTagExists(string tag){
			return SpawnableRegistry.ContainsKey(tag);
		}


		public bool TrySpawn(string tag, Actor spawnPoint, out Actor spawnedActor)
		{
			if (SpawnableRegistry.TryGet(out Queue<Actor> spawnableQueue))
			{
				if (spawnableQueue.TryDequeue(out Actor actor))
				{
					actor.SetActive(true);
					bool isTeleportSuccess = actor.TeleportTo(spawnPoint);
					if (isTeleportSuccess){
						spawnedActor = actor;
						return true
					}else{
						EnqueueSpawnable(tag, actor);
					}
				}
			}
			spawnedActor = default(Actor);
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