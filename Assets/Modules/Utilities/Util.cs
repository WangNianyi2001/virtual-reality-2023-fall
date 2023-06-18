using UnityEngine;

namespace Game {
	public static class Util {
		public static float RectArea(Rect rect) => rect.width * rect.height;
		public static float Area(this Rect rect) => RectArea(rect);
	}
}
