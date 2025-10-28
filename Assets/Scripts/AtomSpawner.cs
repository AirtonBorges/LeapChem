using UnityEngine;

public class AtomSpawner : MonoBehaviour
{
    public GameObject atomPrefab;
    public Transform spawnPoint;
    public Transform parentTransform;
    public GameLogic GameLogic;
    public EKindOfParticle kindOfParticle;
 
    public float spawnInterval = 2f;
    public float forceMagnitude = 1f;
    private float _timer = 0;

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void SpawnAtom()
    {
        if (_timer < spawnInterval)
        {
            return;
        }

        if (atomPrefab == null || spawnPoint == null || parentTransform == null)
        {
            Debug.LogError("AtomSpawner: Missing references.");
            return;
        }
        
        var newAtom = Instantiate(atomPrefab, spawnPoint.position, Quaternion.identity);
        
        var particleComponent = newAtom.GetComponentInChildren<Particle>();
        if (particleComponent)
        {
            particleComponent.KindOfParticle = kindOfParticle;
        }

        newAtom.transform.SetParent(parentTransform);
        newAtom.transform.localScale = parentTransform.localScale;
        var atomRigidbody = newAtom.GetComponent<Rigidbody>();
        if (atomRigidbody)
        {
            atomRigidbody.AddForce(Vector3.up * (forceMagnitude), ForceMode.Acceleration);
        }

        _timer = 0;
        GameLogic!._allAtoms!.Add(newAtom);
    }
}
