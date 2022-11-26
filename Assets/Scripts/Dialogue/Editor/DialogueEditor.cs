using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue.Editor
{

    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/ダイアログエディタ")]
        public static void ShowEditorWindow()
        {
            //Debug.Log("ShowEditorWindow");
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
    }
}