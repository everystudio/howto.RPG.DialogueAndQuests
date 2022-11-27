using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{

    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue;
        private GUIStyle nodeStyle;

        [MenuItem("Window/ダイアログエディタ")]
        public static void ShowEditorWindow()
        {
            //Debug.Log("ShowEditorWindow");
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                //Debug.Log("OpenDialogue");
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            //Debug.Log("選択が変更されました");
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            //Debug.Log("ongui");

            if (selectedDialogue != null)
            {
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    OnGUINode(node);
                }

            }
            else
            {
                EditorGUILayout.LabelField("選択されてません");
            }

        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.position, nodeStyle);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField($"Node:", EditorStyles.whiteLabel);

            string newText = EditorGUILayout.TextField(node.text);
            string newUniqueID = EditorGUILayout.TextField(node.uniqueID);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, $"Update Dialogue Data");

                node.text = newText;
                node.uniqueID = newUniqueID;
            }
            GUILayout.EndArea();
        }
    }
}