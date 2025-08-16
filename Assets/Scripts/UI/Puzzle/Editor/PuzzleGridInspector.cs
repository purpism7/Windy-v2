using Cysharp.Threading.Tasks;
using UI.Puzzle;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleGrid))]
public class PuzzleGridInspector : Editor
{
   public override void OnInspectorGUI()
   {
       base.OnInspectorGUI();

       var puzzleGrid = target as PuzzleGrid;
       if (puzzleGrid == null)
           return;

       EditorGUILayout.BeginVertical();
       if (GUILayout.Button("Create"))
       {
           puzzleGrid.CreateAsync().Forget();
       }
       
       if (GUILayout.Button("Remove All"))
       {
           puzzleGrid.RemoveAllChild();
       }
       EditorGUILayout.EndVertical();
   }
}
