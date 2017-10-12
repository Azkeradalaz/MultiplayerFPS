using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;
	[SerializeField]
	private float thrusterForce = 1000f;
	[SerializeField]
	private float thrusterFuelBurnSpeed = 1f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;

	private float thrusterFuelAmount = 1f;

	public float GetThrusterFuelAmount()
	{
		return thrusterFuelAmount;
	}


	[SerializeField]
	private LayerMask enviromentMask;

	[Header("Spring settings:")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	// Component caching
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	private void Start()
    {
        motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint>();
		animator = GetComponent<Animator>();

		SetJointSetting(jointSpring);
    }

    private void Update()
    {
		if (PauseMenu.isOn)
		{
			if (Cursor.lockState != CursorLockMode.None)
				Cursor.lockState = CursorLockMode.None;

			motor.Move(Vector3.zero);
			motor.Rotate(Vector3.zero);
			motor.RotateCamera(0f);
			return;
		}

		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		//setting target position for string
		//makes physict act right when it comes too applying gravity when flying over objects
		RaycastHit _hit;
		if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, enviromentMask))
		{
			joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
		}
		else
		{
			joint.targetPosition = new Vector3(0f, 0f, 0f);
		}

        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

		//final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

		//animate movement
		animator.SetFloat("ForwardVelocity", _zMov);


		//apply movement
		motor.Move(_velocity);

		//calculate rotation as a 3D vector (turning around)
		float _yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

		//apply rotation
		motor.Rotate(_rotation);

		//calculate camera rotation as a 3D vector (turning around)
		float _xRot = Input.GetAxisRaw("Mouse Y");
		float _cameraRotation = _xRot  * lookSensitivity;

		//apply rotation
		motor.RotateCamera(_cameraRotation);


		Vector3 _thrusterForce = Vector3.zero;
		if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
		{
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

			if (thrusterFuelAmount >= 0.01f)
			{
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSetting(0f);
			}
			
		}
		else
		{
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			
			SetJointSetting(jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

		//apply the thruster force
		motor.ApplyThruster(_thrusterForce);


	}

	private void SetJointSetting(float _jointSpring)
	{
		joint.yDrive = new JointDrive {
			positionSpring = _jointSpring,
			maximumForce = jointMaxForce };
	}

}
