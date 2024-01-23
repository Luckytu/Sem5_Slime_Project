using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSettingsUI : UIGraphElement
{
    [SerializeField] private UIGraphElement CameraSettingsInGameUI;
    [SerializeField] private UIGraphElement PauseMenuUI;
    [SerializeField] private CameraCapture cameraCapture;

    private Button back;
    private Button editInGameView;

    private VisualElement cameraControlsContainer;
    private VisualElement inputImageContainer;
    private Image inputImage;
    private Image outputImage;
    private VisualElement x1, x2, x3, x4;
    private CameraCapture.EdgePointCoords p;

    private void Start()
    {
        root.visible = false;

        back = root.Q("BackButton") as Button;
        back.RegisterCallback<ClickEvent>(backButtonClick);

        editInGameView = root.Q("EditInGameViewButton") as Button;
        editInGameView.RegisterCallback<ClickEvent>(editInGameViewClick);

        cameraControlsContainer = root.Q("CameraControlsContainer");
        
        inputImageContainer = root.Q("CameraInputContainer");
        inputImageContainer.RegisterCallback<GeometryChangedEvent>(initializeEdgePoints);

        inputImage = root.Q("CameraInput") as Image;
        outputImage = root.Q("CameraOutput") as Image;

        x1 = root.Q("x1");
        x2 = root.Q("x2");
        x3 = root.Q("x3");
        x4 = root.Q("x4");
    }

    private void setUpCameraImages()
    {
        Rect layout = cameraControlsContainer.layout;

        float width = layout.width * 0.45f;
        float height = width * 9f / 16f;

        inputImage.image = cameraCapture.cameraMap;
        inputImage.style.width = width;
        inputImage.style.height = height;

        outputImage.image = cameraCapture.resultMap;
        outputImage.style.width = width;
        outputImage.style.height = height;
    }

    private void initializeEdgePoints(GeometryChangedEvent e)
    {
        p = cameraCapture.getEdgePoints();

        positionEdgePoint(x1, p.x1, p.y1);
        positionEdgePoint(x2, p.x2, p.y2);
        positionEdgePoint(x3, p.x3, p.y3);
        positionEdgePoint(x4, p.x4, p.y4);
    }

    private void positionEdgePoint(VisualElement p, float x, float y)
    {
        Rect edgePointLayout = p.layout;
        Rect containerLayout = inputImageContainer.layout;

        float w = containerLayout.width;
        float h = containerLayout.height;

        p.style.left = (x * w) - (edgePointLayout.width / 2);
        p.style.top = ((1 - y) * h) - (edgePointLayout.height / 2);
    }

    private void backButtonClick(ClickEvent e)
    {
        onDisable();

        PauseMenuUI.enable();
    }

    private void editInGameViewClick(ClickEvent e)
    {
        onDisable();

        CameraSettingsInGameUI.enable();
    }

    public override void enable()
    {
        root.visible = true;

        setUpCameraImages();
    }

    public override void onDisable()
    {
        root.visible = false;
    }
}
