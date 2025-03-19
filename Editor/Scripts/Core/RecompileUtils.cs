using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Compilation;
using System;
using System.Reflection;
using UnityEngine;

namespace Trackman.Editor.Core
{
    public static class RecompileUtils
    {
        #region Methods
        [MenuItem("Trackman/Recompile scripts", false, (int)MenuOrder.Functions)]
        public static void RecompileScripts() => CompilationPipeline.RequestScriptCompilation();
        [MenuItem("Trackman/Recompile all scripts", false, (int)MenuOrder.Functions + 1)]
        public static void RecompileAllScripts() => CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        [MenuItem("Trackman/Unlock assemblies reload", false, (int)MenuOrder.Functions + 2)]
        public static void UnlockAssembliesReload() => EditorApplication.UnlockReloadAssemblies();

        [UsedImplicitly]
        public static void SyncSolution()
        {
            Debug.Log($"[{nameof(RecompileUtils)}] Starting solution sync...");

            // enum ProjectGenerationFlag
            // None = 0,
            // Embedded = 1,
            // Local = 2,
            int oldValue = EditorPrefs.GetInt("unity_project_generation_flag", 3);
            EditorPrefs.SetInt("unity_project_generation_flag", 3);

            try
            {
                Type.GetType("Packages.Rider.Editor.RiderScriptEditor, Unity.Rider.Editor").GetMethod("SyncSolution", BindingFlags.Static | BindingFlags.Public).Invoke(null, Array.Empty<object>());
                Debug.Log($"[{nameof(RecompileUtils)}] Solution sync done");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                EditorPrefs.SetInt("unity_project_generation_flag", oldValue);
                EditorApplication.Exit(0);
            }
        }
        #endregion
    }
}