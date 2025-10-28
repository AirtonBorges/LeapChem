using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Eletrosfera : MonoBehaviour
{
    [FormerlySerializedAs("atom")] [SerializeField] private Particle particle;
    [SerializeField] private GameObject electronParticleSystemPrefab;

    private List<ParticleSystem> camadasEletronicas = new();
    private List<int> distribuicaoAnterior = new();

    void Start()
    {
        if (!particle || particle.AmountOfProtons <= 0 || !electronParticleSystemPrefab)
        {
            Debug.LogWarning("Eletrosfera não pode ser inicializada.");
            return;
        }

        if (particle.Elemento != null)
        {
            AtualizarCamadas(particle.Elemento.DistribuicaoEletronica);
        }
    }

    void Update()
    {
        if (!particle)
            return;

        if (particle.ParentParticle)
        {
            foreach (var camada in camadasEletronicas)
            {
                camada.Stop();
                camada.Clear();
            }
        }

        var atual = particle.Elemento.DistribuicaoEletronica;
        if (!atual.SequenceEqual(distribuicaoAnterior))
        {
            AtualizarCamadas(atual);
            distribuicaoAnterior = new List<int>(atual);
        }
    }

    private void AtualizarCamadas(List<int> distribuicao)
    {
        for (var i = 0; i < distribuicao.Count; i++)
        {
            if (i >= camadasEletronicas.Count)
            {
                var camada = CriarCamadaEletronica(distribuicao[i], i);
                if (camada != null)
                    camadasEletronicas.Add(camada);
            }
            else
            {
                var main = camadasEletronicas[i].main;
                main.maxParticles = distribuicao[i];
            }
        }

        for (var i = distribuicao.Count; i < camadasEletronicas.Count; i++)
        {
            camadasEletronicas[i].Stop();
            camadasEletronicas[i].Clear();
        }
    }

    private ParticleSystem CriarCamadaEletronica(int eletrons, int index)
    {
        var obj = Instantiate(electronParticleSystemPrefab, gameObject.transform);

        var raio = (particle.radius) + Mathf.Log(index + 10); // log base e
        obj.transform.localScale = Vector3.one * raio;

        if (index % 2 != 0)
        {
            obj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            obj.transform.localRotation = Quaternion.identity;
        }

        var ps = obj.GetComponent<ParticleSystem>();
        if (!ps)
        {
            Destroy(obj);
            return null;
        }

        var main = ps.main;
        main.maxParticles = eletrons;
        ps.Play();
        return ps;
    }

}
