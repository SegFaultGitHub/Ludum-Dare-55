using UnityEditor;
using UnityEngine;

namespace Code.Utils {
    #if UNITY_EDITOR
    public static class CopyPathMenuItem {
        [MenuItem("GameObject/Copy Path")]
        private static void CopyPath() {
            GameObject obj = Selection.activeGameObject;

            if (obj == null) return;

            string path = obj.name;

            while (obj.transform.parent != null) {
                obj = obj.transform.parent.gameObject;
                path = $"{obj.name}/{path}";
            }

            EditorGUIUtility.systemCopyBuffer = path;
        }

        [MenuItem("GameObject/2D Object/Copy Path", true)]
        private static bool CopyValidation() =>
            // We can only copy the path in case 1 object is selected
            Selection.gameObjects.Length == 1;
    }
    #endif
}
