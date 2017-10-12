using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]

public class PlayerSetup : NetworkBehaviour {


	[SerializeField]
	private Behaviour[] componentsToDisable;

	[SerializeField]
	private string remoteLayerName = "RemotePlayer";

	[SerializeField]
	private string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	private GameObject playerUIPrefab;

	[HideInInspector]
	public GameObject playerUIInstance;

	Camera sceneCamera;



	private void Start()
	{
		if (!isLocalPlayer)
		{


			DisableComponents();
			AssignRemotePlayer();


		}
		else
		{
			sceneCamera = Camera.main;
			if(sceneCamera != null)
			{
				sceneCamera.gameObject.SetActive(false);
			}

			//disable playergraphics for local player
			SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));


			//Create playerUI
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			//configure playerUI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null)
				Debug.LogError("No playerUI component on PlayerUI prefab.");

			ui.SetController(GetComponent<PlayerController>());

			GetComponent<Player>().SetupPlayer();


		}
		

	}
	private void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;

		foreach(Transform child in obj.transform)
		{
			child.gameObject.layer = newLayer;
			SetLayerRecursively(child.gameObject, newLayer);
		}
		

	}



	public override void OnStartClient()
	{
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.RegisterPlayer(_netID, _player);
	}
	private void DisableComponents()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
		{
			componentsToDisable[i].enabled = false;
		}
	}

	private void AssignRemotePlayer()
	{
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}

	private void OnDisable()
	{

		Destroy(playerUIInstance);
		if(isLocalPlayer)
			GameManager.instance.SetSceneCameraActive(true);

		GameManager.UnRegisterPlayer(transform.name);

	}
}
