using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    public ParticleClass[] particleClasses;

    public enum ParticleType { Particle1 };

    private void Awake()
    {
        Instance = this;
    }

    private void OnValidate()
    {
        if (particleClasses == null)
            return;
        if (particleClasses.Length <= 0)
            return;

        for (int i = 0; i < particleClasses.Length; i++)
        {
            particleClasses[i].name = particleClasses[i].particleType.ToString();
        }
    }

    private void Start()
    {
        GameObject parent = new GameObject("PARTICLES");

        for (int i = 0; i < particleClasses.Length; i++)
        {
            particleClasses[i].Init(parent.transform);
        }
    }

    public void PlayParticle(ParticleType particleType, Vector3 worldPosition, Transform parent=null)
    {
        ParticleSystem particle = GetParticleClass(particleType).GetParticle();
        if (particle == null)
        {
            Debug.LogWarning(particleType + " is not attached to ParticleManager.");
            return;
        }
        particle.transform.position = worldPosition;
        particle.Play();
        if (parent != null)
            particle.transform.parent = parent;
    }

    public void PlayParticle(ParticleType particleType, Vector3 worldPosition, Quaternion worldRotation, Transform parent=null)
    {
        ParticleSystem particle = GetParticleClass(particleType).GetParticle();
        if (particle == null)
        {
            Debug.LogWarning(particleType + " is not attached to ParticleManager.");
            return;
        }
        particle.transform.position = worldPosition;
        particle.transform.rotation = worldRotation;
        particle.Play();
        if (parent != null)
            particle.transform.parent = parent;
    }

    private ParticleClass GetParticleClass(ParticleType particleType)
    {
        for (int i = 0; i < particleClasses.Length; i++)
        {
            if (particleClasses[i].particleType == particleType)
            {
                return particleClasses[i];
            }
        }
        return null;
    }

    [System.Serializable]
    public class ParticleClass
    {
        [HideInInspector]
        public string name;
        public ParticleType particleType;
        public GameObject[] particlePrefabs;
        [Min(1)] public int poolCount = 1;
        [HideInInspector]
        public List<ParticleSystem> particles;
        int i = 0;

        public void Init(Transform parent)
        {
            for(int i = 0; i < poolCount; i++)
            {
                for(int j = 0; j < particlePrefabs.Length; j++)
                {
                    particles.Add(Instantiate(particlePrefabs[j], parent).GetComponent<ParticleSystem>());
                }
            }
        }

        public ParticleSystem GetParticle()
        {
            if (particles.Count <= 0)
                return null;

            ParticleSystem p = particles[i];
            i++;
            if (i >= particles.Count)
                i = 0;
            return p;
        }
    }
}
