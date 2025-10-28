using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Leap.PhysicalHands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Particle : MonoBehaviour
{
    static float _phi = Mathf.PI * (3f - Mathf.Sqrt(5f));
    static float _pi2 = Mathf.PI * 2;

    public int Id { get; set; }
    [CanBeNull] public Particle ParentParticle { get; set; }
    public List<Particle> particles = new();
    // public IgnorePhysicalHands ignorePhysicalHands;
    public Elemento Elemento;

    private bool _isMerging;

    public EKindOfParticle KindOfParticle { get; set; }
    public int AmountOfProtons => particles.Count(p => p.KindOfParticle == EKindOfParticle.Proton) 
        + (KindOfParticle == EKindOfParticle.Proton ? 1 : 0);
    public int AmountOfNeutrons => particles.Count(p => p.KindOfParticle == EKindOfParticle.Neutron)
        + (KindOfParticle == EKindOfParticle.Neutron ? 1 : 0);

    public float radius => particles.Count switch
    {
        < 10 => 0.5f,
        < 20 => 1f,
        < 50 => 1.5f,
        < 100 => 2f,
        _ => 3f
    };

    public void Start()
    {
        Id = Random.Range(1, 50000); 
        // KindOfParticle = (EKindOfParticle)Random.Range(0, 2);
        Elemento = Table.Particles[AmountOfProtons];
    }

    public void Update()
    {
        Elemento = Table.Particles[AmountOfProtons];
    }

    // https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere
    public static Vector3 Point(float radius
        , int index
        , int total
        , float min = 0f
        , float max = 1f
        , float angleStartDeg = 0f
        , float angleRangeDeg = 360
    )
    {
        if (total < 2) total = 2;
        var denominator = total - 1f;
        var y = ((index / denominator) * (max - min) + min) * 2f - 1f;
        y = Mathf.Clamp(y, -1f, 1f);

        var theta = _phi * index;

        if (angleStartDeg != 0 || !Mathf.Approximately(angleRangeDeg, 360))
        {
            theta %= _pi2;
            if (theta < 0) theta += _pi2;

            var a1 = angleStartDeg * Mathf.Deg2Rad;
            var a2 = angleRangeDeg * Mathf.Deg2Rad;

            theta = theta * a2 / _pi2 + a1;
        }

        var rY = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
        var x = Mathf.Cos(theta) * rY;
        var z = Mathf.Sin(theta) * rY;

        return new Vector3(x, y, z) * radius;
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Glueable"))
            return;

        var otherAtom = other.gameObject.GetComponentInChildren<Particle>();
        if (otherAtom == null)
            return;

        var thisRoot = GetParentAtom(this);
        var otherRoot = GetParentAtom(otherAtom);

        if (thisRoot == otherRoot)
            return;

        if (thisRoot._isMerging || otherRoot._isMerging)
            return;


        var particlesCount = thisRoot.AmountOfProtons + otherRoot.AmountOfProtons;
        if (particlesCount > 118) // pegar de um singleton
        {
            Debug.LogWarning("Atom has too many particles, cannot merge.");
            return;
        }

        if (thisRoot.particles.Count > otherRoot.particles.Count)
        {
            thisRoot.Absorb(otherRoot);
            thisRoot.ArrangeParticles();
            thisRoot.MorphSize();
        }
        else
        {
            otherRoot.Absorb(thisRoot);
            otherRoot.ArrangeParticles();
            otherRoot.MorphSize();
        }
    }

    private void MorphSize()
    {
        var total = particles.Count;
        var targetScale = total switch
        {
            < 50 => 1f,
            < 100 => 0.8f,
            _ => 0.5f
        };

        foreach (var p in particles)
        {
            p.transform.localScale = Vector3.one;
        }

        StartCoroutine(MorphSizeCoroutine(targetScale));
    }
    
    private IEnumerator MorphSizeCoroutine(float targetScale)
    {
        var startScale = transform.localScale.x;
        var elapsed = 0f;
        var duration = 0.3f;

        while (elapsed < duration)
        {
            var newScale = Mathf.Lerp(startScale, targetScale, elapsed / duration);
            transform.localScale = new Vector3(newScale, newScale, newScale);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    public void ArrangeParticles()
    {
        var total = particles.Count;
        for (var i = 0; i < total; i++)
        {
            var p = particles[i];
            var target = Point(radius, i, total + 1);
            StartCoroutine(p.MoveToPosition(target, 0.3f));
        }
    }

    private void Absorb(Particle otherParticle)
    {
        _isMerging = true;
        otherParticle._isMerging = true;
    
        if (particles.Any(p => p.Id == otherParticle.Id))
            return;
    
        if (otherParticle.particles.Any(p => p.Id == Id))
            return;
    
        var canvas = otherParticle.GetComponentInChildren<Canvas>();
        if (canvas)
            canvas.enabled = false;
    
        otherParticle.transform.parent = transform;
        otherParticle.ParentParticle = this;
        particles.Add(otherParticle);
    
        foreach (var particle in otherParticle.particles)
        {
            particle.ParentParticle = null;
            particle.transform.localScale = Vector3.one;
            Absorb(particle);
        }
    
        otherParticle.particles.Clear();
        otherParticle._isMerging = false;
        _isMerging = false;
    }

    private Particle GetParentAtom(Particle particle)
    {
        var parentAtom = particle.ParentParticle;
        if (parentAtom == null)
            return particle;

        parentAtom = GetParentAtom(parentAtom);
        return parentAtom;
    }
    
    public IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        var start = transform.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
    }
}