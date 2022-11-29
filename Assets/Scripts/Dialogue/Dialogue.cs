using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "新しいDialogue", menuName = Defines.ProjectMenuName + "/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes;

        private Dictionary<string, DialogueNode> nodeLookup = new();

#if UNITY_EDITOR
        private void Awake()
        {
            Debug.Log("Awake from " + name);
            if (nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());
            }
        }
#endif
        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in nodes)
            {
                nodeLookup[node.uniqueID] = node;
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
            foreach (string childID in parent.children)
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }
    }
}
