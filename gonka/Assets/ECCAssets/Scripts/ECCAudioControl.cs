using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EndlessCarChase
{
   
    /// </summary>
    public class ECCAudioControl : MonoBehaviour
    {
    
        static float currentSoundVolume = 1;

       
        static float currentMusicVolume = 1;

              void Awake()
        {
            // Отделите громкость музыки от громкости других звуковых эффектов
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().ignoreListenerVolume = true;

            // Получить значение громкости музыки, записанной в PlayerPrefs
            currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", currentMusicVolume);

            // Установите громкость музыки на основе записи, которую мы получили от PlayerPrefs
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;

            // Получите текущую громкость звука из записи PlayerPrefs
            currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume", currentSoundVolume);

            // Установите громкость аудиослушателя в зависимости от громкости звука. Это управляет всеми звуками в игре, кроме музыки
            AudioListener.volume = currentSoundVolume;

            // Проверьте, является ли эта кнопка кнопкой музыки или звука
            if (transform.Find("Text"))
            {
                // Если это музыкальная кнопка, обновите ее в зависимости от громкости музыки
                if (transform.Find("Text").GetComponent<Text>().text.Contains("MUSIC"))
                {
                    if (currentMusicVolume > 0) transform.Find("Text").GetComponent<Text>().text = "MUSIC:ON";
                    else if (currentMusicVolume <= 0) transform.Find("Text").GetComponent<Text>().text = "MUSIC:OFF";
                } // Otherwise, update it based on the sound volume
                else if (transform.Find("Text").GetComponent<Text>().text.Contains("SOUND"))
                {
                    if (currentSoundVolume > 0) transform.Find("Text").GetComponent<Text>().text = "SOUND:ON";
                    else if (currentSoundVolume <= 0) transform.Find("Text").GetComponent<Text>().text = "SOUND:OFF";
                }
            }
        }

        public void ToggleMusic()
        {
            
            if (currentMusicVolume == 1)
            {
              
                currentMusicVolume = 0;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 0.5f;
                GetComponent<Image>().color = newColor;

               
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:OFF";
            }
            else // Если громкость равна 0, установите ее на полную и обновите текст
            {
                // Установите громкость музыки на полную
                currentMusicVolume = 1;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 1;
                GetComponent<Image>().color = newColor;

                // Установите соответствующий текст
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "MUSIC:ON";
            }

            // Установите громкость для музыкального объекта
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = currentMusicVolume;

            // Сохраните текущую громкость музыки в записи PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        }

       
        public void ToggleSound()
        {
            // Если том заполнен, установить его на 0 и обновите текст
            if (currentSoundVolume == 1)
            {
                // Приглушите громкость звука
                currentSoundVolume = 0;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 0.5f;
                GetComponent<Image>().color = newColor;

                // Установите соответствующий текст
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:OFF";
            }
            else // Если громкость равна 0, установите ее на полную и обновите текст
            {
                // Установите громкость звука на полную
                currentSoundVolume = 1;

                Color newColor = GetComponent<Image>().material.color;
                newColor.a = 1;
                GetComponent<Image>().color = newColor;

                // Установите соответствующий текст
                if (transform.Find("Text")) transform.Find("Text").GetComponent<Text>().text = "SOUND:ON";
            }

            // Установите громкость для объекта audiolistener, который управляет всеми звуками, кроме музыки
            AudioListener.volume = currentSoundVolume;

            // Сохраните текущую громкость звука в записи PlayerPrefs
            PlayerPrefs.SetFloat("SoundVolume", currentSoundVolume);
        }
    }
}