using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Leap.PhysicalHands;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Atom : MonoBehaviour
{
    static float _phi = Mathf.PI * (3f - Mathf.Sqrt(5f));
    static float _pi2 = Mathf.PI * 2;

    public int Id { get; set; }
    [FormerlySerializedAs("rigidbody")]
    public Rigidbody myRigidbody;
    [CanBeNull] public Atom ParentAtom { get; set; }
    public List<Atom> particles = new();
    public IgnorePhysicalHands ignorePhysicalHands;

    private bool _isMerging;

    public EKindOfParticle KindOfParticle { get; set; }
    public int AmountOfProtons => particles.Count(p => p.KindOfParticle == EKindOfParticle.Proton) 
        + (KindOfParticle == EKindOfParticle.Proton ? 1 : 0);
    public int AmountOfNeutrons => particles.Count(p => p.KindOfParticle == EKindOfParticle.Neutron)
        + (KindOfParticle == EKindOfParticle.Neutron ? 1 : 0);

    public void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody)
        {
            myRigidbody.isKinematic = false;
        }

        Id = Random.Range(1, 50000);
        KindOfParticle = (EKindOfParticle)Random.Range(0, 2);
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

        var otherAtom = other.gameObject.GetComponent<Atom>();
        if (otherAtom == null)
            return;

        var thisRoot = GetParentAtom(this);
        var otherRoot = GetParentAtom(otherAtom);

        if (thisRoot == otherRoot)
            return;

        if (thisRoot._isMerging || otherRoot._isMerging)
            return;

        DisableRigidBodies(thisRoot, otherRoot);

        var particlesCount = thisRoot.AmountOfProtons + otherRoot.AmountOfProtons;
        if (particlesCount > 118) // pegar de um singleton
        {
            Debug.LogWarning("Atom has too many particles, cannot merge.");
            return;
        }

        thisRoot.ignorePhysicalHands.DisableAllGrabbing = true;
        otherRoot.ignorePhysicalHands.DisableAllGrabbing = true;
        if (thisRoot.particles.Count > otherRoot.particles.Count)
        {
            thisRoot.Absorb(otherRoot);
            thisRoot.ArrangeParticles();
        }
        else
        {
            otherRoot.Absorb(thisRoot);
            otherRoot.ArrangeParticles();
        }
        thisRoot.ignorePhysicalHands.DisableAllGrabbing = false;
        otherRoot.ignorePhysicalHands.DisableAllGrabbing = false;

        return;

        void DisableRigidBodies(Atom atom, Atom otherRoot1)
        {
            if (atom.myRigidbody != null)
            {
                atom.myRigidbody.isKinematic = true;
            }

            if (otherRoot1.myRigidbody != null)
            {
                otherRoot1.myRigidbody.isKinematic = true;
            }
        }
    }
    
    public void ArrangeParticles()
    {
        var radius = particles.Count switch
        {
            < 10 => 0.5f,
            < 20 => 1f,
            < 50 => 1.5f,
            < 100 => 2f,
            _ => 3f
        };

        var total = particles.Count;
        for (var i = 0; i < total; i++)
        {
            var p = particles[i];
            var target = Point(radius, i, total + 1);
            StartCoroutine(p.MoveToPosition(target, 0.3f));
        }
    }

    private void Absorb(Atom otherAtom)
    {
        _isMerging = true;
        otherAtom._isMerging = true;
    
        if (particles.Any(p => p.Id == otherAtom.Id))
            return;
    
        if (otherAtom.particles.Any(p => p.Id == Id))
            return;
    
        var otherRigidbody = otherAtom.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
            Destroy(otherRigidbody);
    
        var canvas = otherAtom.GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.enabled = false;
    
        otherAtom.transform.parent = transform;
        otherAtom.ParentAtom = this;
        particles.Add(otherAtom);
    
        foreach (var particle in otherAtom.particles)
        {
            particle.ParentAtom = null;
            Absorb(particle);
        }
    
        otherAtom.particles.Clear();
        otherAtom._isMerging = false;
        _isMerging = false;
    }

    private Atom GetParentAtom(Atom atom)
    {
        var parentAtom = atom.ParentAtom;
        if (parentAtom == null)
            return atom;

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