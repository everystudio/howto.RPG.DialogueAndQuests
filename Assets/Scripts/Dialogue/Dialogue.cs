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

#if UNITY_EDITOR
        private void Awake()
        {
        }
#endif
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
                if (node.rect.Contains(position))
                {
                    dialogueNode = node;
                }
            }
            return dialogueNode != null;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parent)
        {
            if (parent.children != null)
            {
                foreach (string childID in parent.children)
                {
                    if (nodeLookup.ContainsKey(childID))
                    {
                        yield return nodeLookup[childID];
                    }
                }
            }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (parentNode != null)
            {
                parentNode.children.Add(newNode.name);
            }
            nodes.Add(newNode);

            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);

        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if (nodes.Count == 0)
            {
                CreateNode(null);
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
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
