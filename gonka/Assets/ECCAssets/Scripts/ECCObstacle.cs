using UnityEngine;

namespace EndlessCarChase
{
   
    public class ECCObstacle : MonoBehaviour
    {
        [Tooltip("Ущерб, причиненный этим препятствием")]
        public float damage = 1;

        [Tooltip("Эффект, который создается в месте расположения этого объекта при его попадании")]
        public Transform hitEffect;

        [Tooltip("Должно ли это препятствие быть устранено при ударе автомобиля?")]
        public bool removeOnHit = false;

        [Tooltip("Случайное вращение, заданное объекту только по оси Y")]
        public float randomRotation = 360;

        void Start()
        {
           
            transform.eulerAngles += Vector3.up * Random.Range( -randomRotation, randomRotation);
            
           
        }

       
        
        void OnTriggerStay(Collider other)
        {
          
            if (other.GetComponent<ECCCar>() )
            {
              
                    other.GetComponent<ECCCar>().ChangeHealth(-damage);

                
                    if (other.GetComponent<ECCCar>().health - damage > 0 && other.GetComponent<ECCCar>().hitEffect) Instantiate(other.GetComponent<ECCCar>().hitEffect, transform.position, transform.rotation);
              
                if (hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);

           
                if ( removeOnHit == true )    Destroy(gameObject);
            }
        }

        public void ResetColor()
        {
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}