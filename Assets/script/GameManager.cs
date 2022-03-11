using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum GameState{
	GAMEPLAY, DIE
}
public enum enemyState{
	IDLE, ALERT, PATROL, FOLLOW, FURY, DIE
}


public class GameManager : MonoBehaviour{

	public GameState gameState;


	[Header("Player Stuffs")]
	public Transform player;

	[Header("Slimer IA")]
	public Transform[] slimeWayPoints;
	public float slimeIdleWaitTime = 5f;
	public float slimeDistanceToAttack = 2.3f;
	public float SlimeAlertTime = 3f;
	public float slimeAttackDelay = 1f;
	public float slimeLookAtSpeed = 1f;


	[Header("Rain manager")]
	public  PostProcessVolume postB;
	public ParticleSystem rainParticle;
	public int rainRateOverTime;
	public int rainIncrement;
	public float rainIncrementDelay;

	private ParticleSystem.EmissionModule rainModule;

	private void Start() {
		rainModule = rainParticle.emission;
	}

	public void onOffRain(bool isRain){
		StopCoroutine("RainManager");
		StartCoroutine("RainManager", isRain);

		StopCoroutine("PostBManager");
		StartCoroutine("PostBManager", isRain);
	}

	IEnumerator RainManager(bool isRain){
		
		switch (isRain){
			case true:
				for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement){
					rainModule.rateOverTime = r;
					yield return new WaitForSeconds(rainIncrementDelay);
				}
				rainModule.rateOverTime = rainRateOverTime;
			break;

			case false:
				for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r -= rainIncrement){
					rainModule.rateOverTime = r;
					yield return new WaitForSeconds(rainIncrementDelay);
				}
				rainModule.rateOverTime = 0;
			break;
		}
	}

	IEnumerator PostBManager(bool isRain){
		
			switch (isRain){
					
				case true:
					for (float w = postB.weight; w < 1; w += 1* Time.deltaTime){
						postB.weight = w;
						yield return new WaitForEndOfFrame();
					}
					postB.weight = 1;
				break;

				case false:
					for (float w = postB.weight; w >= 1; w -= 1 * Time.deltaTime){
						postB.weight = w;
						yield return new WaitForEndOfFrame();
					}
					postB.weight = 0;
				break;
			}
	}

	public void ChangeGameState(GameState newState){
		gameState = newState;
	}

}
