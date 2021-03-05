using UnityEngine;

namespace EndlessCarChase
{
  
    public class ECCItem : MonoBehaviour
    {
        static ECCGameController gameController;
        
        [Tooltip("Функция, которая запускается, когда этот объект прикасается к цели")]
        public string touchFunction = "ChangeScore";

        [Tooltip("Параметр, который будет передан вместе с функцией")]
        public float functionParameter = 100;

        [Tooltip("Тег целевого объекта, с которого будет воспроизводиться функция")]
        public string functionTarget = "GameController";

        [Tooltip("Эффект, который создается в месте расположения этого элемента при его подборе")]
        public Transform pickupEffect;

        [Tooltip("Случайное вращение, заданное объекту только по оси Y")]
        public float randomRotation = 360;

        void Start()
        {
            
            transform.eulerAngles += Vector3.up * Random.Range(-randomRotation, randomRotation);

       
            if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();
        }

                void OnTriggerEnter(Collider other)
        {
           
            if (gameController.playerObject && other.gameObject == gameController.playerObject.gameObject)
            {
               
                if (touchFunction != string.Empty)
                {
              
                    GameObject.FindGameObjectWithTag(functionTarget).SendMessage(touchFunction, functionParameter);
                }

               
                if (pickupEffect) Instantiate(pickupEffect, transform.position, transform.rotation);
                
             
                Destroy(gameObject);
            }
        }
    }
}