using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "新しいDialogue", menuName = Defines.ProjectMenuName + "/Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeLookup = new();
        private readonly Vector2 newNodeOffset = new Vector2(265f, 0f);

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in nodes)
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }
        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public bool TryGetNodeAtPosition(Vector2 position, out DialogueNode dialogueNode)
        {
            dialogueNode = null;
            foreach (DialogueNode node in nodes)
            {
                if (node.GetRect().Contains(position))
                {
                    dialogueNode = node;
                }
            }
            return dialogueNode != null;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parent)
        {
            List<string> childrenIDs = parent.GetChildren();
            if (childrenIDs != null)
            {
                foreach (string childID in childrenIDs)
                {
                    if (nodeLookup.ContainsKey(childID))
                    {
                        yield return nodeLookup[childID];
                    }
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "ノードの削除");

            nodes.Remove(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parentNode.IsPlayerSpeaking());

                Rect parentRect = parentNode.GetRect();
                newNode.SetRectPosition(newNodeOffset + parentRect.position);
            }
            return newNode;
        }
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }
        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }


#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                AddNode(MakeNode(null));
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
