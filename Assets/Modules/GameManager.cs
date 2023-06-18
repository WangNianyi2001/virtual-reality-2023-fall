using UnityEngine;

namespace Game {
	public class GameManager : MonoBehaviour {
		public static GameManager instance;
		private static CaptureSettings staticCaptureSettings;
		public static CaptureSettings CaptureSettings {
			get {
				if(staticCaptureSettings != null)
					return staticCaptureSettings;
				staticCaptureSettings = Instantiate(instance.captureSettings);
				return staticCaptureSettings;
			}
		}

		public CaptureSettings captureSettings;
		public TextAsset cascade;

		protected void OnEnable() {
			if(instance && instance != this) {
				Destroy(gameObject);
				return;
			}
			instance = this;
			Capture.Start();
		}

		protected void OnDisable() {
			Capture.End();
		}

		protected void Start() {
			captureSettings = CaptureSettings;
		}
	}
}