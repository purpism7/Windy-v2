using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public interface IListener
    {
        void OnEnter(GameObject gameObj);
        void OnExit(GameObject gameObj);
        void OnStay(GameObject gameObj);
    }
        
    private IListener _iListener = null;
        
    public void Initialize(IListener iListener)
    {
        _iListener = iListener;
    }

    public void ChainUpdate()
    {
            
    }
        
    private void OnTriggerEnter2D(Collider2D other)
    {
        _iListener?.OnEnter(other?.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _iListener?.OnExit(other?.gameObject);
    }
        
    private void OnTriggerStay2D(Collider2D other)
    {
        _iListener?.OnStay(other?.gameObject);
    }
}

