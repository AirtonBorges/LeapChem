using System;
using System.Collections.Generic;
using Leap.PhysicalHands;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLogic : MonoBehaviour
{
    [FormerlySerializedAs("TextoNome")] public TMPro.TextMeshProUGUI TextoTitulo;
    [FormerlySerializedAs("UIText")] public TMPro.TextMeshProUGUI TextoDescricao;
    
    public GameObject atomPrefab;
    public GameObject parentGameObject;
    public Transform placeToInstantiate;
    public List<GameObject> _allAtoms = new();
    
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

    public void AdicionarParticula(EKindOfParticle? pKindOfParticle = null)
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
        var particleComponent = atom.GetComponentInChildren<Particle>();
        var kindOfParticle = pKindOfParticle ?? (EKindOfParticle)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EKindOfParticle)).Length);
        if (particleComponent)
        {
            particleComponent.KindOfParticle = kindOfParticle;
        }
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

    public void AoDarHover(ContactHand hand, Rigidbody pRigidBody)
    {
        var atom = pRigidBody.gameObject.GetComponent<Atom>();
        if (atom)
        {
            MostrarDescricao(atom);
        }
        
        var atomSpawner = pRigidBody.gameObject.GetComponent<AtomSpawner>();
        if (atomSpawner)
        {
            atomSpawner.SpawnAtom();
        }
    }

    public void MostrarDescricao(Atom atom)
    {
        if (!atom)
        {
            return;
        }

        if (!TextoDescricao)
        {
            return;
        }

        var atomParticle = atom.particle;
        var elemento = atomParticle.Elemento;

        var titulo = $"{elemento.Nome} Neutrons: {atomParticle.AmountOfNeutrons} Protons: {atomParticle.AmountOfProtons}";
        var descricao = TextoDescricao.text = atomParticle.Elemento.Curiosidade;
        AtualizarQuadro(titulo, descricao);
    }

    public void AoSairHover(ContactHand hand, Rigidbody rigidbody)
    {
        if (!TextoDescricao)
        {
            return;
        }

        AtualizarQuadro("", "");
    }

    public void AtualizarQuadro(string titulo, string descricao)
    {
        TextoTitulo.SetText(titulo);
        TextoDescricao.SetText(descricao);
    }
    
}
