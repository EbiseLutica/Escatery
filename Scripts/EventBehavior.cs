// EventBehavior
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using UnityEngine;
using System.Collections;

public enum EventTriggerType
{
	/// <summary>
	/// 調べると発生。
	/// </summary>
	Talk,
	/// <summary>
	/// 接触すると発生。
	/// </summary>
	Collide,
	/// <summary>
	/// 目が合うと発生。
	/// </summary>
	EyeContact,
	/// <summary>
	/// イベントからの呼び出し専用。
	/// </summary>
	FromEvent
}

public abstract class EventBehavior : BaseBehavior {
	
	public EventTriggerType TriggerType;

	/// <summary>
	/// イベント ロジックを記述します。
	/// </summary>
	public abstract IEnumerator OnActivated(GameObject activator);

}
