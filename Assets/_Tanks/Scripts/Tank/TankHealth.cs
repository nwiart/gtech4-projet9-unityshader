﻿using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Tanks.Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;    // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;      // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        public GameObject m_TankBody;
        public Material m_DeathMat;
        [HideInInspector] public bool m_HasShield;          // Has the tank picked up a shield power up?
        
        
        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
        private float m_CurrentHealth;                      // How much health the tank currently has.
        private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?
        private float m_ShieldValue;                        // Percentage of reduced damage when the tank has a shield.
        private bool m_IsInvincible;                        // Is the tank invincible in this moment?
        private int m_RunningCoroutines = 0;

        public float Health { get => m_CurrentHealth; }

        private void Awake ()
        {
            // Set the slider max value to the max health the tank can have
            m_Slider.maxValue = m_StartingHealth;
        }

        private void OnDestroy()
        {
        }

        private void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead.
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
            m_HasShield = false;
            m_ShieldValue = 0;
            m_IsInvincible = false;



            Renderer[] renderers = m_TankBody.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];

                for (int j = 0; j < renderer.materials.Length; ++j)
                {
                    if (renderer.materials[j].name.Contains("TankColor"))
                    {
                        renderer.materials[j].SetFloat("_isLowLife", 0);
                    }
                }            
            }

            // Update the health slider's value and color.
            SetHealthUI();
        }

        public void TakeDamage (float amount)
        {
            // Check if the tank is not invincible
            if (!m_IsInvincible)
            {
                // Reduce current health by the amount of damage done.
                m_CurrentHealth -= amount * (1 - m_ShieldValue);

                // Change the UI elements appropriately.
                SetHealthUI ();


                // If the current health is at or below zero and it has not yet been registered, call OnDeath.
                if (m_CurrentHealth <= 0f && !m_Dead)
                {
                    OnDeath ();
                }

                if (m_CurrentHealth <= m_StartingHealth * .3f && !m_Dead)
                {
                    LowLife();
                }
            }
        }

        void LowLife()
        {
            Renderer[] renderers = m_TankBody.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];

                for (int j = 0; j < renderer.materials.Length; ++j)
                {
                    if (renderer.materials[j].name.Contains("TankColor"))
                    {
                        renderer.materials[j].SetFloat("_isLowLife", 1);
                    }
                }
            }
        }

        public void IncreaseHealth(float amount)
        {
            // Check if adding the amount would keep the health within the maximum limit
            if (m_CurrentHealth + amount <= m_StartingHealth)
            {
                // If the new health value is within the limit, add the amount
                m_CurrentHealth += amount;
            }
            else
            {
                // If the new health exceeds the starting health, set it at the maximum
                m_CurrentHealth = m_StartingHealth;
            }

            // Change the UI elements appropriately.
            SetHealthUI();
        }


        public void ToggleShield (float shieldAmount)
        {
            // Inverts the value of has shield.
            m_HasShield = !m_HasShield;

            // Stablish the amount of damage that will be reduced by the shield
            if (m_HasShield)
            {
                m_ShieldValue = shieldAmount;
            }
            else
            {
                m_ShieldValue = 0;
            }
        }

        public void ToggleInvincibility()
        {
            m_IsInvincible = !m_IsInvincible;
        }


        private void SetHealthUI ()
        {
            // Set the slider's value appropriately.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        private void OnDeath ()
        {
            SpawnParticles();
            // Set the flag so that this function is only called once.
            m_Dead = true;

            Renderer[] renderers = m_TankBody.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                StartCoroutine(Dissolve(renderer));
            }
        }

        private void SpawnParticles()
        {
            Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);
        }

        IEnumerator Dissolve(Renderer renderer)
        {
            m_RunningCoroutines += 1;

            Material[] tempMats = renderer.materials;

            Material[] copy = renderer.materials;
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = m_DeathMat;
                copy[i].SetFloat("_IsDead", 1);
            }
            renderer.materials = copy;

            //renderer.material = m_DeathMat;
            //renderer.material.SetFloat("_IsDead", 1);

            float elapsedTime = 0;
            float dissolveDuration = 2f;

            while (elapsedTime < dissolveDuration)
            {
                elapsedTime += Time.deltaTime;

                float alphaThreshold = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].SetFloat("_AlphaThreshold", alphaThreshold);
                }

                yield return null;
            }
            renderer.materials = tempMats;
            m_RunningCoroutines -= 1;
            CheckRunningCoroutine();
        }

        private void CheckRunningCoroutine()
        {
            if (m_RunningCoroutines == 0) gameObject.SetActive(false);
        }
    }
}