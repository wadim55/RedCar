using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace EndlessCarChase
{

	public class ECCSelectButton : MonoBehaviour 
	{
		
		public GameObject selectedButton;

	
		void OnEnable() 
		{
            if ( selectedButton )    
			{
			
				if ( EventSystem.current )    EventSystem.current.SetSelectedGameObject(selectedButton);
			}	
		}
	}
}
