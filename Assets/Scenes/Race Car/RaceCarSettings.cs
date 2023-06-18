using UnityEngine;
using System;

namespace Game.Racecar {
	[CreateAssetMenu(menuName = "Game/Race Car Settings")]
	public class RaceCarSettings : ScriptableObject {
		[Serializable]
		public struct Input {
			[Range(-1f, 1f)] public float xAmplifier;
			[Range(0, 1)] public float zDamp;
		}
		public Input input;

		[Serializable]
		public struct Control {
			[Range(10, 100)] public float maxSpeed;
			[Range(5, 30)] public float acceleration;
			[Range(1, 20)] public float steeringTorque;
		}
		public Control control;
	}
}