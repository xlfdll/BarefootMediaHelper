using System;
using System.IO;

namespace BarefootMediaHelper
{
    public enum NCAdjustmentType
    {
        InsertClip,
        RemoveRange
    }

    public class NCAdjustment
    {
        protected NCAdjustment(NCAdjustmentType type, TimeSpan startPosition)
        {
            this.Type = type;
            this.StartPosition = startPosition;
        }

        public NCAdjustmentType Type { get; }
        public TimeSpan StartPosition { get; }
    }

    public class NCInsertClipAdjustment : NCAdjustment
    {
        public NCInsertClipAdjustment(NCAdjustmentType type, TimeSpan startPosition, String newClipFilePath)
            : base(type, startPosition)
        {
            this.NewClipFilePath = newClipFilePath;
        }

        public String NewClipFilePath { get; }

        public String NewClipFileName
            => Path.GetFileName(this.NewClipFilePath);
    }

    public class NCRemoveRangeAdjustment : NCAdjustment
    {
        public NCRemoveRangeAdjustment(NCAdjustmentType type, TimeSpan startPosition, TimeSpan endPosition)
            : base(type, startPosition)
        {
            this.EndPosition = endPosition;
        }

        public TimeSpan EndPosition { get; }
    }
}