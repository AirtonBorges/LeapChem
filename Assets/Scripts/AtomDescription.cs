using UnityEngine;
using UnityEngine.Serialization;

public class AtomDescription : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public Canvas canvas;
    [FormerlySerializedAs("atom")] public Particle particle;

    private Transform _parentTransform;
    private float _offset;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _parentTransform = transform.parent;
    }

    public void Update()
    {
        if (!canvas || !_camera || !particle) return;

        canvas.worldCamera = _camera;
        canvas.transform.LookAt(_camera.transform);
        canvas.transform.rotation = Quaternion.LookRotation(_camera.transform.forward);

        _offset = particle.AmountOfProtons switch
        {
            > 100 => 1.5f,
            _ => 1
        };

        canvas.transform.position = (particle.transform.position + (Vector3.up * (_parentTransform.lossyScale.magnitude * _offset)));

        if (text && text.enabled && Table.Particles.TryGetValue(particle.AmountOfProtons, out var particula))
        {
            text.text = particula.Nome;
        }
    }
}
