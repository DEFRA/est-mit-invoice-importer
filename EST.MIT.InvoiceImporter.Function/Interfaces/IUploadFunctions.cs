using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EST.MIT.Importer.Function.Interfaces;
public interface IUploadFunctions
{
    Task<IActionResult> GetUploadsByUser([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Uploads/{UserId}")] HttpRequest req, string UserId, ILogger log);
}
