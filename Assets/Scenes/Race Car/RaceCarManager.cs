using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Racecar {
	[ExecuteInEditMode]
	public class RaceCarManager : MonoBehaviour {
		#region Serialized fields
		[Header("Capture")]
		public new Camera camera;
		public RectTransform screen;

		[Header("Control")]
		public RaceCarSettings settings;
		public ArcadeVehicleController vehicleController;
		public Transform spawnPoint;
		public Slider XSlider, ZSlider;
		#endregion

		Vector3 rawInput, dampedInput;

		public void OnZAxis(InputValue value) {
			rawInput.z = value.Get<float>();
		}

		public void OnReset(InputValue _) {
			vehicleController.transform.position = spawnPoint.position;
			vehicleController.transform.rotation = spawnPoint.rotation;
		}

		void UpdateControlSettings() {
			vehicleController.MaxSpeed = settings.control.maxSpeed;
			vehicleController.accelaration = settings.control.acceleration;
			vehicleController.turn = settings.control.steeringTorque;
		}

		#region Life cycle
		void EditorUpdate() {
			if(spawnPoint != null && vehicleController != null) {
				vehicleController.transform.position = spawnPoint.position;
			}
		}

		void Update() {
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
		}

		void FixedUpdate() {
			// Capture
			Vector3 playerPos = Capture.PlayerPos;
			camera.transform.localPosition = playerPos;

			// Calculate FOV
			float expectedFov = Mathf.Atan2(1, Mathf.Abs(playerPos.z)) * 180 / Mathf.PI;
			camera.fieldOfView = expectedFov;

			// Control
			dampedInput.x = Capture.instance.PlayerPos.x * Mathf.Exp(settings.input.xAmplifier);
			dampedInput.z = Mathf.Lerp(dampedInput.z, rawInput.z, settings.input.zDamp);

			UpdateControlSettings();

			vehicleController.input = dampedInput;
			XSlider.value = dampedInput.x;
			ZSlider.value = dampedInput.z;
		}
		#endregion
	}
}