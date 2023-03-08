using System;

namespace BarefootMediaHelper
{
    public class SubtitleParameters
    {
        public SubtitleParameters
            (Int32 topLeftX,
            Int32 topLeftY,
            Int32 bottomRightX,
            Int32 bottomRightY,
            Boolean applyEvenAdjustments = true)
        {
            if (applyEvenAdjustments)
            {
                if (topLeftX % 2 != 0)
                {
                    topLeftX--;
                }
                if (bottomRightX % 2 != 0)
                {
                    bottomRightX++;
                }
            }

            this.TopLeftX = topLeftX;
            this.TopLeftY = topLeftY;
            this.BottomRightX = bottomRightX;
            this.BottomRightY = bottomRightY;
            this.ApplyToAllFrames = true;
        }

        public SubtitleParameters
            (Int32 topLeftX,
            Int32 topLeftY,
            Int32 bottomRightX,
            Int32 bottomRightY,
            Int32 startFrameIndex,
            Int32 endFrameIndex)
            : this(topLeftX, topLeftY, bottomRightX, bottomRightY)
        {
            this.StartFrameIndex = startFrameIndex;
            this.EndFrameIndex = endFrameIndex;
            this.ApplyToAllFrames = false;
        }

        public Int32 TopLeftX { get; }
        public Int32 TopLeftY { get; }
        public Int32 BottomRightX { get; }
        public Int32 BottomRightY { get; }
        public Int32 StartFrameIndex { get; }
        public Int32 EndFrameIndex { get; }
        public Boolean ApplyToAllFrames { get; }

        public override String ToString()
        {
            String topLeftPositionText = $"({this.TopLeftX},{this.TopLeftY})";
            String bottomRightPositionText = $"({this.BottomRightX},{this.BottomRightY})";
            String frameText = this.ApplyToAllFrames ? "for all frames" : $"for frame {this.StartFrameIndex} - {this.EndFrameIndex}";

            return $"{topLeftPositionText} - {bottomRightPositionText} {frameText}";
        }
    }
}