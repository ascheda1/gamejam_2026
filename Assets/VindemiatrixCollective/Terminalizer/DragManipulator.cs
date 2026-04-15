// Assembly-CSharp © 2025-2026 Vindemiatrix Collective

#region using

using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public class DragManipulator : PointerManipulator
    {
        private readonly VisualElement sensibleArea;
        private bool enabled;
        private Vector2 targetStartPosition;
        private Vector3 pointerStartPosition;

        public DragManipulator(VisualElement sensibleArea, VisualElement target)
        {
            this.sensibleArea = sensibleArea;
            this.target       = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            sensibleArea.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            sensibleArea.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            sensibleArea.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            sensibleArea.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            sensibleArea.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            sensibleArea.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        // This method stores the starting position of target and the pointer,
        // makes target capture the pointer, and denotes that a drag is now in progress.
        private void PointerDownHandler(PointerDownEvent evt)
        {
            StyleTranslate t = target.style.translate;
            targetStartPosition  = new Vector2(t.value.x.value, t.value.y.value);
            pointerStartPosition = evt.position;
            sensibleArea.CapturePointer(evt.pointerId);
            enabled = true;
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, calculates a new position for target within the bounds of the window.
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && sensibleArea.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;

                target.transform.position =
                    new Vector2(Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
            }
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, makes target release the pointer.
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && sensibleArea.HasPointerCapture(evt.pointerId))
            {
                sensibleArea.ReleasePointer(evt.pointerId);
            }
        }
    }
}