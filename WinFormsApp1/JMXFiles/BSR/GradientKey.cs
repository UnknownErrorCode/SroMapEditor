namespace SimpleGridFly.JMXFiles.BSR
{
    public struct GradientKey
    {
        /// <summary>
        /// In milliseconds.
        /// </summary>
        public uint Time { get; set; }

        /// <summary>
        /// Color4 ??
        /// </summary>
        public List<float> Values { get; set; }
    }
}