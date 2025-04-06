#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Runtime.Gameplay.Pathfinder.Editor
{
    public class PathfinderBehaviourEditor
    {
        [CustomEditor(typeof(PathfinderBehaviour))]
        public class PathfinderEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                PathfinderBehaviour pathfinder2D = (PathfinderBehaviour)target;

                GUILayout.Space(10);

                if (GUILayout.Button(pathfinder2D._drawGridInEditor ? "Hide Grid" : "Show Grid"))
                {
                    pathfinder2D._drawGridInEditor = !pathfinder2D._drawGridInEditor;
                    EditorUtility.SetDirty(pathfinder2D);
                }

                if (GUILayout.Button("Rebuild Grid"))
                {
                    pathfinder2D.CreateGrid();
                    EditorUtility.SetDirty(pathfinder2D);
                }
            }
        }
    }
}
#endif
