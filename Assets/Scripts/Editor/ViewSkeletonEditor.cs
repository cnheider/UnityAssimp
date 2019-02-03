using UnityEditor;
using UnityEngine;

namespace Editor {
    
    [CustomEditor(typeof(ViewSkeleton))]
    public class ViewSkeletonEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var myTarget = (ViewSkeleton)this.target;

            EditorGUILayout.BeginHorizontal("Box");
            myTarget.rootNode = (Transform)EditorGUILayout.ObjectField(myTarget.rootNode, typeof(Transform));

            if (myTarget != null) {
                if (GUILayout.Button("PopulateChildren")) {
                    myTarget.PopulateChildren();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }

}