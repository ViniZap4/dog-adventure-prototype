using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour{

	private GameManager _GameManager;
	public bool isRain;

	void Start(){
		_GameManager  = FindObjectOfType(typeof(GameManager)) as GameManager;

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			_GameManager.onOffRain(isRain);
		}
	}

}
