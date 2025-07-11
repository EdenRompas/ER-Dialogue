/*using UnityEngine;
using UnityEditor;

public static class EditorZoomArea
{
    private static Matrix4x4 prevGuiMatrix;

    public static void Begin(float zoomScale, Rect screenCoordsArea)
    {
        *//*GUI.EndGroup();

        Rect clippedArea = screenCoordsArea.ScaleSizeBy(1f / zoomScale, screenCoordsArea.TopLeft());
        clippedArea.y += 21;

        GUI.BeginGroup(clippedArea);

        prevGuiMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUI.matrix = translation * scale * translation.inverse * GUI.matrix;*//*
    }

    public static void End()
    {
        GUI.matrix = prevGuiMatrix;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0, 21, Screen.width, Screen.height));
    }

    public static Vector2 TopLeft(this Rect rect) => new Vector2(rect.x, rect.y);
}
*/