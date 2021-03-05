using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EndlessCarChase
{
	/// <summary>
	/// Этот скрипт анимирует спрайт, текстовую сетку или изображение пользовательского интерфейса несколькими цветами с течением времени.
	/// </summary>
	public class ECCAnimateColors : MonoBehaviour 
	{
		[Tooltip("Список цветов, которые будут анимированы")]
		public Color[] colorList;
		
		[Tooltip("Индексный номер текущего цвета в списке")]
		public int colorIndex = 0;
		
		[Tooltip("Как долго длится анимация изменения цвета, и счетчик для ее отслеживания")]
		public float changeTime = 1;
		public float changeTimeCount = 0;
		
		[Tooltip("Как быстро спрайт переходит от одного цвета к другому")]
		public float changeSpeed = 1;
		
		[Tooltip("Анимация приостановлена?")]
		public bool isPaused = false;
		
		[Tooltip("Анимация возобновлена?")]
		public bool isLooping = true;

		[Tooltip("Должны ли мы начать со случайного цветового индекса")]
		public bool randomColor = false;

		
		void Start() 
		{
			//Применяем выбранный цвет к спрайту или текстовой сетке
			SetColor();
		}
		
		
		void Update() 
		{
			//
			if ( isPaused == false )
			{
				if ( changeTime > 0 )
				{
					//Обратный отсчет до следующего изменения цвета
					if ( changeTimeCount > 0 )
					{
						changeTimeCount -= Time.deltaTime;
					}
					else
					{
						changeTimeCount = changeTime;
						
						//свич на следующий цвет
						if ( colorIndex < colorList.Length - 1 )
						{
							colorIndex++;
						}
						else
						{
							if ( isLooping == true )    colorIndex = 0;
						}
					}
				}

                //Если к этому объекту прикреплен компонент mesh renderer, установите его цвет
                if (GetComponent<Renderer>())
                {
                    GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, colorList[colorIndex], changeSpeed * Time.deltaTime);
                }

                //Если у нас есть text mesh, анимируйте ее цвет
                if ( GetComponent<TextMesh>() )
				{
					GetComponent<TextMesh>().color = Color.Lerp(GetComponent<TextMesh>().color, colorList[colorIndex], changeSpeed * Time.deltaTime);
				}
				
				//Если у нас есть SpriteRenderer, анимируйте его цвет
				if ( GetComponent<SpriteRenderer>() )
				{
					GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, colorList[colorIndex], changeSpeed * Time.deltaTime);
				}

				//Если у нас есть Image UI, анимируйте его цвет
				if ( GetComponent<Image>() )
				{
					GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, colorList[colorIndex], changeSpeed * Time.deltaTime);;
				}
			}
			else
			{
				//Примените выбранный цвет к спрайту или text mesh
				SetColor();
			}
		}

	
		void SetColor()
		{
			//random color
			int tempColor = 0;

			//назначаем
			if ( randomColor == true )    tempColor = Mathf.FloorToInt(Random.value * colorList.Length);

            //If you have a mesh renderer component attached to this object, set its color
            if (GetComponent<Renderer>())
            {
                GetComponent<Renderer>().material.color = colorList[tempColor];
            }

            //Если к этому объекту прикреплен компонент text mesh, установите его цвет
            if ( GetComponent<TextMesh>() )
			{
				GetComponent<TextMesh>().color = colorList[tempColor];
			}
			
			//Если к этому объекту прикреплен компонент sprite renderer, установите его цвет
			if ( GetComponent<SpriteRenderer>() )
			{
				GetComponent<SpriteRenderer>().color = colorList[tempColor];
			}

			// Если к этому объекту прикреплено Image UI, установите его цвет
			if ( GetComponent<Image>() )
			{
				GetComponent<Image>().color = colorList[tempColor];
			}
		}

		
				void Pause( bool pauseState )
		{
			isPaused = pauseState;
		}

	}
}
