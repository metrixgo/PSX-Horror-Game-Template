#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Trigger))]
public class TriggerPropertyDrawer : PropertyDrawer
{
    private float gap = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        SerializedProperty triggerType = property.FindPropertyRelative("triggerType");
        DrawProperty(ref rect, property, "triggerType", "Trigger Type");

        switch ((TriggerType)triggerType.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                DrawProperty(ref rect, property, "displayDialogueSpeaker", "Speaker");
                DrawProperty(ref rect, property, "displayDialogueContent", "Content");
                DrawProperty(ref rect, property, "displayDialogueSpeakerColor", "Speaker Color");
                DrawProperty(ref rect, property, "displayDialogueContentColor", "Content Color");
                DrawProperty(ref rect, property, "displayDialogueSub", "Sub");
                SerializedProperty displayDialogueFlash = DrawProperty(ref rect, property, "displayDialogueFlash", "Flash");
                if (!displayDialogueFlash.boolValue)
                    DrawProperty(ref rect, property, "displayDialogueSkippable", "Skippable");
                else
                    DrawProperty(ref rect, property, "displayDialogueFlashLength", "Flash Length");
                break;

            case TriggerType.ChangeScreen:
                DrawProperty(ref rect, property, "changeScreenStartColor", "Start Color");
                DrawProperty(ref rect, property, "changeScreenEndColor", "End Color");
                DrawProperty(ref rect, property, "changeScreenLength", "Length");
                DrawProperty(ref rect, property, "changeScreenSub", "Sub");
                DrawProperty(ref rect, property, "changeScreenFlash", "Flash");
                break;

            case TriggerType.Wait:
                DrawProperty(ref rect, property, "waitLength", "Length");
                break;

            case TriggerType.DisplayPrompt:
                DrawProperty(ref rect, property, "displayPromptPrompt", "Prompt");
                DrawProperty(ref rect, property, "displayPromptColor", "Color");
                DrawProperty(ref rect, property, "displayPromptSub", "Sub");
                DrawProperty(ref rect, property, "displayPromptFlash", "Flash");
                break;

            case TriggerType.ManageTasks:
                SerializedProperty manageTasksType = DrawProperty(ref rect, property, "manageTasksType", "Type");
                if ((ManageTasksType)manageTasksType.enumValueIndex != ManageTasksType.ClearTasks)
                    DrawProperty(ref rect, property, "manageTasksTask", "Task");
                break;

            case TriggerType.PlayerCanDo:
                DrawProperty(ref rect, property, "playerCanDoType", "Type");
                DrawProperty(ref rect, property, "playerCanDoCanDo", "Can Do");
                break;

            case TriggerType.MovePlayer:
                DrawProperty(ref rect, property, "movePlayerType", "Type");
                DrawProperty(ref rect, property, "movePlayerVector", "Vector");
                break;

            case TriggerType.JumpscareAt:
                DrawProperty(ref rect, property, "jumpscareAtObject", "Object");
                DrawProperty(ref rect, property, "jumpscareAtLength", "Length");
                DrawProperty(ref rect, property, "jumpscareAtEffect", "Effect");
                break;

            case TriggerType.DisplayCanvas:
                DrawProperty(ref rect, property, "displayCanvasCanvas", "Canvas");
                DrawProperty(ref rect, property, "displayCanvasEffect", "Effect");
                SerializedProperty displayCanvasFlash = DrawProperty(ref rect, property, "displayCanvasFlash", "Flash");
                if (displayCanvasFlash.boolValue)
                    DrawProperty(ref rect, property, "displayCanvasFlashLength", "Flash Length");
                break;

            case TriggerType.PlaySound:
                DrawProperty(ref rect, property, "playSoundSound", "Sound");
                SerializedProperty playSoundLocal = DrawProperty(ref rect, property, "playSoundLocal", "Local");
                if (!playSoundLocal.boolValue)
                    DrawProperty(ref rect, property, "playSoundIsEffect", "Is Effect");
                else
                    DrawProperty(ref rect, property, "playSoundSource", "Source");
                break;

            case TriggerType.SetObject:
                DrawProperty(ref rect, property, "setObjectObject", "Object");
                DrawProperty(ref rect, property, "setObjectSetActive", "Set Active");
                break;

            case TriggerType.LoadScene:
                DrawProperty(ref rect, property, "loadSceneScene", "Scene");
                DrawProperty(ref rect, property, "loadSceneLength", "Length");
                DrawProperty(ref rect, property, "loadSceneSave", "Save");
                break;

            case TriggerType.DisplayEnding:
                DrawProperty(ref rect, property, "displayEndingTitle", "Title");
                DrawProperty(ref rect, property, "displayEndingDescription", "Description");
                break;

            case TriggerType.Custom:
                break;

            default:
                Debug.LogWarning("Unimplemented Trigger Type: " + (TriggerType)triggerType.enumValueIndex);
                break;
        }

        EditorGUI.EndProperty();
    }

    private SerializedProperty DrawProperty(ref Rect rect, SerializedProperty property, string name, string label)
    {
        SerializedProperty prop = property.FindPropertyRelative(name);
        rect.height = EditorGUI.GetPropertyHeight(prop, new GUIContent(label));
        EditorGUI.PropertyField(rect, prop, new GUIContent(label));
        rect.y += rect.height + gap;

        return prop;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0;

        SerializedProperty triggerType = property.FindPropertyRelative("triggerType");
        AddHeightOfProperty(ref height, property, "triggerType");

        switch ((TriggerType)triggerType.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                AddHeightOfProperty(ref height, property, "displayDialogueSpeaker");
                AddHeightOfProperty(ref height, property, "displayDialogueContent");
                AddHeightOfProperty(ref height, property, "displayDialogueSpeakerColor");
                AddHeightOfProperty(ref height, property, "displayDialogueContentColor");
                AddHeightOfProperty(ref height, property, "displayDialogueSub");
                SerializedProperty displayDialogueFlash = AddHeightOfProperty(ref height, property, "displayDialogueFlash");
                if (!displayDialogueFlash.boolValue)
                    AddHeightOfProperty(ref height, property, "displayDialogueSkippable");
                else
                    AddHeightOfProperty(ref height, property, "displayDialogueFlashLength");
                break;

            case TriggerType.ChangeScreen:
                AddHeightOfProperty(ref height, property, "changeScreenStartColor");
                AddHeightOfProperty(ref height, property, "changeScreenEndColor");
                AddHeightOfProperty(ref height, property, "changeScreenLength");
                AddHeightOfProperty(ref height, property, "changeScreenSub");
                AddHeightOfProperty(ref height, property, "changeScreenFlash");
                break;

            case TriggerType.Wait:
                AddHeightOfProperty(ref height, property, "waitLength");
                break;

            case TriggerType.DisplayPrompt:
                AddHeightOfProperty(ref height, property, "displayPromptPrompt");
                AddHeightOfProperty(ref height, property, "displayPromptColor");
                AddHeightOfProperty(ref height, property, "displayPromptSub");
                AddHeightOfProperty(ref height, property, "displayPromptFlash");
                break;

            case TriggerType.ManageTasks:
                SerializedProperty manageTasksType = AddHeightOfProperty(ref height, property, "manageTasksType");
                if ((ManageTasksType)manageTasksType.enumValueIndex != ManageTasksType.ClearTasks)
                    AddHeightOfProperty(ref height, property, "manageTasksTask");
                break;

            case TriggerType.PlayerCanDo:
                AddHeightOfProperty(ref height, property, "playerCanDoType");
                AddHeightOfProperty(ref height, property, "playerCanDoCanDo");
                break;

            case TriggerType.MovePlayer:
                AddHeightOfProperty(ref height, property, "movePlayerType");
                AddHeightOfProperty(ref height, property, "movePlayerVector");
                break;

            case TriggerType.JumpscareAt:
                AddHeightOfProperty(ref height, property, "jumpscareAtObject");
                AddHeightOfProperty(ref height, property, "jumpscareAtLength");
                AddHeightOfProperty(ref height, property, "jumpscareAtEffect");
                break;

            case TriggerType.DisplayCanvas:
                AddHeightOfProperty(ref height, property, "displayCanvasCanvas");
                AddHeightOfProperty(ref height, property, "displayCanvasEffect");
                SerializedProperty displayCanvasFlash = AddHeightOfProperty(ref height, property, "displayCanvasFlash");
                if (displayCanvasFlash.boolValue)
                    AddHeightOfProperty(ref height, property, "displayCanvasFlashLength");
                break;

            case TriggerType.PlaySound:
                AddHeightOfProperty(ref height, property, "playSoundSound");

                SerializedProperty playSoundLocal = AddHeightOfProperty(ref height, property, "playSoundLocal");

                if (!playSoundLocal.boolValue)
                    AddHeightOfProperty(ref height, property, "playSoundIsEffect");
                else
                    AddHeightOfProperty(ref height, property, "playSoundSource");
                break;

            case TriggerType.SetObject:
                AddHeightOfProperty(ref height, property, "setObjectObject");
                AddHeightOfProperty(ref height, property, "setObjectSetActive");
                break;

            case TriggerType.LoadScene:
                AddHeightOfProperty(ref height, property, "loadSceneScene");
                AddHeightOfProperty(ref height, property, "loadSceneLength");
                AddHeightOfProperty(ref height, property, "loadSceneSave");
                break;

            case TriggerType.DisplayEnding:
                AddHeightOfProperty(ref height, property, "displayEndingTitle");
                AddHeightOfProperty(ref height, property, "displayEndingDescription");
                break;

            case TriggerType.Custom:
                break;

            default:
                break;
        }

        return height;
    }

    private SerializedProperty AddHeightOfProperty(ref float height, SerializedProperty property, string name)
    {
        SerializedProperty prop = property.FindPropertyRelative(name);
        height += EditorGUI.GetPropertyHeight(prop) + gap;

        return prop;
    }
}

#endif