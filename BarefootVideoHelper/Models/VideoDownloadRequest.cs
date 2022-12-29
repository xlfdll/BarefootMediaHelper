using System;

namespace BarefootVideoHelper
{
    public class VideoDownloadRequest
    {
        public VideoDownloadRequest(String url)
        {
            this.URL = url;
        }

        public String URL { get; }
        public String DisplayText { get; set; }

        public override Boolean Equals(Object obj)
        {
            if (obj is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return this.URL == (obj as VideoDownloadRequest).URL;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode() ^ this.URL.GetHashCode();
        }

        public override String ToString()
        {
            return this.DisplayText;
        }
    }
}