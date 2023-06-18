using UnityEngine;
using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game {
	public class CaptureInstance : MonoBehaviour {
		#region Core fields
		VideoCapture capture;
		Mat buffer;
		CascadeClassifier cascade;
		Coroutine coroutine;
		List<UnityEngine.Rect> detectedFaces;
		Vector2Int size;
		KalmanFilterVector3 playerPosKalman;
		Vector3 playerPos;
		#endregion

		#region Core methods
		static GameManager manager => GameManager.instance;
		CaptureSettings settings => manager.captureSettings;

		Vector3? InstantPlayerPos {
			set {
				if(!value.HasValue)
					return;
				Vector3 temp = playerPosKalman.Update(value.Value, settings.capture.kalmanQ, settings.capture.kalmanR);
				temp = settings.compensation * temp;
				playerPos = temp;
			}
		}

		IEnumerator CaptureCoroutine() {
			using(buffer) {
				while(true) {
					bool success = capture.Read(buffer);
					if(!success) {
						Debug.LogWarning("Capture failed");
						break;
					}
					if(buffer.Empty())
						break;
					detectedFaces = cascade
						.DetectMultiScale(buffer)
						.Select(rect => new UnityEngine.Rect{
							width = rect.Width,
							height = rect.Height,
							x = rect.X,
							y = rect.Y,
						})
						.ToList();
					InstantPlayerPos = AnalyzePlayerPosFromFaceRect(MainFaceRect);
					yield return new WaitForSeconds(Mathf.Exp(-settings.capture.frequency));
				}
			}
			enabled = false;
			Destroy(gameObject);
		}
		#endregion

		#region Public interfaces
		public Vector2Int Size => size;
		public List<UnityEngine.Rect> DetectedFaces => detectedFaces;
		public Vector3 PlayerPos => playerPos;

		public UnityEngine.Rect? MainFaceRect {
			get {
				var faces = DetectedFaces;
				if(faces == null || faces.Count == 0)
					return null;
				var biggest = faces.Aggregate((a, b) => Util.RectArea(a) > Util.RectArea(b) ? a : b);
				return biggest.Area() > 0 ? biggest : null;
			}
		}

		public Vector3? AnalyzePlayerPosFromFaceRect(UnityEngine.Rect? rawRect) {
			if(rawRect == null)
				return null;

			Vector2Int frameSize = Size;
			float captureArea = frameSize.x * frameSize.y;
			float actualHeadArea = rawRect.Value.Area() / captureArea;
			float z = -settings.capture.standardHeadArea / actualHeadArea;

			Vector2 xy = rawRect.Value.center;
			xy.x /= frameSize.x;
			xy.y /= frameSize.y;
			xy -= Vector2.one * .5f;
			xy *= z;

			Vector3 result = new Vector3(xy.x, xy.y, z);

			return result;
		}
		#endregion

		#region Life cycle
		void OnEnable() {
			capture = new VideoCapture(0);
			size = Vector2Int.FloorToInt(new Vector2(capture.FrameWidth, capture.FrameHeight) * settings.capture.rate);
			capture.FrameWidth = size.x;
			capture.FrameHeight = size.y;
			buffer = new Mat(size.x, size.y, MatType.CV_32SC4);
			capture.AutoExposure = 1;
			capture.AutoFocus = true;
			cascade = new CascadeClassifier(GameManager.instance.cascade);
			playerPosKalman = new KalmanFilterVector3(settings.capture.kalmanQ, settings.capture.kalmanR);

			coroutine = StartCoroutine(CaptureCoroutine());
		}

		void OnDisable() {
			if(coroutine != null)
				StopCoroutine(coroutine);
			cascade?.Dispose();
			capture?.Dispose();
			buffer?.Dispose();
		}
		#endregion
	}
}