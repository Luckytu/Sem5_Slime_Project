using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettingsInGameUI : UIGraphElement
{
    private void Start()
    {
        root.visible = false;
    }

    public override void enable()
    {
        root.visible = true;
    }

    public override void onDisable()
    {
        throw new System.NotImplementedException();
    }
}
