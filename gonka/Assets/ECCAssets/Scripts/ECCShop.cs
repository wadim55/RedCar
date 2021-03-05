using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessCarChase.Types;

namespace EndlessCarChase
{
	public class ECCShop : MonoBehaviour 
	{
      
        internal ECCGameController gameController;


        internal int moneyLeft = 0;

        [Tooltip("Текстовый объект, который отображает деньги, которые у нас есть")]
        public Transform moneyText;

        [Tooltip("Текстовый объект, отображающий цену текущего товара")]
        public Text priceText;
    
        [Tooltip("Знак, который появляется рядом с текстом денег")]
        public string moneySign = "$";

        [Tooltip("Игрок префс запись денег у нас есть")]
        public string moneyPlayerprefs = "деньги";

    
        internal int currentItem = 0;

        [Tooltip("Запись префов игрока текущего выбранного нами элемента")]
        public string currentPlayerprefs = "CurrentCar";

        [Tooltip("Список предметов в игре, их имена, сцена, на которую они ссылаются, цена, чтобы разблокировать ее, и состояние предмета ( -1 - заблокирован и не может быть воспроизведен, 0 - разблокирован без звезд, 1-Х звездный рейтинг, который мы получили на предмет")]
        public ShopItem[] items;
        
              internal Transform currentIcon;
        
        [Tooltip("Эффект, который появляется, когда мы разблокируем этот предмет")]
        public Transform unlockEffect;

        [Tooltip("Кнопки для перехода к следующему/предыдущему элементу и выбора элементов")]
        public Button buttonNextItem;
        public Button buttonPrevItem;
        public Button buttonSelectItem;

        [Tooltip("Кнопки клавиатуры/геймпада для изменения и выбора элементов")]
        public string changeItemButton = "Horizontal";
        public string selectItemButton = "Submit";
        internal bool buttonPressed = false;

        [Tooltip("Объект камеры, который включается при активации этого магазина")]
        public GameObject shopCamera;

        [Tooltip("Положение выбранного в данный момент элемента в 3D пространстве")]
        public Vector3 itemIconPosition;

        [Tooltip("Скорость вращения выбранного в данный момент элемента в 3D пространстве")]
        public float itemSpinSpeed = 100;
        internal float itemRotation = 0;

               internal float speedMax = 0;
        internal float rotateSpeedMax = 0;
        internal float healthMax = 0;
        internal float damageMax = 0;

        internal int index;
        
        public void OnEnable()
        {
         
            if (shopCamera) shopCamera.SetActive(true);

     
            moneyLeft = PlayerPrefs.GetInt(moneyPlayerprefs, moneyLeft);


            moneyText.GetComponent<Text>().text = moneyLeft.ToString() + moneySign;

           
            if (speedMax == 0 && rotateSpeedMax == 0 && healthMax == 0 && damageMax == 0)
            {
                for (index = 0; index < items.Length; index++)
                {
                    if (speedMax < items[index].itemIcon.GetComponent<ECCCar>().speed) speedMax = items[index].itemIcon.GetComponent<ECCCar>().speed;
                    if (rotateSpeedMax < items[index].itemIcon.GetComponent<ECCCar>().rotateSpeed) rotateSpeedMax = items[index].itemIcon.GetComponent<ECCCar>().rotateSpeed;
                    if (healthMax < items[index].itemIcon.GetComponent<ECCCar>().health) healthMax = items[index].itemIcon.GetComponent<ECCCar>().health;
                    if (damageMax < items[index].itemIcon.GetComponent<ECCCar>().damage) damageMax = items[index].itemIcon.GetComponent<ECCCar>().damage;
                }
            }

      
            currentItem = PlayerPrefs.GetInt(currentPlayerprefs, currentItem);

            ChangeItem(0);
        }

        public void OnDisable()
        {
        
            if (shopCamera) shopCamera.SetActive(false);

            if (currentIcon)
            {
              
                itemRotation = currentIcon.eulerAngles.y;

                Destroy(currentIcon.gameObject);
            }
        }

        void Start()
        {
           
            if (items.Length <= 0) return;

   
            if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();

      
            buttonNextItem.onClick.AddListener(delegate { ChangeItem(1); });

         
            buttonPrevItem.onClick.AddListener(delegate { ChangeItem(-1); });

           
            buttonSelectItem.onClick.AddListener(delegate { StartCoroutine("SelectItem"); });
        }

        void Update()
        {
        
            if (items.Length <= 0) return;

           
            if (currentIcon) currentIcon.Rotate(Vector3.up * itemSpinSpeed * Time.deltaTime, Space.World);

            if ( buttonPressed == false )
            {
                if (Input.GetAxisRaw(changeItemButton) > 0) buttonNextItem.onClick.Invoke();// ChangeItem(1);
                if (Input.GetAxisRaw(changeItemButton) < 0) buttonPrevItem.onClick.Invoke();// ChangeItem(-1);
                if (Input.GetButtonDown(selectItemButton)) buttonSelectItem.onClick.Invoke();// SelectItem();

                if (Input.GetAxisRaw(changeItemButton) != 0 || Input.GetButton(selectItemButton)) buttonPressed = true;

            }
            else if (Input.GetAxisRaw(changeItemButton) == 0 || Input.GetButtonUp(selectItemButton)) buttonPressed = false;
        }

              public void ChangeItem( int changeValue )
        {

            if (items.Length <= 0) return;

       
            currentItem += changeValue;

      
            if (currentItem > items.Length - 1) currentItem = 0;
            else if (currentItem < 0) currentItem = items.Length - 1;

           
            if (currentIcon)
            {
          
                itemRotation = currentIcon.eulerAngles.y;

                Destroy(currentIcon.gameObject);
            }

          
            if ( items[currentItem].itemIcon )
            {
              
                currentIcon = Instantiate(items[currentItem].itemIcon.transform, itemIconPosition, Quaternion.identity) as Transform;

              
                currentIcon.eulerAngles = Vector3.up * itemRotation;

            
                if (currentIcon.GetComponent<Animation>()) currentIcon.GetComponent<Animation>().Play();

               
                if ( currentIcon.GetComponent<ECCCar>())
                {
                  
                    if (transform.Find("Base/Stats/Speed/Full"))    transform.Find("Base/Stats/Speed/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<ECCCar>().speed/speedMax;
                    if (transform.Find("Base/Stats/RotateSpeed/Full")) transform.Find("Base/Stats/RotateSpeed/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<ECCCar>().rotateSpeed/rotateSpeedMax;
                    if (transform.Find("Base/Stats/Health/Full")) transform.Find("Base/Stats/Health/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<ECCCar>().health/healthMax;
                    if (transform.Find("Base/Stats/Damage/Full")) transform.Find("Base/Stats/Damage/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<ECCCar>().damage / damageMax;

                 
                    if (transform.Find("Base/Stats/Speed/Text")) transform.Find("Base/Stats/Speed/Text").GetComponent<Text>().text = currentIcon.GetComponent<ECCCar>().speed.ToString();
                    if (transform.Find("Base/Stats/RotateSpeed/Text")) transform.Find("Base/Stats/RotateSpeed/Text").GetComponent<Text>().text = currentIcon.GetComponent<ECCCar>().rotateSpeed.ToString();
                    if (transform.Find("Base/Stats/Health/Text")) transform.Find("Base/Stats/Health/Text").GetComponent<Text>().text = currentIcon.GetComponent<ECCCar>().health.ToString();
                    if (transform.Find("Base/Stats/Damage/Text")) transform.Find("Base/Stats/Damage/Text").GetComponent<Text>().text = currentIcon.GetComponent<ECCCar>().damage.ToString();
                }
            }

          
            items[currentItem].lockState = PlayerPrefs.GetInt(currentIcon.name, items[currentItem].lockState);

          
            if ( items[currentItem].lockState == 1 )
            {
                if (priceText) priceText.text = "GO!";

               
                PlayerPrefs.SetInt(currentPlayerprefs, currentItem);
            }
            else 
            {
               
                MeshRenderer meshRenderer = currentIcon.transform.Find("Base/Chasis").GetComponent<MeshRenderer>();
                
               
                for (index = 0; index < meshRenderer.materials.Length; index++)
                {
                    meshRenderer.materials[index].SetColor("_Color", Color.black);
                    meshRenderer.materials[index].SetColor("_EmissionColor", Color.black);
                }

               
                if (priceText) priceText.text = items[currentItem].price.ToString() + moneySign.ToString();
            }
        }
        
               public void SelectItem()
        {
          
            if (items[currentItem].lockState == 1)
            {
        
                gameController.playerObject = items[currentItem].itemIcon.GetComponent<ECCCar>();

            
                gameController.StartGame();

            
                Destroy(currentIcon.gameObject);

               
                if (shopCamera) shopCamera.SetActive(false);

           
                gameObject.SetActive(false);
            }
            else if ( moneyLeft >= items[currentItem].price )
            {
             
                items[currentItem].lockState = 1;

             
                PlayerPrefs.SetInt(currentIcon.name, items[currentItem].lockState);

              
                moneyLeft -= items[currentItem].price;
                
      
                PlayerPrefs.SetInt(moneyPlayerprefs, moneyLeft);

              
                moneyText.GetComponent<Text>().text = moneyLeft.ToString() + moneySign;

                if (unlockEffect) Instantiate(unlockEffect, currentIcon.position, currentIcon.rotation);

               
                ChangeItem(0);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawCube(itemIconPosition, new Vector3(1,0.1f,1));
        }
    }
}