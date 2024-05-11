using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "MyScriptable/Attibute")]
public class Attibute: ScriptableObject
{
    public string ID;
    public string attributeName;
    public Sprite icon;
}
