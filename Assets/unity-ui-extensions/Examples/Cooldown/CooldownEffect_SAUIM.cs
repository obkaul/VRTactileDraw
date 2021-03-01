﻿/// Credit SimonDarksideJ
/// Sourced from my head

using Scripts.Controls;
using Scripts.Effects;
using UnityEngine;

namespace Examples.Cooldown
{
    [RequireComponent(typeof(SoftMaskScript))]
    public class CooldownEffect_SAUIM : MonoBehaviour {

        public CooldownButton cooldown;
        private SoftMaskScript sauim;

        // Use this for initialization
        private void Start() {
            if (cooldown == null)
            {
                Debug.LogError("Missing Cooldown Button assignment");
            }
            sauim = GetComponent<SoftMaskScript>();
        }

        // Update is called once per frame
        private void Update() {
            sauim.CutOff = Mathf.Lerp(0,1, cooldown.CooldownTimeElapsed / cooldown.CooldownTimeout);
        }
    }
}