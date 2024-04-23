using System.Collections.Generic;
using MuseDotNet.Framework;
namespace PlayerUtil
{	
	public interface IPlayer {
		static public Player GetPlayerFromName(string name){
			object namedPlayer = null;
			Muse.ForEachPlayer((Player player) => {
				if (player.Name == name){
					namedPlayer = player;
				}
			});
			switch(namedPlayer){
				case Player player:
					return player;
				default:
					throw new System.Exception($"no player exists of the name '{name}'");			
			}
		}
		static public bool GetIfPlayerExists(string name){
			bool playerExists = false;
			Muse.ForEachPlayer((Player player) => {
				if (player.Name == name){
					playerExists = true;
				}
			});
			return playerExists;
		}
	}
}