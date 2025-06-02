using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AtomDescription : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public Canvas canvas;
    public Atom atom;

    private Transform _parentTransform;
    private float _offset;
    private Camera _camera;

    private Dictionary<int, string> _kindOfElement = new()
    {
        { 1, "Hidrogênio (H)" },
        { 2, "Hélio (He)" },
        { 3, "Lítio (Li)" },
        { 4, "Berílio (Be)" },
        { 5, "Boro (B)" },
        { 6, "Carbono (C)" },
        { 7, "Nitrogênio (N)" },
        { 8, "Oxigênio (O)" },
        { 9, "Flúor (F)" },
        { 10, "Neônio (Ne)" },
        { 11, "Sódio (Na)" },
        { 12, "Magnésio (Mg)" },
        { 13, "Alumínio (Al)" },
        { 14, "Silício (Si)" },
        { 15, "Fósforo (P)" },
        { 16, "Enxofre (S)" },
        { 17, "Cloro (Cl)" },
        { 18, "Argônio (Ar)" },
        { 19, "Potássio (K)" },
        { 20, "Cálcio (Ca)" },
        { 21, "Escândio (Sc)" },
        { 22, "Titânio (Ti)" },
        { 23, "Vanádio (V)" },
        { 24, "Cromo (Cr)" },
        { 25, "Manganês (Mn)" },
        { 26, "Ferro (Fe)" },
        { 27, "Cobalto (Co)" },
        { 28, "Níquel (Ni)" },
        { 29, "Cobre (Cu)" },
        { 30, "Zinco (Zn)" },
        { 31, "Gálio (Ga)" },
        { 32, "Germânio (Ge)" },
        { 33, "Arsênio (As)" },
        { 34, "Selênio (Se)" },
        { 35, "Bromo (Br)" },
        { 36, "Criptônio (Kr)" },
        { 37, "Rubídio (Rb)" },
        { 38, "Estrôncio (Sr)" },
        { 39, "Ítrio (Y)" },
        { 40, "Zircônio (Zr)" },
        { 41, "Nióbio (Nb)" },
        { 42, "Molibdênio (Mo)" },
        { 43, "Tecnécio (Tc)" },
        { 44, "Rutênio (Ru)" },
        { 45, "Ródio (Rh)" },
        { 46, "Paládio (Pd)" },
        { 47, "Prata (Ag)" },
        { 48, "Cádmio (Cd)" },
        { 49, "Índio (In)" },
        { 50, "Estanho (Sn)" },
        { 51, "Antimônio (Sb)" },
        { 52, "Telúrio (Te)" },
        { 53, "Iodo (I)" },
        { 54, "Xenônio (Xe)" },
        { 55, "Césio (Cs)" },
        { 56, "Bário (Ba)" },
        { 57, "Lantânio (La)" },
        { 58, "Cério (Ce)" },
        { 59, "Praseodímio (Pr)" },
        { 60, "Neodímio (Nd)" },
        { 61, "Promécio (Pm)" },
        { 62, "Samário (Sm)" },
        { 63, "Európio (Eu)" },
        { 64, "Gadolínio (Gd)" },
        { 65, "Térbio (Tb)" },
        { 66, "Disprósio (Dy)" },
        { 67, "Hólmio (Ho)" },
        { 68, "Érbio (Er)" },
        { 69, "Túlio (Tm)" },
        { 70, "Itérbio (Yb)" },
        { 71, "Lutécio (Lu)" },
        { 72, "Háfnio (Hf)" },
        { 73, "Tântalo (Ta)" },
        { 74, "Tungstênio (W)" },
        { 75, "Rênio (Re)" },
        { 76, "Ósmio (Os)" },
        { 77, "Irídio (Ir)" },
        { 78, "Platina (Pt)" },
        { 79, "Ouro (Au)" },
        { 80, "Mercúrio (Hg)" },
        { 81, "Tálio (Tl)" },
        { 82, "Chumbo (Pb)" },
        { 83, "Bismuto (Bi)" },
        { 84, "Polônio (Po)" },
        { 85, "Astato (At)" },
        { 86, "Radônio (Rn)" },
        { 87, "Frâncio (Fr)" },
        { 88, "Rádio (Ra)" },
        { 89, "Actínio (Ac)" },
        { 90, "Tório (Th)" },
        { 91, "Protactínio (Pa)" },
        { 92, "Urânio (U)" },
        { 93, "Netúnio (Np)" },
        { 94, "Plutônio (Pu)" },
        { 95, "Amerício (Am)" },
        { 96, "Cúrio (Cm)" },
        { 97, "Bérquio (Bk)" },
        { 98, "Califórnio (Cf)" },
        { 99, "Einstênio (Es)" },
        { 100, "Férmio (Fm)" },
        { 101, "Mendelévio (Md)" },
        { 102, "Nobélio (No)" },
        { 103, "Laurêncio (Lr)" },
        { 104, "Rutherfórdio (Rf)" },
        { 105, "Dúbnio (Db)" },
        { 106, "Seabórgio (Sg)" },
        { 107, "Bóhrio (Bh)" },
        { 108, "Hássio (Hs)" },
        { 109, "Meitnério (Mt)" },
        { 110, "Darmstádio (Ds)" },
        { 111, "Roentgênio (Rg)" },
        { 112, "Copernício (Cn)" },
        { 113, "Nihônio (Nh)" },
        { 114, "Fleróvio (Fl)" },
        { 115, "Moscóvio (Mc)" },
        { 116, "Livermório (Lv)" },
        { 117, "Tenessino (Ts)" },
        { 118, "Oganessônio (Og)" }
    };

    private void Start()
    {
        _camera = Camera.main;
        _parentTransform = transform.parent;
    }

    public void Update()
    {
        if (!canvas || !_camera || !atom) return;

        canvas.worldCamera = _camera;
        canvas.transform.LookAt(_camera.transform);
        canvas.transform.rotation = Quaternion.LookRotation(_camera.transform.forward);

        _offset = atom.AmountOfProtons switch
        {
            > 100 => 1.5f,
            _ => 1
        };
        
        canvas.transform.position = (atom.transform.position + (Vector3.up * (_parentTransform.lossyScale.magnitude * _offset)));

        if (text && text.enabled && _kindOfElement.TryGetValue(atom.AmountOfProtons, out var atomName))
        {
            text.text = atomName;
        }
    }
}
