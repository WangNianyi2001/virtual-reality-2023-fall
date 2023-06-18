using UnityEngine;
using System.Collections.Generic;

namespace Game {
	[RequireComponent(typeof(Camera))]
	public class PostProcess : MonoBehaviour {
		public List<Material> materials;

		void OnRenderImage(RenderTexture source, RenderTexture destination) {
			RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
			foreach(Material mat in materials) {
				Graphics.Blit(source, temp, mat);
				Graphics.Blit(temp, source);
			}
			Graphics.Blit(temp, destination);
			RenderTexture.ReleaseTemporary(temp);
		}
	}
}
