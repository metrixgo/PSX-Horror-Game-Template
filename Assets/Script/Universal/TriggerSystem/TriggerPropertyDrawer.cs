using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Trigger))]
public class TriggerPropertyDrawer : PropertyDrawer
{
    private float gap = EditorGUIUtility.singleLineHeight + 0.2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty type = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(rect, type);

        rect.y += gap;

        switch ((TriggerType)type.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                SerializedProperty dialogueSpeaker = property.FindPropertyRelative("dialogueSpeaker");
                EditorGUI.PropertyField(rect, dialogueSpeaker, new GUIContent("Speaker"));
                rect.y += gap;
                SerializedProperty dialogueContent = property.FindPropertyRelative("dialogueContent");
                EditorGUI.PropertyField(rect, dialogueContent, new GUIContent("Content"));
                rect.y += gap;
                SerializedProperty dialogueSkippable = property.FindPropertyRelative("dialogueSkippable");
                EditorGUI.PropertyField(rect, dialogueSkippable, new GUIContent("Skippable"));
                rect.y += gap;
                SerializedProperty dialogueFlash = property.FindPropertyRelative("dialogueFlash");
                EditorGUI.PropertyField(rect, dialogueFlash, new GUIContent("Flash"));
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
                break;
            case TriggerType.Wait:
                SerializedProperty waitLength = property.FindPropertyRelative("waitLength");
                EditorGUI.PropertyField(rect, waitLength, new GUIContent("Length"));
                rect.y += gap;
                SerializedProperty waitFlash = property.FindPropertyRelative("waitFlash");
                EditorGUI.PropertyField(rect, waitFlash, new GUIContent("Flash"));
                rect.y += gap;
                break;
            default:
                Debug.LogWarning("Unimplemented Trigger Type: " + (TriggerType)type.enumValueIndex);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty type = property.FindPropertyRelative("type");

        switch ((TriggerType)type.enumValueIndex)
        {
            default:
                return gap * 10;
        }
    }
}
#endif