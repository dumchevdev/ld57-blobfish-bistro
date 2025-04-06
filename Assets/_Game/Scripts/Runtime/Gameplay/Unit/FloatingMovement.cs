using UnityEngine;

namespace Game.Runtime.Gameplay.Character
{
    public class FloatingMovement : MonoBehaviour
    {
        public float verticalAmplitude = 1f;    // Высота волны
        public float horizontalAmplitude = 0.5f; // Ширина волны (боковое движение)
        public float frequency = 1f;            // Частота колебаний
        public float speed = 1f;                // Общая скорость движения

        public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Кривая для плавности
        private float timeOffset;

        void Start()
        {
            timeOffset = Random.Range(0f, 2f * Mathf.PI); // Случайное смещение для уникальности движения
        }

        void Update()
        {
            // Вычисляем время с учетом скорости
            float time = Time.time * speed + timeOffset;

            // Вычисляем вертикальное и горизонтальное смещение
            float verticalSin = Mathf.Sin(time * frequency) * verticalAmplitude;
            float horizontalSin = Mathf.Cos(time * frequency * 0.5f) * horizontalAmplitude;

            // Применяем кривую плавности
            float easedValue = easeCurve.Evaluate(Mathf.PingPong(time, 1f));

            // Создаем новую позицию
            Vector3 newPosition = transform.position + 
                                  new Vector3(horizontalSin * easedValue, 
                                      verticalSin * easedValue, 
                                      0);

            // Плавно перемещаем объект
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
        }
    }
}