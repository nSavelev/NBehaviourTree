using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    public interface IPropertyDrawer
    {
        void Draw(ref Rect rect);
        string Name { get; }
    }

    public abstract class BaseNodeParamDrawer : IPropertyDrawer
    {
        protected abstract Type ValueType();
        protected abstract string GetName();
        protected abstract object GetValue();
        protected abstract void SetValue(object value);
        
        public void Draw(ref Rect rect)
        {
            var type = ValueType();
            var value = GetValue();
            var name = GetName();
            if (type == typeof(int))
            {
                var intVal = (int)value;
                intVal = EditorGUI.IntField(rect, name, intVal);
                if (intVal != (int)value)
                {
                    SetValue(intVal);
                }
            }
            if (type == typeof(float))
            {
                var floatVal = (float)value;
                floatVal = EditorGUI.FloatField(rect, name, floatVal);
                if (floatVal != (float)value)
                {
                    SetValue(floatVal);
                }
            }
            if (type == typeof(string) || type == typeof(String))
            {
                var strVal = value?.ToString() ?? string.Empty;
                strVal = EditorGUI.TextField(rect, name, strVal);
                if (strVal != (value?.ToString() ?? string.Empty))
                {
                    SetValue(strVal);
                }
            }

            if (value is Enum)
            {
                var enumVal = (Enum)value;
                var result = EditorGUI.EnumPopup(rect, Name, enumVal);
                if (!Equals(result, enumVal))
                {
                    SetValue(result);
                }
            }
        }

        public string Name => GetName();
    }

    public class NodePropertyDrawer : BaseNodeParamDrawer
    {
        private readonly PropertyInfo _info;
        private readonly object _target;

        public NodePropertyDrawer(PropertyInfo info, object target)
        {
            _info = info;
            _target = target;
        }
        
        protected override Type ValueType()
        {
            return _info.PropertyType;
        }

        protected override string GetName()
        {
            return _info.Name;
        }

        protected override object GetValue()
        {
            return _info.GetValue(_target);
        }

        protected override void SetValue(object value)
        {
            _info.SetValue(_target, value);
        }
    }
    
    public class NodeFieldDrawer : BaseNodeParamDrawer
    {
        private readonly FieldInfo _info;
        private readonly object _target;

        public NodeFieldDrawer(FieldInfo info, object target)
        {
            _info = info;
            _target = target;
        }
        
        protected override Type ValueType()
        {
            return _info.FieldType;
        }

        protected override string GetName()
        {
            return _info.Name;
        }

        protected override object GetValue()
        {
            return _info.GetValue(_target);
        }

        protected override void SetValue(object value)
        {
            _info.SetValue(_target, value);
        }
    }

    public class ObjectDataDrawer : IPropertyDrawer
    {
        private const int ItemHeight = 20;
        private readonly object _target;
        private List<IPropertyDrawer> _drawers = new List<IPropertyDrawer>();

        public ObjectDataDrawer(object target)
        {
            _target = target;
            _drawers.AddRange(GetFields());
            _drawers.AddRange(GetProperties());
            _drawers = _drawers.OrderBy(e => e.Name).ToList();
        }

        private IEnumerable<IPropertyDrawer> GetFields()
        {
            return _target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(e=>new NodeFieldDrawer(e, _target));
        }

        private IEnumerable<IPropertyDrawer> GetProperties()
        {
            return _target.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(e => e.GetMethod == null && e.SetMethod == null)
                .Select(e => new NodePropertyDrawer(e, _target));
        }

        public void Draw(ref Rect rect)
        {
            var drect = rect;
            drect.height = _drawers.Count * ItemHeight + ItemHeight;
            EditorGUI.DrawRect(drect, Color.black);
            var nameRect = new Rect(drect.x, drect.y, drect.width, ItemHeight);
            EditorGUI.LabelField(nameRect, Name);
            for (int i = 0; i < _drawers.Count; i++)
            {
                nameRect.y += ItemHeight;
                _drawers[i].Draw(ref nameRect);
            }
            rect = drect;
        }

        public string Name => _target.GetType().Name;
    }
}