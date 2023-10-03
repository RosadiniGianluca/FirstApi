using FirstApi.Entities;
using RestSharp;

namespace FirstApi.Clients
{
    public class WebhookClient
    {
        private readonly RestClient _client;

        public WebhookClient(string webhookUrl)
        {
            _client = new RestClient(webhookUrl);
        }

        public void SendPostRequest(object data)
        {
            RestRequest request = new RestRequest("/resources/", Method.Post);  // /resources/ è il path del webhook, Method.Post è il metodo HTTP, in questo caso POST, che vogliamo utilizzare, ma potrebbe essere anche GET, PUT, DELETE, ecc.
            request.AddJsonBody(data);  // Aggiungi il corpo della richiesta, in questo caso il corpo è un oggetto JSON, ma potrebbe essere anche un oggetto XML, un file, ecc., a seconda del tipo di richiesta che si vuole inviare

            RestResponse response = _client.Execute(request);  // Esegui la richiesta e salva la risposta in una variabile di tipo IRestResponse (interfaccia) che contiene i dati della risposta HTTP (codice di stato, corpo, intestazioni, ecc.)
            
            if (response.IsSuccessful)
            {
                // La richiesta è andata a buon fine
                Console.WriteLine("Messaggio inviato con successo.");
            }
            else
            {
                // La richiesta ha avuto esito negativo
                Console.WriteLine("Errore nell'invio del messaggio.");
                Console.WriteLine(response.ErrorMessage);
            }
        }

        public async Task<RestResponse> SendMessageToWebhookAsync(WebhookMessage messageData)
        {
            RestRequest request = new RestRequest("/resources/", Method.Post);
            request.AddJsonBody(messageData);

            try
            {
                RestResponse response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // La richiesta è andata a buon fine
                    Console.WriteLine("Messaggio inviato con successo.");
                }
                else
                {
                    // La richiesta ha avuto esito negativo
                    Console.WriteLine("Errore nell'invio del messaggio.");
                    Console.WriteLine(response.ErrorMessage);
                }
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Errore durante l'invio della richiesta: {ex.Message}");
                return null;
            }
        }
    }
}
