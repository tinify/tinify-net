namespace TinifyAPI
{
    /// <summary>
    /// Media types that images can be converted to.
    /// </summary>
    /// <example>
    /// <code>
    /// await Tinify.FromFile("input.png")
    ///     .Convert(new { type = Format.Jxl })
    ///     .ToFile("output.jxl");
    /// </code>
    /// Multiple types may be supplied, in which case the API returns the
    /// smallest result:
    /// <code>
    /// new { type = new[] { Format.Jxl, Format.WebP } }
    /// </code>
    /// </example>
    public static class Format
    {
        /// <summary>WebP, <c>image/webp</c>.</summary>
        public const string WebP = "image/webp";

        /// <summary>PNG, <c>image/png</c>.</summary>
        public const string Png = "image/png";

        /// <summary>JPEG, <c>image/jpeg</c>.</summary>
        public const string Jpeg = "image/jpeg";

        /// <summary>
        /// JPEG, under the alternative <c>image/jpg</c> media type the API also
        /// accepts. The API normalises this to <c>image/jpeg</c>, so the result is
        /// identical to <see cref="Jpeg"/>. The two constants hold different
        /// strings but select the same format.
        /// </summary>
        public const string Jpg = "image/jpg";

        /// <summary>AVIF, <c>image/avif</c>.</summary>
        public const string Avif = "image/avif";

        /// <summary>JPEG XL, <c>image/jxl</c>.</summary>
        public const string Jxl = "image/jxl";

        /// <summary>Wildcard, <c>*/*</c>. Returns the smallest of the supported types.</summary>
        public const string Any = "*/*";
    }
}
