using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit
{
    public class FloatingMovement : MonoBehaviour
    {
        public float verticalAmplitude = 1f;    // Высота волны
        public float horizontalAmplitude = 0.5f; // Ширина волны (боковое движение)
        public float frequency = 1f;            // Частота колебаний
        public float speed = 1f;                // Общая скорость движения

        public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Кривая для плавности
        private float timeOffset;
        
        private Vector2 velocity; 
        public float smoothTime = 0.3F; 
        public float maxVelocity = 10f; 
        private Vector2 currentVelocity;

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
            Vector2 newPosition = transform.position + 
                                  new Vector3(horizontalSin * easedValue, 
                                      verticalSin * easedValue);
            
            Vector2 smoothDamp = Vector2.SmoothDamp(transform.position, newPosition, ref currentVelocity, smoothTime, maxVelocity, Time.deltaTime);
            velocity = (newPosition - (Vector2)transform.position) / Time.deltaTime;

            if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
            {
                velocity = velocity.normalized * maxVelocity;
            }

            transform.position = smoothDamp + velocity * Time.deltaTime;

        }
    }
}