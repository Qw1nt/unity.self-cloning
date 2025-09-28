using System;
using NUnit.Framework.Interfaces;
using Runtime.CustomTypeTest;
using SelfCloning.Attributes;
using UnityEngine;

namespace Runtime
{
    [Serializable]
    [SelfCloneable]
    public partial class SelfCloneTest2 : ITE
    {
        [SerializeField] private int _field1;
        [SerializeField] private int _field2;
        [SerializeField] private bool _field3;
        [SerializeField] private MyCustomType _field4;

        private int f;
        private int ss;
        
        [field:SerializeField] public int Fifof { get; set; }
        
        public int Field1 => _field1;
    }
}