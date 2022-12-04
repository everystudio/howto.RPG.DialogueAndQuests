using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0f, 0f, 200f, 100f);

        public bool IsPlayerSpeaking() { return isPlayerSpeaking; }
        public Rect GetRect()
        {
            return rect;
        }

        public string GetText() { return text; }
        public List<string> GetChildren() { return children; }

#if UNITY_EDITOR
        public void SetRectPosition(Vector2 position)
        {
            Undo.RecordObject(this, "ノードの移動");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (text != newText)
            {
                Undo.RecordObject(this, $"Update Dialogue Data");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "ノードのリンクセット");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "リンクの解除");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        public void SetPlayerSpeaking(bool isPlayerSpeaking)
        {
            if (this.isPlayerSpeaking != isPlayerSpeaking)
            {
                Undo.RecordObject(this, "ダイアログスピーカーの変更");
                this.isPlayerSpeaking = isPlayerSpeaking;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
