// GameController
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class GameController : BaseBehavior {

	public float MessageSpeed = 0.125f;

	public new GameObject Player;

	public bool IsLocked;

	public MapData BeginMapData;

	public Vector3 BeginPos;

	// ゲームのはじまり
	public new IEnumerator Start() {
		base.Start();
		yield return StartCoroutine(UIController.FadeOut(0));
		SpawnPlayer();
		yield return StartCoroutine(MapManager?.Change(BeginMapData, BeginPos, 0, 0.5f));
	}

	public GameObject SpawnPlayer(Vector3 position = default(Vector3), float angle = default(float)) => Instantiate(Player, position, Quaternion.Euler(0, angle, 0));

	public void WarpPlayer(Vector3 position, float angle)
	{
		Destroy(base.Player);
		SpawnPlayer(position, angle);
	}


	public void KillPlayer() => Destroy(base.Player);

	public override void Update()
	{
		base.Update();
		if (base.Player == null)
			return;
		UIController.Crosshair.SetActive(base.Player.GetComponent<PlayerEventHandler>().enabled = base.Player.GetComponent<FirstPersonController>().enabled = !IsLocked);
	}



}
