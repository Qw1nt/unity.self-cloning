using System;
using Runtime.CustomTypeTest;
using SelfCloning.Attributes;
using UnityEngine;

namespace Runtime
{
    [Serializable]
    [SelfCloneable]
    public partial class SelfCloneTest
    {
        [SerializeField] private int _field1;
        [SerializeField] private int _field2;
        [SerializeField] private bool _field3;
        [SerializeField] private MyCustomType _field4;

        [field:SerializeField] public int Fifof { get; set; }
        
        public int Field1 => _field1;
    }
}