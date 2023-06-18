using UnityEngine;

namespace Game {
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(PostProcess))]
	public class SkewCamera : MonoBehaviour {
		static Shader skewShader;

		Material skew;

		public RectTransform screen;
		PostProcess pp;
		new Camera camera;

		Vector3 ScreenToWorld(Vector2 screenPos) =>
			screen.localToWorldMatrix.MultiplyPoint(Vector2.Scale(screen.sizeDelta, screenPos - Vector2.one * .5f));
		Vector2 ScreenToViewport(Vector2 screenPos) =>
			camera.WorldToViewportPoint(ScreenToWorld(screenPos));
		Vector2 ScreenToViewport(float x, float y) => ScreenToViewport(new Vector2(x, y));

		void Awake() {
			skewShader = Shader.Find("Skew");
		}

		void OnEnable() {
			pp = GetComponent<PostProcess>();
			camera = GetComponent<Camera>();

			skew = new Material(skewShader);
			pp.materials.Add(skew);

			Vector3 size = GameManager.CaptureSettings.compensation.MultiplyVector(Vector3.one);
			size /= size.z;
			size.x = 1 / size.x;
			size.y = 1 / size.y;
			size.z = 0;
			size /= size.magnitude;
			screen.sizeDelta = size;
		}

		void OnDisable() {
			Destroy(skew);
		}

		void FixedUpdate() {
			camera.fieldOfView = 90 * Mathf.Atan(Mathf.Abs(2 / camera.transform.localPosition.z)) / Mathf.PI;
			skew.SetVector("tl", ScreenToViewport(0, 0));
			skew.SetVector("tr", ScreenToViewport(1, 0));
			skew.SetVector("bl", ScreenToViewport(0, 1));
			skew.SetVector("br", ScreenToViewport(1, 1));
		}
	}
}