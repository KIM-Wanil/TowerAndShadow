using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    private bool isCombating;
    public Func<bool> GetIsCombatingFunc;
    public void Start()
    {

    }
    protected override void Awake()
    {
        base.Awake();
        isCombating = false;
        GetIsCombatingFunc = GetIsCombating;
    }
    public bool GetIsCombating()
    {
        return isCombating;
    }
    public void SetIsCombating(bool state)
    {
        isCombating = state;
    }


    public void StartCombat()
    {
        isCombating = true;
    }
    public void EndCombat()
    {
        isCombating = false;
    }
    public void ssibal()
    {
        this.gameObject.SetActive(false);
    }
}