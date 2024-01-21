// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.26
// 

using Colyseus.Schema;
using Action = System.Action;

namespace Network.StateSchema {
	public partial class MotifMotionState : Schema {
		[Type(0, "boolean")]
		public bool enableSinusoidalMotion = default(bool);

		[Type(1, "float32")]
		public float moveXAmp = default(float);

		[Type(2, "float32")]
		public float moveYAmp = default(float);

		[Type(3, "float32")]
		public float moveZAmp = default(float);

		[Type(4, "float32")]
		public float moveXFreq = default(float);

		[Type(5, "float32")]
		public float moveYFreq = default(float);

		[Type(6, "float32")]
		public float moveZFreq = default(float);

		[Type(7, "float32")]
		public float moveXPhase = default(float);

		[Type(8, "float32")]
		public float moveYPhase = default(float);

		[Type(9, "float32")]
		public float moveZPhase = default(float);

		[Type(10, "boolean")]
		public bool enableRandomMotion = default(bool);

		[Type(11, "float32")]
		public float randomProbability = default(float);

		[Type(12, "float32")]
		public float randomDistance = default(float);

		[Type(13, "boolean")]
		public bool enableLookAtTarget = default(bool);

		[Type(14, "float32")]
		public float lookAtTargetX = default(float);

		[Type(15, "float32")]
		public float lookAtTargetY = default(float);

		[Type(16, "float32")]
		public float lookAtTargetZ = default(float);

		[Type(17, "float32")]
		public float lookAtAmount = default(float);

		[Type(18, "float32")]
		public float lerpCenter = default(float);

		[Type(19, "float32")]
		public float lerpReset = default(float);

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<bool> __enableSinusoidalMotionChange;
		public Action OnEnableSinusoidalMotionChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.enableSinusoidalMotion));
			__enableSinusoidalMotionChange += __handler;
			if (__immediate && this.enableSinusoidalMotion != default(bool)) { __handler(this.enableSinusoidalMotion, default(bool)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(enableSinusoidalMotion));
				__enableSinusoidalMotionChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveXAmpChange;
		public Action OnMoveXAmpChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveXAmp));
			__moveXAmpChange += __handler;
			if (__immediate && this.moveXAmp != default(float)) { __handler(this.moveXAmp, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveXAmp));
				__moveXAmpChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveYAmpChange;
		public Action OnMoveYAmpChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveYAmp));
			__moveYAmpChange += __handler;
			if (__immediate && this.moveYAmp != default(float)) { __handler(this.moveYAmp, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveYAmp));
				__moveYAmpChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveZAmpChange;
		public Action OnMoveZAmpChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveZAmp));
			__moveZAmpChange += __handler;
			if (__immediate && this.moveZAmp != default(float)) { __handler(this.moveZAmp, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveZAmp));
				__moveZAmpChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveXFreqChange;
		public Action OnMoveXFreqChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveXFreq));
			__moveXFreqChange += __handler;
			if (__immediate && this.moveXFreq != default(float)) { __handler(this.moveXFreq, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveXFreq));
				__moveXFreqChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveYFreqChange;
		public Action OnMoveYFreqChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveYFreq));
			__moveYFreqChange += __handler;
			if (__immediate && this.moveYFreq != default(float)) { __handler(this.moveYFreq, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveYFreq));
				__moveYFreqChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveZFreqChange;
		public Action OnMoveZFreqChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveZFreq));
			__moveZFreqChange += __handler;
			if (__immediate && this.moveZFreq != default(float)) { __handler(this.moveZFreq, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveZFreq));
				__moveZFreqChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveXPhaseChange;
		public Action OnMoveXPhaseChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveXPhase));
			__moveXPhaseChange += __handler;
			if (__immediate && this.moveXPhase != default(float)) { __handler(this.moveXPhase, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveXPhase));
				__moveXPhaseChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveYPhaseChange;
		public Action OnMoveYPhaseChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveYPhase));
			__moveYPhaseChange += __handler;
			if (__immediate && this.moveYPhase != default(float)) { __handler(this.moveYPhase, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveYPhase));
				__moveYPhaseChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __moveZPhaseChange;
		public Action OnMoveZPhaseChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.moveZPhase));
			__moveZPhaseChange += __handler;
			if (__immediate && this.moveZPhase != default(float)) { __handler(this.moveZPhase, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(moveZPhase));
				__moveZPhaseChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<bool> __enableRandomMotionChange;
		public Action OnEnableRandomMotionChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.enableRandomMotion));
			__enableRandomMotionChange += __handler;
			if (__immediate && this.enableRandomMotion != default(bool)) { __handler(this.enableRandomMotion, default(bool)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(enableRandomMotion));
				__enableRandomMotionChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __randomProbabilityChange;
		public Action OnRandomProbabilityChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.randomProbability));
			__randomProbabilityChange += __handler;
			if (__immediate && this.randomProbability != default(float)) { __handler(this.randomProbability, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(randomProbability));
				__randomProbabilityChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __randomDistanceChange;
		public Action OnRandomDistanceChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.randomDistance));
			__randomDistanceChange += __handler;
			if (__immediate && this.randomDistance != default(float)) { __handler(this.randomDistance, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(randomDistance));
				__randomDistanceChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<bool> __enableLookAtTargetChange;
		public Action OnEnableLookAtTargetChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.enableLookAtTarget));
			__enableLookAtTargetChange += __handler;
			if (__immediate && this.enableLookAtTarget != default(bool)) { __handler(this.enableLookAtTarget, default(bool)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(enableLookAtTarget));
				__enableLookAtTargetChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lookAtTargetXChange;
		public Action OnLookAtTargetXChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lookAtTargetX));
			__lookAtTargetXChange += __handler;
			if (__immediate && this.lookAtTargetX != default(float)) { __handler(this.lookAtTargetX, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lookAtTargetX));
				__lookAtTargetXChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lookAtTargetYChange;
		public Action OnLookAtTargetYChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lookAtTargetY));
			__lookAtTargetYChange += __handler;
			if (__immediate && this.lookAtTargetY != default(float)) { __handler(this.lookAtTargetY, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lookAtTargetY));
				__lookAtTargetYChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lookAtTargetZChange;
		public Action OnLookAtTargetZChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lookAtTargetZ));
			__lookAtTargetZChange += __handler;
			if (__immediate && this.lookAtTargetZ != default(float)) { __handler(this.lookAtTargetZ, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lookAtTargetZ));
				__lookAtTargetZChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lookAtAmountChange;
		public Action OnLookAtAmountChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lookAtAmount));
			__lookAtAmountChange += __handler;
			if (__immediate && this.lookAtAmount != default(float)) { __handler(this.lookAtAmount, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lookAtAmount));
				__lookAtAmountChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lerpCenterChange;
		public Action OnLerpCenterChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lerpCenter));
			__lerpCenterChange += __handler;
			if (__immediate && this.lerpCenter != default(float)) { __handler(this.lerpCenter, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lerpCenter));
				__lerpCenterChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __lerpResetChange;
		public Action OnLerpResetChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.lerpReset));
			__lerpResetChange += __handler;
			if (__immediate && this.lerpReset != default(float)) { __handler(this.lerpReset, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(lerpReset));
				__lerpResetChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(enableSinusoidalMotion): __enableSinusoidalMotionChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
				case nameof(moveXAmp): __moveXAmpChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveYAmp): __moveYAmpChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveZAmp): __moveZAmpChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveXFreq): __moveXFreqChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveYFreq): __moveYFreqChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveZFreq): __moveZFreqChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveXPhase): __moveXPhaseChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveYPhase): __moveYPhaseChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(moveZPhase): __moveZPhaseChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(enableRandomMotion): __enableRandomMotionChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
				case nameof(randomProbability): __randomProbabilityChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(randomDistance): __randomDistanceChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(enableLookAtTarget): __enableLookAtTargetChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
				case nameof(lookAtTargetX): __lookAtTargetXChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(lookAtTargetY): __lookAtTargetYChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(lookAtTargetZ): __lookAtTargetZChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(lookAtAmount): __lookAtAmountChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(lerpCenter): __lerpCenterChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(lerpReset): __lerpResetChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
