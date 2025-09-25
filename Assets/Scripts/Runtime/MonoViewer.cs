using System;
using UnityEngine;

namespace Runtime
{
    public class MonoViewer : MonoBehaviour
    {
        [SerializeField] private SelfCloneTest _test;

        private void Awake()
        {
            var t2 = _test.MakeTypedClone();
            Debug.Log(_test.Field1);
            Debug.Log(t2.Field1);
        }
    }
}