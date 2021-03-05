using UnityEngine;
using System.Collections;

namespace EndlessCarChase
{
	public class ECCRemoveAfterTime : MonoBehaviour 
	{
        [Tooltip("Сколько секунд нужно подождать, прежде чем удалить этот объект")]
        public float removeAfterTime = 1;

	
		void Start() 
		{
	
			Destroy( gameObject, removeAfterTime);
		}
	}
}
