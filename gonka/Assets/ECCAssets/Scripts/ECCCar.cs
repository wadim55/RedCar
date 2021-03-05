using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EndlessCarChase
{
    /// <summary>
    /// Этот сценарий определяет автомобиль, который имеет здоровье, скорость, скорость вращения, повреждения и другие атрибуты, связанные с поведением автомобиля. Он также определяет элементы управления ИИ, когда автомобиль не управляется игроком.
    /// </summary>
    public class ECCCar : MonoBehaviour
    {
      
        internal Transform thisTransform;
        static ECCGameController gameController;
        static Transform targetPlayer;
        internal Vector3 targetPosition;

        internal RaycastHit groundHitInfo;
        internal Vector3 groundPoint;
        internal Vector3 forwardPoint;
        internal float forwardAngle;
        internal float rightAngle;

        [Tooltip("Здоровье игрока. Если это значение достигает 0, игрок умирает")]
        public float health = 10;
        internal float healthMax;

        internal Transform healthBar;
        internal Image healthBarFill;

        [Tooltip("Когда автомобиль получает удар и ушиб, возникает задержка, во время которой он не может быть сбит снова")]
        public float hurtDelay = 2;
        internal float hurtDelayCount = 0;

        [Tooltip("Цвет, в котором автомобиль мигает, когда ему больно")]
        public Color hurtFlashColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Tooltip("Скорость игрока, насколько быстро он двигается игроком. Игрок постоянно движется вперед")]
        public float speed = 10;

        [Tooltip("Как быстро автомобиль игрока вращается в обоих направлениях")]
        public float rotateSpeed = 200;
        internal float currentRotation = 0;

        [Tooltip("Ущерб, который этот автомобиль наносит при столкновении с другими автомобилями. Урон от здоровья уменьшается.")]
        public int damage = 1;

        [Tooltip("Эффект, который возникает, когда этот автомобиль сбивает другой автомобиль")]
        public Transform hitEffect;

        [Tooltip("Эффект, который появляется, когда этот автомобиль умирает")]
        public Transform deathEffect;

        [Tooltip("Небольшое дополнительное вращение, которое происходит с автомобилем при повороте, дает эффект дрейфа")]
        public float driftAngle = 50;

        [Tooltip("Небольшой боковой наклон, который происходит с шасси автомобиля при повороте, заставляя его наклоняться внутрь или наружу от центра вращения")]
        public float leanAngle = 10;

        [Tooltip("Шасси объект автомобиля который наклоняется при вращении автомобиля")]
        public Transform chassis;

        [Tooltip("Колеса автомобиля, которые вращаются в зависимости от скорости автомобиля. Передние колеса также вращаются в направлении поворота автомобиля")]
        public Transform[] wheels;

        [Tooltip("Передние колеса автомобиля также вращаются в направлении поворота автомобиля")]
        public int frontWheels = 2;

        internal int index;

        [Header("Атрибуты автомобиля AI")]
        [Tooltip("Случайная величина, которая добавляется к базовой скорости автомобиля ИИ, чтобы сделать их движения более разнообразными")]
        public float speedVariation = 2;

        // The angle range that AI cars try to chase the player at. So for example if 0 they will target the player exactly, while at 30 angle they stop rotating when they are at a 30 angle relative to the player
        internal float chaseAngle;

        [Tooltip("Случайное значение, которое относится к углу погони, чтобы сделать автомобили ИИ более разнообразными в том, как преследовать игрока")]
        public Vector2 chaseAngleRange = new Vector2(0, 30);

        [Tooltip("Сделайте так, чтобы ИИ-автомобили старались избегать препятствий. Препятствие - это объекты, к которым прикреплен компонент ECCObstacle")]
        public bool avoidObstacles = true;

        [Tooltip("Ширина зоны обнаружения препятствий для этого автомобиля AI")]
        public float detectAngle = 2;

        [Tooltip("Переднее расстояние зоны обнаружения препятствий для этого автомобиля с искусственным интеллектом")]
        public float detectDistance = 3;

       
        public float moveHeight = 0;

        private void Start()
        {
            thisTransform = this.transform;

          
            if ( gameController == null )    gameController = GameObject.FindObjectOfType<ECCGameController>();
            if ( targetPlayer == null && gameController.gameStarted == true && gameController.playerObject )    targetPlayer = gameController.playerObject.transform;


            RaycastHit hit;

            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) forwardPoint = hit.point;

            thisTransform.Find("Base").LookAt(forwardPoint);

           
            if (gameController.playerObject != this)
            {
               
                chaseAngle = Random.Range(chaseAngleRange.x, chaseAngleRange.y);

              
                speed += Random.Range(0, speedVariation);
            }

         
            if ( thisTransform.Find("HealthBar") )
            {
                healthBar = thisTransform.Find("HealthBar");

                healthBarFill = thisTransform.Find("HealthBar/Empty/Full").GetComponent<Image>();
            }

          
            healthMax = health;

           
            ChangeHealth(0);
        }

        private void OnValidate()
        {
            
            frontWheels = Mathf.Clamp(frontWheels, 0, wheels.Length);
        }

      
        void Update()
        {
           
            if (gameController && gameController.gameStarted == false) return;

        
            thisTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

           
            if ( health > 0 )
            {
                if (targetPlayer) targetPosition = targetPlayer.transform.position;

                if (healthBar)    healthBar.LookAt(Camera.main.transform);
            }
            else
            {
                if (healthBar && healthBar.gameObject.activeSelf == true ) healthBar.gameObject.SetActive(false);
            }

         
            if ( gameController.playerObject != this )
            {
                
                Ray rayRight = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
                Ray rayLeft = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

                RaycastHit hit;
                
               
                if ( avoidObstacles == true && Physics.Raycast(rayRight, out hit, detectDistance) && (hit.transform.GetComponent<ECCObstacle>() || (hit.transform.GetComponent<ECCCar>() && gameController.playerObject != this)) )
                {
                  
                    Rotate(-1);

                    
                }
                else if ( avoidObstacles == true && Physics.Raycast(rayLeft, out hit, detectDistance) && (hit.transform.GetComponent<ECCObstacle>() || (hit.transform.GetComponent<ECCCar>() && gameController.playerObject != this))) // Otherwise, if we detect an obstacle on our left side, swerve to the right
                {
                    
                    Rotate(1);

                }
                else
                {
                   
                    if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > chaseAngle)
                    {
                        Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                    }
                    else 
                    {
                        Rotate(0);
                    }
                }
            }

           
            if ( gameController.groundObject == null || gameController.groundObject.gameObject.activeSelf == false )    DetectGround();

     
            if (hurtDelayCount > 0 && health > 0)
            {
                hurtDelayCount -= Time.deltaTime;

              
                if ( GetComponentInChildren<MeshRenderer>() )
                {
                    foreach ( Material part in GetComponentInChildren<MeshRenderer>().materials )
                    {
                        if (Mathf.Round(hurtDelayCount * 10) % 2 == 0) part.SetColor("_EmissionColor", Color.black);
                        else part.SetColor("_EmissionColor", hurtFlashColor);

                  
                    }
                }

            }
        }
        

        /// <summary>
        /// Вычисляет угол сближения объекта с другим объектом
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="targetDirection"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
        {
           
            float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);
            
          
            if (approachAngle > 0f)
            {
                return 1f;
            }
            else if (approachAngle < 0f) 
            {
                return -1f;
            }
            else 
            {
                return 0f;
            }
        }


        /// <summary>
        ///Поворачивает автомобиль влево или вправо и применяет соответствующие эффекты наклона и дрейфа
        /// </summary>
        /// <param name="rotateDirection"></param>
        public void Rotate( float rotateDirection )
        {
          
            if ( rotateDirection != 0 )
            {
               
                thisTransform.localEulerAngles += Vector3.up * rotateDirection * rotateSpeed * Time.deltaTime;

                thisTransform.eulerAngles = new Vector3(thisTransform.eulerAngles.x, thisTransform.eulerAngles.y, thisTransform.eulerAngles.z);

               

                currentRotation += rotateDirection * rotateSpeed * Time.deltaTime;

                if (currentRotation > 360) currentRotation -= 360;
               
                thisTransform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

               
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);
                
              
                GetComponent<Animator>().Play("Skid");

               
                for (index = 0; index < wheels.Length; index++)
                {
                
                    if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);

                 
                    wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 20, Space.Self);
                }
            }
            else 
            {
            
                thisTransform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);

              
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * 5);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

               
                GetComponent<Animator>().Play("Move");

              
                for (index = 0; index < wheels.Length; index++)
                {
                   
                    if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);

              
                    wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 30, Space.Self);
                }
            }
        }

       
       
        void OnTriggerStay(Collider other)
        {
           
            if ( hurtDelayCount <= 0  && other.GetComponent<ECCCar>() )
            {
              
                hurtDelayCount = hurtDelay;

                
                other.GetComponent<ECCCar>().ChangeHealth(-damage);

          
                if (health - damage > 0 && hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);
            }
        }

       
               public void ChangeHealth(float changeValue)
        {
            
            health += changeValue;

        
            if (health > healthMax) health = healthMax;

        
            if ( healthBar )
            {
                healthBarFill.fillAmount = health / healthMax;
            }

      
            if (changeValue < 0 && gameController.playerObject == this) Camera.main.transform.parent.GetComponent<Animation>().Play();

        
            if (health <= 0)
            {
                if (gameController.playerObject && gameController.playerObject != this)
                {
                    DelayedDie();
                }
                else
                {
                    Die();
                }

           
                if (gameController.playerObject && gameController.playerObject == this)
                {
                    gameController.SendMessage("GameOver", 1.2f);

                   
                    Time.timeScale = 0.5f;
                }
            }

       
            if ( gameController.playerObject && gameController.playerObject == this && gameController.healthCanvas)
            {
           
                if (gameController.healthCanvas.Find("Full")) gameController.healthCanvas.Find("Full").GetComponent<Image>().fillAmount = health / healthMax;

                if (gameController.healthCanvas.Find("Text")) gameController.healthCanvas.Find("Text").GetComponent<Text>().text = health.ToString();

        
                if (gameController.healthCanvas.GetComponent<Animation>()) gameController.healthCanvas.GetComponent<Animation>().Play();
            }
        }

     
        public void Die()
        {
           
            if (deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);

   
            Destroy(gameObject);
        }

      
        public void DelayedDie()
        {
           
            for (index = 0; index < wheels.Length; index++)
            {
                wheels[index].transform.SetParent(chassis);
            }

            targetPosition = thisTransform.forward * -10;

            leanAngle = Random.Range(100,300);

            driftAngle = Random.Range(100, 150); ;



            Invoke("Die", Random.Range(0,0.8f));
        }

                public void DetectGround()
        {
           
            Ray carToGround = new Ray(thisTransform.position + Vector3.up * 10, -Vector3.up * 20);

          
            if (Physics.Raycast(carToGround, out groundHitInfo, 20, gameController.groundLayer))
            {
                //transform.position = new Vector3(transform.position.x, groundHitInfo.point.y, transform.position.z);
            }
            
          
            thisTransform.position = new Vector3(thisTransform.position.x, groundHitInfo.point.y + 0.1f, thisTransform.position.z);

            RaycastHit hit;

          
            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer))
            {
                forwardPoint = hit.point;
            }
            else if ( gameController.groundObject && gameController.groundObject.gameObject.activeSelf == true )
            {
                forwardPoint = new Vector3(thisTransform.position.x, gameController.groundObject.position.y, thisTransform.position.z);
            }

          
            thisTransform.Find("Base").LookAt(forwardPoint);
        }


        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

            Gizmos.DrawSphere(forwardPoint, 0.5f);
        }
    }
}
