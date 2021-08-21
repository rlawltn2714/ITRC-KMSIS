

namespace TriLibCore.Editor
{
#if !TRILIB_DISABLE_EDITOR_GLTF_IMPORT
    [UnityEditor.AssetImporters.ScriptedImporter(2, new[] { "gltf", "glb"})]
#endif
    public class TriLibGLTFScriptedImporter : TriLibScriptedImporter
    {

    }
}