using UnityEngine;
using System.Collections;

public class InputTest : MonoBehaviour {
	
	private bool isMoving = false;
	private int direction = 0;
	public GameObject holdObj;
		
	// Use this for initialization
	void Start () {
		
		//bool supportsMultiTouch = Input.multiTouchEnabled;
		
		//print("MultiTouchSupport : " + supportsMultiTouch);
	}
	
	
	// Update is called once per frame
	void Update () {
		
		//int nbTouches = Input.touchCount;
		
		/*
		if(nbTouches > 0)
		{
			print(nbTouches + " touch(es) detected");
			
			for (int i = 0; i < nbTouches; i++)
			{
				Touch touch = Input.GetTouch(i);
				
				print("Touch index " + touch.fingerId + " detected at position " + touch.position);
			}
		}
		*/
		
		/*
		if(Input.touchCount > 0)
		{
			foreach(Touch t in Input.touches)
			{
				if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
				{
				
					Debug.Log(":x=" + t.position.x + " y=" +  t.position.y);
				}
			}
		}*/

		/*				
		if(Input.touchCount > 0)
		{
			foreach(Touch t in Input.touches)
			{
				if (t.phase == TouchPhase.Began)
				{
					Debug.Log("fid=" + t.fingerId + " x=" + t.position.x + " y=" +  t.position.y);
				}
				else if (t.phase == TouchPhase.Moved)
				{
					print("Touch pos " + t.position + " has moved by " + t.deltaPosition);
				}
			}
		}
		*/
		
		/*   delete bybtap
		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			
			if(t.phase == TouchPhase.Began)
			{		
				Vector3 touchPoint_screen = new Vector3(t.position.x,t.position.y,0);
				Vector3 touchPoint_world = Camera.main.ScreenToWorldPoint(touchPoint_screen);
				
				Collider2D col =  Physics2D.OverlapPoint(touchPoint_world);
				if(col)
				{
					GameObject obj = col.gameObject;
					Destroy(obj);
				}
			}
		}
		*/


		// Hold and Move object
		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			
			switch(t.phase)
			{		
				case TouchPhase.Began :
				
					isMoving = false;
					direction = 0;
					
					Vector3 touchPoint_screen = new Vector3(t.position.x,t.position.y,1);
					Vector3 touchPoint_world = Camera.main.ScreenToWorldPoint(touchPoint_screen);
				
					Collider2D col =  Physics2D.OverlapPoint(touchPoint_world);
					if(col)
					{
						GameObject obj = col.gameObject;
						this.holdObj = obj;
					}								
					break;
					
				case TouchPhase.Stationary :
					break;
			
				case TouchPhase.Moved :
					if(this.holdObj)
					{
						Vector3 newPos;
						Vector3 curPos = transform.position;
						Vector3 myDeltaPos = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x,t.position.y,1)) - curPos;
						
						//print (transform.position);
						//print (myDeltaPos);
						print(myDeltaPos.sqrMagnitude);
					
						if(isMoving == false)
						{
							if(myDeltaPos.magnitude > 5)
							{
								isMoving = true;
								
								if(Mathf.Abs(myDeltaPos.x) > Mathf.Abs(myDeltaPos.y))
								{
									direction = 1;
								}
								else
								{
									direction = 2;
								}
							}
						}
						
						if(direction == 1)
						{
							newPos = curPos + new Vector3(myDeltaPos.x,0,0);
						}
						else
						{
							newPos = curPos + new Vector3(0,myDeltaPos.y,0);
						}
						//print(newPos);
						this.holdObj.transform.position = newPos;
												
					}
					break;
				
				default:	// End or Cancel
					isMoving = false;
					direction = 0;
					this.holdObj = null;
					print(t.phase);
					break;
			}
			
		}
		
												
		
		/*
		if(nbTouches > 0)
		{
			for (int i = 0; i < nbTouches; i++)
			{
				Touch touch = Input.GetTouch(i);
				
				TouchPhase phase = touch.phase;
				
				switch(phase)
				{
				case TouchPhase.Began:
					print("New touch detected at position " + touch.position + " , index " + touch.fingerId);
					break;
				case TouchPhase.Moved:
					print("Touch index " + touch.fingerId + " has moved by " + touch.deltaPosition);
					break;
				case TouchPhase.Stationary:
					print("Touch index " + touch.fingerId + " is stationary at position " + touch.position);
					break;
				case TouchPhase.Ended:
					print("Touch index " + touch.fingerId + " ended at position " + touch.position);
					break;
				case TouchPhase.Canceled:
					print("Touch index " + touch.fingerId + " cancelled");
					break;
				}
			}
		}
		*/
		
	}
}
