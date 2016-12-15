// BaseBehavior
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;


public abstract class BaseBehavior : MonoBehaviour {
	protected GameObject Player { get; private set; }
	protected UIController UIController { get; private set; }
	protected MusicManager MusicManager { get; private set; }
	protected GameController GameController { get; private set; }
	protected AudioSource audioSource;
	protected MapManager MapManager { get; private set; }
	private AudioClip se;

	protected void HandlerUpdate()
	{
		UIController = GameObject.FindGameObjectWithTag("UI")?.GetComponent<UIController>();
		Player = GameObject.FindGameObjectWithTag("Player");
		MusicManager = GameObject.FindGameObjectWithTag("MusicManager")?.GetComponent<MusicManager>();
		GameController = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameController>();
		MapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();
	}

	public virtual void Start()
	{
		HandlerUpdate();
		audioSource = gameObject.GetComponent<AudioSource>();
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();
		audioCache = new Dictionary<string, AudioClip>();
	}

	public virtual void Update()
	{
		HandlerUpdate();
	}

	static Dictionary<string, AudioClip> audioCache;

	public void PlaySound(string path)
	{
		path = $"Sounds\\{path}";
		if (!audioCache.ContainsKey(path)) audioCache[path] = Resources.Load<AudioClip>(path);
		audioSource.PlayOneShot(audioCache[path]);
	}

	public void Lock()
	{
		GameController.IsLocked = true;
	}

	public void UnLock()
	{
		GameController.IsLocked = false;
	}

	public IEnumerator MessageAndNod(string mes, string name = null) => UIController.MessageAndNod(mes, name);
	public IEnumerator Message(string mes, string name = null) => UIController.Message(mes, name);
	public IEnumerator Nod() => UIController.Nod();

	public void ShowBox() => UIController.ShowBox();
	public void HideBox() => UIController.HideBox();

	public IEnumerator ChangeMap(MapData mapData, Vector3 pos, float angle, float time = 2f) => MapManager.Change(mapData, pos, angle, time);

	public IEnumerator FadeOut(float time) => UIController.FadeOut(time);
	public IEnumerator FadeIn(float time) => UIController.FadeIn(time);

	/// <summary>
	/// 指定した<see cref="GameObject"/>を叩いてイベントを発生させます。
	/// </summary>
	/// <param name="mogura">対象の <see cref="GameObject"/>。</param>
	/// <param name="conditions">イベントを発生するかどうか判断する式。</param>
	/// <returns></returns>
	public IEnumerator ActivateEventOf(GameObject mogura, System.Predicate<EventBehavior> conditions = null)
	{
		EventBehavior eb = mogura.GetComponent<EventBehavior>();
		if (eb == null)
			yield break;
		if (conditions?.Invoke(eb) ?? true)
			yield return eb.OnActivated(gameObject);
	}

	protected CoroutineSequence InitRunner() => new CoroutineSequence(GameController);
	protected Coroutine Run(IEnumerator coroutine) => GameController?.StartCoroutine(coroutine);
}
