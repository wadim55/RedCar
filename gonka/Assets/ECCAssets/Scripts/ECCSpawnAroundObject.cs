using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessCarChase.Types;

namespace EndlessCarChase
{
	/// <summary>
	/// Этот скрипт порождает объекты вокруг центрального объекта, такого как игрок. Порожденные объекты могут быть, например, предметами, или вражескими автомобилями, или препятствиями.
	/// </summary>
	public class ECCSpawnAroundObject : MonoBehaviour 
	{
        static ECCGameController gameController;

        [Tooltip("Тег объекта, вокруг которого порождаются другие объекты в ограниченном диапазоне")]
        public string spawnAroundTag = "Player";
        internal Transform spawnAroundObject;

        [Tooltip("Тумблер, который включает и выключает нерест. Если Это правда, то сейчас мы порождаем объекты")]
        public bool isSpawning = false;

        [System.Serializable]
        public class SpawnGroup
        {
            [Tooltip("Список всех объектов, которые будут порождены")]
            public Transform[] spawnObjects;

            [Tooltip("Скорость, с которой объекты порождаются, в секундах.")]
            public float spawnRate = 5;
            internal float spawnRateCount = 0;
            internal int spawnIndex = 0;

            [Tooltip("Расстояние, на котором этот объект порождается относительно объекта spawnAroundObject")]
            public Vector2 spawnObjectDistance = new Vector2(10, 20);
        }

        [Tooltip("Массив групп спавна. Это могут быть вражеские автомобили, пикапы или камни-препятствия, например")]
        public SpawnGroup[] spawnGroups;

        internal int index;

        private void Start()
        {
     
            if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();
        }

       
        void Update()
		{
          
            if ( spawnAroundObject == null && spawnAroundTag != string.Empty && GameObject.FindGameObjectWithTag(spawnAroundTag) ) spawnAroundObject = GameObject.FindGameObjectWithTag(spawnAroundTag).transform;

    
            if (isSpawning == false) return;
          
            for ( index = 0; index < spawnGroups.Length; index++ )
            {
           
                if ( spawnGroups[index].spawnObjects.Length > 0 )
                {
                 
                    if (spawnGroups[index].spawnRateCount > 0) spawnGroups[index].spawnRateCount -= Time.deltaTime;
                    else
                    {
                    
                        Spawn(spawnGroups[index].spawnObjects, spawnGroups[index].spawnIndex, spawnGroups[index].spawnObjectDistance);

                     
                        spawnGroups[index].spawnIndex++;

                      
                        if (spawnGroups[index].spawnIndex > spawnGroups[index].spawnObjects.Length - 1) spawnGroups[index].spawnIndex = 0;

                      
                        spawnGroups[index].spawnRateCount = spawnGroups[index].spawnRate;
                    }

                }
            }
		}

       
        public void Spawn( Transform[] spawnArray, int spawnIndex, Vector2 spawnGap )
        {
      
            if (spawnArray[spawnIndex] == null) return;

         
            Transform newSpawn = Instantiate(spawnArray[spawnIndex]) as Transform;
            
       
            if (spawnAroundObject) newSpawn.position = spawnAroundObject.transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

       
            newSpawn.eulerAngles = Vector3.up * Random.Range(0, 360);
            newSpawn.Translate(Vector3.forward * Random.Range(spawnGap.x, spawnGap.y), Space.Self);

           
            newSpawn.eulerAngles += Vector3.up * 180;

          
            RaycastHit hit;

            if (Physics.Raycast(newSpawn.position + Vector3.up * 5, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) newSpawn.position = hit.point;


        }
    }
}