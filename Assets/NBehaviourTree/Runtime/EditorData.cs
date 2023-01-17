using System;
using Newtonsoft.Json;
using UnityEngine;

namespace NBehaviourTree.Runtime
{
    [Serializable]
    public class EditorData
    {
        public string Id;

        // [JsonIgnore]
        public UnityEngine.Vector2 Position;

        // private Vector2Holder _position;
    }
}