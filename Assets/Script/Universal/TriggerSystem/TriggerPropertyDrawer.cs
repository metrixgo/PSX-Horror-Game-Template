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
                SerializedProperty displayDialogueSpeaker = property.FindPropertyRelative("displayDialogueSpeaker");
                EditorGUI.PropertyField(rect, displayDialogueSpeaker, new GUIContent("Speaker"));
                rect.y += gap;
                SerializedProperty displayDialogueContent = property.FindPropertyRelative("displayDialogueContent");
                EditorGUI.PropertyField(rect, displayDialogueContent, new GUIContent("Content"));
                rect.y += gap;
                SerializedProperty displayDialogueSub = property.FindPropertyRelative("displayDialogueSub");
                EditorGUI.PropertyField(rect, displayDialogueSub, new GUIContent("Sub"));
                rect.y += gap;
                SerializedProperty displayDialogueFlash = property.FindPropertyRelative("displayDialogueFlash");
                EditorGUI.PropertyField(rect, displayDialogueFlash, new GUIContent("Flash"));
                rect.y += gap;

                if (!displayDialogueFlash.boolValue)
                {
                    SerializedProperty displayDialogueSkippable = property.FindPropertyRelative("displayDialogueSkippable");
                    EditorGUI.PropertyField(rect, displayDialogueSkippable, new GUIContent("Skippable"));
                    rect.y += gap;
                }
                else
                {
                    SerializedProperty displayDialogueFlashLength = property.FindPropertyRelative("displayDialogueFlashLength");
                    EditorGUI.PropertyField(rect, displayDialogueFlashLength, new GUIContent("Flash Length"));
                    rect.y += gap;
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

                if ((ManageTasksType)manageTasksType.enumValueIndex != ManageTasksType.ClearTasks)
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

            case TriggerType.JumpscareAt:
                SerializedProperty jumpscareAtPosition = property.FindPropertyRelative("jumpscareAtPosition");
                EditorGUI.PropertyField(rect, jumpscareAtPosition, new GUIContent("Position"));
                rect.y += gap;
                SerializedProperty jumpscareAtLength = property.FindPropertyRelative("jumpscareAtLength");
                EditorGUI.PropertyField(rect, jumpscareAtLength, new GUIContent("Length"));
                rect.y += gap;
                SerializedProperty jumpscareAtEffect = property.FindPropertyRelative("jumpscareAtEffect");
                EditorGUI.PropertyField(rect, jumpscareAtEffect, new GUIContent("Effect"));
                rect.y += gap;
                break;

            case TriggerType.DisplayCanvas:
                SerializedProperty displayCanvasCanvas = property.FindPropertyRelative("displayCanvasCanvas");
                EditorGUI.PropertyField(rect, displayCanvasCanvas, new GUIContent("Canvas"));
                rect.y += gap;
                SerializedProperty displayCanvasEffect = property.FindPropertyRelative("displayCanvasEffect");
                EditorGUI.PropertyField(rect, displayCanvasEffect, new GUIContent("Effect"));
                rect.y += gap;
                SerializedProperty displayCanvasFlash = property.FindPropertyRelative("displayCanvasFlash");
                EditorGUI.PropertyField(rect, displayCanvasFlash, new GUIContent("Flash"));
                rect.y += gap;

                if (displayCanvasFlash.boolValue)
                {
                    SerializedProperty displayCanvasFlashLength = property.FindPropertyRelative("displayCanvasFlashLength");
                    EditorGUI.PropertyField(rect, displayCanvasFlashLength, new GUIContent("Flash Length"));
                    rect.y += gap;
                }

                break;

            case TriggerType.PlaySound:
                SerializedProperty playSoundSound = property.FindPropertyRelative("playSoundSound");
                EditorGUI.PropertyField(rect, playSoundSound, new GUIContent("Sound"));
                rect.y += gap;
                SerializedProperty playSoundLocal = property.FindPropertyRelative("playSoundLocal");
                EditorGUI.PropertyField(rect, playSoundLocal, new GUIContent("Local"));
                rect.y += gap;
                if (!playSoundLocal.boolValue)
                {
                    SerializedProperty playSoundIsEffect = property.FindPropertyRelative("playSoundIsEffect");
                    EditorGUI.PropertyField(rect, playSoundIsEffect, new GUIContent("IsEffect"));
                    rect.y += gap;
                }
                else
                {
                    SerializedProperty playSoundSource = property.FindPropertyRelative("playSoundSource");
                    EditorGUI.PropertyField(rect, playSoundSource, new GUIContent("Source"));
                    rect.y += gap;
                }
                break;

            case TriggerType.SetObject:
                SerializedProperty setObjectObject = property.FindPropertyRelative("setObjectObject");
                EditorGUI.PropertyField(rect, setObjectObject, new GUIContent("Object"));
                rect.y += gap;
                SerializedProperty setObjectSetActive = property.FindPropertyRelative("setObjectSetActive");
                EditorGUI.PropertyField(rect, setObjectSetActive, new GUIContent("Set Active"));
                rect.y += gap;
                break;

            case TriggerType.LoadScene:
                SerializedProperty loadSceneScene = property.FindPropertyRelative("loadSceneScene");
                EditorGUI.PropertyField(rect, loadSceneScene, new GUIContent("Scene"));
                rect.y += gap;
                SerializedProperty loadSceneLength = property.FindPropertyRelative("loadSceneLength");
                EditorGUI.PropertyField(rect, loadSceneLength, new GUIContent("Length"));
                rect.y += gap;
                SerializedProperty loadSceneSave = property.FindPropertyRelative("loadSceneSave");
                EditorGUI.PropertyField(rect, loadSceneSave, new GUIContent("Save"));
                rect.y += gap;
                break;

            case TriggerType.DisplayEnding:
                SerializedProperty displayEndingTitle = property.FindPropertyRelative("displayEndingTitle");
                EditorGUI.PropertyField(rect, displayEndingTitle, new GUIContent("Title"));
                rect.y += gap;
                SerializedProperty displayEndingDescription = property.FindPropertyRelative("displayEndingDescription");
                EditorGUI.PropertyField(rect, displayEndingDescription, new GUIContent("Description"));
                rect.y += gap;
                break;

            case TriggerType.Custom:
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
                return gap * 6;

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

            case TriggerType.JumpscareAt:
                return gap * 4;

            case TriggerType.DisplayCanvas:
                SerializedProperty displayCanvasFlash = property.FindPropertyRelative("displayCanvasFlash");
                return gap * (displayCanvasFlash.boolValue ? 5 : 4);

            case TriggerType.PlaySound:
                return gap * 4;

            case TriggerType.SetObject:
                return gap * 3;

            case TriggerType.LoadScene:
                return gap * 3;

            case TriggerType.DisplayEnding:
                return gap * 3;

            case TriggerType.Custom:
                return gap;

            default:
                return gap;
        }
    }
}
#endif