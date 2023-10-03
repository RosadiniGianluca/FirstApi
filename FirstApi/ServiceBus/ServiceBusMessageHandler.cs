using FirstApi.Properties;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

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

        // Registra un gestore di messaggi per la sottoscrizione, con un gestore di eccezioni, un numero massimo di chiamate simultanee e un valore booleano che indica se il messaggio deve essere completato automaticamente dopo la chiamata del gestore.
        public void RegisterMessageHandler(Func<Message, CancellationToken, Task> messageHandler, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var messageHandlerOptions = new MessageHandlerOptions(exceptionHandler)  // Gestore delle eccezioni
            {
                MaxConcurrentCalls = 1,  // Numero massimo di chiamate simultanee
                AutoComplete = false     // Il messaggio non viene completato automaticamente dopo la chiamata del gestore
            };
            _subscriptionClient.RegisterMessageHandler(messageHandler, messageHandlerOptions);  // Registra il gestore dei messaggi
        }

        // Completa il messaggio per rimuoverlo dalla coda
        public async Task CompleteAsync(string lockToken)
        {
            await _subscriptionClient.CompleteAsync(lockToken);  // Completa il messaggio e lo rimuove dalla coda di Service Bus
        }

        // Sospende il gestore dei messaggi
        public async Task CloseAsync()
        {
            await _subscriptionClient.CloseAsync();  // Sospende il gestore dei messaggi
        }
    }
}
