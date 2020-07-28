using System;
using Xunit;
using Microsoft.Extensions.Logging;
using TransferWiseWebhook.Controllers;
using Moq;

namespace XUnitTest_TransferWiseWebhook
{
    public class WebhookControllerTests
    {
        private readonly ReceiveDepositController _controllerMock;

        public WebhookControllerTests()
        {
            var logger = new Mock<ILogger<ReceiveDepositController>>();
            _controllerMock = new ReceiveDepositController(logger.Object);
        }

        [Fact]
        public void Test1()
        {
        }
    }
}