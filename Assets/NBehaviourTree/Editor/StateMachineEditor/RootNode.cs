using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    public class RootNode
    {
        private readonly NBehaviorEditorWindow _window;
        private Vector2 _offset;

        public RootNode(NBehaviorEditorWindow window)
        {
            _window = window;
        }
        
        public void Draw(Vector2 offset)
        {
            _offset = offset;
            var rect = new Rect(0, 0, 80, 80);
            rect.center = Vector2.zero + offset;
            GUI.DrawTexture(rect, _window._rootNodeTexture);
            GUI.DrawTexture(RootConnectionRect, _window._nodePointTexture);
        }

        public Rect RootConnectionRect {
            get{
                var rect = new Rect(0, 0, 10, 10);
                rect.center = Vector2.zero + _offset + Vector2.up * 40;
                return rect;
            }
        }

        public bool ProcessEvent(Event evnt)
        {
            if (evnt.type == EventType.MouseDown && evnt.button == 0)
            {
                if (RootConnectionRect.Contains(evnt.mousePosition))
                {
                    _window.BeginConnection(NBehaviorEditorWindow.ROOD_ID, null);
                }
            }

            if (evnt.type == EventType.MouseUp && evnt.button == 0 && _window.ConnectionDraw != null)
            {
                if (_window._root.RootConnectionRect.Contains(evnt.mousePosition))
                {
                    _window.ConnectionDraw.From = NBehaviorEditorWindow.ROOD_ID;
                    _window.EndConnection();
                    evnt.Use();
                }
            }
            return false;
        }
    }
}