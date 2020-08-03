using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using TransferWiseWebhooks.Controllers;
using TransferWiseWebhooks.DataModels;
using TransferWiseWebhooks.Interfaces;
using TransferWiseWebhooks;

namespace XUnitTest_TransferWiseWebhook
{
    public class WebhookControllerTests
    {
        private readonly TransferWiseEventsController _mockController;

        private readonly string json_BalanceCreditEvent = @"{""data"":{""resource"":{""id"":111,""type"":""balance-account"",""profile_id"":222},""amount"":1.23,""currency"":""EUR"",""transaction_type"":""credit"",""post_transaction_balance_amount"":2.34,""occurred_at"":""2020-07-06T22:46:24.766Z""},""subscription_id"":""01234567-89ab-cdef-0123-456789abcdef"",""event_type"":""balances#credit"",""schema_version"":""2.0.0"",""sent_at"":""2020-07-06T22:46:24.764Z""}";

        private readonly string json_NotBalanceCreditEvent = @"{""data"":{""resource"":{""id"":111,""type"":""balance-account"",""profile_id"":222},""amount"":1.23,""currency"":""EUR"",""transaction_type"":""credit"",""post_transaction_balance_amount"":2.34,""occurred_at"":""2020-07-06T22:46:24.766Z""},""subscription_id"":""01234567-89ab-cdef-0123-456789abcdef"",""event_type"":""transfers#state-change"",""schema_version"":""2.0.0"",""sent_at"":""2020-07-06T22:46:24.764Z""}";

        public WebhookControllerTests()
        {
            var mockLogger = new Mock<ILogger<Program>>();
            var mockProcessor = new Mock<TransferWiseEventProcessor>(mockLogger.Object);
            _mockController = new TransferWiseEventsController(mockProcessor.Object, mockLogger.Object);
        }

        [Fact]
        public async Task BalanceCreditEvent_Post_WithValidSignature_AndCorrectEventType_Returns200()
        {
            // Arrange
            var json = json_BalanceCreditEvent;

            var payload = JsonConvert.DeserializeObject<BalanceDepositDTO>(json);

            // Add request header with a valid X-Signature
            _mockController.ControllerContext = ControllerContextHelper.Create(true);

            // Act
            var result = await _mockController.ProcessBalanceCreditEventAsync(payload);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task BalanceCreditEvent_Post_WithValidSignature_AndWrongEventType_Returns400()
        {
            // Arrange
            var json = json_NotBalanceCreditEvent;

            var payload = JsonConvert.DeserializeObject<BalanceDepositDTO>(json);

            // Add request header with a valid X-Signature
            _mockController.ControllerContext = ControllerContextHelper.Create(true);

            // Act
            var result = await _mockController.ProcessBalanceCreditEventAsync(payload);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task BalanceCreditEvent_Post_WithInvalidSignature_AndCorrectEventType_Returns400()
        {
            // Arrange
            var json = json_BalanceCreditEvent;

            var payload = JsonConvert.DeserializeObject<BalanceDepositDTO>(json);

            // Add request header with an invalid X-Signature
            _mockController.ControllerContext = ControllerContextHelper.Create(false);

            // Act
            var result = await _mockController.ProcessBalanceCreditEventAsync(payload);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task BalanceCreditEvent_Post_WithInvalidSignature_AndWrongEventType_Returns400()
        {
            // Arrange
            var json = json_NotBalanceCreditEvent;

            var payload = JsonConvert.DeserializeObject<BalanceDepositDTO>(json);

            // Add request header with an invalid X-Signature
            _mockController.ControllerContext = ControllerContextHelper.Create(false);

            // Act
            var result = await _mockController.ProcessBalanceCreditEventAsync(payload);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task BalanceCreditEvent_Post_WithEmptyBody_Returns400()
        {
            // Arrange
            var json = @"{}";

            var payload = JsonConvert.DeserializeObject<BalanceDepositDTO>(json);

            // Add request header with a valid X-Signature
            _mockController.ControllerContext = ControllerContextHelper.Create(true);

            // Act
            var result = await _mockController.ProcessBalanceCreditEventAsync(payload);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }
    }
}