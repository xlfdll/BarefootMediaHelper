using System;

namespace BarefootVideoHelper
{
    public class SubtitleParameters
    {
        public SubtitleParameters(Int32 topLeftX, Int32 topLeftY, Boolean applyEvenAdjustments = true)
        {
            if (applyEvenAdjustments && topLeftX % 2 != 0)
            {
                topLeftX--;
            }

            this.TopLeftX = topLeftX;
            this.TopLeftY = topLeftY;
            this.ApplyToAllFrames = true;
        }

        public SubtitleParameters(Int32 topLeftX, Int32 topLeftY, Int32 startFrameIndex, Int32 endFrameIndex)
            : this(topLeftX, topLeftY)
        {
            this.StartFrameIndex = startFrameIndex;
            this.EndFrameIndex = endFrameIndex;
            this.ApplyToAllFrames = false;
        }

        public Int32 TopLeftX { get; }
        public Int32 TopLeftY { get; }
        public Int32 StartFrameIndex { get; }
        public Int32 EndFrameIndex { get; }
        public Boolean ApplyToAllFrames { get; }

        public override String ToString()
        {
            String endText = this.ApplyToAllFrames ? "for all frames" : $"for frame {this.StartFrameIndex} - {this.EndFrameIndex}";

            return $"({this.TopLeftX.ToString("F0")},{this.TopLeftY.ToString("F0")}) {endText}";
        }
    }
}