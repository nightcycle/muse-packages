// written by CJ_Oyer (@nightcycle)
// A way to create threads utilizing the timer callbacks. 
// Just create a spawner, and have it enqueue whatever it spawns into TaskQueue.Queue

using System;
using System.Collections.Generic;
using MuseDotNet.Framework;

namespace TaskUtil
{
	public class TaskQueue
	{
		public static Queue<Actor> Queue = new();
	}

	public interface ITask
	{
		public static bool Delay(float delay, Action callback)
		{
			if (TaskQueue.Queue.TryDequeue(out Actor actor))
			{
				actor.SetOnTimerCallback(() =>
				{
					TaskQueue.Queue.Enqueue(actor);
					callback.Invoke();
				}, 1, delay, false);

				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool Spawn(Action callback)
		{
			if (TaskQueue.Queue.TryDequeue(out Actor actor))
			{
				actor.SetOnTimerCallback(() =>
				{
					TaskQueue.Queue.Enqueue(actor);
					callback.Invoke();
				}, 1, 0, false);

				return true;
			}
			else
			{
				return false;
			}
		}
	}

}