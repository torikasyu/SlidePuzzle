using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public enum STATE {stay,attempt_move,moving}
	public enum DIRECTION {none = -1,up,right,down,left}
	
	public STATE state;
	public LayerMask blockLayer;

	public Vector3 fixedPosition;
												
	private DIRECTION attemptDirection;
	private DIRECTION prevDirection;
	
	private GameObject hpGauge;
	private float initHpGaugeLength;
	public int hp;
	public static int hpMax = 40;
	
	private AudioSource[] sources;
	
	// Use this for initialization
	void Start () {
		this.hpGauge = GameObject.Find("HpGauge").gameObject;
		this.initHpGaugeLength = this.hpGauge.transform.localScale.x;
		hp = hpMax;
		
		sources = GetComponents<AudioSource>();
	}

	// Will be called by GameManager when game starts.	
	public void MoveStart()
	{
		state = STATE.stay;
		prevDirection = DIRECTION.none;
		
		StartCoroutine ("_Update1");
		
		sources[1].Play();
	}
	
	public void MoveStop()
	{
		state = STATE.stay;
		StopCoroutine("_Update1");
		
		sources[1].Stop();
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		
		print(col.gameObject.tag);
		
		if(col.gameObject.tag == "Treasure")
		{
			//Destroy(col.gameObject);
			col.gameObject.GetComponent<Treasure>().Picked();
			sources[0].Play();
		}
		else if(col.gameObject.tag == "Stair")
		{
			GameManager.instance.StageClear();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		print (state);
	
		Vector2 curPos = transform.position;
		int move_x = 0;
		int move_y = 0;
		bool connect_up = false;
		bool connect_down = false;
		bool connect_right = false;
		bool connect_left = false;
		
		 // Update HP Gauge
		Vector3 hpScale = new Vector3(initHpGaugeLength*((float)hp/(float)hpMax),this.hpGauge.transform.localScale.y,this.hpGauge.transform.localScale.z);
		this.hpGauge.transform.localScale = hpScale;
		
		if(hp <= 0)
		{
			GameManager.instance.GameOver();
			return;
		}
		
		// Prevent player move when Floor is moving.
		//if(GameManager.instance.gameState == GameManager.GAMESTATE.FloorMoving)
		if(GameManager.instance.floorState == GameManager.FLOORSTATE.FloorDirectionDicided
			|| GameManager.instance.floorState == GameManager.FLOORSTATE.FloorMoving)
		{
			print (GameManager.instance.floorState);
			state = STATE.stay;
			return;
		}
		
		// fixed position for public
		if(state == STATE.stay)
		{
			this.fixedPosition = transform.position;
		}
		else if(state == STATE.attempt_move)
		{
			Collider2D col = Physics2D.OverlapPoint(curPos,blockLayer);
			
			if(col == null)
			{
				state = STATE.stay;
				return;
			}
			
			Block block = col.gameObject.GetComponent<Block>();

			int offset_r = 1;
			int offset_l = -1;
			int offset_u = 1;
			int offset_d = -1;
																		
			//if(col) {
				/*				
				if(curPos.x == 0)
				{
					offset_r = 1;
					offset_l = 4;
				}
				else if(curPos.x == 4)
				{
					offset_r = -4;
					offset_l = -1;
				}
				
				if(curPos.y == 0)
				{
					offset_u = 1;
					offset_d = 4;
				}
				else if(curPos.y == 4)
				{
					offset_u = -4;
					offset_d = -1;
				}

				col_right = Physics2D.OverlapPoint (new Vector2(curPos.x + offset_r,curPos.y),blockLayer);
				col_left = Physics2D.OverlapPoint (new Vector2(curPos.x + offset_l,curPos.y),blockLayer);				
				col_up = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + offset_u),blockLayer);
				col_down = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + offset_d),blockLayer);
				*/
				
				/*
				Collider2D col_right = null;
				Collider2D col_left = null;
				Collider2D col_up = null;
				Collider2D col_down = null;
				
				col_right = Physics2D.OverlapPoint (new Vector2(curPos.x + 1,curPos.y),blockLayer);
				col_left = Physics2D.OverlapPoint (new Vector2(curPos.x - 1,curPos.y),blockLayer);				
				col_up = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + 1),blockLayer);
				col_down = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y - 1),blockLayer);
				
				if(col_up)
				{
					if(col_up.GetComponent<Block>().down && block.up){connect_up=true;}
				}		
				
				if(col_down)
				{
					if(col_down.GetComponent<Block>().up && block.down){connect_down=true;}
				}		
				
				if(col_right)
				{
					if(col_right.GetComponent<Block>().left && block.right){connect_right=true;}
				}		
				
				if(col_left)
				{
					if(col_left.GetComponent<Block>().right && block.left){connect_left=true;}
				}
				*/
			// }
			
			connect_up = block.checkNearBlockConnect(Block.DIRECTION.up);
			connect_down = block.checkNearBlockConnect(Block.DIRECTION.down);
			connect_right = block.checkNearBlockConnect(Block.DIRECTION.right);
			connect_left = block.checkNearBlockConnect(Block.DIRECTION.left);
			
			//  Decide direction when init or blocked
			if(prevDirection == DIRECTION.none)
			{
				if(connect_up)
				{
					attemptDirection = DIRECTION.up;
				}
				else if(connect_down)
				{
					attemptDirection = DIRECTION.down;
				}
				else if(connect_right)
				{
					attemptDirection = DIRECTION.right;
				}
				else if(connect_left)
				{
					attemptDirection = DIRECTION.left;
				}
			}
			else
			{
				attemptDirection = prevDirection;
			}
			
			int rand = UnityEngine.Random.Range (0,2);
			if(attemptDirection == DIRECTION.up)
			{
				if(connect_up){}	// no probrem go ahead
				else if(!connect_left && !connect_down && !connect_right) attemptDirection = DIRECTION.none;	// deadlock
				else if(connect_left && connect_right)
				{
					// T-type route
					if(rand == 0) attemptDirection = DIRECTION.left;
					else attemptDirection = DIRECTION.right;
				}
				else if(!connect_left && !connect_right) attemptDirection = DIRECTION.down;
				else if(connect_left && !connect_right) attemptDirection = DIRECTION.left;
				else if(!connect_left && connect_right) attemptDirection = DIRECTION.right;
			}
			else if(attemptDirection == DIRECTION.down)
			{
				if(connect_down){}	// no probrem go ahead
				else if(!connect_up && !connect_left && !connect_right){attemptDirection = DIRECTION.none;}	// deadlock
				else if(connect_left && connect_right){
					if(rand == 0) attemptDirection = DIRECTION.left;
					else attemptDirection = DIRECTION.right;
				}
				else if(!connect_left && !connect_right){attemptDirection = DIRECTION.up;}
				else if(connect_left && !connect_right){attemptDirection = DIRECTION.left;}
				else if(!connect_left && connect_right){attemptDirection = DIRECTION.right;}
			}
			else if(attemptDirection == DIRECTION.right)
			{
				if(connect_right){}	// no probrem go ahead
				else if(!connect_up && !connect_down && !connect_left){attemptDirection = DIRECTION.none;}	// deadlock
				else if(connect_up && connect_down)
				{
					if(rand == 0) attemptDirection = DIRECTION.up;
					else attemptDirection = DIRECTION.down;
				}
				else if(!connect_up && !connect_down) attemptDirection = DIRECTION.left;
				else if(connect_up && !connect_down){attemptDirection = DIRECTION.up;}
				else if(!connect_up && connect_down){attemptDirection = DIRECTION.down;}
			}
			else if(attemptDirection == DIRECTION.left)
			{
				if(connect_left){}	// no probrem go ahead
				else if(!connect_up && !connect_down && !connect_right){attemptDirection = DIRECTION.none;}	// deadlock
				else if(connect_up && connect_down)
				{
					if(rand == 0) attemptDirection = DIRECTION.up;
					else attemptDirection = DIRECTION.down;	
				}
				else if(!connect_up && !connect_down){attemptDirection = DIRECTION.right;}
				else if(connect_up && !connect_down){attemptDirection = DIRECTION.up;}
				else if(!connect_up && connect_down){attemptDirection = DIRECTION.down;}   
			}

			//print (attemptDirection);
			if(attemptDirection == DIRECTION.up) GetComponent<Animator>().SetTrigger("Up");
			else if(attemptDirection == DIRECTION.down) GetComponent<Animator>().SetTrigger("Down");
			else if(attemptDirection == DIRECTION.left) GetComponent<Animator>().SetTrigger("Left");
			else if(attemptDirection == DIRECTION.right) GetComponent<Animator>().SetTrigger("Right");
			
			// Detect x,y value
			if(attemptDirection == DIRECTION.up)
			{
				move_y = offset_u;
			}
			else if(attemptDirection == DIRECTION.down)
			{
				move_y = offset_d;
			}
			else if(attemptDirection == DIRECTION.right)
			{
				move_x = offset_r;
			}
			else if(attemptDirection == DIRECTION.left)
			{
				move_x = offset_l;
			}
			else
			{
				move_x = 0;
				move_y = 0;
			}
						
			prevDirection = attemptDirection;
			
			if(move_x != 0 || move_y != 0)
			{
				Vector3 targetPos = new Vector3(curPos.x + move_x,curPos.y + move_y,0);
				state = STATE.moving;
				hp--;
				if(hp <=0 ) hp=0;
				//print (hp);
				StartCoroutine("SmoothMovement",targetPos);
			}
		}
	}

	IEnumerator _Update1()
	{
		while (true) 
		{
			yield return new WaitForSeconds(1.0f);
			if(state == STATE.stay)
			{
				state = STATE.attempt_move;
			}
		}
	}

	protected IEnumerator SmoothMovement (Vector3 end) {
	
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		float inverseMoveTime = 1f / 0.2f;

		while (sqrRemainingDistance > float.Epsilon) {		
			Vector3 newPosition = Vector3.MoveTowards (transform.position, end, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		//print ("coroutine finished");
		state = STATE.stay;

	}

}
