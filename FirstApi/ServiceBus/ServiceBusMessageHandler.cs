using FirstApi.Properties;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Options;
using System.Text;

namespace FirstApi.ServiceBus
{
    public class ServiceBusMessageHandler
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        public ServiceBusMessageHandler(IOptions<ServiceBusConfig> serviceBusConfig)
        {
            _connectionString = serviceBusConfig.Value.ConnectionString;
            _topicName = serviceBusConfig.Value.TopicName;
            _subscriptionName = serviceBusConfig.Value.SubscriptionName;
            _subscriptionClient = new SubscriptionClient(_connectionString, _topicName, _subscriptionName);
        }

        public void RegisterMessageHandler(Func<Message, CancellationToken, Task> messageHandler, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var messageHandlerOptions = new MessageHandlerOptions(exceptionHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(messageHandler, messageHandlerOptions);
        }

        public async Task CompleteAsync(string lockToken)
        {
            await _subscriptionClient.CompleteAsync(lockToken);
        }
    }
}
