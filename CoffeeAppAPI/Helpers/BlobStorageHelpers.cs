namespace CoffeeAppAPI.Helpers
{
    public static class BlobStorageHelpers
    {
        public static string GetBlobName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var blobName = $"{Guid.NewGuid()}{extension}";
            return blobName;
        }

        public static string GetFileExtensionFromContentType(string contentType)
        {
            var mimeTypeMappings = new Dictionary<string, string>
            {
                {"image/jpeg", ".jpg"},
                {"image/png", ".png"},
                {"image/gif", ".gif"},
                // Add more mappings if you want to support other image types.
            };

            return mimeTypeMappings.TryGetValue(contentType, out string fileExtension) ? fileExtension : string.Empty;
        }
    }

}