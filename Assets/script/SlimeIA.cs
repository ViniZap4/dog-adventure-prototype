using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour{

	private GameManager _GameManager;
	private Animator anim;

	public ParticleSystem particle;

	public int HP;
	private bool isDie;

	public enemyState state;

	// IA 
	private NavMeshAgent agent;
	private int idWaypoint;
	private bool isPlayerVisible;
	private Vector3 destination;
	private bool isWalk;
	private bool isAlert;

	private bool isAttack;


	void Start(){
		_GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		changeState(state);


	}

	IEnumerator Died(){
		isDie = true;
		yield return new WaitForSeconds(2.5f);
		Destroy(this.gameObject);
	}

	void Update(){
			stateManager();


		if(agent.desiredVelocity.magnitude >= 0.1f){
			isWalk =true;
		}else{
			isWalk = false;
		}

		anim.SetBool("isWalk", isWalk);
		anim.SetBool("isAlert", isAlert);

	}

	private void OnTriggerEnter(Collider other) {
		if(_GameManager.gameState != GameState.GAMEPLAY ) return;

		if(other.gameObject.tag== "Player"){
			isPlayerVisible = true;
			if (state == enemyState.IDLE || state == enemyState.PATROL){
				changeState(enemyState.ALERT); 
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.tag == "Player"){
			isPlayerVisible = false;
		}
	}

	#region  my methodys


	void GetHit(int amountDmg){

		if(isDie == true) return;

		HP -= amountDmg;

		if(HP > 0){
			changeState(enemyState.FURY);
			anim.SetTrigger("GetHit");
			particle.Emit(10);
		}else{
			changeState(enemyState.DIE);
			anim.SetTrigger("Die");
			StartCoroutine("Died");
			
		}

	}

	void stateManager(){
		if(_GameManager.gameState == GameState.DIE && (state == enemyState.FOLLOW || state == enemyState.FURY || state == enemyState.ALERT)){
			StayStill(30);
		}
		switch(state){
			
			case enemyState.ALERT:
				LooAt();
				break;

			case enemyState.FOLLOW:
				destination = _GameManager.player.position;
				agent.destination = destination;
				LooAt();

				if(agent.remainingDistance <= agent.stoppingDistance){
					Attack();
				}

				break;
			
			case enemyState.FURY:
				destination = _GameManager.player.position;
				agent.destination = destination;
				LooAt();

				if(agent.remainingDistance <= agent.stoppingDistance){
					Attack();
				}

				break;
			
			case enemyState.PATROL:
				break;
		
		}
	}

	//execute when start state
	void changeState( enemyState newState){
		
		StopAllCoroutines();
		isAlert= false;
		print(newState);
		state = newState;


		switch(newState){
			case enemyState.IDLE:
			  agent.stoppingDistance = 0;
				destination = transform.position;
				agent.destination = destination;
				
				StartCoroutine("IDLE");
				break;
			
			case enemyState.ALERT:

				agent.stoppingDistance = 0;
				destination = transform.position;
				agent.destination = destination;
				isAlert = true;

				StartCoroutine("ALERT");
				break;
			
			case enemyState.FOLLOW:

				agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
				StartCoroutine("FOLLOW");
				break;

			case enemyState.FURY:
				agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
				
				break;

			case enemyState.PATROL:

				agent.stoppingDistance = 0;
				idWaypoint = Random.Range(0, _GameManager.slimeWayPoints.Length);
				destination = _GameManager.slimeWayPoints[idWaypoint].position;
				agent.destination = destination;
				
				StartCoroutine("PATROL");
				break;
			case enemyState.DIE:
				destination = transform.position;
				agent.destination = destination;
				break;
		}

	}

	IEnumerator IDLE(){
		yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
		StayStill(50);

	}
	
	IEnumerator PATROL(){

		yield return new WaitUntil(() => agent.remainingDistance <= 0);
		StayStill(30);

	}

	IEnumerator FOLLOW(){
	yield return new WaitUntil(() => !isPlayerVisible);
	yield return new WaitForSeconds(_GameManager.SlimeAlertTime);
	StayStill(50);
}

	IEnumerator ALERT(){
		yield return new WaitForSeconds(_GameManager.SlimeAlertTime);

		if(isPlayerVisible){
			changeState(enemyState.FOLLOW);
		}else{
			StayStill(10);
		}

	}

	IEnumerator ATTACK(){
		yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
		isAttack = false;
	}

	void Attack(){
		if(!isAttack && isPlayerVisible){
			LooAt();
			isAttack = true;
			anim.SetTrigger("Attack");
		}
	}

	void AttackIsDone(){
		StartCoroutine("ATTACK");
	}


	void LooAt(){
		
		Vector3 LookDirection = (_GameManager.player.position - transform.position).normalized;
		Quaternion LookRotation = Quaternion.LookRotation(LookDirection);
		transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);

	}

	void StayStill(int yes){
		if(Rand() <= yes){
			changeState(enemyState.IDLE);
		}else{
			changeState(enemyState.PATROL);
		}
	}	

	int Rand(){
		int rand = Random.Range(0, 100); //0 ... 99

		return rand;
	}

	#endregion

}
