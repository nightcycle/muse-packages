// written by CJ_Oyer (@nightcycle)
// a basic maid class
using System;
using System.Collections.Generic;
using MuseDotNet.Framework;
namespace Maid
{

	// the event to fire
	public class Maid {

		private readonly List<Action> Tasks = new List<Action>();
		private bool IsAlive = true;

		// goes through tasks and cleans them
		public void DoCleaning(){
			List<Action> toDoList = new List<Action>(this.Tasks);
			this.Tasks.Clear();
			foreach (Action task in toDoList){
				try{
					task();
				}catch (System.Exception ex){
					Debug.Log(LogLevel.Error, ex.Message);
				}       
			}
		}

		public void Destroy(){
			if (this.IsAlive){
				this.IsAlive = false;
				this.DoCleaning();
			}
		}

		public void GiveTask(Action task){
			if (this.IsAlive){
				this.Tasks.Add(task);
			}
		}
		
		public Maid(){

		}		
	}
}