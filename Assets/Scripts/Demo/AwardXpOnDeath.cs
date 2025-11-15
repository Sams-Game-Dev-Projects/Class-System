using UnityEngine;

namespace ClassSystem.Demo
{
    // Attach to an enemy; awards XP to recipient when this enemy dies.
    public class AwardXpOnDeath : MonoBehaviour
    {
        public Runtime.CharacterStats recipient;
        public int minXp = 75;
        public int maxXp = 100;

        Runtime.CharacterStats _self;
        bool _awarded;

        void Awake()
        {
            _self = GetComponent<Runtime.CharacterStats>();
        }

        void OnEnable()
        {
            if (_self != null)
                _self.OnDied += HandleDied;
        }

        void OnDisable()
        {
            if (_self != null)
                _self.OnDied -= HandleDied;
        }

        void HandleDied()
        {
            if (_awarded) return;
            _awarded = true;
            if (recipient == null) return;
            int lo = Mathf.Min(minXp, maxXp);
            int hi = Mathf.Max(minXp, maxXp);
            int amount = Random.Range(lo, hi + 1);
            recipient.AddExperience(amount);
            Debug.Log($"Awarded {amount} XP to {recipient.name} for defeating {name}", this);
        }
    }
}

