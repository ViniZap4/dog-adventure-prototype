using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCam : MonoBehaviour{

	[Header("Camera virtual 2")]
	[Space]

	public GameObject vCam2;


	void OnTriggerEnter(Collider other){
		
		switch(other.gameObject.tag){

			case "CamTrigger":
				vCam2.SetActive(true);
				break;

		}
	
	}

	private void OnTriggerExit(Collider other) {

		switch (other.gameObject.tag){
			case "CamTrigger":
				vCam2.SetActive(false);
				break;
		}

	}
}
