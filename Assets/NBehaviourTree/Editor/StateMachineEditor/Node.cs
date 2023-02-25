using System;
using NBehaviourTree.Runtime;
using UnityEditor;
using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    public class Node
    {
        public Vector2 Position;

        private static readonly Vector2 Size = new Vector2(100, 80);
        private const float _fieldHeight = 20f;
        private Type _type => _node.GetType();
        private Vector2 _offset;
        private bool _isDragged;
        private readonly EditorData _data;
        private readonly BaseNode _node;
        private readonly NBehaviorEditorWindow _window;
        private bool _click;

        private Rect Rect
        {
            get
            {
                var rect = new Rect(Vector2.zero, Size);
                rect.center = Position + _offset;
                return rect;
            }
        }

        private Rect InRect => new Rect(Position + _offset + Vector2.down * (5 + Size.y * 0.5f) + Vector2.left * 5, new Vector2(10, 10));
        public BaseNode StateNode => _node;
        private Rect[] EndRects;

        public Node(Vector2 positon, BaseNode node, EditorData editorData, NBehaviorEditorWindow window)
        {
            _window = window;
            _data = editorData;
            _node = node;
            Position = positon;
            switch (_node)
            {
                case BaseDecoratorNode decoratorNode:
                    EndRects = new[] { new Rect(new Vector2(0, -5), new Vector2(10, 10)) };
                    break;
                case BaseCompositeNode compositeNode:
                    var count = compositeNode.MaxChilds;
                    EndRects = new Rect[count];
                    for (int i = 0; i < count; i++)
                    {
                        var rect = new Rect(Vector2.zero, new Vector2(10, 10));
                        rect.center = Vector2.right * (Size.x / count * (i+1) - Size.x * 0.5f - 5);
                        EndRects[i] = rect;
                    }
                    break;
                default:
                    EndRects = Array.Empty<Rect>();
                    break;
            }
        }

        public void Drag(Vector2 delta)
        {
            Position += delta;
            _data.Position = Position;
        }

        public void Draw(Vector2 offset)
        {
            _offset = offset;
            var color = GUI.color;
            switch (_node.Status)
            {
                case BehaviourStatus.Success:
                    GUI.color = Color.green;
                    break;
                case BehaviourStatus.Running:
                    GUI.color = Color.blue;
                    break;
                case BehaviourStatus.Failure:
                    GUI.color = Color.red;
                    break;
            }
            GUI.Box(Rect, _type.Name, "Button");
            // GUI.Box(Rect, _type.Name);
            GUI.color = color;
            GUI.DrawTexture(InRect, _window._nodePointTexture);
            DrawEndRects();
        }

        private Rect RecalculateFromRect(int index)
        {
            var r = EndRects[index];
            r.center += Vector2.up * Size.y * 0.5f + Rect.center + Vector2.left * 5;
            return r;
        }

        private void DrawEndRects()
        {
            for (int i = 0; i < EndRects.Length; i++)
            {
                GUI.DrawTexture(RecalculateFromRect(i), _window._nodePointTexture);
            }
        }

        public bool ProcessEvent(Event evnt)
        {
            switch (evnt.type)
            {
                case EventType.MouseDown:
                    if (evnt.button == 0)
                    {
                        if (Rect.Contains(evnt.mousePosition))
                        {
                            _click = true;
                        }

                        if (InRect.Contains(evnt.mousePosition))
                        {
                            _window.BeginConnection(null, _node.ID);
                            evnt.Use();
                            return true;
                        }
                        for (int i = 0; i < EndRects.Length; i++)
                        {
                            if (RecalculateFromRect(i).Contains(evnt.mousePosition))
                            {
                                _window.BeginConnection(_node.ID, null, i);
                                evnt.Use();
                                return true;
                            }
                        }
                        if (Rect.Contains(evnt.mousePosition))
                        {
                            _isDragged = true;
                        }
                    }
                    if (evnt.button == 1 && Rect.Contains(evnt.mousePosition))
                    {
                        ShowContextMenu();
                        evnt.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (evnt.button == 0 && _click)
                    {
                        if (Rect.Contains(evnt.mousePosition))
                        {
                            _click = false;
                            _window.NodeClicked(this);
                            evnt.Use();
                        }
                    }
                    if (_window.ConnectionDraw != null)
                    {
                        if (InRect.Contains(evnt.mousePosition) && evnt.button == 0 && !_isDragged)
                        {
                            if (string.IsNullOrEmpty(_window.ConnectionDraw.To))
                            {
                                _window.ConnectionDraw.To = _node.ID;
                                _window.EndConnection();
                                evnt.Use();
                                return true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < EndRects.Length; i++)
                            {
                                if (RecalculateFromRect(i).Contains(evnt.mousePosition))
                                {
                                    if (string.IsNullOrEmpty(_window.ConnectionDraw.From))
                                    {
                                        _window.ConnectionDraw.From = _node.ID;
                                        _window.ConnectionDraw.FromIndex = i;
                                        _window.EndConnection();
                                        evnt.Use();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    if (Rect.Contains(evnt.mousePosition) && _isDragged)
                    {
                        _isDragged = false;
                        evnt.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    _click = false;
                    if (_isDragged)
                    {
                        Drag(evnt.delta);
                        evnt.Use();
                        GUI.changed = true;
                    }

                    break;
            }

            return false;
        }

        private void ShowContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                _window.RemoveNode(_node);
            });
            menu.ShowAsContext();
        }

        public Vector2 GetInPoint()
        {
            return InRect.center;
        }

        public Vector2 GetFromPoint(int fromIndex)
        {
            return RecalculateFromRect(fromIndex).center;
        }
    }
}