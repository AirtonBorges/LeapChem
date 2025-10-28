using UnityEngine;
using UnityEngine.Serialization;

public class ParticleColor : MonoBehaviour
{
    [FormerlySerializedAs("atom")] public Particle particle;
    public Material protonMaterial;
    public Material neutronMaterial;
    private Renderer _particleRenderer;

    private void Start()
    {
        if (particle == null)
        {
            Debug.LogError("Atom reference is not set.");
            return;
        }

        _particleRenderer = GetComponent<Renderer>();
        if (_particleRenderer == null)
        {
            Debug.LogError("Renderer component not found on the particle.");
        }
    }

    private void Update()
    {
        _particleRenderer.material = particle.KindOfParticle switch
        {
            EKindOfParticle.Proton => protonMaterial,
            EKindOfParticle.Neutron => neutronMaterial,
            _ => _particleRenderer.material
        };
    }
}
