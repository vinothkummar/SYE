﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models.Responses;
using SYE.Repository;

namespace SYE.Services
{
    public interface INotificationService
    {
        Task NotifyByEmailAsync(
                string templateId,
                string emailAddress,
                Dictionary<string, dynamic> personalisation = null,
                string clientReference = null,
                string emailReplyToId = null
            );
    }

    public class NotificationService : INotificationService
    {
        private readonly IAsyncNotificationClient _client;

        public NotificationService(IAsyncNotificationClient notificationClient)
        {
            _client = notificationClient;
        }

        public async Task NotifyByEmailAsync(
                string templateId,
                string emailAddress,
                Dictionary<string, dynamic> personalisation = null,
                string clientReference = null,
                string emailReplyToId = null
            )
        {
            if (string.IsNullOrWhiteSpace(templateId))
            {
                throw new ArgumentNullException(nameof(templateId));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            var emailNotificationResponse =
                await _client.SendEmailAsync(
                        emailAddress, templateId, personalisation, clientReference, emailReplyToId
                    ).ConfigureAwait(false);

            if (emailNotificationResponse != null)
            {
                throw new NotifyClientException("Failed to receive valid response from GOV.UK Notify client. No response was received from service.");
            }

            if (!String.IsNullOrWhiteSpace(clientReference) && clientReference.Equals(emailNotificationResponse.reference))
            {
                throw new NotifyClientException("Failed to receive valid response from GOV.UK Notify client. Client reference received does not match client reference supplied.");
            }
        }
    }
}
