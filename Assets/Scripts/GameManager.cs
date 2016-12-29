using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	
	public enum GAMESTATE {
			Ready,
			Explore,
			Battle,
			StageClear,
			GameOver
		}
	public enum FLOORSTATE {
			Stay,
			AttemptFloorMove,
			FloorDirectionDicided,
			FloorMoving
		}
	public GAMESTATE gameState;
	public FLOORSTATE floorState;
	
	public Text stateLabel;
	
	public LayerMask blockLayer;	// Other object's layer (set by Inspector)
	
	//public bool isMoving = false;
	private int direction = 0;	// 1 = horizontal , 2 = vertical
	private GameObject holdObj = null;	// Touched object
	private ArrayList holdObjs = null;	// Other object	

	private Player player;	// for cache
	private SpriteRenderer srCantMove;	// for cache
	
	
	// Use this for initialization
	void Awake(){
		
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)	
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);   
				
		holdObjs = new ArrayList();
	}
	
	void Start () {
		this.player = GameObject.Find("Player").gameObject.GetComponent<Player>();
		this.srCantMove = GameObject.Find("no_slide").gameObject.GetComponent<SpriteRenderer>();
		Ready();
	}
	
	void Ready()
	{
		gameState = GAMESTATE.Ready;
	}
	
	void GameStart()
	{
		gameState = GAMESTATE.Explore;
		this.player.MoveStart();

		SpriteRenderer sr = GameObject.Find("touch_start").GetComponent<SpriteRenderer>();
		sr.enabled = false;
	}
	
	public void GameOver()
	{
		gameState = GAMESTATE.GameOver;
		this.player.MoveStop();

		SpriteRenderer sr = GameObject.Find("gameover").GetComponent<SpriteRenderer>();
		sr.enabled = true;		
	}
	
	public void StageClear()
	{
		gameState = GAMESTATE.StageClear;
		this.player.MoveStop();
		
		SpriteRenderer sr = GameObject.Find("stage_clear").GetComponent<SpriteRenderer>();
		sr.enabled = true;
	}
	
	void LateUpdate()
	{
		switch(gameState)
		{
			case GAMESTATE.Ready:
				if(Input.touchCount > 0) GameStart();
				break;
			case GAMESTATE.StageClear:
			case GAMESTATE.GameOver:
			
				
				if(Input.touchCount > 0)
				{
					Application.LoadLevel(Application.loadedLevel);
				}
				break;
			default : break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		//print (gameState);
	
		if(gameState == GAMESTATE.Ready || gameState == GAMESTATE.StageClear || gameState == GAMESTATE.GameOver)
		{
			// Game is not started or over
			return;
		}
		
		//if(this.player.state == Player.STATE.moving)
		if(this.player.state == Player.STATE.moving)
		{
			// Now Player is at Smooth Moving coroutine, so prevent to move floor.
			return;
		}
	
		if(this.floorState == FLOORSTATE.Stay)
		{
			FeverChecker fc = new FeverChecker();
			fc.checkConnectAll();
		}
	
		// Hold and Move object
		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			
			switch(t.phase)
			{		
				case TouchPhase.Began :
					
					//isMoving = false;
					floorState = FLOORSTATE.Stay;
					direction = 0;
					this.srCantMove.enabled = false;
									
					Vector3 touchPoint_screen = new Vector3(t.position.x,t.position.y,1);
					Vector3 touchPoint_world = Camera.main.ScreenToWorldPoint(touchPoint_screen);
					
					Collider2D col =  Physics2D.OverlapPoint(touchPoint_world,blockLayer);	// Detect Touched Block
					if(col)
					{
						// Hold Touched Block										
						this.holdObj = col.gameObject;	
						//this.holdObj.layer = LayerMask.NameToLayer("TouchedBlock");
						
						//gameState = GAMESTATE.FloorMoving;
						floorState = FLOORSTATE.AttemptFloorMove;
					}								
					break;
					
				case TouchPhase.Stationary :
					break;
					
				case TouchPhase.Moved :
					on_move(t);
					break;
					
				default:	// End or Cancel
					print(t.phase);
					on_end ();				
					break;
			}
			
		}
	}
	
	private void on_move(Touch t)
	{
		if(this.holdObj == null)
		{
			return;
		}
		else if(floorState == FLOORSTATE.Stay)
		{
			this.holdObj = null;
			return;
		}
		else
		{
			Vector3 curPos = holdObj.transform.position;
			Vector3 myDeltaPos = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x,t.position.y,1)) - curPos;
			
			//if(isMoving == false)
			if(floorState == FLOORSTATE.AttemptFloorMove)
			{
				if(Mathf.Abs(myDeltaPos.x) > 0.3 || Mathf.Abs(myDeltaPos.y) > 0.3)
				{					
					if(Mathf.Abs(myDeltaPos.x) > Mathf.Abs(myDeltaPos.y))
					{
						direction = 1;	// Move horizontal(X axis)
					}
					else
					{
						direction = 2;	// Move vertical(Y axis)
					}
					
					// Prevent to move player's same line floors								
					if(
						(direction == 1 && this.holdObj.transform.position.y == this.player.fixedPosition.y)
						|| (direction == 2 &&  this.holdObj.transform.position.x == this.player.fixedPosition.x)
					)
					{
						print ("Player exists same line!");
						direction = 0;
						//this.stateLabel.text = "Sorry,can't move";
						this.srCantMove.enabled = true;
					}
					else
					{
						//isMoving = true;
						floorState = FLOORSTATE.FloorDirectionDicided;
						//this.stateLabel.text = "";
						this.srCantMove.enabled = false;
					}
				}
			}
			
			if(floorState == FLOORSTATE.FloorDirectionDicided)
			{												
				floorState = FLOORSTATE.FloorMoving;
				// Detect other objects on same line
				Vector2 origin;
				RaycastHit2D[] cols;
				if(direction == 1)
				{
					origin = new Vector2(0,curPos.y);
					cols = Physics2D.RaycastAll(origin,Vector2.right,5.0f,blockLayer);							
				}
				else
				{
					origin = new Vector2(curPos.x,0);
					cols = Physics2D.RaycastAll(origin,Vector2.up,5.0f,blockLayer);								
				}
				
				foreach(RaycastHit2D hit in cols)
				{
					this.holdObjs.Add(hit.collider.gameObject);
				}
				
				// Create out of camera objects
				GameObject plus;
				GameObject minus;
				
				ArrayList tmp = new ArrayList();
				foreach(GameObject obj in this.holdObjs)
				{												
					plus = Object.Instantiate(obj);
					minus = Object.Instantiate(obj);
					
					if(direction == 1)
					{
						plus.transform.position = new Vector3(obj.transform.position.x + 5, obj.transform.position.y, 0);						
						minus.transform.position = new Vector3(obj.transform.position.x - 5, obj.transform.position.y, 0);
					}
					else
					{
						plus.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + 5, 0);							
						minus.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 5, 0);
					}

					tmp.Add(plus);
					tmp.Add(minus);
				}
				this.holdObjs.AddRange(tmp);

				// HighLight
				foreach(GameObject obj in this.holdObjs)
				{
					obj.GetComponent<Block>().isHighLight = true;
				}
				
				/*
				plus = Object.Instantiate(this.holdObj);
				minus = Object.Instantiate(this.holdObj);
				
				if(direction == 1)
				{
					plus.transform.position = new Vector3(curPos.x+5,curPos.y,0);						
					minus.transform.position = new Vector3(curPos.x-5,curPos.y,0);
				}
				else
				{
					plus.transform.position = new Vector3(curPos.x,curPos.y+5,0);							
					minus.transform.position = new Vector3(curPos.x,curPos.y-5,0);
				}
				
				this.holdObjs.Add(plus);
				this.holdObjs.Add(minus);	
				*/
				
				//print (holdObjs.Count);
			}
			
			// Move block
			//if(isMoving)
			if(floorState == FLOORSTATE.FloorMoving)
			{
				Vector3 newPos = Vector3.zero;

				// Move touched block
				newPos = Vector3.zero;
				if(direction == 1)
				{
					newPos = curPos + new Vector3(myDeltaPos.x,0,0);
				}
				else if(direction == 2)
				{
					newPos = curPos + new Vector3(0,myDeltaPos.y,0);
				}
				
				if(newPos.x > 4 || newPos.x < 0 || newPos.y > 4 || newPos.y < 0)
				{
					print ("Out of Range!");
				}
				else
				{			
					//this.holdObj.transform.position = newPos;
							
					// Move same line blocks
					foreach(GameObject obj in holdObjs)
					{
						newPos = Vector3.zero;
						Vector3 curPos2 = obj.transform.position;
						
						if(direction == 1)
						{
							newPos = curPos2 + new Vector3(myDeltaPos.x,0,0);
						}
						else if(direction == 2)
						{
							newPos = curPos2 + new Vector3(0,myDeltaPos.y,0);
						}
						
						obj.transform.position = newPos;
					}
				}
			}												
		}
	}
	
	private void on_end()
	{		
		if(this.holdObj)
		{
			Vector3 fixPos;
			Vector3 curPos;

			// fix touched block position
			/*	
			curPos = this.holdObj.transform.position;
			fixPos = new Vector3(Mathf.Round(curPos.x),Mathf.Round(curPos.y),0);
			holdObj.transform.position = fixPos;
			*/
		
			// fix other block position and Destroy
			for(int i=0;i < this.holdObjs.Count; i++)
			{
				GameObject obj = (GameObject)this.holdObjs[i];
				curPos = obj.transform.position;
				fixPos = new Vector3(Mathf.RoundToInt(curPos.x),Mathf.RoundToInt(curPos.y),0);				
				obj.transform.position = fixPos;
				
				// HighLight off
				obj.GetComponent<Block>().isHighLight = false;
				
				if(fixPos.x > 4 || fixPos.x < 0 || fixPos.y > 4 || fixPos.y < 0)
				{
					Destroy(obj);		
				}
			}
			
			//isMoving = false;
			floorState = FLOORSTATE.Stay;
			direction = 0;

			// layer reset
			//this.holdObj.layer = LayerMask.NameToLayer("Block");
			
			this.holdObj = null;
			this.holdObjs.Clear();
		}
		this.srCantMove.enabled = false;
	}
	
	/*
	void CheckFever()
	{
						
		for(int x = 0; x < 5; x++)
		{
			for(int y = 0; y < 5; y++)
			{
				Collider2D col = Physics2D.OverlapPoint(new Vector3(x,y,0),blockLayer);
				Block block = col.gameObject.GetComponent<Block>();

				Block a2ndBlock = null;
				Block a3rdBlock = null;
				
				a2ndBlock = block.getConnectedNearBlock(Block.DIRECTION.up);
				if(a2ndBlock)
				{
					a3rdBlock = a2ndBlock.getConnectedNearBlock(Block.DIRECTION.left);
					if(a3rdBlock)
					{
						block.isFever = true;
						a2ndBlock.isFever = true;
						a3rdBlock.isFever = true;
					}
					
					a3rdBlock = a2ndBlock.getConnectedNearBlock(Block.DIRECTION.up);
					if(a3rdBlock)
					{
						block.isFever = true;
						a2ndBlock.isFever = true;
						a3rdBlock.isFever = true;
					}
					
					a3rdBlock = a2ndBlock.getConnectedNearBlock(Block.DIRECTION.right);
					if(a3rdBlock)
					{
						block.isFever = true;
						a2ndBlock.isFever = true;
						a3rdBlock.isFever = true;
					}
				}
								
			}
		}
	}
	*/	
	
}
