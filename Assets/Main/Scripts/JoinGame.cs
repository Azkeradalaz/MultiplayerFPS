using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour {

	private NetworkManager networkManager;

	[SerializeField]
	private Text status;

	[SerializeField]
	private GameObject roomListItemPrefab;

	[SerializeField]
	private Transform roomListParent;

	List<GameObject> roomList = new List<GameObject>();

	private void Start()
	{
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		RefreshRoomList();
	}


	public void RefreshRoomList()
	{
		ClearRoomList();

		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
		status.text = "Loading...";
	}

	public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
	{
		status.text = "";

		if (!success || matchList == null)
		{
			status.text = "No matches found";
			return;
		}
		ClearRoomList();


		foreach(MatchInfoSnapshot match in matchList)
		{
			GameObject _roomlistItemGO = Instantiate(roomListItemPrefab);
			_roomlistItemGO.transform.SetParent(roomListParent);

			RoomListItem _roomListItem = _roomlistItemGO.GetComponent<RoomListItem>();
			if (_roomListItem != null)
			{
				_roomListItem.Setup(match,JoinRoom);
			}

			//have a components sit on gameobject to take care of settings up the name/amout of users and set callback to join the game
			roomList.Add(_roomlistItemGO);

		}
		if (roomList.Count == 0)
		{
			status.text = "No matches currently available";
		}
	}




	private void ClearRoomList()
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			Destroy(roomList[i]);
		}

		roomList.Clear();

	}

	public void JoinRoom(MatchInfoSnapshot _match)
	{
		Debug.Log("Joining " + _match.name);

		networkManager.matchMaker.JoinMatch(_match.networkId,"","","",0,0,networkManager.OnMatchJoined);
		StartCoroutine(WaitForJoin());
	}
	private IEnumerator WaitForJoin()
	{
		ClearRoomList();
		

		int countdown = 15;
		while (countdown > 0)
		{
			status.text = "Joining...(" + countdown + ")";
			yield return new WaitForSeconds(1);
			countdown--;

		}
		status.text = "Failed to connect";

		MatchInfo matchInfo = networkManager.matchInfo;
		if (matchInfo != null)
		{
			networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
			networkManager.StopHost();
		}
		

		yield return new WaitForSeconds(2);

		RefreshRoomList();
	}


}

