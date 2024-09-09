using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack :MonoBehaviour,IEntityAttack
{
    [SerializeField]
    private AttackDamage damage;
    public AttackDamage attackDamage =>damage;


}
