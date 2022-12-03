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
        private DialogueNode draggingNode = null;
        private Vector2 draggingOffset;
        private DialogueNode creatingNode = null;

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
            if (selectedDialogue != null)
            {
                ProcessEvents();
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                    DrawConnections(node);
                }
            }
            else
            {
                EditorGUILayout.LabelField("選択されてません");
            }

            if (creatingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "ノードの追加");
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                if (selectedDialogue.TryGetNodeAtPosition(Event.current.mousePosition, out draggingNode))
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "ノードの移動");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField($"Node:", EditorStyles.whiteLabel);

            string newText = EditorGUILayout.TextField(node.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, $"Update Dialogue Data");

                node.text = newText;
            }

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndArea();
        }

        // ノードや線の描画順が気になる場合は先にDrawConnectionsを呼んだ後にDrawNodeしてください
        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector2 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
                Vector2 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);

                Vector2 offset = (endPosition - startPosition) * 0.5f;
                offset.y = 0;

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + offset,
                    endPosition - offset, Color.white, null, 4f);
            }
        }

    }
}