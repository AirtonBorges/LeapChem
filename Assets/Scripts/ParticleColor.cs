using UnityEngine;

public class ParticleColor : MonoBehaviour
{
    public Atom atom;
    public Material protonMaterial;
    public Material neutronMaterial;
    private Renderer _particleRenderer;

    private void Start()
    {
        if (atom == null)
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
        _particleRenderer.material = atom.KindOfParticle switch
        {
            EKindOfParticle.Proton => protonMaterial,
            EKindOfParticle.Neutron => neutronMaterial,
            _ => _particleRenderer.material
        };
    }
}
