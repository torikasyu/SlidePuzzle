using UnityEngine;
using System.Collections;

public class FeverChecker {

	private ArrayList feverBlocks = null;	
	private ArrayList tmpList = null;
	private int blockLayer;
						
	public void checkConnectAll()
	{
		feverBlocks = new ArrayList();
		LayerMask blockLayerMask = LayerMask.NameToLayer("Block");
		blockLayer = 1 << blockLayerMask.value;
		
		// init for check
		for(int x = 0; x < 5; x++)
		{
			for(int y = 0; y < 5; y++)
			{
				Collider2D col = Physics2D.OverlapPoint(new Vector3(x,y,0),blockLayer);				
				if(col)
				{
					Block block = col.gameObject.GetComponent<Block>();
				
					block.isFever = false;
					block.feverChecked = false;				
				}
			}
		}

		/*
		for(int x = 0; x < 5; x++)
		{
			for(int y = 0; y < 5; y++)
			{
				checkConnect(x,y);
			}
		}
		*/
		checkConnect(0,0);
		
								
	}
	
	private void checkConnect(int posX,int posY)
	{
		tmpList = new ArrayList();
		this.checkConnectRecursive(posX,posY);
	
		Debug.Log("tmp:" + tmpList.Count);		
	}
	
	private void checkConnectRecursive(int posX,int posY)
	{
			
		Collider2D tmpCol = Physics2D.OverlapPoint (new Vector2(posX,posY),blockLayer);
		if(!tmpCol)
		{
			Debug.Log("no col");
		}
		else
		{
			Block block = tmpCol.GetComponent<Block>();
			if(block.feverChecked)
			{
				return;
			}
			
			if(tmpList.Count == 0)
			{		
				tmpList.Add(block);
				Debug.Log("aaa" + tmpList.Count);
			}

			Block b;
			b = block.getConnectedNearBlock(Block.DIRECTION.up);
			if(b)
			{
				tmpList.Add(b);
				checkConnectRecursive(posX, posY + 1);
			}
			b = block.getConnectedNearBlock(Block.DIRECTION.down);
			if(b)
			{
				tmpList.Add(b);
				checkConnectRecursive(posX, posY - 1);
			}			
			b = block.getConnectedNearBlock(Block.DIRECTION.right);
			if(b)
			{
				tmpList.Add(b);
				checkConnectRecursive(posX + 1, posY);
			}
			b = block.getConnectedNearBlock(Block.DIRECTION.left);
			if(b)
			{
				tmpList.Add(b);
				checkConnectRecursive(posX - 1, posY);
			}
			
			/*
			if(block.checkNearBlockConnect(Block.DIRECTION.down))
			{
				tmpList.Add(block.getConnectedNearBlock(Block.DIRECTION.down));
				checkConnectRecursive(posX, posY - 1);	
			}
			if(block.checkNearBlockConnect(Block.DIRECTION.left))
			{
				tmpList.Add(block.getConnectedNearBlock(Block.DIRECTION.left));
				checkConnectRecursive(posX - 1, posY);
				
			}
			if(block.checkNearBlockConnect(Block.DIRECTION.right))
			{
				tmpList.Add(block.getConnectedNearBlock(Block.DIRECTION.right));
				checkConnectRecursive(posX + 1, posY);
			}
			*/
									
			block.feverChecked = true;
		}
	}
	
}
