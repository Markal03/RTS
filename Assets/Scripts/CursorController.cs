using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    // Start is called before the first frame update
    public static CursorController instance;

    public Texture2D defaultCursor, attackCursor;
    private void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    public void ActivateAttackCursor()
    {
        Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
    }
}
