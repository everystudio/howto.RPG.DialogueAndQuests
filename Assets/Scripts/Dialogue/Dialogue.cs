using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "新しいDialogue", menuName = Defines.ProjectMenuName + "/Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private DialogueNode[] nodes;


}
