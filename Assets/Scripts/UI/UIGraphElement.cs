using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public abstract class UIGraphElement : MonoBehaviour
{
    protected Transform uiBackGround { get; } = UIProperties.uiBackground;
    protected UIDocument uiDocument { get; set; }
    protected VisualElement root { get; set; }
    protected bool active { get; set; }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uiBackGround.gameObject.SetActive(false);
            root.visible = false;
        }
    }

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        root.visible = false;
    }

    public abstract void onDisable();

    public abstract void enable();
}
