using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessCarChase.Types;

namespace EndlessCarChase
{
    /// <summary>
  // Этот класс управляет игрой, начиная ее, следя за ходом игры и заканчивая окончанием игры или победой.
    /// </summary>
    public class ECCGameController : MonoBehaviour
    {
        public GameObject reklama;
        int scoreRek;
        [Tooltip("The camera object and the camera holder that contains it and follows the player")]
        public Transform cameraHolder;

        //Неиспользуемая переменная, содержащая миникарту камеры
        internal Transform miniMap;

        // Неиспользуемый код, который заставляет камеру преследовать игрока и вращаться за ним
        internal float cameraRotate = 0;

        [Tooltip("Объект игрока, назначенный из папки проекта или из магазина")]
        public ECCCar playerObject;
        internal float playerDirection;

        [Tooltip("Наземный объект, который следует за позицией игрока и дает ощущение движения.")]
        public Transform groundObject;

        [Tooltip("Слой, по которому машины движутся вверх и вниз по рельефу местности. groundObject не должен быть назначен или скрыт для того, чтобы сделать эту работу")]
        public LayerMask groundLayer;

        [Tooltip("Скорость, с которой движется текстура наземного объекта, заставляет игрока казаться, будто он движется по земле")]
        public float groundTextureSpeed = -0.4f;

        [Tooltip("Как долго ждать, прежде чем начать игру. - Готовы? ИДИ! время")]
        public float startDelay = 1;
        internal bool gameStarted = false;

        [Tooltip("Эффект, отображаемый перед началом игры")]
        public Transform readyGoEffect;

        [Tooltip("Счет игрока")]
        public int score = 0;

        [Tooltip("Сколько очков мы получаем в секунду")]
        public int scorePerSecond = 1;

        [Tooltip("Текстовый объект score, который отображает текущий счет игрока")]
        public Transform scoreText;

        [Tooltip("Текст, который добавляется к значению балла. Мы используем это, чтобы добавить денежный знак к счету")]
        public string scoreTextSuffix = "Руб";

        [Tooltip("записываем общий балл, который у нас есть ( не текущий балл, а общий балл, который мы собрали во всех играх, который используется в качестве денег")]
        public string moneyPlayerPrefs = "Бабки";

        internal int highScore = 0;
        internal int scoreMultiplier = 1;

        [Tooltip("Меню холста, которое отображает магазин, где мы можем разблокировать и выбрать автомобили")]
        public ECCShop shopMenu;

        [Tooltip("Холст рулевого колеса, который позволяет управлять игроком, перетаскивая колесо влево/вправо")]
        public Slider steeringWheel;

        // Различные холсты для пользовательского интерфейса
        public Transform gameCanvas;
        public Transform healthCanvas;
        public Transform pauseCanvas;
        public Transform gameOverCanvas;

        // флаг конец игры
        internal bool isGameOver = false;

        // Уровень главного меню, который может быть загружен после окончания игры
        public string mainMenuLevelName = "Меню Старт";

        // Различные звуки и их источник
        public AudioClip soundGameOver;
        public string soundSourceTag = "GameController";
        public GameObject soundSource;

        // Кнопка, которая перезапустит игру после окончания игры
        public string confirmButton = "перезагрузка";

        // Кнопка, которая приостанавливает игру. Нажатие на кнопку паузы в пользовательском интерфейсе также приостанавливает игру
        public string pauseButton = "пауза";
        internal bool isPaused = false;

        // Индекс общего пользования
        internal int index = 0;

       

        // Неиспользуемые переменные, ограничивающие игровую область и заставляющие ее вращаться от края до края
        internal Vector2 gameArea = new Vector2(10, 10);
        internal bool wrapAroundGameArea = false;

        void Awake()
        {
            Time.timeScale = 1;

           
            if (shopMenu)
            {
                //Получить номер текущего элемента
                shopMenu.currentItem = PlayerPrefs.GetInt(shopMenu.currentPlayerprefs, shopMenu.currentItem);

                // Обновляем объект игрока на основе выбранного нами автомобиля магазина
                playerObject = shopMenu.items[shopMenu.currentItem].itemIcon.GetComponent<ECCCar>();
            }

        }

      
        void Start()
        {
           
            // сбрасывыем счет
            ChangeScore(0);

            //прячем канвасы
            if (shopMenu) shopMenu.gameObject.SetActive(false);
            if (gameOverCanvas) gameOverCanvas.gameObject.SetActive(false);
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(false);

            //Получаем рекорд для игрока
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);

            //Назначаем источник звука для более легкого доступа
            if (GameObject.FindGameObjectWithTag(soundSourceTag)) soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

            if (steeringWheel) steeringWheel.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            scoreRek = 0;
            // Спавним автомобиль игрока, если он существует
            if (playerObject)
            {
                // Создаем объект игрока в сцене
                playerObject = Instantiate(playerObject);

                // Установим тег игрока игрока так, чтобы мы могли ссылаться на него

                playerObject.tag = "Player";
            }

            // порождаем объекты в сцене
            if (GetComponent<ECCSpawnAroundObject>()) GetComponent<ECCSpawnAroundObject>().isSpawning = true;

            // Показать игровой UI
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);

            // создать эффект Ready? GO
            if (readyGoEffect) Instantiate(readyGoEffect);

            // флаг игра началась
            gameStarted = true;

            // врубаем счетчик игрока каждую секунду
            if (scorePerSecond > 0) InvokeRepeating("ScorePerSecond", startDelay, 1);

            if (steeringWheel && Application.isMobilePlatform) steeringWheel.gameObject.SetActive(true);

        }

      
        void Update()
        {
            // Если игра еще не началась, ничего не происходит
            if (gameStarted == false) return;

            // Отложите начало игры
            if (startDelay > 0)
            {
                startDelay -= Time.deltaTime;
            }
            else
            {
                //Если игра закончена, активировать  кнопки Перезапуска и MainMenu
                if (isGameOver == true)
                {
                    // кнопка рестарт
                    if (Input.GetButtonDown(confirmButton))
                    {
                        Restart();
                    }

                    //кнопка ставит игру на паузу и переводит на главное меню
                    if (Input.GetButtonDown(pauseButton))
                    {
                        MainMenu();
                    }
                }
                else
                {
                    // Если есть объект игрока, переместите его вперед и поверните в правильном направлении
                    if (playerObject)
                    {
                        // Если мы используем мобильные элементы управления, поверните влево/вправо в зависимости от положения крана сбоку на экране
                        if (Application.isMobilePlatform)
                        {
                            // Если у нас есть ползунок рулевого колеса, используйте его
                            if (steeringWheel)
                            {
                                // Если мы нажмем кнопку мыши, проверьте наше положение относительно центра экрана
                                if (Input.GetMouseButton(0))
                                {
                                    playerDirection = steeringWheel.value;
                                }
                                else // В противном случае, если мы ничего не нажимали, то тачка не вращается и не выпрямляется
                                {
                                    steeringWheel.value = playerDirection = 0;
                                }

                                steeringWheel.transform.Find("Wheel").eulerAngles = Vector3.forward * playerDirection * -100;
                            }
                            else if (Input.GetMouseButton(0)) // Если мы нажмем кнопку мыши, проверьте наше положение относительно центра экрана
                            {
                                // Если мы находимся справа от экрана, повернитесь вправо
                                if (Input.mousePosition.x > Screen.width * 0.5f)
                                {
                                    playerDirection = 1;
                                }
                                else // иначе налево
                                {
                                    playerDirection = -1;
                                }
                            }
                            else // В противном случае, если мы ничего не нажимали, не вращайтесь и не выпрямляйтесь

                            {
                                playerDirection = 0;
                            }
                        }
                        else // В противном случае используйте элементы управления геймпадом/клавиатурой
                        {
                            playerDirection = Input.GetAxis("Horizontal");
                        }

                        // Вычисляем направление вращения
                        playerObject.Rotate(playerDirection);

                        // код, который заставляет игрока обернуться вокруг края игровой зоны
                        if (wrapAroundGameArea == true)
                        {
                            if (playerObject.transform.position.x > gameArea.x * 0.5f) playerObject.transform.position -= Vector3.right * gameArea.x;
                            if (playerObject.transform.position.x < gameArea.x * -0.5f) playerObject.transform.position += Vector3.right * gameArea.x;
                            if (playerObject.transform.position.z > gameArea.y * 0.5f) playerObject.transform.position -= Vector3.forward * gameArea.y;
                            if (playerObject.transform.position.z < gameArea.y * -0.5f) playerObject.transform.position += Vector3.forward * gameArea.y;
                        }
                    }

                    //Переключение паузы/отмены паузы в игре
                    if (Input.GetButtonDown(pauseButton))
                    {
                        if (isPaused == true) Unpause();
                        else Pause(true);
                    }
                }
            }
        }

        void LateUpdate()
        {
            if (playerObject)
            {
                if (cameraHolder)
                {
                    // Заставляем держатель камеры следить за положением плеера
                    cameraHolder.position = playerObject.transform.position;

                    // Заставляем держатель камеры вращаться в том направлении, в котором движется игрок
                    if (cameraRotate > 0) cameraHolder.eulerAngles = Vector3.up * Mathf.LerpAngle(cameraHolder.eulerAngles.y, playerObject.transform.eulerAngles.y, Time.deltaTime * cameraRotate);
                }

                // Если есть миникарта, заставляем ее следовать за игроком
                if (miniMap) miniMap.position = playerObject.transform.position;

                // Если есть наземный объект, заставьте его UV-карту двигаться в зависимости от позиции игрока ( что дает ощущение движения по краю )
                if (groundObject)
                {
                    // удерживаем наземный объект за позицией игрока
                    groundObject.position = playerObject.transform.position;

                    // Обновляем текстуру UV земли в зависимости от позиции игрока
                    groundObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(playerObject.transform.position.x, playerObject.transform.position.z) * groundTextureSpeed);
                }
            }
        }

       
       
        public void ChangeHealth(float changeValue)
        {
            if (playerObject) playerObject.ChangeHealth(changeValue);
        }

     
       
        public void ChangeScore(int changeValue)
        {
          

            // Изменяем счет
			score += changeValue;

            //обновляем счет
            if (scoreText)
            {
                scoreText.GetComponent<Text>().text = score.ToString();

                // анимация смены очков в счете
                if (scoreText.GetComponent<Animation>()) scoreText.GetComponent<Animation>().Play();
            }
        }
        
       
        void SetScoreMultiplier( int setValue )
		{
			// множитель очков
			scoreMultiplier = setValue;
		}

       
        public void ScorePerSecond()
        {
            ChangeScore(scorePerSecond);
        }

               public void Pause(bool showMenu)
        {
            isPaused = true;

            //системное время пррррррр....
            Time.timeScale = 0;

            //показать экран паузы и скрыть экран игры
            if (showMenu == true)
            {
                if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
            }
        }

              public void Unpause()
        {
            isPaused = false;

            //системное время игого
            Time.timeScale = 1;

            //Скрыть экран паузы и покажите экран игры
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);
        }

      
        IEnumerator GameOver(float delay)
		{
			isGameOver = true;

			yield return new WaitForSeconds(delay);
			
			//Снимаем экраны паузы и игры
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
            if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);

            //количество денег, которые у нас есть

            int totalMoney = PlayerPrefs.GetInt(moneyPlayerPrefs, 0);

            //Добавить к этому количество денег, которые мы собрали в этой игре
            totalMoney += score;

            //Записать количество денег, которые у нас есть
            PlayerPrefs.SetInt(moneyPlayerPrefs, totalMoney);

            
            if ( gameOverCanvas )    
			{
				//Показать конец игры на экране
				gameOverCanvas.gameObject.SetActive(true);
				
				//вывести очки на экран
				gameOverCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
				
				//проверить, может мы, Мать его, побили рекорд
				if ( score > highScore )    
				{
					highScore = score;
					
					//записать новый рекорд
					PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
				}
				
				//преобразовать в тект и вывести на экран
				gameOverCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

				//Если есть источник и звук, воспроизводите его из источника
				if ( soundSource && soundGameOver )    
				{
					soundSource.GetComponent<AudioSource>().pitch = 1;
					
					soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
				}
			}
		}
		
			void  Restart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		
				void  MainMenu()
		{
			SceneManager.LoadScene(mainMenuLevelName);
		}

      
    }
}