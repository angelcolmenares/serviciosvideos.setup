using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static TimeSpan GetRemainingTimeSpan(this DateTime issuedAt, int expireInSeconds, bool utc = true)
        =>
            TimeSpan.FromSeconds((int)Math.Abs(expireInSeconds - (Now(utc) - issuedAt).TotalSeconds));


        public static int GetRemainingTimeInSeconds(this DateTime issuedAt, int expireInSeconds, bool utc = true)
        =>
            (int)Math.Abs(expireInSeconds - (Now(utc) - issuedAt).TotalSeconds);

        public static bool HasExpired(this DateTime issuedAt, int expireInSeconds, bool utc = true) => issuedAt.AddSeconds(expireInSeconds) <= Now(utc);

        public static TimeSpan GetEllapsedTimeSpan(this DateTime issuedAt, bool utc = true) => (Now(utc) - issuedAt);

        public static int GetEllapsedTimeInSeconds(this DateTime issuedAt, bool utc = true) => (int)(Now(utc) - issuedAt).TotalSeconds;



        public static string ToIso8601Date(this DateTime date)
            => date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);

        /// <summary> 
        /// Froms the iso8601 date. 
        /// </summary>
        public static DateTime FromIso8601Date(this string date)
            => DateTime.Parse(date, null, DateTimeStyles.RoundtripKind);


        public static string NowToString(bool utc = true)
            => utc ? DateTime.UtcNow.ToIso8601Date() : DateTime.Now.ToIso8601Date();

        public static DateTime Now(bool utc = true)
            => utc ? DateTime.UtcNow : DateTime.Now;


        public static string NowToFilenameString(bool utc = true)
            => (utc ? DateTime.UtcNow.ToIso8601Date() : DateTime.Now.ToIso8601Date()).Replace(":", "-");


        public static string ToTimestampWithoutTimezone(this DateTime date)
            => date.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF", CultureInfo.InvariantCulture);


        public static string ToTimestampWithTimezone(this DateTime date)
        {
            var hours = (int)(date - date.ToUniversalTime()).TotalHours;
            var offset = (hours >= 0 ? "+" : "-") + Math.Abs(hours).ToString().PadLeft(2, '0');
            return date.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF", CultureInfo.InvariantCulture) + offset;
        }

        public static string ToDatestamp(this DateTime date)
            => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public static string ToTimeSpan(this DateTime date)
            => date.ToString("HH:mm:ss.FFFFFF", CultureInfo.InvariantCulture);
    }
}
