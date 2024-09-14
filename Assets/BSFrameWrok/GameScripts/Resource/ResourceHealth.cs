using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHealth : MonoBehaviour, IResourceHealth
{
    [SerializeField]
    private int _MaxHealth;
    public int MaxHealth { get => _MaxHealth; set => _MaxHealth = value; }

    [SerializeField]
    private int _CurHealth;
    public int CurHealth { get => _CurHealth; set =>_CurHealth=value; }
    bool IHealthEntity._canIncrease { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    bool IHealthEntity._canDecrease { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    bool IHealthEntity._isDead { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    Action OnHit;
    Action OnDestory;

    private void Start()
    {
        CurHealth=MaxHealth;
    }

    private void OnEnable()
    {
        OnHit += ResHitted;
        OnDestory += ResDestory;
    }

    private void OnDisable()
    {
        OnHit -= ResHitted;
        OnDestory -= ResDestory;
    }
    public void ResHitted()
    {
        //Debug.Log("ResHitted");
    }

    public void ResDestory()
    {

    }

    public void TakeDamage(int damage)
    {
        OnHit?.Invoke();
        CurHealth -= damage;
        if(CurHealth <=0)
        {
            Die();
        }
    }

    public void Healing(int heal)
    {
        
    }

    public void Die()
    {
        OnDestory?.Invoke();
        Destroy(gameObject);
    }


}
