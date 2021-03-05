using UnityEngine;
using UnityEngine.UI;

namespace EndlessCarChase
{
     public class ECCPlaySound : MonoBehaviour
    {
        [Tooltip("Звук для воспроизведения")]
        public AudioClip sound;

        [Tooltip("Метка источника звука")]
        public string soundSourceTag = "Sound";

        [Tooltip("Воспроизведение звука сразу же после активации объекта")]
        public bool playOnStart = true;

        [Tooltip("Воспроизведение звука при нажатии на эту кнопку")]
        public bool playOnClick = false;

        [Tooltip("Случайный диапазон для высоты звука источника звука, чтобы сделать звук более разнообразным")]
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

       
        void Start()
        {
            if (playOnStart == true) PlayCurrentSound();

          
            if (playOnClick && GetComponent<Button>()) GetComponent<Button>().onClick.AddListener(delegate { PlayCurrentSound(); });

        }

                public void PlaySound(AudioClip sound)
        {
        
            if (soundSourceTag != string.Empty && sound)
            {
              
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

              
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
              
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                GetComponent<AudioSource>().PlayOneShot(sound);
            }

        }


              public void PlayCurrentSound()
        {
           
            if (soundSourceTag != string.Empty && sound)
            {
              
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

               
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
               
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                GetComponent<AudioSource>().PlayOneShot(sound);
            }
        }
    }
}