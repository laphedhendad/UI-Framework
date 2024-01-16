using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor
{
    [CustomEditor(typeof(MonoWindow))]
    public class WindowCustomInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MonoWindow window = (MonoWindow)target;
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Find subwindows automatically"))
            {
                FindSubwindows(window);
                GUIUtility.ExitGUI();
            }
        }

        private void FindSubwindows(MonoWindow window)
        {
            List<MonoWindow> subwindows = window.GetComponentsInChildren<MonoWindow>(true).ToList();
            subwindows.RemoveAt(0);
            foreach (MonoWindow subwindow in subwindows)
            {
                FindSubwindows(subwindow);
            }
            window.SetSubwindows(subwindows.ToArray());
        }
    }
}