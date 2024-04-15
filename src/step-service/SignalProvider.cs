// downloaded from 'https://github.com/nightcycle/muse-packages' and compiled using 'github.com/nightcycle/muse-package-manager'

using System;
using System.Collections.Generic;
namespace SignalProvider
{
	// the bound functionality, needs to be disconnected to avoid memory leaks
	public class SignalConnection<V>(Action<V> onInvoke, Action<SignalConnection<V>> onDisconnectInvoke)
	{
		private readonly Action<SignalConnection<V>> OnDisconnectInvoke = onDisconnectInvoke;
		public Action<V> OnInvoke = onInvoke;
		public void Disconnect(){
			this.OnDisconnectInvoke(this);
		}
	}
	// the event to fire
	public class Signal<V> {
		// list of connections the signal iterates through when firing
		private readonly List<SignalConnection<V>> Connections = new();
		private bool IsAlive = true;
		// the internal method passed to all connections to allow them to disconnect
		private void Disconnect(SignalConnection<V> connection){
			if (this.Connections.Contains(connection) == true){
				this.Connections.Remove(connection);
			}
		}
		// get if this signal has anything connected
		public bool HasConnections(){
			return this.Connections.Count > 0;
		}
		// fire all the connections
		public void Fire(V value){
			List<SignalConnection<V>> connections = new(this.Connections);
			foreach (SignalConnection<V> connection in connections){
				connection.OnInvoke.Invoke(value);
			}
		}
		// create a signal connection bound to a specific action
		public SignalConnection<V> Connect(Action<V> onInvoke){
			SignalConnection<V> connection = new SignalConnection<V>(onInvoke, this.Disconnect);
			this.Connections.Add(connection);
			return connection;
		}
		public void Destroy(){
			if (this.IsAlive){
				this.IsAlive = false;
				List<SignalConnection<V>> connections = new(this.Connections);
				foreach (SignalConnection<V> connection in connections){
					connection.Disconnect();
				}
			}
		}
		
		public Signal(){}		
	}
}