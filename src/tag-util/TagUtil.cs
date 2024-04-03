using System.Collections.Generic;
using MuseDotNet.Framework;
namespace TagUtil
{	

	public interface ITag {
		static public Actor GetFirstActorFromTag(string tag){
			object taggedActor = null;
			Muse.ForEachActor((Actor actor) => {
				if (actor.HasTag(tag)){
					taggedActor = actor;
				}
			});
			switch(taggedActor){
				case Actor actor:
					return actor;
				default:
					throw new System.Exception($"no actor exists with tag '{tag}'");			
			}
		}

		static public Actor GetFirstActorFromUnionTag(string tagA, string tagB){
			object taggedActor = null;
			Muse.ForEachActor((Actor actor) => {
				if (actor.HasTag(tagA) && actor.HasTag(tagB)){
					taggedActor = actor;
				}
			});
			switch(taggedActor){
				case Actor actor:
					return actor;
				default:
					throw new System.Exception($"no actor exists with tag '{tagA}' and '{tagB}'");			
			}
		}
		static public List<Actor> GetTagged(string tag){
			List<Actor> taggedActors = [];
			Muse.ForEachActor((Actor actor) => {
				if (actor.HasTag(tag)){
					taggedActors.Add(actor);
				}
			});
			return taggedActors;
		}
		static public List<Actor> GetUnionTagged(string tagA, string tagB){
			List<Actor> taggedActors = [];
			Muse.ForEachActor((Actor actor) => {
				if (actor.HasTag(tagA) && actor.HasTag(tagB)){
					taggedActors.Add(actor);
				}
			});
			return taggedActors;
		}
	}
}
