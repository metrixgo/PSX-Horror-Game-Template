using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Trigger))]
public class TriggerPropertyDrawer : PropertyDrawer
{
    private float gap = EditorGUIUtility.singleLineHeight * 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty type = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(position, type);

        position.y += gap;

        switch ((TriggerType)type.enumValueIndex)
        {
            case TriggerType.DisplayDialogue:
                SerializedProperty dialogueSpeaker = property.FindPropertyRelative("dialogueSpeaker");
                EditorGUI.PropertyField(position, dialogueSpeaker, new GUIContent("Speaker"));
                position.y += gap;
                SerializedProperty dialogueContent = property.FindPropertyRelative("dialogueContent");
                EditorGUI.PropertyField(position, dialogueContent, new GUIContent("Content"));
                break;
            case TriggerType.ChangeScreen:
                break;
            case TriggerType.Wait:
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 10;
    }
}
