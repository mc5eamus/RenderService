using Microsoft.AspNetCore.Mvc;
using RenderDomain.Contract;

namespace RenderServiceWebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessingStatusController : ControllerBase
    {
        private readonly IQueueConsumer qs;
        private readonly ILogger logger;

        public ProcessingStatusController( 
            IQueueConsumer qs,  
            ILogger<ProcessingStatusController> logger)
        {
            this.qs = qs;
            this.logger = logger;
        }

        [HttpGet(Name = "GetProcessingStatus")]
        public string Get()
        {
            var status = qs.GetStatus();
            logger.LogInformation($"processing status = {status}");
            return status;
        }
    }
}
