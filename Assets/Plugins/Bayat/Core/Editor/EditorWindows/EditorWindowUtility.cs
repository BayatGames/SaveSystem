using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bayat.Core.EditorWindows
{

    public static class EditorWindowUtility
    {

        private static readonly Type ContainerWindowType; // internal sealed class ContainerWindow : ScriptableObject
        private static readonly PropertyInfo ContainerWindow_position; // public Rect ContainerWindow.position;
        private static readonly FieldInfo ContainerWindow_m_ShowMode; // private int ContainerWindow.m_ShowMode;

        static EditorWindowUtility()
        {
            ContainerWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ContainerWindow", true);
            ContainerWindow_m_ShowMode = ContainerWindowType.GetField("m_ShowMode", BindingFlags.Instance | BindingFlags.NonPublic);
            ContainerWindow_position = ContainerWindowType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);
        }

        public static Rect MainEditorWindowPosition
        {
            get
            {
                try
                {
                    var containerWindow = Resources.FindObjectsOfTypeAll(ContainerWindowType).FirstOrDefault(window => (int)ContainerWindow_m_ShowMode.GetValue(window) == 4);

                    if (containerWindow == null)
                    {
                        return new Rect(0, 0, Screen.width, Screen.height);
                    }

                    return (Rect)ContainerWindow_position.GetValue(containerWindow, null);
                }
                catch (Exception ex)
                {
                    throw new UnityEditorInternalException(ex);
                }
            }
        }

    }

}