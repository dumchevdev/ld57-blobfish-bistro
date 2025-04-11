using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit
{
    public class BreathingEffect : MonoBehaviour
    {
        [Header("Breathing Settings")] [Tooltip("Base scale of the character (normal size)")]
        public Vector3 baseScale = Vector3.one;

        [Tooltip("Scale multiplier when inhaling (full breath in)")]
        public float inhaleScale = 1.05f;

        [Tooltip("Scale multiplier when exhaling (full breath out)")]
        public float exhaleScale = 0.95f;

        [Tooltip("Duration of one full breath cycle in seconds")]
        public float breathDuration = 3f;

        [Tooltip("Curve to control breathing pattern")]
        public AnimationCurve breathingCurve =
            new AnimationCurve(new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0));

        [Header("Randomness")] [Tooltip("Random variation to breath duration")] [Range(0, 0.5f)]
        public float durationRandomness = 0.1f;

        [Tooltip("Random variation to scale intensity")] [Range(0, 0.2f)]
        public float scaleRandomness = 0.05f;

        private float timer;
        private float currentBreathDuration;
        private float currentInhaleScale;
        private float currentExhaleScale;

        void Start()
        {
            ResetBreathCycle();
        }

        void Update()
        {
            timer += Time.deltaTime;

            // Normalized progress through current breath cycle (0 to 1)
            float progress = Mathf.Clamp01(timer / currentBreathDuration);

            // Evaluate curve at current progress
            float curveValue = breathingCurve.Evaluate(progress);

            // Calculate current scale between exhale and inhale scales
            float currentScaleMultiplier = Mathf.Lerp(currentExhaleScale, currentInhaleScale, curveValue);

            // Apply scale
            transform.localScale = baseScale * currentScaleMultiplier;

            // Reset cycle when complete
            if (progress >= 1f)
            {
                ResetBreathCycle();
            }
        }

        private void ResetBreathCycle()
        {
            timer = 0f;

            // Apply randomness if enabled
            currentBreathDuration = breathDuration * (1 + Random.Range(-durationRandomness, durationRandomness));
            currentInhaleScale = inhaleScale * (1 + Random.Range(-scaleRandomness, scaleRandomness));
            currentExhaleScale = exhaleScale * (1 + Random.Range(-scaleRandomness, scaleRandomness));
        }

        // Call this to temporarily stop breathing (e.g. when character is holding breath)
        public void PauseBreathing(float pauseDuration)
        {
            CancelInvoke(nameof(ResumeBreathing));
            enabled = false;
            Invoke(nameof(ResumeBreathing), pauseDuration);
        }

        private void ResumeBreathing()
        {
            enabled = true;
            ResetBreathCycle();
        }
    }
}