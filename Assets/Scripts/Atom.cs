using UnityEngine;

public class Atom : MonoBehaviour
{
    public Particle particle;

    public void OnCollisionEnter(Collision other)
    {
         particle.OnCollisionEnter(other);
    }

    void Update()
    {
        if (!particle)
        {
            return;
        }

        if (particle.ParentParticle)
        {
            Destroy(gameObject);
        }
    }
} 
