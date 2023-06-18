using UnityEngine;
using System;

namespace Game {
	[CreateAssetMenu(menuName = "Game/Capture Settings")]
	public class CaptureSettings : ScriptableObject {
		[Serializable]
		public struct Capture {
			/// <summary>采集图像的频率</summary>
			/// <remarks>实际时间间隔为 exp(-frequency)。</remarks>
			[Range(0, 3)] public float frequency;
			/// <summary>玩家距离屏幕一单位远时头所占全部摄像头画面的比例</summary>
			[Range(0, 1)] public float standardHeadArea;
			/// <summary>采样率</summary>
			[Range(.2f, 1)] public float rate;
			[Min(0)] public float kalmanQ;
			[Min(0)] public float kalmanR;
		}
		public Capture capture;

		[NonSerialized] public Matrix4x4 compensation;

		public CaptureSettings() {
			compensation = Matrix4x4.identity;
		}
	}
}
