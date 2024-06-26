using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/CreateEnemy")]
public class Enemy : Character
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }

    [SerializeField] private Sprite eyesSprite;
    public Sprite EyesSprite
    {
        get { return eyesSprite; }
        set { eyesSprite = value; }
    }

    [SerializeField] private int exp;
    public int Exp
    {
        get { return exp; }
        set { exp = value; }
    }

    [SerializeField] private int gold;
    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    [SerializeField] private Item dropItemOne;
    public Item DropItemOne
    {
        get { return dropItemOne; }
        set { dropItemOne = value; }
    }

    [SerializeField] private float dropRateOne;
    public float DropRateOne
    {
        get { return dropRateOne; }
        set { dropRateOne = value; }
    }

    [SerializeField] private Item dropItemTwo;
    public Item DropItemTwo
    {
        get { return dropItemTwo; }
        set { dropItemTwo = value; }
    }

    [SerializeField] private float dropRateTwo;
    public float DropRateTwo
    {
        get { return dropRateTwo; }
        set { dropRateTwo = value; }
    }

    [SerializeField] private Item stealItemOne;
    public Item StealItemOne
    {
        get { return stealItemOne; }
        set { stealItemOne = value; }
    }

    [SerializeField] private float stealRateOne;
    public float StealRateOne
    {
        get { return stealRateOne; }
        set { stealRateOne = value; }
    }

    [SerializeField] private Item stealItemTwo;
    public Item StealItemTwo
    {
        get { return stealItemTwo; }
        set { stealItemTwo = value; }
    }

    [SerializeField] private float stealRateTwo;
    public float StealRateTwo
    {
        get { return stealRateTwo; }
        set { stealRateTwo = value; }
    }

    private bool isStolen;
    public bool IsStolen
    {
        get { return isStolen; }
        set { isStolen = value; }
    }

    private EnemyComponent component;
    public EnemyComponent Component
    {
        get { return component; }
        set { component = value; }
    }

    public Enemy()
    {
        MaxHp = BaseMaxHp;
        MaxMp = BaseMaxMp;
        
        HP = BaseMaxHp;
        MP = BaseMaxMp;
    }

    /// <summary>
    /// 状態異常・ステータス変化アイコン更新
    /// </summary>
    public void UpdateStatusEffectSprites()
    {
        if (component != null)
        {
            int maxVisibleEffectCount = 6;
            int activateEffectCount = ActivateStatusEffects.Count;

            for (int i = 0; i < maxVisibleEffectCount; i++)
            {
                SpriteRenderer imageComponent = effectSpriteObjects[i].GetComponent<SpriteRenderer>();
                if (imageComponent != null)
                {
                    if (i < activateEffectCount)
                    {
                        imageComponent.enabled = true;
                        var effectName = ActivateStatusEffects[i];
                        imageComponent.sprite = CommonController.GetSpriteForEffect(effectName);
                    }
                    else
                    {
                        imageComponent.enabled = false;
                    }
                }
            }
        }        
    }
}
