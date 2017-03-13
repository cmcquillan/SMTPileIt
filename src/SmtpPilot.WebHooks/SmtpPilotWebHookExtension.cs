﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmtpPilot.Server;
using SmtpPilot.Server.Conversation;
using SmtpPilot.WebHooks.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmtpPilot.WebHooks
{
    internal class SmtpPilotWebHookExtension
    {
        private HttpClient _client;
        private CancellationTokenSource _cancellationTokenSource;
        private List<Task> _pendingTasks = new List<Task>();
        private string _webHookUrl;
        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        internal SmtpPilotWebHookExtension(SmtpPilotConfiguration config, string webHookUrl)
        {
            _webHookUrl = webHookUrl;

            if (!_webHookUrl.EndsWith("/"))
            {
                _webHookUrl = _webHookUrl + "/";
            }
            
            _cancellationTokenSource = new CancellationTokenSource();

            config.ServerEvents.EmailProcessed += EmailProcessedWebHook;
            config.ServerEvents.ServerStarted += ServerStarted;
            config.ServerEvents.ServerStopped += ServerStopped;
        }

        internal int MaxWebHookAttempts { get; set; }

        internal int WebHookRetryTimeFactor { get; set; }

        private void ServerStopped(object sender, ServerEventArgs eventArgs)
        {
            _cancellationTokenSource.Cancel();
            _client?.Dispose();
            _client = null;
        }

        private void ServerStarted(object sender, ServerEventArgs eventArgs)
        {
            _client = new HttpClient();
        }

        private void EmailProcessedWebHook(object sender, EmailProcessedEventArgs eventArgs)
        {
            _pendingTasks.RemoveAll(p => p.IsCompleted);
            _pendingTasks.Add(Task.Run(async () => await SendMessageToWebHook(eventArgs.Message, _cancellationTokenSource.Token)));
        }

        private async Task SendMessageToWebHook(IMessage message, CancellationToken token)
        {
            var evt = EmailProcessedServerEvent.CreateMessageProcessed(message);
            var data = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(evt, Formatting.None, _settings), token);
            string uri = _webHookUrl + "event/email";

            int attemptNumber = 0;
            HttpResponseMessage result = null;

            do
            {
                await Task.Delay(attemptNumber * WebHookRetryTimeFactor * 1000, token);
                attemptNumber++;

                if (token.IsCancellationRequested)
                    break;

                Debug.WriteLine($"Firing webhook to {uri}, attempt #{attemptNumber}", WebHookConstants.TraceCategory);

                try
                {
                    result = await _client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"), token);
                }
                catch (HttpRequestException) { }
                
            } while (result?.IsSuccessStatusCode != true && attemptNumber < MaxWebHookAttempts);
        }
    }
}
