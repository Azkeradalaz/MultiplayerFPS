using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon;

	private PlayerWeapon currentWeapon;
	private WeaponGraphics currentWeaponGraphics;





	private void Start()
	{
		EquipWeapon(primaryWeapon);
	}


	public PlayerWeapon GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentWeaponGraphics()
	{
		return currentWeaponGraphics;
	}


	private void EquipWeapon (PlayerWeapon _weapon)
	{
		currentWeapon = _weapon;

		GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);

		_weaponIns.transform.SetParent(weaponHolder);

		currentWeaponGraphics = _weaponIns.GetComponent<WeaponGraphics>();

		if (currentWeaponGraphics == null)
			Debug.Log("No Weapon graphics component on the weapon object: " + _weaponIns.name);

		if(isLocalPlayer)
			Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
	}
	
}
