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

using MuseDotNet.Framework;
using Packages;
namespace StepService
{
	public class StepService
	{
		private static readonly StepService instance = new StepService();
		public Signal<double> OnStep = new();
		public double Time = 0;

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
	

	public class VolumeScriptNameHere : Spawner
	{
		readonly Timer Timer = new();
		double LastUpdate = 0;

		protected override void OnBegin()
		{
			base.OnBegin();
			Timer.Start();
		}

		protected override void OnEnd()
		{
			base.OnEnd();
		}

		public override Actor Spawn()
		{

			StepService.Instance.Time = Timer.ElapsedSeconds;
			double deltaTime = StepService.Instance.Time - LastUpdate;
			if (deltaTime > 0.005){
				LastUpdate = StepService.Instance.Time;
				StepService.Instance.OnStep.Fire(deltaTime);
			}


			Actor actor = base.Spawn();
			actor.Remove();

			return actor;
		}
	}
}
