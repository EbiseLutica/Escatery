// CoroutineEventBehavior
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete(nameof(EventBehavior) + " に移行してください")]
public abstract class CoroutineEventBehavior : EventBehavior
{
	public override sealed IEnumerator OnActivated(GameObject go) => OnActivatedCoroutine(go);

	public abstract IEnumerator OnActivatedCoroutine(GameObject go);
}