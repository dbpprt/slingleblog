using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Http;
using FileBiggy.Common;
using Nito.AsyncEx;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Logging;
using SlingleBlog.Properties;
using DirectoryUtilities = SlingleBlog.Common.Utilities.DirectoryUtilities;

namespace SlingleBlog.Controllers
{
    [UsedImplicitly]
    public class DeploymentController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly AsyncReaderWriterLock.UpgradeableReaderKey _upgradeableReaderLock;
        private readonly ILog _log;

        public DeploymentController(
            IConfiguration configuration,
            AsyncReaderWriterLock.UpgradeableReaderKey upgradeableReaderLock,
            ILog log
            )
        {
            _configuration = configuration;
            _upgradeableReaderLock = upgradeableReaderLock;
            _log = log;
        }

        [HttpPut]
        [Route("api/deploy")]
        [UsedImplicitly]
        public async Task<IHttpActionResult> HandleDeployment()
        {
            if (Request.Headers.All(header => header.Key != _configuration.ApiKeyHeaderField))
            {
                return BadRequest();
            }

            var accessToken = Request.Headers
                .First(header => header.Key == _configuration.ApiKeyHeaderField)
                .Value
                .FirstOrDefault();

            var targetFolder = "";

            if (Request.Headers.Any(header => header.Key == _configuration.TargetFolderHeaderField))
            {
                targetFolder = Request.Headers
                    .First(header => header.Key == _configuration.TargetFolderHeaderField)
                    .Value
                    .FirstOrDefault() ?? "";
            }

            if (accessToken == null || accessToken != _configuration.ApiKey)
            {
                _log.Write()
                    .IsFailureAudit()
                    .SetMessage("Unauthorized deployment from " + Request.GetOwinContext().Request.RemoteIpAddress)
                    .Save(_log);

                return StatusCode(HttpStatusCode.Unauthorized);
            }

            var data = await Request.Content.ReadAsByteArrayAsync();

            using (await _upgradeableReaderLock.UpgradeAsync())
            {
                var workingName = Guid.NewGuid().ToString();
                var workingPath = Path.Combine(_configuration.TempDirectory, workingName + ".zip");
                var extractDirectory = DirectoryUtilities.EnsureExists(
                    Path.Combine(_configuration.TempDirectory, workingName));

                try
                {
                    File.WriteAllBytes(workingPath, data);
                    ZipFile.ExtractToDirectory(workingPath, extractDirectory);

                    DirectoryUtilities.Merge(extractDirectory, Path.Combine(_configuration.PublicPath, targetFolder));

                    Directory.Delete(extractDirectory, true);

                    File.Delete(workingPath);
                }
                catch (Exception ex)
                {
                    ex.ToEvent()
                        .IsError()
                        .SetMessage("Unable to handle deployment")
                        .Save(_log);

                    return InternalServerError(ex);
                }
            }

            return Ok("Deployment succeeded");
        }
    }
}
