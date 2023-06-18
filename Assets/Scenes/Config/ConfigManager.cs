using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Game {
	public class ConfigManager : MonoBehaviour {
		public static ConfigManager instance;

		#region Serialized fields
		[SerializeField] private TMP_Text promptText;
		[SerializeField] private TMP_Text continueText;
		public List<Nianyi.Callback> instructions;
		public RectTransform captureFrame;
		#endregion

		#region Internal fields
		private int nextInstructionIndex = 0;
		Vector3 c1, c2, c3;
		#endregion

		#region Internal functions
		private IEnumerator PerformNextInstructionCoroutine() {
			if(nextInstructionIndex < 0 || nextInstructionIndex >= instructions.Count) {
				nextInstructionIndex = -1;
				yield break;
			}
			yield return instructions[nextInstructionIndex]?.InvokeAsync();
			++nextInstructionIndex;
		}
		#endregion

		#region Public interfaces
		public string PromptText {
			set {
				if(promptText)
					promptText.text = value;
			}
		}

		public string ContinuationText {
			set {
				if(continueText)
					continueText.text = $"- {value} -";
			}
		}

		public void PerformNextInstruction() {
			StartCoroutine(PerformNextInstructionCoroutine());
		}

		public void StartRacecar() {
			SceneManager.LoadScene("Race Car");
		}

		public void OnNext(InputValue _) {
			PerformNextInstruction();
		}

		public void SetCenter1() {
			c1 = Capture.PlayerPos;
		}

		public void SetCenter2() {
			c2 = Capture.PlayerPos;
		}

		public void SetCenter3() {
			c3 = Capture.PlayerPos;
		}

		public void ApplyCenter() {
			Vector3 origin = c1 - (c2 - c1) * (c2.z - c1.z);
			c1 -= origin;
			c2 -= origin;
			c3 -= origin;
			Vector4 column3 = origin;
			column3.w = 1;

			Vector3
				k = Vector3.back * c1.z,
				i = Vector3.left * c3.x,
				j = Vector3.up * c3.y;
			Matrix4x4 affine = new Matrix4x4(i, j, k, column3);
			Matrix4x4 inversed = Matrix4x4.identity;
			Matrix4x4.Inverse3DAffine(affine, ref inversed);

			GameManager.CaptureSettings.compensation = inversed;
		}
		#endregion

		#region Internal functions
		private void UpdateCapture(CaptureInstance capture) {
			if(captureFrame == null)
				return;
			if(capture == null)
				return;
			var faceRect = capture.MainFaceRect;
			if(!faceRect.HasValue)
				return;
			captureFrame.sizeDelta = faceRect.Value.size;
			captureFrame.localPosition = faceRect.Value.center;
		}
		#endregion

		#region Life cycle
		protected void Start() {
			instance = this;
			PerformNextInstruction();
		}

		protected void FixedUpdate() {
			UpdateCapture(Capture.instance);
		}
		#endregion
	}
}