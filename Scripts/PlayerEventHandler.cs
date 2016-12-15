// PlayerEventHandler
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Skybox))]
public class PlayerEventHandler : BaseBehavior {

	public Transform eye;

	// Use this for initialization
	public override void Start () {
		base.Start();
	}



	// Update is called once per frame
	public override void Update () {
		base.Update();
		if (eye == null)
		{
			print("Please set player's own eye transform!");
			return;
		}

		Skybox s;
		if ((s = GetComponentInChildren<Skybox>()) != null)
			s.material = MapManager?.CurrentMapData?.SkyMaterial ?? s.material;
		
		RaycastHit hit;
		UIController.Crosshair.GetComponent<Image>().color = Color.white;
		if (Physics.Raycast(eye.position, eye.forward, out hit, 100f))
		{
			if (hit.collider?.gameObject != null)
			{
				Run(ActivateEventOf(hit.collider.gameObject, eb =>
				{
					UIController.Crosshair.GetComponent<Image>().color = eb.TriggerType == EventTriggerType.Talk ? Color.red : Color.white;
					return eb != null && eb.TriggerType == EventTriggerType.EyeContact || (eb.TriggerType == EventTriggerType.Talk && CrossPlatformInputManager.GetButtonDown("Submit"));
				}));
			}
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject != null)
		{
			Run(ActivateEventOf(other.gameObject, eb => eb?.TriggerType == EventTriggerType.Collide));
		}
	}

}
