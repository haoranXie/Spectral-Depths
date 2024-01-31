using UnityEngine;

namespace SpectralDepths.Tools
{
	public class PLReorderableAttributeAttribute : PropertyAttribute {

		public bool add;
		public bool remove;
		public bool draggable;
		public bool singleLine;
		public string elementNameProperty;
		public string elementNameOverride;
		public string elementIconPath;

		public PLReorderableAttributeAttribute()
			: this(null) {
		}

		public PLReorderableAttributeAttribute(string elementNameProperty)
			: this(true, true, true, elementNameProperty, null, null) {
		}

		public PLReorderableAttributeAttribute(string elementNameProperty, string elementIconPath)
			: this(true, true, true, elementNameProperty, null, elementIconPath) {
		}

		public PLReorderableAttributeAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
			: this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath) {
		}

		public PLReorderableAttributeAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementIconPath = null) 
			: this(add, remove, draggable, elementNameProperty, null, elementIconPath) {
		}

		public PLReorderableAttributeAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null) {

			this.add = add;
			this.remove = remove;
			this.draggable = draggable;
			this.elementNameProperty = elementNameProperty;
			this.elementNameOverride = elementNameOverride;
			this.elementIconPath = elementIconPath;
		}
	}
}