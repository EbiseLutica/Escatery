// MessageBoxController
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : BaseBehavior {

	GameObject goText;
	RectTransform rect;
	/// <summary>
	///行ごとの文字数(全角基準)
	/// </summary>
	public int textLengthInEachLine = 20;

	// Use this for initialization
	public override void Start () {
		base.Start();
		goText = UIController.MessageBoxText;

		rect = GetComponent<RectTransform>();
		
	}

	// Update is called once per frame
	public override void Update() {
		base.Update();
		Vector2 delta = goText.GetComponent<RectTransform>().sizeDelta;
		Vector2 nameTextDelta = UIController.MessageBoxNameText.GetComponent<RectTransform>().sizeDelta;
		Text text = goText.GetComponent<Text>();
		text.fontSize = UIController.MessageBoxNameText.GetComponent<Text>().fontSize = (int)((Screen.width + rect.sizeDelta.x + delta.x) / textLengthInEachLine);
		rect.sizeDelta = new Vector2(rect.sizeDelta.x, text.fontSize * 2 + ((float)text.font.lineHeight / text.font.fontSize * text.fontSize) - delta.y);
		UIController.MessageBoxName.GetComponent<RectTransform>().sizeDelta = new Vector2(text.fontSize * UIController.MessageBoxNameText.GetComponent<Text>().text.Length - nameTextDelta.x , text.fontSize + ((float)text.font.lineHeight / text.font.fontSize * text.fontSize * 0.6f));
	}
}
