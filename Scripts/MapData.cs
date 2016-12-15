// MapData
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapData : BaseBehavior
{
	[Tooltip("マップ名を指定します。シーンと同じ名前にしてください。")]
	public string Name;

	[Tooltip("マップで再生される音楽を指定します。")]
	public MusicData Music;

	[Tooltip("このマップの SkyBox に使うマテリアル。")]
	public Material SkyMaterial;
	
}

[System.Serializable]
public struct MusicData
{
	public AudioClip Clip;

	[Tooltip("ループするかどうか。")]
	public bool UseLoop;
}