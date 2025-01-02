namespace SimpleGridFly.Texture
{
    /// <summary>
    /// Represents information about a texture, including its identifiers, region, and file path.
    /// This struct is designed to encapsulate metadata for managing and organizing textures.
    /// </summary>
    public struct TextureInfo
    {
        /// <summary>
        /// The primary numeric identifier for the texture.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The region associated with the texture, typically used for categorization or grouping.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// A secondary string-based identifier for the texture.
        /// </summary>
        public string SecondaryId { get; set; }

        /// <summary>
        /// The file path to the texture, indicating its location on disk or within a resource system.
        /// </summary>
        public string TexturePath { get; set; }
    }
}