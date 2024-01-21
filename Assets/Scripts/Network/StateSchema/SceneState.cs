// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.26
// 

using Colyseus.Schema;
using Action = System.Action;

namespace Network.StateSchema {
	public partial class SceneState : Schema {
		[Type(0, "map", typeof(MapSchema<ArtworkState>))]
		public MapSchema<ArtworkState> artworks = new MapSchema<ArtworkState>();

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<MapSchema<ArtworkState>> __artworksChange;
		public Action OnArtworksChange(PropertyChangeHandler<MapSchema<ArtworkState>> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.artworks));
			__artworksChange += __handler;
			if (__immediate && this.artworks != null) { __handler(this.artworks, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(artworks));
				__artworksChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(artworks): __artworksChange?.Invoke((MapSchema<ArtworkState>) change.Value, (MapSchema<ArtworkState>) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
