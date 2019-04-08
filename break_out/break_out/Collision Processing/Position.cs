using System;

namespace break_out.Collision_Processing
{
    [Flags]
    enum Position
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        LeftTop = Left|Top,
        LeftBottom = Left|Bottom,
        RightTop = Right|Top,
        RightBottom = Right|Bottom
    }
}