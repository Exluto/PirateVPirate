﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnableNetworkScripts : NetworkBehaviour {

	public Canvas m_playerHUD;

	void Start() {
		if(isLocalPlayer) {
			GetComponent<Health>().enabled = true;
			GetComponent<CharacterMovement>().enabled = true;
			this.gameObject.tag = "Player";
		}
		GameManager.Instance.AddPlayer(gameObject);
	}

	public void SetupHUD() {
		if(isLocalPlayer) {
			m_playerHUD.gameObject.SetActive(true);
		}
	}
}
