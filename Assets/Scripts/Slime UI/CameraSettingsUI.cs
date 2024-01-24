using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slime_UI
{
    public class CameraSettingsUI : UIGraphElement
    {
        [SerializeField] private UIGraphElement CameraSettingsInGameUI;
        [SerializeField] private UIGraphElement PauseMenuUI;
        [SerializeField] private CameraCapture cameraCapture;

        private VisualElement dragDropTarget;
        private bool dragDropInProgress = false;

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

            registerDragDropCallbacks();
        }

        private void registerDragDropCallbacks()
        {
            x1.RegisterCallback<PointerDownEvent>(startDrag);
            x1.RegisterCallback<PointerMoveEvent>(drag);
            x1.RegisterCallback<PointerUpEvent>(drop);

            x2.RegisterCallback<PointerDownEvent>(startDrag);
            x2.RegisterCallback<PointerMoveEvent>(drag);
            x2.RegisterCallback<PointerUpEvent>(drop);

            x3.RegisterCallback<PointerDownEvent>(startDrag);
            x3.RegisterCallback<PointerMoveEvent>(drag);
            x3.RegisterCallback<PointerUpEvent>(drop);

            x4.RegisterCallback<PointerDownEvent>(startDrag);
            x4.RegisterCallback<PointerMoveEvent>(drag);
            x4.RegisterCallback<PointerUpEvent>(drop);
        }

        private void startDrag(PointerDownEvent e)
        {
            dragDropTarget = e.target as VisualElement;
            dragDropTarget.CapturePointer(e.pointerId);
            dragDropInProgress = true;

            UnityEngine.Cursor.visible = false;
        }

        private void drag(PointerMoveEvent e)
        {
            if (dragDropInProgress && dragDropTarget.HasPointerCapture(e.pointerId))
            {
                float x = dragDropTarget.layout.x + e.localPosition.x;
                float y = dragDropTarget.layout.y + e.localPosition.y;

                positionEdgePoint(dragDropTarget, x, y);

                updateSourcePoints();
            }
        }

        private void updateSourcePoints()
        {
            float x = (dragDropTarget.layout.x + (dragDropTarget.layout.width / 2)) / inputImageContainer.layout.width;
            float y = 1 - ((dragDropTarget.layout.y + (dragDropTarget.layout.height / 2)) / inputImageContainer.layout.height);

            //might be useful
            //x = (float) Math.Round(x, 2);
            //y = (float) Math.Round(y, 2);

            Debug.Log("x:" + x + " y:" + y);

            cameraCapture.setEdgePoints(dragDropTarget.name, x, y);
        }

        private void drop(PointerUpEvent e)
        {
            if (dragDropInProgress && dragDropTarget.HasPointerCapture(e.pointerId))
            {
                dragDropTarget.ReleasePointer(e.pointerId);
                dragDropInProgress = false;

                UnityEngine.Cursor.visible = true;
            }
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

            positionEdgePointUV(x1, p.x1, p.y1);
            positionEdgePointUV(x2, p.x2, p.y2);
            positionEdgePointUV(x3, p.x3, p.y3);
            positionEdgePointUV(x4, p.x4, p.y4);
        }

        private void positionEdgePoint(VisualElement p, float x, float y)
        {
            Rect edgePointLayout = p.layout;
            Rect containerLayout = inputImageContainer.layout;

            if(x >= 0 && x < containerLayout.width)
            {
                p.style.left = x - (edgePointLayout.width / 2);
            }

            if(y >= 0 && y < containerLayout.height)
            {
                p.style.top = y - (edgePointLayout.height / 2);
            }
        }

        private void positionEdgePointUV(VisualElement p, float u, float v)
        {
            Rect containerLayout = inputImageContainer.layout;

            float w = containerLayout.width;
            float h = containerLayout.height;

            positionEdgePoint(p, u * w, (1 - v) * h);
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

        protected override void fallback()
        {
            throw new NotImplementedException();
        }
    }
}
