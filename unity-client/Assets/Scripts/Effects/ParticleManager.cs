using System.Collections.Generic;
using UnityEngine;

namespace CardgameDungeon.Unity.Effects
{
    public class ParticleManager : MonoBehaviour
    {
        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem damageParticles;
        [SerializeField] private ParticleSystem healParticles;
        [SerializeField] private ParticleSystem exileParticles;
        [SerializeField] private ParticleSystem eliminationParticles;
        [SerializeField] private ParticleSystem levelUpParticles;

        [Header("Pool Settings")]
        [SerializeField] private int initialPoolSize = 5;

        private Dictionary<ParticleSystem, Queue<ParticleSystem>> pools =
            new Dictionary<ParticleSystem, Queue<ParticleSystem>>();

        private void Awake()
        {
            InitializePool(damageParticles);
            InitializePool(healParticles);
            InitializePool(exileParticles);
            InitializePool(eliminationParticles);
            InitializePool(levelUpParticles);
        }

        private void InitializePool(ParticleSystem prefab)
        {
            if (prefab == null) return;

            Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                ParticleSystem instance = CreateInstance(prefab);
                pool.Enqueue(instance);
            }

            pools[prefab] = pool;
        }

        private ParticleSystem CreateInstance(ParticleSystem prefab)
        {
            ParticleSystem instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private ParticleSystem GetFromPool(ParticleSystem prefab)
        {
            if (prefab == null) return null;

            if (!pools.ContainsKey(prefab))
            {
                InitializePool(prefab);
            }

            Queue<ParticleSystem> pool = pools[prefab];

            // Try to find an inactive instance in the pool
            int count = pool.Count;
            for (int i = 0; i < count; i++)
            {
                ParticleSystem instance = pool.Dequeue();

                if (instance == null)
                {
                    // Instance was destroyed, create replacement
                    instance = CreateInstance(prefab);
                }

                if (!instance.isPlaying)
                {
                    pool.Enqueue(instance);
                    return instance;
                }

                pool.Enqueue(instance);
            }

            // All instances are in use, create a new one
            ParticleSystem newInstance = CreateInstance(prefab);
            pool.Enqueue(newInstance);
            return newInstance;
        }

        private void PlayParticle(ParticleSystem prefab, Vector3 position)
        {
            ParticleSystem instance = GetFromPool(prefab);
            if (instance == null) return;

            instance.transform.position = position;
            instance.gameObject.SetActive(true);
            instance.Clear();
            instance.Play();
        }

        public void PlayDamage(Vector3 position, int amount)
        {
            ParticleSystem instance = GetFromPool(damageParticles);
            if (instance == null) return;

            instance.transform.position = position;

            // Scale emission rate based on damage amount
            var emission = instance.emission;
            emission.rateOverTimeMultiplier = Mathf.Clamp(amount * 5f, 5f, 100f);

            // Scale particle size with damage
            var main = instance.main;
            main.startSizeMultiplier = Mathf.Clamp(0.1f + amount * 0.05f, 0.1f, 1f);

            instance.gameObject.SetActive(true);
            instance.Clear();
            instance.Play();

            Debug.Log($"[ParticleManager] Damage particles at {position}, amount: {amount}");
        }

        public void PlayHeal(Vector3 position, int amount)
        {
            ParticleSystem instance = GetFromPool(healParticles);
            if (instance == null) return;

            instance.transform.position = position;

            // Scale emission based on heal amount
            var emission = instance.emission;
            emission.rateOverTimeMultiplier = Mathf.Clamp(amount * 3f, 3f, 60f);

            instance.gameObject.SetActive(true);
            instance.Clear();
            instance.Play();

            Debug.Log($"[ParticleManager] Heal particles at {position}, amount: {amount}");
        }

        public void PlayExile(Vector3 position)
        {
            PlayParticle(exileParticles, position);
            Debug.Log($"[ParticleManager] Exile particles at {position}");
        }

        public void PlayElimination(Vector3 position)
        {
            PlayParticle(eliminationParticles, position);
            Debug.Log($"[ParticleManager] Elimination particles at {position}");
        }

        public void PlayLevelUp(Vector3 position)
        {
            PlayParticle(levelUpParticles, position);
            Debug.Log($"[ParticleManager] Level up particles at {position}");
        }
    }
}
