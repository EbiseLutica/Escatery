// MapManager
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MapManager : BaseBehavior
{

	public MapData CurrentMapData { get; private set; }

	public IEnumerator Change(MapData mapData, Vector3 pos, float angle, float time = 2f)
	{
		Lock();
		Run(UIController.FadeOut(time * 0.3f));
		MusicData actualMusic = mapData.Music;
		bool changeMusic = MusicManager != null && !MusicManager.CurrentMusic.Equals(actualMusic);
		if (changeMusic)
			Run(MusicManager.Stop(time * 0.8f));
		yield return new WaitForSeconds(time);

		if (CurrentMapData != null)
			Unload();
		
		var result = SceneManager.LoadSceneAsync(mapData.Name, LoadSceneMode.Additive);
		
		CurrentMapData = mapData;
		RenderSettings.skybox = mapData.SkyMaterial ?? RenderSettings.skybox;
		DynamicGI.UpdateEnvironment();

		yield return new WaitUntil(() => result.isDone);
		GameController.WarpPlayer(pos, angle);
		if (changeMusic)
			MusicManager.PlayMusic(actualMusic);
		yield return UIController.FadeIn(0.75f);
		UnLock();
	}

	public void Unload()
	{
		if (CurrentMapData == null)
			return;

		SceneManager.UnloadSceneAsync(CurrentMapData.Name);
		CurrentMapData = null;
	}



}
