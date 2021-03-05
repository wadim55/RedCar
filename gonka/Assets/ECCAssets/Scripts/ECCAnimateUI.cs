using UnityEngine;
using System.Collections;

namespace EndlessCarChase
{
	/// <summary>
	/// Этот скрипт анимирует пользовательский интерфейс во время паузы игры
	/// </summary>
	public class ECCAnimateUI : MonoBehaviour
	{
		// Текущее реальное время, не связанное с глобальным временем.
		public float currentTime;
		
		// Предыдущее зарегистрированное время, это используется для расчета дельта-времени
		public float previousTime;
		
		// Дельта-время ( изменение во времени), рассчитанное для того, чтобы позволить анимацию с течением времени
		public float deltaTime;
		
		[Tooltip("The intro animation for this UI element")]
		public AnimationClip introAnimation;

		// Анимационный компонент, содержащий анимационные клипы
		public Animation animationObject;
		
		// Текущее время анимации.  сбрасывается при запуске новой анимации
		public float animationTime = 0;
		
		// анимация запущена?
		public bool isAnimating = false;
		
		[Tooltip("Должна ли анимация воспроизводиться сразу же после включения элемента пользовательского интерфейса?")]
		public bool playOnEnabled = true;
		
		
		void Awake()
		{
			// Регистрация текущего времени
			previousTime = currentTime = Time.realtimeSinceStartup;
			
			// регистрируем компонент для более быстрого доступа
			animationObject = GetComponent<Animation>();
		}
		
	
		void Update()
		{
			// анимация
			if ( introAnimation && isAnimating == true )
			{

				// Получаем текущее реальное время, независимо от масштаба времени
				currentTime = Time.realtimeSinceStartup;
				
				
				deltaTime = currentTime - previousTime;
				
				// Установливаем текущее время таким же, как и предыдущее, для проверки следующего обновления ().
				previousTime = currentTime;
				
				// Вычислияем текущее время в текущей анимации
				animationObject[introAnimation.name].time = animationTime;
				
				// Пример анимации с заданного нами времени ( отображение правильного кадра на основе времени анимации )
				animationObject.Sample();
				
				// ++ к времени анимации
				animationTime += deltaTime;
				
				// Если анимация достигает длины клипа, завершите анимацию
				if ( animationTime >= animationObject.clip.length )
				{
					// Установить время анимации на длину клипа ( прверить, что мы добрались до конца анимации )
					animationObject[introAnimation.name].time = animationObject.clip.length;
					
					// Пример анимации с заданного нами времени ( отображение правильного кадра на основе времени анимации )
					animationObject.Sample();
					
					// откл анимацию
					isAnimating = false;
				}
			}
		}

		/// <summary>
		/// Запускается, когда объект включен. ( Если он был отключен раньше )
		/// </summary>
		void OnEnable()
		{
			// Если объект был включен. воспроизведение анимации
			if ( playOnEnabled == true )
			{
				PlayAnimation();
			}
		}

		/// <summary>
		/// Воспроизведение анимации независимо от масштаба времени
		/// </summary>
		public void PlayAnimation()
		{
			if ( introAnimation ) 
			{
				// сброс таймера анимации
				animationTime = 0;
			
				//
				previousTime = currentTime = Time.realtimeSinceStartup;
			
			
				isAnimating = true;

				animationObject.Play();
			}
		}
	}
}

