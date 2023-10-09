using FirstApi.Clients;
using FirstApi.ServiceBus;
using FirstApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Newtonsoft.Json;
using FirstApi.Entities;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ServiceBusMessageHandler _messageHandler;
        private readonly WebhookClient _webhookClient;

        // Dependency Injection (DI) 
        public WebhookController(ServiceBusMessageHandler messageHandler, WebhookClient webhookClient)
        {
            _messageHandler = messageHandler;
            _webhookClient = webhookClient;
        }

        // Metodo che fa partire il listener interno in automatico all'avvio dell'applicazione
        [HttpPost("startListenerInternal")]
        public IActionResult StartListenerInternal()
        {
            _messageHandler.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);
            return Ok("Listener internal avviato.");
        }

        // Metodo che ferma il listener interno, quello avviato all'avvio dell'applicazione
        [HttpPost("stopListener")]
        public async Task<IActionResult> StopListener()
        {
            // Sospende il gestore dei messaggi
            await _messageHandler.CloseAsync();

            return Ok("Listener sospeso.");
        }

        // FIXME: Fa partire un eccezione non gestita quando si invia la richiesta dopo che il controller è stato stoppato, da risolvere
        [HttpPost("startListener")]
        public IActionResult StartListener()
        {
            // Registra il gestore dei messaggi e gestore delle eccezioni
            _messageHandler.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            return Ok("Listener avviato.");
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Processa il messaggio
            string messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Messaggio ricevuto: SequenceNumber={message.SystemProperties.SequenceNumber} Body={messageBody}");

            try
            {
                // Completa il messaggio per rimuoverlo dalla coda
                await _messageHandler.CompleteAsync(message.SystemProperties.LockToken);

                // Deserializza il corpo del messaggio JSON in un oggetto WebhookMessage
                var webhookMessage = JsonConvert.DeserializeObject<WebhookMessage>(messageBody);

                // Invia il messaggio al webhook utilizzando il metodo SendMessageToWebhookAsync
                var response = await _webhookClient.SendMessageToWebhookAsync(webhookMessage);

                if (response != null && response.IsSuccessful)
                {
                    Console.WriteLine("Messaggio inviato con successo al webhook.");
                }
                else
                {
                    Console.WriteLine("Errore nell'invio del messaggio al webhook.");
                    Console.WriteLine(response?.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'elaborazione del messaggio: {ex.Message}");
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Eccezione: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }
}
