using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [JsonProperty("use-scheduler")]
        public bool UseScheduler { get; set; }

        [JsonProperty("rewrite-rules")]
        public List<RewriteRule> Rules { get; set; }

        [JsonProperty("accounts")]
        public List<JsonAccount> JsonAccounts { get; set; }

        public List<IAccount> Accounts { get; set; }

        public List<IRewriteRule> RewriteRules { get; set; }

        public IPrerendererSettings PrerendererSettings { get; set; }

        [JsonProperty("prerenderer")]
        public JsonPrerendererSettings JsonPrerendererSettings { get; set; }

        [JsonProperty("public-client-id")]
        public string PublicClientId { get; set; }

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
                // workaround for iis 

                var fileName = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
                path = Path.Combine(path, "bin", fileName);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Config file not found! " + path, path);
                }
            }

            var config = JsonConvert.DeserializeObject<JsonConfiguration>(File.ReadAllText(path));
            config.RewriteRules = config.Rules.Cast<IRewriteRule>().ToList();
            config.Accounts = config.JsonAccounts.Cast<IAccount>().ToList();
            config.PrerendererSettings = config.JsonPrerendererSettings;

            return config;
        }

        public void ToFile(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
