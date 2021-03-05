using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EndlessCarChase
{
    /// <summary>
    /// Includes functions for loading levels and URLs. It's intended for use with UI Buttons
    /// </summary>
    public class ECCLoadLevel : MonoBehaviour
    {
        [Tooltip("Сколько секунд ждать перед загрузкой уровня или URL")]
        public float loadDelay = 1;

        [Tooltip("Имя загружаемого URL - адреса")]
        public string urlName = "";

        [Tooltip("Имя загружаемого уровня")]
        public string levelName = "";

        [Tooltip("Звук, который воспроизводится при загрузке/перезапуске/и т. Д")]
        public AudioClip soundLoad;

        [Tooltip("Тег исходного объекта, из которого воспроизводятся звуки")]
        public string soundSourceTag = "Sound";

        [Tooltip("Исходный объект, из которого воспроизводятся звуки. Вы можете назначить это прямо со сцены")]
        public GameObject soundSource;

        [Tooltip("Анимация, которая воспроизводится, когда мы начинаем загружать уровень")]
        public string loadAnimation;

        [Tooltip("Эффект перехода, который появляется, когда мы начинаем загружать уровень")]
        public Transform transition;

        [Tooltip("Должна ли эта кнопка быть вызвана ngclicki?")]
        public bool loadOnClick = false;

               void Start()
        {
           

            if (loadOnClick == true) GetComponent<Button>().onClick.AddListener(LoadLevel);

        }


        
        public void LoadURL()
        {
            Time.timeScale = 1;

        
            if (soundLoad && soundSource) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

        
            Invoke("ExecuteLoadURL", loadDelay);
        }

                void ExecuteLoadURL()
        {
            Application.OpenURL(urlName);
        }

                public void LoadLevel()
        {
            Time.timeScale = 1;

  
            if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

            if (transition) Invoke("ShowTransition", loadDelay - 1);

         
            Invoke("ExecuteLoadLevel", loadDelay);
        }

        public void ShowTransition()
        {
            Instantiate(transition);
        }

       
        void ExecuteLoadLevel()
        {
            SceneManager.LoadScene(levelName);
        }

       
        public void RestartLevel()
        {
            Time.timeScale = 1;

           
            if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

            if (transition) Instantiate(transition);


          
            Invoke("ExecuteRestartLevel", loadDelay);
        }

       
        void ExecuteRestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}