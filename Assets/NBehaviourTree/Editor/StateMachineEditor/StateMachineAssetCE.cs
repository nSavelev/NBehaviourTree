using NBehaviourTree.Runtime;
using UnityEditor;
using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    [CustomEditor(typeof(StateMachineAsset))]
    public class StateMachineAssetCE : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Edit"))
            {
                NBehaviorEditorWindow.Create(target as StateMachineAsset);
            }
        }
    }
}