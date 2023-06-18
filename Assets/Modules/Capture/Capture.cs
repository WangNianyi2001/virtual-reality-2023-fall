using UnityEngine;

namespace Game {
	public static class Capture {
		public static CaptureInstance instance;

		public static Vector3 PlayerPos => instance.PlayerPos;

		public static void Start() {
			if(instance != null)
				return;
			GameObject obj = new GameObject("Capture Instance");
			instance = obj.AddComponent<CaptureInstance>();
			instance.enabled = true;
		}
		public static void End() {
			if(instance == null)
				return;
			instance.enabled = false;
			Object.Destroy(instance.gameObject);
			instance = null;
		}
	}
}