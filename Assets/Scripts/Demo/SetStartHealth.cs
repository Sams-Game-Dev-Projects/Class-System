using UnityEngine;

namespace ClassSystem.Demo
{
    // Ensures a character starts with a specific current health after CharacterStats initializes.
    // Execution order is high so it runs after CharacterStats.Start() recalculates vitals.
    [DefaultExecutionOrder(1000)]
    public class SetStartHealth : MonoBehaviour
    {
        public float targetHealth = 20f;

        Runtime.CharacterStats _stats;

        void Awake()
        {
            _stats = GetComponent<Runtime.CharacterStats>();
        }

        void Start()
        {
            if (_stats == null) return;
            // Clamp to max health so we don't exceed
            float max = _stats.GetMaxHealth();
            _stats.currentHealth = Mathf.Min(targetHealth, max);
        }
    }
}

