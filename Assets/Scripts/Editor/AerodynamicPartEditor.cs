using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AerodynamicPart))]
public class AerodynamicPartEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AerodynamicPart aerodynamicPart = (AerodynamicPart)target;

        EditorGUILayout.LabelField("Aerodynamic Part", EditorStyles.boldLabel);

        SerializedProperty aerodynamicPartDataProp = serializedObject.FindProperty("_aerodynamicPartData");

        if (aerodynamicPartDataProp != null)
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(aerodynamicPartDataProp, new GUIContent("Aerodynamic Part Data"), true);

            SerializedProperty autoAspectRatioProp = aerodynamicPartDataProp.FindPropertyRelative("autoAspectRatio");
            SerializedProperty chordProp = aerodynamicPartDataProp.FindPropertyRelative("chord");
            SerializedProperty spanProp = aerodynamicPartDataProp.FindPropertyRelative("span");
            SerializedProperty aspectRatioProp = aerodynamicPartDataProp.FindPropertyRelative("aspectRatio");

            if (autoAspectRatioProp.boolValue)
            {
                float chord = chordProp.floatValue;
                float span = spanProp.floatValue;

                aspectRatioProp.floatValue = span / chord;

                if (aspectRatioProp.floatValue <= 0)
                {
                    EditorGUILayout.HelpBox("Aspect Ratio вычислено как отрицательное или нулевое значение. Проверьте значения chord и span.", MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Reset to Default"))
        {
            aerodynamicPart.SetAerodynamicPartData(AerodynamicPartData.DefaultData());
            EditorUtility.SetDirty(aerodynamicPart); 
        }
    }
}