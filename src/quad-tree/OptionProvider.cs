// downloaded from 'https://github.com/nightcycle/muse-packages' and compiled using 'github.com/nightcycle/muse-package-manager'

using System.Collections.Generic;
using System;
namespace OptionProvider
{
	public class Option<T>{
		private Dictionary<int, T> Container = [];
		private Action<T> OnSet = (T value) => {};
		private Action OnClear = () => {};
		public void Set(T value){
			this.Container[0] = value;
			OnSet(value);
		}
		public bool GetIfEmpty(){
			return this.Container.ContainsKey(0) == false;
		}
		public bool GetIfSafe(){
			return !GetIfEmpty();
		}
		public void SetOnSetCallback(
			Action<T> callback
		){
			OnSet = callback;
			if (GetIfEmpty() == false){
				callback(Get());
			}
		}
		public void SetOnClearCallback(
			Action callback
		){
			OnClear = callback;
		}
		public void Clear(){
			this.Container = [];
			this.OnClear();
		}
		public T Get(){
			if (GetIfEmpty()){
				throw new SystemException("Option is null, use GetIfNull before call");
			}else{
				return this.Container[0];
			}
		}
	}
}