// a rust inspired quick workaround to the "no-null" limitation
using System;
using System.Collections.Generic;

namespace Option{
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
			if (GetIfSafe()){
				this.Container = [];
				this.OnClear();
			}
		}

		public T Get(){
			if (GetIfEmpty()){
				throw new SystemException("Option is null, use GetIfNull before call");
			}else{
				return this.Container[0];
			}
		}

		public bool TryInvoke(Action<T> onInvoke){
			if (GetIfSafe())
			{
				onInvoke.Invoke(Get());
				return true;
			}
			else
			{
				return false;
			}
		}

		// public bool TryGet(out T value){
		// 	if (GetIfSafe())
		// 	{
		// 		value = Get();
		// 		return true;
		// 	}
		// 	else
		// 	{
		// 		value = default(T);
		// 		return false;
		// 	}
		// }
	}
}
