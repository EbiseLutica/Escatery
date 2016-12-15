// UIController
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


public class UIController : BaseBehavior
{
	public GameObject MessageBox;
	public GameObject Crosshair;
	public GameObject MessageBoxText;
	public GameObject MessageBoxName;
	public GameObject MessageBoxNameText;
	public GameObject MessageBoxPrompt;
	public GameObject NowLoadingScreen;


	public new void ShowBox() => Show(MessageBox);
	public new void HideBox() => Hide(MessageBox);

	public new IEnumerator FadeIn(float time) => Fade(fadeState, 0, time);
	public new IEnumerator FadeOut(float time) => Fade(fadeState, 1, time);


	public void Show(GameObject go) => go?.SetActive(true);
	public void Hide(GameObject go) => go?.SetActive(false);
	
	public void ShowCrosshair() => Show(Crosshair);
	public void HideCrosshair() => Hide(Crosshair);

	public void ShowNowLoading() => Show(NowLoadingScreen);
	public void HideNowLoading() => Hide(NowLoadingScreen);

	private Color ChangeColor(Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);

	private float fadeState;

	public IEnumerator Fade(float f1, float f2, float time)
	{
		float count = 0;
		Image fade = NowLoadingScreen?.GetComponent<Image>();
		Text[] texts = NowLoadingScreen?.GetComponentsInChildren<Text>();
		if (fade == null || texts == null)
		{
			yield break;
		}

		while (count < time)
		{
			fadeState = Mathf.Lerp(f1, f2, count / time);
			fade.color = ChangeColor(fade.color, fadeState);
			foreach (Text t in texts)
				t.color = ChangeColor(t.color, fadeState);

			count += Time.deltaTime;
			yield return null;
		}
		fadeState = f2;
		fade.color = ChangeColor(fade.color, fadeState);
		foreach (Text t in texts)
			t.color = ChangeColor(t.color, fadeState);
	}

	/// <summary>
	/// メッセージボックスにメッセージを表示します。
	/// </summary>
	/// <param name="text">表示するメッセージ。</param>
	/// <param name="name">セリフの場合、話者の名前。<see cref="null"/>を指定するとネームプレートは表示されません。</param>
	/// <returns></returns>
	public new IEnumerator Message(string text, string name = null)
	{
		string s = "";
		Hide(MessageBoxPrompt);
		MessageBoxName.SetActive(!string.IsNullOrEmpty(name));
		MessageBoxNameText.GetComponent<Text>().text = name;
		foreach (char c in text)
		{
			s += c;
			if (MessageBoxText?.GetComponent<Text>() != null)
			{
				MessageBoxText.GetComponent<Text>().text = s;
			}
			yield return new WaitForSeconds(GameController?.MessageSpeed ?? 0.125f);
		}
	}

	public new IEnumerator MessageAndNod(string text, string name = null)
	{
		yield return StartCoroutine(Message(text, name));
		yield return StartCoroutine(Nod());
	}

	public new IEnumerator Nod()
	{
		Show(MessageBoxPrompt);
		while (!CrossPlatformInputManager.GetButtonDown("Submit"))
			yield return null;

		PlaySound("Press");
		MessageBoxText.GetComponent<Text>().text = "";
		Hide(MessageBoxPrompt);
	}

}
