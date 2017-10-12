using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {


	private const string PLAYER_TAG = "Player";
	
	private PlayerWeapon currentWeapon;

	[SerializeField]
	private Camera cam;


	[SerializeField]
	private LayerMask mask;

	private WeaponManager weaponManager;

	private void Start()
	{
		if (cam == null)
		{
			Debug.LogError("PlayerShoot: no camera referenced!");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager>();

	}


	private void Update()
	{
		currentWeapon = weaponManager.GetCurrentWeapon();

		if (PauseMenu.isOn)
			return;
		if (currentWeapon.fireRate <= 0f)
		{



			if (Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		}
		else
		{
			if (Input.GetButtonDown("Fire1"))
			{
				InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);

			}
			else if (Input.GetButtonUp("Fire1"))
			{
				CancelInvoke("Shoot");
			}
		}
	}
	// is callde on the server when player shoots
	[Command]
	private void CmdShoot()
	{

		RpcDoShootEffect();

	}


	//is called on all clients when shoot effect needed
	[ClientRpc]
	private void RpcDoShootEffect()
	{
		weaponManager.GetCurrentWeaponGraphics().muzzleFlash.Play();
		

	}

	//is called on the server when we hit something
	//takes in the hit ppoint and the normal of the surface
	[Command]
	private void CmdOnHit(Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffect(_pos, _normal);

	}

	//is called on all clients
	//spawn hit effects
	[ClientRpc]
	private void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentWeaponGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
		Destroy(_hitEffect, 1f);

	}


	[Client]
	private void Shoot()
	{
		if (!isLocalPlayer)
		{
			return;
		}



		CmdShoot();


		Debug.Log("Bang");
		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
		{
			if (_hit.collider.tag == PLAYER_TAG)
			{
				CmdPlayerHit(_hit.collider.name, currentWeapon.damage);
			}
			//Debug.Log("Hit " + _hit.collider.name);
			CmdOnHit(_hit.point, _hit.normal);
		}
	}


	[Command]
	private void CmdPlayerHit(string _playerID, int _damage)
	{
		Debug.Log(_playerID + " has been hit.");

		Player _player = GameManager.GetPlayer(_playerID);
		_player.RpcTakeDamage(_damage);
		
	}


}
