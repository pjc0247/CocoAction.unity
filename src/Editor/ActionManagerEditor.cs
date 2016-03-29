using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace CocoAction.Editor
{
    [CustomEditor(typeof(ActionManager))]
    public class ActionManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var actionManager = ActionManager.instance;

            EditorGUILayout.LabelField("CocoAction.ActionManager");
            EditorGUILayout.LabelField("Running Action(s) : " + actionManager.runningActions.ToString());
        }
    }
}
