// written by CJ_Oyer (@nightcycle)
// a way to call "GetTags" on an actor
//
using System;
using MuseDotNet.Framework;
using System.Collections.Generic;
namespace TagService
{
	public class TagService
	{
		private readonly Dictionary<Actor, List<string>> ActorTagRegistry = [];
		private readonly Dictionary<string, List<Actor>> TagActorRegistry = [];
		private readonly Dictionary<string, Action<Actor>> OnTagAddedCallbackRegistry = [];
		private readonly Dictionary<string, Action<Actor>> OnTagRemovedCallbackRegistry = [];

		private bool HasStarted = false;
		private string[] InitialTags = [];

		public void SetOnTagAdded(string tag, Action<Actor> callback){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			OnTagAddedCallbackRegistry[tag] = callback;

			if (TagActorRegistry.TryGetValue(tag, out List<Actor> actors)){
				List<Actor> actorsCopy = new(actors);
				foreach (Actor actor in actorsCopy){
					callback(actor);
				}
			}
		}

		public void SetOnTagRemoved(string tag, Action<Actor> callback){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			OnTagRemovedCallbackRegistry[tag] = callback;
		}

		public void AddTag(Actor actor, string tag){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			if (ActorTagRegistry.ContainsKey(actor) == false){
				ActorTagRegistry[actor] = [];
			}
			if (ActorTagRegistry[actor].Contains(tag) == false){
				ActorTagRegistry[actor].Add(tag);
			}

			if (TagActorRegistry.ContainsKey(tag) == false){
				TagActorRegistry[tag] = [];
			}
			if (TagActorRegistry[tag].Contains(actor) == false){
				TagActorRegistry[tag].Add(actor);
			}
			if (OnTagAddedCallbackRegistry.TryGetValue(tag, out Action<Actor> callback)){
				callback(actor);
			}
		}

		public void RemoveTag(Actor actor, string tag){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			if (ActorTagRegistry.ContainsKey(actor) == true){
				ActorTagRegistry[actor].Remove(tag);
				if (ActorTagRegistry[actor].Count <= 0){
					ActorTagRegistry.Remove(actor);
				}
			}
			if (TagActorRegistry.ContainsKey(tag) == true){
				TagActorRegistry[tag].Remove(actor);
				if (TagActorRegistry[tag].Count <= 0){
					TagActorRegistry.Remove(tag);
				}
			}
			if (OnTagRemovedCallbackRegistry.TryGetValue(tag, out Action<Actor> callback)){
				callback(actor);
			}
		}

		public bool HasTag(Actor actor, string tag){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			if (ActorTagRegistry.ContainsKey(actor) == true){
				return ActorTagRegistry[actor].Contains(tag);
			}else{
				return false;
			}
		}

		public string[] GetTags(Actor actor){
			if (!HasStarted){
				throw new System.Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			if (ActorTagRegistry.ContainsKey(actor) == true){
				return [.. ActorTagRegistry[actor]];
			}else{
				return [];
			}
		}

		public Actor[] GetTagged(string tag){
			if (!HasStarted){
				throw new Exception("TagService hasn't been started, call 'TagService.Instance.Start()'");
			}
			if (TagActorRegistry.ContainsKey(tag) == true){
				return [.. TagActorRegistry[tag]];
			}else{
				return [];
			}
		}

		public void Register(Actor actor){
			if (HasStarted && ActorTagRegistry.ContainsKey(actor) == false){
				foreach (string tag in InitialTags){
					if (actor.HasTag(tag)){
						AddTag(actor, tag);
					}
				}
			}
		}

		public void Deregister(Actor actor){
			if (HasStarted && ActorTagRegistry.TryGetValue(actor, out List<string> tags)){
				List<string> tagsCopy = new(tags);
				foreach (string tag in tagsCopy){
					if (HasTag(actor, tag)){
						RemoveTag(actor, tag);
					}
				}
			}
		}

		public void Start(string[] initialTags, bool skipMassRegistry=false){
			if (HasStarted == false){
				InitialTags = initialTags;
				HasStarted = true;
				if (!skipMassRegistry){
					Muse.ForEachActor((Actor actor) => {
						Register(actor);	
					});
				}
			}else{
				throw new Exception("'TagService.Instance.Start()' has already been called");
			}
		}

		// set up as singleton
		private static TagService instance;
		public static TagService Instance
		{
			get
			{
				instance ??= new TagService();
				return instance;
			}
		}
		private TagService(){}
	}
}
