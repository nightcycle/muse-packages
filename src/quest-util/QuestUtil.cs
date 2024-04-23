using System;
using MuseDotNet.Framework;

namespace QuestUtil
{
	public interface IQuest
	{
		public static void ForEachQuestPlayer(QuestData questData, Action<Player> callback)
		{
			Muse.ForEachPlayer((Player player) =>
			{
				if (questData.HasPlayer(player))
				{
					callback.Invoke(player);
				}
			});
		}
	}
}