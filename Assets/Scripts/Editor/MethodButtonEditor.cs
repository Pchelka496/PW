using System;
using System.Reflection;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class MethodButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetObject = target as MonoBehaviour;
        if (targetObject == null) return;

        // Получаем все методы, помеченные атрибутом MethodButton
        var methods = targetObject.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes(
                typeof(MethodButtonAttribute), true);
            if (attributes.Length > 0)
            {
                // Создаем кнопку с названием метода
                if (GUILayout.Button(method.Name))
                {
                    method.Invoke(targetObject, null); // Вызываем метод
                }
            }
        }
    }
}
