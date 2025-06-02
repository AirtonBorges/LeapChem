using UnityEngine;

public class Orbital : MonoBehaviour
{
    public float speed = 50f;
    public float radius = 1.5f;
    public Atom atom;
    public int amountOfElectrons;
    public GameObject electronPrefab;

    void Start()
    {
        for (var i = 0; i < amountOfElectrons; i++)
        {
            var angle = i * Mathf.PI * 2f / amountOfElectrons;
            var offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

            var electron = Instantiate(electronPrefab, transform);
            electron.transform.localPosition = offset;
        }

        if (atom)
        {
            transform.parent = atom.transform;
            transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * (speed * Time.deltaTime));
    }
}

