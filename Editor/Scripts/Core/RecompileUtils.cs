using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Compilation;
using System;
using System.Reflection;

namespace Trackman.Editor.Core
{
    public static class RecompileUtils
    {
        #region Methods
        [UsedImplicitly] public static void SyncSolution()
        {
            // enum ProjectGenerationFlag
            // None = 0,
            // Embedded = 1,
            // Local = 2,
            int oldValue = EditorPrefs.GetInt("unity_project_generation_flag", 3);
            EditorPrefs.SetInt("unity_project_generation_flag", 3);
            Type.GetType("Packages.Rider.Editor.RiderScriptEditor, Unity.Rider.Editor").GetMethod("SyncSolution", BindingFlags.Static | BindingFlags.Public).Invoke(null, Array.Empty<object>());
            EditorPrefs.SetInt("unity_project_generation_flag", oldValue);
        }
        #endregion
    }
}
