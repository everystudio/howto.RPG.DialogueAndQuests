using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "新しいDialogue", menuName = Defines.ProjectMenuName + "/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes;

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
    }
}
