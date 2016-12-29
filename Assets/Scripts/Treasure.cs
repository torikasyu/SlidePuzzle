using UnityEngine;
using System.Collections;

public class Treasure : MonoBehaviour {

	private bool isPicked = false;
	private GameObject coinParticle;
	private GameObject lightEffect;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void Picked()
	{
		if(isPicked)return;

		isPicked = true;
	
		GameObject prefabCoin = (GameObject)Resources.Load ("Prefabs/CoinParticle");
		coinParticle = (GameObject)Instantiate (prefabCoin, transform.position, Quaternion.identity);
		coinParticle.GetComponent<Renderer>().sortingLayerName = "SLCoinParticle";
		
		GameObject prefabLight = (GameObject)Resources.Load ("Prefabs/LightEffect");
		lightEffect = (GameObject)Instantiate(prefabLight, transform.position, Quaternion.identity);
		
		Destroy(gameObject,1.0f);
		Destroy(coinParticle,1.0f);
		Destroy(lightEffect,1.0f);
	}
	
	// Update is called once per frame
	void Update () {
			
		if(lightEffect)
		{
			if(lightEffect.transform.localScale.x < 1.5)
			{
				Vector3 scale = new Vector3(lightEffect.transform.localScale.x + 0.02f,lightEffect.transform.localScale.y + 0.02f,0);
				lightEffect.transform.localScale = scale;
				
				//lightEffect.transform.rotation = 
				Vector3 curAngle = lightEffect.transform.eulerAngles;
				Vector3 newAngle = new Vector3(curAngle.x,curAngle.y,curAngle.z + 1.0f);
				lightEffect.transform.eulerAngles = newAngle;
			}
			else
			{
				Destroy(lightEffect);
			}
		}
		
	}
}
