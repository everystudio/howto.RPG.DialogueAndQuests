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
        private GUIStyle playerNodeStyle;
        private DialogueNode draggingNode = null;
        private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;

        const float canvasSize = 4000f;
        const float backgroundSize = 50f;

        [MenuItem("Window/ダイアログエディタ")]
        public static void ShowEditorWindow()
        {
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

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);


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
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("選択されてません");
                return;
            }

            ProcessEvents();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            Rect canvasRect = GUILayoutUtility.GetRect(canvasSize, canvasSize);
            Texture2D backgroundTex = Resources.Load("background") as Texture2D;
            Rect texCoords = new Rect(0f, 0f, canvasSize / backgroundSize, canvasSize / backgroundSize);
            GUI.DrawTextureWithTexCoords(canvasRect, backgroundTex, texCoords);

            foreach (var node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
                DrawConnections(node);
            }
            EditorGUILayout.EndScrollView();


            if (creatingNode != null)
            {
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }

            if (deletingNode != null)
            {
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                Vector2 mousePosition = Event.current.mousePosition + scrollPosition;
                if (selectedDialogue.TryGetNodeAtPosition(mousePosition, out draggingNode))
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = mousePosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);

            EditorGUILayout.LabelField($"Node:", EditorStyles.whiteLabel);
            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        // ノードや線の描画順が気になる場合は先にDrawConnectionsを呼んだ後にDrawNodeしてください
        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Rect nodeRect = node.GetRect();
                Rect childNodeRect = childNode.GetRect();

                Vector2 startPosition = new Vector2(nodeRect.xMax, nodeRect.center.y);
                Vector2 endPosition = new Vector2(childNodeRect.xMin, childNodeRect.center.y);

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