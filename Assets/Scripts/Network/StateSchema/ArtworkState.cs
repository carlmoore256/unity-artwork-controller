// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.26
// 

using Colyseus.Schema;
using Action = System.Action;

namespace Network.StateSchema {
	public partial class ArtworkState : Schema {
		[Type(0, "string")]
		public string name = default(string);

		[Type(1, "ref", typeof(MotifMotionState))]
		public MotifMotionState motifMotion = new MotifMotionState();

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __nameChange;
		public Action OnNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.name));
			__nameChange += __handler;
			if (__immediate && this.name != default(string)) { __handler(this.name, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(name));
				__nameChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<MotifMotionState> __motifMotionChange;
		public Action OnMotifMotionChange(PropertyChangeHandler<MotifMotionState> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.motifMotion));
			__motifMotionChange += __handler;
			if (__immediate && this.motifMotion != null) { __handler(this.motifMotion, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(motifMotion));
				__motifMotionChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(name): __nameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(motifMotion): __motifMotionChange?.Invoke((MotifMotionState) change.Value, (MotifMotionState) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
