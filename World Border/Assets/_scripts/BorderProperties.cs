using UnityEngine;

public struct BorderProperties
{
    public BorderProperties(float height, float radius,
        bool pixelated, int pixelation,
        int speed,
        Color nearColor, Color farColor,
        float minDistance, float maxDistance,
        int nearPoint, int lineWidth,
        int horizontalDistortion, 
        VerticalDirection verticalDirection, 
        HorizontalDirection horizontalDirection,
        bool centerVertical)
    {
        Height = height;
        Radius = radius;
        Pixelated = pixelated;
        Pixelation = pixelation;
        Speed = speed;
        NearColor = nearColor;
        FarColor = farColor;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
        NearPoint = nearPoint;
        LineWidth = lineWidth;
        HorizontalDistortion = horizontalDistortion;
        VerticalDirection = verticalDirection;
        HorizontalDirection = horizontalDirection;
        CenterVertical = centerVertical;
    }

    public float Height;
    public float Radius;
    public bool Pixelated;
    public int Pixelation;
    public int Speed;
    public Color NearColor;
    public Color FarColor;
    public float MinDistance;
    public float MaxDistance;
    public int NearPoint;
    public int LineWidth;
    public int HorizontalDistortion;
    public VerticalDirection VerticalDirection;
    public HorizontalDirection HorizontalDirection;
    public bool CenterVertical;
}
