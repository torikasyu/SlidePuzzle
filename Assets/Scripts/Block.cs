using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public bool up = false;
	public bool down = false;
	public bool right = false;
	public bool left = false;
	
	public enum DIRECTION {up,right,down,left}
	
	public LayerMask blockLayer;
	
	// for fever
	public bool isFever = false;
	public bool feverChecked = false;
	
	// Use this for initialization
	void Start () {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.color = new Color(0.8f,0.8f,0.8f);
	}
	
	public bool isHighLight = false;
	
	// Update is called once per frame
	void Update () {

		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		
		if(isHighLight)
		{			
			sr.color = new Color(1.0f,1.0f,1.0f);				
		}
		else
		{
			sr.color = new Color(0.8f,0.8f,0.8f);
		}
	
		/*		
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		
		bool connect = false;
		Vector3 curPos = transform.position;		
		
		Collider2D col_right = Physics2D.OverlapPoint (new Vector2(curPos.x + 1,curPos.y),blockLayer);
		Collider2D col_left = Physics2D.OverlapPoint (new Vector2(curPos.x - 1,curPos.y),blockLayer);
		Collider2D col_up = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + 1),blockLayer);
		Collider2D col_down = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y - 1),blockLayer);
				
		if(col_up)
		{
			if(col_up.GetComponent<Block>().down && this.up){connect=true;}
		}		
		
		if(col_down)
		{
			if(col_down.GetComponent<Block>().up && this.down){connect=true;}
		}		
		
		if(col_right)
		{
			if(col_right.GetComponent<Block>().left && this.right){connect=true;}
		}		
		
		if(col_left)
		{
			if(col_left.GetComponent<Block>().right && this.left){connect=true;}
		}		
				
		
		if(connect)
		{
			sr.color = new Color(1.0f,1.0f,1.0f);				
		}
		else
		{
			sr.color = new Color(0.5f,0.5f,0.5f);
		}
		*/
	}
	
	public bool checkNearBlockConnect(DIRECTION dir)
	{
		Vector3 curPos = transform.position;
					
		Collider2D col_right = null;
		Collider2D col_left = null;
		Collider2D col_up = null;
		Collider2D col_down = null;		
		
		if(dir == DIRECTION.up)
		{
			col_up = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + 1),blockLayer);
			if(col_up && col_up.GetComponent<Block>().down && this.up) return true;
		}
		else if(dir == DIRECTION.down)
		{
			col_down = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y - 1),blockLayer);
			if(col_down && col_down.GetComponent<Block>().up && this.down) return true;
		}
		else if(dir == DIRECTION.right)
		{
			col_right = Physics2D.OverlapPoint (new Vector2(curPos.x + 1,curPos.y),blockLayer);
			if(col_right && col_right.GetComponent<Block>().left && this.right) return true;	
		}
		else if(dir == DIRECTION.left)
		{
			col_left = Physics2D.OverlapPoint (new Vector2(curPos.x - 1,curPos.y),blockLayer);				
			if(col_left && col_left.GetComponent<Block>().right && this.left) return true;	
		}
		else return false;
		
		return false;					
	}

	public Block getConnectedNearBlock(DIRECTION dir)
	{
		Vector3 curPos = transform.position;

		Collider2D tmpCol = null;
		Block tmpBlock = null;

		if(dir == DIRECTION.up)
		{
			tmpCol = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y + 1),blockLayer);
			if (tmpCol != null && tmpCol.GetComponent<Block> ().down)
				return tmpCol.GetComponent<Block> ();
			else
				return null;
		}
		else if(dir == DIRECTION.down)
		{
			tmpCol = Physics2D.OverlapPoint (new Vector2(curPos.x,curPos.y - 1),blockLayer);
			if(tmpCol != null && tmpCol.GetComponent<Block>().up) return tmpCol.GetComponent<Block>();				
			else
				return null;
			
		}
		else if(dir == DIRECTION.right)
		{
			tmpCol = Physics2D.OverlapPoint (new Vector2(curPos.x + 1,curPos.y),blockLayer);
			if(tmpCol != null && tmpCol.GetComponent<Block>().left) return tmpCol.GetComponent<Block>();					
			else
				return null;
			
		}
		else if(dir == DIRECTION.left)
		{
			tmpCol = Physics2D.OverlapPoint (new Vector2(curPos.x - 1,curPos.y),blockLayer);
			if(tmpCol != null && tmpCol.GetComponent<Block>().right) return tmpCol.GetComponent<Block>();
			else
				return null;
		}
		else return null;
	}

	
}
