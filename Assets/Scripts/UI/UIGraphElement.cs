using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIGraphElement : MonoBehaviour, IUIGraph
{
    [SerializeField] private Transform UIBackground;

    public void onDisable()
    {
        throw new System.NotImplementedException();
    }

    public abstract void onEnable();

    [ContextMenu("Editor_Setup")]
    public void dingusBingus() {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
