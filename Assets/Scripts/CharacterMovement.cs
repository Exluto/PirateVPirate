﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterMovement : NetworkBehaviour {
	public float m_speed = 5.0f;
	public float m_speedMultiplier = 1.0f;
	public float m_gravity = 20.0f;
	public float m_jumpSpeed = 8.0f;
	public MeshCollider m_swordCollider;
	public int m_numOfBlockedAttacks = 0;
	public bool m_cantTakeDamage = false;
	public GameObject m_camtarget;
	private Vector3 m_moveDirection = Vector3.zero;
	private bool m_isJumping;
	private bool m_isGrounded = false;
	private CharacterController m_controller;
	public Animator m_animController;
	public SwordCollider m_swordColliderScript;
	private bool m_isAttacking;
	private Health m_healthScript;
	private bool m_disableMovement;
	public SoundMGR m_soundManager;

	// Use this for initialization
	void Start() {
		m_controller = GetComponent<CharacterController>();
		Camera.main.GetComponent<CameraController>().m_target = transform;
		m_animController = GetComponent<Animator>();
		m_swordColliderScript = GetComponentInChildren<SwordCollider>();
		m_healthScript = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_disableMovement) {
			m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			if(Input.GetButtonDown("Jump") && m_isGrounded) {
				m_moveDirection.y = m_jumpSpeed;
				m_isJumping = true;
			}

			m_moveDirection *= m_speed * m_speedMultiplier;

			m_animController.SetFloat("Forward", m_moveDirection.z);
			m_animController.SetFloat("Right", m_moveDirection.x);

			m_moveDirection = transform.TransformDirection(m_moveDirection);
			

			m_moveDirection.y -= m_gravity * Time.deltaTime;
			m_isGrounded = ((m_controller.Move(m_moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0;


			if(Input.GetMouseButtonDown(0) && m_isAttacking == false) {
				m_swordCollider.enabled = true;			
				m_animController.SetBool("isAttacking", true);
				m_isAttacking = true;
				Debug.Log(m_animController.GetBool("isAttacking"));
			}
		}

		if(Input.GetMouseButtonDown(1) && m_numOfBlockedAttacks <= 3) {
			m_disableMovement = true;
			m_animController.SetBool("isBlocking", true);
			m_cantTakeDamage = true;
			Debug.Log("Cant tank damage should be true: " + m_cantTakeDamage);
		}

		if(Input.GetMouseButtonUp(1)) {
			m_disableMovement = false;
			m_animController.SetBool("isBlocking", false);
			m_cantTakeDamage = false;
			Debug.Log("Cant tank damage should be false: " + m_cantTakeDamage);
			Debug.Log("Stopped Blocking...");
		}

		if(Input.GetKeyDown(KeyCode.R)) {
			m_healthScript.TakeDamage(10);
		}
	}

	void BlockedAttack() {
		m_animController.SetBool("blockedAttack", false);
	}

	// void TakeDamage() {
	// 	m_healthScript.RpcTakeDamage(10);
	// }

	public void ResetAttack() {
		if(m_animController != null) {
			m_animController.SetBool("isAttacking", false);
			Debug.Log(m_animController.GetBool("isAttacking"));
		}
		m_swordCollider.enabled = false;
		m_swordColliderScript.m_hasDealtDamage = false;
		m_isAttacking = false;
	}

	 [Command]
     public void CmdSetAuth(NetworkInstanceId objectId, NetworkIdentity player)
     {
         var iObject = NetworkServer.FindLocalObject(objectId);
         var networkIdentity = iObject.GetComponent<NetworkIdentity>();
         var otherOwner = networkIdentity.clientAuthorityOwner;
 
         if (otherOwner == player.clientAuthorityOwner)
         {
             Debug.Log("Same");
             return;
         }
         else
         {
             if (otherOwner != null)
             {
                 networkIdentity.RemoveClientAuthority(otherOwner);
             }
             networkIdentity.AssignClientAuthority(player.connectionToClient);
         }
     }
}

