using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "MyScriptable/Attibute")]
public class AttributeType: ScriptableObject
{
    public string ID;
    public Constants.Attribute attribute;
    public Sprite icon;
}
