using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaterialSound
{
    public PhysicMaterial material;
    public AudioClip footstepSound;
}

public class MaterialSoundManager : MonoBehaviour
{
    public List<MaterialSound> materialSounds = new List<MaterialSound>();

    public AudioClip GetFootstepSound(PhysicMaterial material)
    {
        foreach (var materialSound in materialSounds)
        {
            if (materialSound.material == material)
            {
                return materialSound.footstepSound;
            }
        }
        return null;
    }
}