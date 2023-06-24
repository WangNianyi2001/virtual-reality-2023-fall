using UnityEngine;
using System;

public class ArcadeVehicleController : MonoBehaviour {
	public enum groundCheck { rayCast, sphereCaste };
	public enum MovementMode { Velocity, AngularVelocity };
	public MovementMode movementMode;
	public groundCheck GroundCheck;
	public LayerMask drivableSurface;

	public float MaxSpeed, accelaration, turn, gravity = 7f;
	public Rigidbody rb, carBody;

	[HideInInspector] public RaycastHit hit;
	public AnimationCurve frictionCurve;
	public AnimationCurve turnCurve;
	public PhysicMaterial frictionMaterial;
	[Header("Visuals")]
	public Transform BodyMesh;
	public Transform[] FrontWheels = new Transform[2];
	public Transform[] RearWheels = new Transform[2];
	[HideInInspector] public Vector3 carVelocity;

	[Range(0, 10)] public float BodyTilt;
	[Header("Audio settings")] public AudioSource engineSound;
	[Range(0, 1)] public float minPitch;
	[Range(1, 3)] public float MaxPitch;
	public AudioSource SkidSound;

	[HideInInspector] public float skidWidth;

	float radius;
	[NonSerialized] public Vector3 input;
	Vector3 origin;
	public void UpdateAudio() {
		engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / MaxSpeed);
		if(Mathf.Abs(carVelocity.x) > 10 && IsGrounded)
			SkidSound.mute = false;
		else
			SkidSound.mute = true;
	}

	void DoGroundedLogic() {
		// Steering
		float sign = Mathf.Sign(carVelocity.z);
		float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);
		if(input.z * carVelocity.z > 0)
			carBody.AddTorque(Vector3.up * input.x * sign * turn * 100 * TurnMultiplyer);

		// Braking
		if(Input.GetAxis("Jump") > 0.1f)
			rb.constraints = RigidbodyConstraints.FreezeRotationX;
		else
			rb.constraints = RigidbodyConstraints.None;

		// Accelarating

		if(movementMode == MovementMode.AngularVelocity) {
			rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * input.z * MaxSpeed / radius, accelaration * Time.deltaTime);
		}
		else if(movementMode == MovementMode.Velocity) {
			if(Input.GetAxis("Jump") < 0.1f)
				rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * input.z * MaxSpeed, accelaration / 10 * Time.deltaTime);
		}

		// Tilting
		carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
	}
	void DoUngroundedLogic() {
		carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
		rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity, Time.deltaTime * gravity);
	}
	void UpdateVisual() {
		// Tires
		foreach(Transform FW in FrontWheels) {
			FW.localRotation = Quaternion.Slerp(
				FW.localRotation,
				Quaternion.Euler(
					FW.localRotation.eulerAngles.x,
					30 * input.x,
					FW.localRotation.eulerAngles.z
				),
				0.1f
			);
			FW.GetChild(0).localRotation = rb.transform.localRotation;
		}
		RearWheels[0].localRotation = rb.transform.localRotation;
		RearWheels[1].localRotation = rb.transform.localRotation;

		// Body
		if(carVelocity.z > 1) {
			BodyMesh.localRotation = Quaternion.Slerp(
				BodyMesh.localRotation,
				Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
				BodyMesh.localRotation.eulerAngles.y, BodyTilt * input.x),
				0.05f
			);
		}
		else {
			BodyMesh.localRotation = Quaternion.Slerp(
				BodyMesh.localRotation,
				Quaternion.Euler(0, 0, 0),
				0.05f
			);
		}
	}

	#region Public interfaces
	public bool IsGrounded {
		get {
			origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
			var direction = -transform.up;
			var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

			if(GroundCheck == groundCheck.rayCast)
				return Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface);
			else if(GroundCheck == groundCheck.sphereCaste)
				return Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface);
			else
				return false;
		}
	}
	#endregion

	#region Life cycle
	void Start() {
		radius = rb.GetComponent<SphereCollider>().radius;
		if(movementMode == MovementMode.AngularVelocity)
			Physics.defaultMaxAngularSpeed = 100;
		// Create a copy at start, otherwise asset would be modified
		frictionMaterial = Instantiate(frictionMaterial);
	}

	void FixedUpdate() {
		carVelocity = carBody.transform.InverseTransformDirection(carBody.velocity);
		frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));

		if(IsGrounded)
			DoGroundedLogic();
		else
			DoUngroundedLogic();

		UpdateVisual();
		UpdateAudio();
	}

	void OnDrawGizmos() {
		float width = 0.02f;
		radius = rb.GetComponent<SphereCollider>().radius;

		if(!Application.isPlaying) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
			if(GetComponent<BoxCollider>()) {
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
			}
		}
	}
	#endregion
}
