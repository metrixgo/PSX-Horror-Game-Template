using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Trigger))]
public class TriggerPropertyDrawer : PropertyDrawer
{
    private float gap = EditorGUIUtility.singleLineHeight + 2.0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty triggerType = property.FindPropertyRelative("triggerType");
        EditorGUI.PropertyField(rect, triggerType);

        rect.y += gap;

        switch ((TriggerType)triggerType.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                SerializedProperty displayDialogueType = property.FindPropertyRelative("displayDialogueType");
                EditorGUI.PropertyField(rect, displayDialogueType, new GUIContent("Type"));
                rect.y += gap;
                SerializedProperty displayDialogueSpeaker = property.FindPropertyRelative("displayDialogueSpeaker");
                EditorGUI.PropertyField(rect, displayDialogueSpeaker, new GUIContent("Speaker"));
                rect.y += gap;
                SerializedProperty displayDialogueContent = property.FindPropertyRelative("displayDialogueContent");
                EditorGUI.PropertyField(rect, displayDialogueContent, new GUIContent("Content"));
                rect.y += gap;

                if ((DisplayDialogueType)displayDialogueType.enumValueIndex == DisplayDialogueType.Main || 
                    (DisplayDialogueType)displayDialogueType.enumValueIndex == DisplayDialogueType.Sub)
                {
                    SerializedProperty displayDialogueSkippable = property.FindPropertyRelative("displayDialogueSkippable");
                    EditorGUI.PropertyField(rect, displayDialogueSkippable, new GUIContent("Skippable"));
                    rect.y += gap;
                }
                else if ((DisplayDialogueType)displayDialogueType.enumValueIndex == DisplayDialogueType.FlashMain ||
                    (DisplayDialogueType)displayDialogueType.enumValueIndex == DisplayDialogueType.FlashSub)
                {
                    SerializedProperty displayDialogueFlashLength = property.FindPropertyRelative("displayDialogueFlashLength");
                    EditorGUI.PropertyField(rect, displayDialogueFlashLength, new GUIContent("Flash Length"));
                    rect.y += gap;
                }
                else
                {
                    Debug.LogWarning("Unimplemented Display Dialogue Type: " + (DisplayDialogueType)displayDialogueType.enumValueIndex);
                }
                break;

            case TriggerType.ChangeScreen:
                SerializedProperty changeScreenStartColor = property.FindPropertyRelative("changeScreenStartColor");
                EditorGUI.PropertyField(rect, changeScreenStartColor, new GUIContent("Start Color"));
                rect.y += gap;
                SerializedProperty changeScreenEndColor = property.FindPropertyRelative("changeScreenEndColor");
                EditorGUI.PropertyField(rect, changeScreenEndColor, new GUIContent("End Color"));
                rect.y += gap;
                SerializedProperty changeScreenLength = property.FindPropertyRelative("changeScreenLength");
                EditorGUI.PropertyField(rect, changeScreenLength, new GUIContent("Length"));
                rect.y += gap;
                SerializedProperty changeScreenSub = property.FindPropertyRelative("changeScreenSub");
                EditorGUI.PropertyField(rect, changeScreenSub, new GUIContent("Sub"));
                rect.y += gap;
                SerializedProperty changeScreenFlash = property.FindPropertyRelative("changeScreenFlash");
                EditorGUI.PropertyField(rect, changeScreenFlash, new GUIContent("Flash"));
                rect.y += gap;
                break;

            case TriggerType.Wait:
                SerializedProperty waitLength = property.FindPropertyRelative("waitLength");
                EditorGUI.PropertyField(rect, waitLength, new GUIContent("Length"));
                rect.y += gap;
                break;

            case TriggerType.DisplayPrompt:
                SerializedProperty displayPromptPrompt = property.FindPropertyRelative("displayPromptPrompt");
                EditorGUI.PropertyField(rect, displayPromptPrompt, new GUIContent("Prompt"));
                rect.y += gap;
                SerializedProperty displayPromptColor = property.FindPropertyRelative("displayPromptColor");
                EditorGUI.PropertyField(rect, displayPromptColor, new GUIContent("Color"));
                rect.y += gap;
                SerializedProperty displayPromptSub = property.FindPropertyRelative("displayPromptSub");
                EditorGUI.PropertyField(rect, displayPromptSub, new GUIContent("Sub"));
                rect.y += gap;
                SerializedProperty displayPromptFlash = property.FindPropertyRelative("displayPromptFlash");
                EditorGUI.PropertyField(rect, displayPromptFlash, new GUIContent("Flash"));
                rect.y += gap;
                break;

            case TriggerType.ManageTasks:
                SerializedProperty manageTasksType = property.FindPropertyRelative("manageTasksType");
                EditorGUI.PropertyField(rect, manageTasksType, new GUIContent("Type"));
                rect.y += gap;

                if((ManageTasksType)manageTasksType.enumValueIndex != ManageTasksType.ClearTasks)
                {
                    SerializedProperty manageTasksTask = property.FindPropertyRelative("manageTasksTask");
                    EditorGUI.PropertyField(rect, manageTasksTask, new GUIContent("Task"));
                    rect.y += gap;
                }
                break;

            case TriggerType.PlayerCanDo:
                SerializedProperty playerCanDoType = property.FindPropertyRelative("playerCanDoType");
                EditorGUI.PropertyField(rect, playerCanDoType, new GUIContent("Type"));
                rect.y += gap;
                SerializedProperty playerCanDoCanDo = property.FindPropertyRelative("playerCanDoCanDo");
                EditorGUI.PropertyField(rect, playerCanDoCanDo, new GUIContent("Can Do"));
                rect.y += gap;
                break;

            case TriggerType.MovePlayer:
                SerializedProperty movePlayerType = property.FindPropertyRelative("movePlayerType");
                EditorGUI.PropertyField(rect, movePlayerType, new GUIContent("Type"));
                rect.y += gap;
                SerializedProperty movePlayerVector = property.FindPropertyRelative("movePlayerVector");
                EditorGUI.PropertyField(rect, movePlayerVector, new GUIContent("Vector"));
                rect.y += gap;
                break;

            default:
                Debug.LogWarning("Unimplemented Trigger Type: " + (TriggerType)triggerType.enumValueIndex);
                break;
        }

        EditorGUI.EndProperty();
    }
    

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty triggerType = property.FindPropertyRelative("triggerType");

        switch ((TriggerType)triggerType.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                return gap * 5;

            case TriggerType.ChangeScreen:
                return gap * 6;

            case TriggerType.Wait:
                return gap * 2;

            case TriggerType.DisplayPrompt:
                return gap * 5;

            case TriggerType.ManageTasks:
                SerializedProperty manageTasksType = property.FindPropertyRelative("manageTasksType");
                return gap * ((ManageTasksType)manageTasksType.enumValueIndex == ManageTasksType.ClearTasks ? 2 : 3);

            case TriggerType.PlayerCanDo:
                return gap * 3;

            case TriggerType.MovePlayer:
                return gap * 3;

            default:
                return gap;
        }
    }
}
#endif