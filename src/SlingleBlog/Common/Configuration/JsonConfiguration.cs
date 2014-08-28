using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SlingleBlog.Common.Utilities;

namespace SlingleBlog.Common.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonConfiguration : IConfiguration
    {
        [JsonProperty("enable-error-page")]
        public bool UseErrorPage { get; set; }

        [JsonProperty("default-files")]
        public List<string> DefaultFiles { get; set; }

        [JsonProperty("api-error-policy")]
        public string ApiErrorPolicy { get; set; }

        [JsonProperty("data-directory")]
        public string ContractedDataDirectory { get; set; }

        [JsonProperty("api-key-header-field")]
        public string ApiKeyHeaderField { get; set; }

        [JsonProperty("target-folder-header-field")]
        public string TargetFolderHeaderField { get; set; }

        [JsonProperty("api-key")]
        public string ApiKey { get; set; }

        public string BasePath
        {
            get
            {
                return DirectoryUtilities.EnsureExists(
                    Environment.ExpandEnvironmentVariables(ContractedDataDirectory)
                    );
            }
        }

        public string StorageBasePath
        {
            get
            {
                return DirectoryUtilities.EnsureExists(
                    Path.Combine(BasePath, "db")
                    );
            }
        }

        public string TempDirectory
        {
            get
            {
                return DirectoryUtilities.EnsureExists(
                    Path.Combine(BasePath, "temp")
                    );
            }
        }


        public string PublicPath
        {
            get
            {
                return DirectoryUtilities.EnsureExists(
                    Path.Combine(BasePath, "public")
                    );
            }
        }

        public static IConfiguration FromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Config file not found!", path);
            }

            return JsonConvert.DeserializeObject<JsonConfiguration>(File.ReadAllText(path));
        }

        public void ToFile(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
