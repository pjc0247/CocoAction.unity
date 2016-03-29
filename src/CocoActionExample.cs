using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

namespace CocoAction.Examples
{
    public class CocoActionExamples
    {
        private Vector2 scrollPosition = new Vector2(0, 0);
        private GameObject testObject { get; set; }

        void Start()
        {
            testObject = new GameObject("TestObject");
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

            var methods = 
                typeof(CocoActionExamples).GetMethods(BindingFlags.Public | BindingFlags.Instance);

            GUILayout.BeginScrollView(scrollPosition);

            foreach(var method in methods)
            {
                if (GUILayout.Button(method.Name))
                {
                    Reset();
                    method.Invoke(this, new object[] { });
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void Reset()
        {
            testObject.transform.position = new Vector3(0, 0, 0);
        }
        public void _MoveBy()
        {
            testObject.RunAction(
                MoveBy.Create(2.0f, 100, 100));
        }
        public void _MoveTo()
        {
            testObject.RunAction(
                MoveTo.Create(2.0f, -100, -100));
        }
    }
}
