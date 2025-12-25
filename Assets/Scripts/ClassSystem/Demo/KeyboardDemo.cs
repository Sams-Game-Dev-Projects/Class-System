using UnityEngine;
using ClassSystem.Runtime;
using ClassSystem.Spells;
using ClassSystem.Combat;

namespace ClassSystem.Demo
{
    public class KeyboardDemo : MonoBehaviour
    {
        public KeyCode attackKey = KeyCode.Mouse0;
        public KeyCode castKey = KeyCode.Mouse1;
        public KeyCode prevSpellKey = KeyCode.Q;
        public KeyCode nextSpellKey = KeyCode.E;

        Attacker _attacker;
        SpellCaster _caster;
        CharacterStats _stats;
        int _selectedSpellIndex = 0;

        void Awake()
        {
            _attacker = GetComponent<Attacker>();
            _caster = GetComponent<SpellCaster>();
            _stats = GetComponent<CharacterStats>();
        }

        void Update()
        {
            if (Input.GetKeyDown(attackKey))
            {
                var tgt = FindTargetUnderMouse();
                if (tgt != null) _attacker.TryAttack(tgt);
            }
            if (Input.GetKeyDown(castKey))
            {
                var spell = GetCurrentSpell();
                if (spell != null)
                {
                    var dir = GetDirectionToMouse();
                    _caster.Cast(spell, dir);
                }
            }

            // Cycle selected spell
            if (Input.GetKeyDown(prevSpellKey)) CycleSpell(-1);
            if (Input.GetKeyDown(nextSpellKey)) CycleSpell(1);
        }

        Spell GetCurrentSpell()
        {
            if (_stats?.playerClass == null) return null;
            var list = _stats.playerClass.knownSpells;
            if (list == null || list.Count == 0) return null;
            _selectedSpellIndex = Mathf.Clamp(_selectedSpellIndex, 0, list.Count - 1);
            return list[_selectedSpellIndex];
        }

        void CycleSpell(int delta)
        {
            if (_stats?.playerClass == null) return;
            var list = _stats.playerClass.knownSpells;
            if (list == null || list.Count == 0) return;
            int count = list.Count;
            _selectedSpellIndex = ((_selectedSpellIndex + delta) % count + count) % count; // wrap around
            var cur = list[_selectedSpellIndex];
            if (cur != null)
            {
                Debug.Log($"Selected spell: {cur.displayName}", this);
            }
        }

        Vector2 GetDirectionToMouse()
        {
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m.z = 0;
            return (m - transform.position).normalized;
        }

        IDamageable FindTargetUnderMouse()
        {
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m.z = 0;
            var col = Physics2D.OverlapPoint(m);
            if (col == null) return null;
            return col.GetComponentInParent<IDamageable>();
        }
    }
}
