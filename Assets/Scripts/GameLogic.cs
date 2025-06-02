using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class GameLogic : MonoBehaviour
{
    public GameObject atomPrefab;
    public GameObject parentGameObject;
    public Transform placeToInstantiate;
    private List<GameObject> _allAtoms = new();
    
    void Start()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e2) =>
        {
            Debug.Log("Unhandled exception: " + e2.ExceptionObject.ToString());
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdicionarParticula();
        }
    }

    public void AdicionarParticula()
    {
        if (!atomPrefab)
        {
            Debug.LogError("Atom prefab is not assigned.");
            return;
        }

        if (!placeToInstantiate)
        {
            Debug.LogError("Place to instantiate is not assigned.");
            return;
        }

        var atom = Instantiate(atomPrefab, placeToInstantiate.position, Quaternion.identity);
        atom.transform.localScale = parentGameObject.transform.localScale;
        atom.transform.SetParent(parentGameObject.transform);
        _allAtoms.Add(atom);
    }

    public void RemoverTodasAsParticulas()
    {
        foreach (GameObject child in _allAtoms)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log("All atoms deleted.");
    }
}
