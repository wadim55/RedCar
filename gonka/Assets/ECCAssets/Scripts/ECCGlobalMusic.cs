using UnityEngine;
using System.Collections;

namespace EndlessCarChase
{

	public class ECCGlobalMusic : MonoBehaviour 
	{
		
		public string musicTag = "Music";
		
		
		internal float instanceTime = 0;

		
		void  Awake()
		{
		
			GameObject[] musicObjects = GameObject.FindGameObjectsWithTag(musicTag);
			
			
			if ( musicObjects.Length > 1 )
			{
				foreach( var musicObject in musicObjects )
				{
					if ( musicObject.GetComponent<ECCGlobalMusic>().instanceTime <= 0 )    Destroy(gameObject);
				}
			}
		}

	
		void  Start()
		{
		
			DontDestroyOnLoad(transform.gameObject);
		}
		
	}
}
