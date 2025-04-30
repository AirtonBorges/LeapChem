using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    static float _phi = Mathf.PI * ( 3f - Mathf.Sqrt( 5f ) );
    static float _pi2 = Mathf.PI * 2;
    private int _amountOfParticles = 0;
    
    private Dictionary<int, string> _kindOfParticle = new()
    {
        { 0, "H" },
        { 1, "He" },
        { 2, "Li" },
        { 3, "Be" },
        { 4, "B" },
        { 5, "C" },
        { 6, "N" },
        { 7, "O" },
        { 8, "F" },
        { 9, "Ne" },
        { 10, "Na" },
        { 11, "Mg" },
        { 12, "Al" },
        { 13, "Si" },
        { 14, "P" },
        { 15, "S" },
        { 16, "Cl" },
        { 17, "Ar" }
    };

    public TMPro.TextMeshProUGUI text;

    public void Update()
    {
        if (text)
        {
            text.text = _kindOfParticle[_amountOfParticles];
        }
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
        var y = ( ( index / ( total - 1f ) ) * ( max - min ) + min ) * 2f - 1f;

        // golden angle increment
        var theta = _phi * index ; 
        
        if (angleStartDeg != 0 || !Mathf.Approximately(angleRangeDeg, 360) )
        {
            theta = ( theta % ( _pi2 ) ) ;
            theta = theta < 0 ? theta + _pi2 : theta ;
            
            var a1 = angleStartDeg * Mathf.Deg2Rad;
            var a2 = angleRangeDeg * Mathf.Deg2Rad;
            
            theta = theta * a2 / _pi2 + a1;
        }

        var rY = Mathf.Sqrt( 1 - y * y ); 
    
        var x = Mathf.Cos( theta ) * rY;
        var z = Mathf.Sin( theta ) * rY;

        return  new Vector3( x, y, z ) * radius;
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Glueable"))
        {
            other.transform.parent = gameObject.transform;
            _amountOfParticles++;
        }
    }
}
