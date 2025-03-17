using System;
using GameObjects.CameraControllers.Workshop;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class CameraBordersDrawer
{
    [Obsolete("Obsolete")]
    static CameraBordersDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUIAlways;
    }

    [Obsolete("Obsolete")]
    private static void OnSceneGUIAlways(SceneView sceneView)
    {
        var controllers = Object.FindObjectsOfType<CameraMovementController>();
        foreach (var controller in controllers)
        {
            if (controller == null || !controller.VisualizeBorders)
                continue;

            DrawBorders(controller);
            DrawReducedSpeedZone(controller);
        }
    }
    
    private static void DrawBorders(CameraMovementController controller)
    {
        var borders = controller.CameraBorder;

        if (borders.UpBorder == null || borders.DownBorder == null ||
            borders.LeftBorder == null || borders.RightBorder == null ||
            borders.FrontBorder == null || borders.BackBorder == null)
            return;

        var minX = borders.LeftBorder.position.x + controller.CameraMoveRadius;
        var maxX = borders.RightBorder.position.x - controller.CameraMoveRadius;

        var minY = borders.DownBorder.position.y + controller.CameraMoveRadius;
        var maxY = borders.UpBorder.position.y - controller.CameraMoveRadius;

        var minZ = borders.BackBorder.position.z + controller.CameraMoveRadius;
        var maxZ = borders.FrontBorder.position.z - controller.CameraMoveRadius;

        var topLeftFront = new Vector3(minX, maxY, maxZ);
        var topRightFront = new Vector3(maxX, maxY, maxZ);
        var topLeftBack = new Vector3(minX, maxY, minZ);
        var topRightBack = new Vector3(maxX, maxY, minZ);

        var bottomLeftFront = new Vector3(minX, minY, maxZ);
        var bottomRightFront = new Vector3(maxX, minY, maxZ);
        var bottomLeftBack = new Vector3(minX, minY, minZ);
        var bottomRightBack = new Vector3(maxX, minY, minZ);

        var color = new Color(0f, 1f, 0f, 0.3f);

        Handles.color = color;
        Handles.DrawLine(topLeftFront, topRightFront);
        Handles.DrawLine(topLeftFront, topLeftBack);
        Handles.DrawLine(topRightFront, topRightBack);
        Handles.DrawLine(topLeftBack, topRightBack);

        Handles.DrawLine(bottomLeftFront, bottomRightFront);
        Handles.DrawLine(bottomLeftFront, bottomLeftBack);
        Handles.DrawLine(bottomRightFront, bottomRightBack);
        Handles.DrawLine(bottomLeftBack, bottomRightBack);

        Handles.DrawLine(topLeftFront, bottomLeftFront);
        Handles.DrawLine(topRightFront, bottomRightFront);
        Handles.DrawLine(topLeftBack, bottomLeftBack);
        Handles.DrawLine(topRightBack, bottomRightBack);

        DrawTransparentWall(topLeftFront, topRightFront, topRightBack, topLeftBack, color);
        DrawTransparentWall(bottomLeftFront, bottomRightFront, bottomRightBack, bottomLeftBack, color);
        DrawTransparentWall(bottomLeftFront, topLeftFront, topLeftBack, bottomLeftBack, color);
        DrawTransparentWall(bottomRightFront, topRightFront, topRightBack, bottomRightBack, color);
        DrawTransparentWall(bottomLeftFront, topLeftFront, topRightFront, bottomRightFront, color);
        DrawTransparentWall(bottomLeftBack, topLeftBack, topRightBack, bottomRightBack, color);
    }

    private static void DrawReducedSpeedZone(CameraMovementController controller)
    {
        var borders = controller.CameraBorder;
        float minDist = controller.MinDistanceToReduceSpeed;

        if (borders.UpBorder == null || borders.DownBorder == null ||
            borders.LeftBorder == null || borders.RightBorder == null ||
            borders.FrontBorder == null || borders.BackBorder == null)
            return;

        var minX = borders.LeftBorder.position.x + controller.CameraMoveRadius + minDist;
        var maxX = borders.RightBorder.position.x - controller.CameraMoveRadius - minDist;

        var minY = borders.DownBorder.position.y + controller.CameraMoveRadius + minDist;
        var maxY = borders.UpBorder.position.y - controller.CameraMoveRadius - minDist;

        var minZ = borders.BackBorder.position.z + controller.CameraMoveRadius + minDist;
        var maxZ = borders.FrontBorder.position.z - controller.CameraMoveRadius - minDist;

        var topLeftFront = new Vector3(minX, maxY, maxZ);
        var topRightFront = new Vector3(maxX, maxY, maxZ);
        var topLeftBack = new Vector3(minX, maxY, minZ);
        var topRightBack = new Vector3(maxX, maxY, minZ);

        var bottomLeftFront = new Vector3(minX, minY, maxZ);
        var bottomRightFront = new Vector3(maxX, minY, maxZ);
        var bottomLeftBack = new Vector3(minX, minY, minZ);
        var bottomRightBack = new Vector3(maxX, minY, minZ);

        var color = new Color(0f, 1f, 1f, 0.3f);

        Handles.color = color;
        Handles.DrawLine(topLeftFront, topRightFront);
        Handles.DrawLine(topLeftFront, topLeftBack);
        Handles.DrawLine(topRightFront, topRightBack);
        Handles.DrawLine(topLeftBack, topRightBack);

        Handles.DrawLine(bottomLeftFront, bottomRightFront);
        Handles.DrawLine(bottomLeftFront, bottomLeftBack);
        Handles.DrawLine(bottomRightFront, bottomRightBack);
        Handles.DrawLine(bottomLeftBack, bottomRightBack);

        Handles.DrawLine(topLeftFront, bottomLeftFront);
        Handles.DrawLine(topRightFront, bottomRightFront);
        Handles.DrawLine(topLeftBack, bottomLeftBack);
        Handles.DrawLine(topRightBack, bottomRightBack);

        DrawTransparentWall(topLeftFront, topRightFront, topRightBack, topLeftBack, color);
        DrawTransparentWall(bottomLeftFront, bottomRightFront, bottomRightBack, bottomLeftBack, color);
        DrawTransparentWall(bottomLeftFront, topLeftFront, topLeftBack, bottomLeftBack, color);
        DrawTransparentWall(bottomRightFront, topRightFront, topRightBack, bottomRightBack, color);
        DrawTransparentWall(bottomLeftFront, topLeftFront, topRightFront, bottomRightFront, color);
        DrawTransparentWall(bottomLeftBack, topLeftBack, topRightBack, bottomRightBack, color);
    }

    private static void DrawTransparentWall(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color)
    {
        Handles.DrawSolidRectangleWithOutline(new[] { p1, p2, p3, p4 }, color, Color.clear);
    }
}
