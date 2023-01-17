using UnityEditor;
using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    public class ConnectionDraw
    {
        public string From;
        public string To;
        public int FromIndex;
        private readonly NBehaviorEditorWindow _wnd;

        public ConnectionDraw(NBehaviorEditorWindow window)
        {
            _wnd = window;
        }
        
        public void Draw(Event evnt)
        {
            if (string.IsNullOrEmpty(From))
            {
                DrawToConnection(evnt.mousePosition);
            }
            else if (string.IsNullOrEmpty(To))
            {
                DrawFromConnection(evnt.mousePosition);
            }
        }

        private void DrawFromConnection(Vector2 mousePosition)
        {
            if (From == NBehaviorEditorWindow.ROOD_ID)
            {
                DrawConnection(_wnd._root.RootConnectionRect.center, mousePosition);
            }
            else
            {
                DrawConnection(_wnd._nodes[From].GetFromPoint(FromIndex), mousePosition);
            }
        }

        public static void DrawConnection(Vector2 inPoint, Vector2 outPoint)
        {
            Handles.DrawBezier(inPoint, 
                outPoint, 
                inPoint + Vector2.up * 40, 
                outPoint + Vector2.down * 40, 
                Color.green, 
                null, 
                2f);
            GUI.changed = true;
        }

        private void DrawToConnection(Vector2 evntMousePosition)
        {
            DrawConnection(evntMousePosition, _wnd._nodes[To].GetInPoint());
        }
    }
}