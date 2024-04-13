// written by CJ_Oyer (@nightcycle)
// a hacky way to get a 30 FPS update loop
// simply create a spawner, add this as the code for it
// have it spawn a barrel at a 0.01 interval (it'll round up to 0.033333 internally)
// requires the Signal class I wrote earlier, be sure to disconnect any connections you make when you're done with them
// example use from any other script:
//
// StepService.Instance.OnStep.Connect((double deltaTime) => {
// 	Debug.Log(LogLevel.Display, $"step {deltaTime}!");
// });
//
using System;
using MuseDotNet.Framework;
using Packages;
namespace StepService
{
	public class StepService
	{
		private static readonly StepService instance = new StepService();
		public Signal<double> OnStep = new();
		public double Time = 0;

		private bool HasStarted = false;
		private readonly Timer Timer = new();
		private double LastUpdate = 0;


		public void Start(Actor volume){
			if (!HasStarted){
				HasStarted = true;
				Timer.Start();
				volume.SetOnActorSpawnedCallback((Actor spawnedActor) => {
					Time = Timer.ElapsedSeconds;

					double deltaTime = Time - LastUpdate;
					if (deltaTime > 0.005){
						LastUpdate = Time;
						OnStep.Fire(deltaTime);
					}

					spawnedActor.Remove();
				});
			}else{
				throw new Exception("'StepService.Instance.Start()' has already been called");
			}
		}

		// set up as singleton
		private static StepService instance;
		public static StepService Instance
		{
			get
			{
				instance ??= new StepService();
				return instance;
			}
		}
		private StepService(){}
	}
}
