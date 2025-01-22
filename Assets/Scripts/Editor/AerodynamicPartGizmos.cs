using UnityEditor;
using UnityEngine;

public static class AerodynamicPartGizmos
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    public static void DrawAerodynamicGizmos(AerodynamicPart part, GizmoType gizmoType)
    {
        if (part == null || part.AerodynamicPartData.chord <= 0) return;

        AerodynamicPartData data = part.AerodynamicPartData;

        // Центр объекта
        Vector3 position = part.transform.position;
        Quaternion rotation = part.transform.rotation;

        // Отображение зоны действия (ширина и длина)
        DrawRectangle(position, rotation, data.span, data.chord, Color.magenta);

        // Отображение направления подъемной силы
        Vector3 liftDirection = rotation * Vector3.up * data.liftSlope;
        DrawArrow(position, liftDirection, Color.blue, 0.1f);

        // Отображение направления сопротивления
        Vector3 dragDirection =  Vector3.forward * data.skinFriction;
        DrawArrow(position, dragDirection, Color.red, 0.1f);

        // Отображение центра давления
        if (data.autoAspectRatio)
        {
            Vector3 centerOfPressure = position + rotation * Vector3.forward * (data.aspectRatio * 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerOfPressure, 0.1f);
        }
    }

    private static void DrawArrow(Vector3 position, Vector3 vector, Color color, float width)
    {
        Vector3 vn = vector.normalized;
        Vector3 cross = Vector3.Cross(vn, Camera.current.transform.forward).normalized;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        // Линия стрелки
        Handles.color = color;
        Handles.DrawAAPolyLine(width, position, position + vector);

        // Головка стрелки
        Handles.DrawAAPolyLine(width, position + vector, position + vector - vn * 0.2f + cross * 0.1f);
        Handles.DrawAAPolyLine(width, position + vector, position + vector - vn * 0.2f - cross * 0.1f);
    }

    private static void DrawRectangle(Vector3 position, Quaternion rotation, float width, float height, Color color)
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.DrawSolidRectangleWithOutline(
            GetRectangleVertices(position, rotation, width, height),
            color,
            Color.black);
    }

    private static Vector3[] GetRectangleVertices(Vector3 position, Quaternion rotation, float width, float height)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-width / 2, 0, -height / 2);
        vertices[1] = new Vector3(-width / 2, 0, height / 2);
        vertices[2] = new Vector3(width / 2, 0, height / 2);
        vertices[3] = new Vector3(width / 2, 0, -height / 2);
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = rotation * vertices[i] + position;
        }
        return vertices;
    }
}
