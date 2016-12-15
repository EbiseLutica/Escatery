// MusicManager
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : BaseBehavior
{

	public MusicData CurrentMusic { get; private set; }

	[Range(0, 1)]
	public float Volume = 1;

	public override void Start()
	{
		base.Start();
	}



	public void PlayMusic(MusicData data)
	{
		if (audioSource == null || CurrentMusic.Equals(data))
			return;
		audioSource.clip = data.Clip;
		audioSource.loop = data.UseLoop;
		if (data.Clip != null)
			audioSource.Play();
	}

	public IEnumerator Stop(float time = 0)
	{
		if (audioSource == null)
			yield break;
		float count = 0;
		while (count < time)
		{
			audioSource.volume = Mathf.Lerp(Volume, 0, count / time);
			count += Time.deltaTime;
			yield return null;
		}
		audioSource.Stop();
		audioSource.volume = Volume;
	}

}
