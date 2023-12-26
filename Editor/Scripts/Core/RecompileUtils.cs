using UnityEditor;
using UnityEditor.Compilation;

namespace Trackman.Editor.Core
{
    public static class RecompileUtils
    {
        #region Methods
        [MenuItem("Trackman/Recompile scripts %r", false, (int)MenuOrder.Functions)]
        public static void RecompileScripts() => CompilationPipeline.RequestScriptCompilation();
        [MenuItem("Trackman/Recompile all scripts %&r", false, (int)MenuOrder.Functions + 1)]
        public static void RecompileAllScripts() => CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        #endregion
    }
}
