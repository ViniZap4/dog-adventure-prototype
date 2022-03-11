using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

	private GameManager _GameManager;
	private CharacterController controller;
	private Animator anim;

	[Header("Config Player")]
	[Space]
	public int HP;
	public float Movementspeed = 3f;
	[Space]
	public Transform HitBox;
	[Space]
	[Range(0.2f, 1f)]
	public float hitRange = 0.5f;
	[Space]

	[Header("Attack Config")]
	public ParticleSystem fxAttack;
	public LayerMask hitMask;
	public int amountDmg;
	
	
	
	private Vector3 direction;
	
	private bool isWalk;
	private bool isAttack;
	// inputs variables
	private float horizontal;
	private float vertical;

	public Collider[] hitInfo;



	void Start(){
		_GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
		controller = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();

	}

	void Update(){
		if(_GameManager.gameState != GameState.GAMEPLAY) return;

		Inputs();
		
		MoveCharacter();
		
		UpdateAnimator();
	}

	void OnTriggerEnter(Collider other){
			if(other.gameObject.tag == "TakeDamage"){
				GetHit(1);
			}
	}

 #region  My Methodys

	void Inputs(){
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		if(Input.GetButtonDown("Fire1") && !isAttack){
			Attack();
		}

	}

	void MoveCharacter(){

		// set direction in axis x and z and normalized
		direction = new Vector3(horizontal, 0f, vertical).normalized;

		// if player is in moving 
		if(direction.magnitude > 0.1f){
			
			//is get angle of direction and converting for deg
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

			// set transform rotation of player
			transform.rotation = Quaternion.Euler(0, targetAngle, 0);

			isWalk = true;

		}else{
			isWalk = false;

		}

		controller.Move(direction * Movementspeed * Time.deltaTime);
	}


	void UpdateAnimator(){
		anim.SetBool("isWalk", isWalk);

	}


	void Attack(){
		
		isAttack = true;
		anim.SetTrigger("Attack");
		fxAttack.Emit(1);
		
		hitInfo = Physics.OverlapSphere(HitBox.position, hitRange, hitMask);

		foreach(Collider c in hitInfo){
			c.gameObject.SendMessage("GetHit", amountDmg, SendMessageOptions.DontRequireReceiver);
		}

	}

	void AttackIsDone(){
		isAttack = false;
	}

	void GetHit(int amount){
		HP -= amount;

		if(HP > 0){
			anim.SetTrigger("Hit");
		}else{
			_GameManager.ChangeGameState(GameState.DIE);
			anim.SetTrigger("Die");
		}

	}

	#endregion

	void OnDrawGizmosSelected() {
	
		Gizmos.DrawWireSphere(HitBox.position, hitRange);
	
	}

}