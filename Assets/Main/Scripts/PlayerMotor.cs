using UnityEngine;


[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

    private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;

	[SerializeField]
	private float cameraRotationLimit = 85f;


	private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

	//get a movement vector	
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
        
    }
	//get a rotation vector	
	public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;

	}
	//get a rotational  vector for the camera
	public void RotateCamera(float _cameraRotationX)
	{
		cameraRotationX = _cameraRotationX;

	}

	//get force vector for thrusters
	public void ApplyThruster (Vector3 _thrusterForce)
	{
		thrusterForce = _thrusterForce;
	}


	private void FixedUpdate()
    {
		if (PauseMenu.isOn)
			return;
		PerformMovement();
		PerformRotation();
    }


    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

		if (thrusterForce != Vector3.zero)
		{
			rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
    }

	private void PerformRotation()
	{
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		if (cam != null)
		{
			//set rotation and clamp it
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			//apply rotation to the trensform of camera
		
			cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
			
		}
	}


	

}
