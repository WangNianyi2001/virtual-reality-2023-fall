using UnityEngine;

namespace Game.Racecar {
	public class Coin : MonoBehaviour {
		const string protagonistTag = "Player";

		void OnTriggerEnter(Collider other) {
			if(other.gameObject.tag != protagonistTag)
				return;
			Destroy(gameObject);
			Debug.Log("Coin collected");
		}
	}
}