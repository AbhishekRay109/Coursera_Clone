//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Jose;
//using Newtonsoft;
//using Kitana.Api.KitanaCore.Models;
//using Newtonsoft.Json;
//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json.Linq;
//using System.Net;
//using Amazon.Runtime.Internal.Util;
//using BaseCodeSetup.Core.logger;
//using Kitana.Core.Logger;
//using Microsoft.AspNetCore.Mvc.Controllers;

//namespace Kitana.Service.Services.ExternalServices
//{
//    public class CyraxService
//    {
//        private readonly IHttpContextAccessor httpContextAccessor;
//        private readonly ISession _session;
//        private readonly RaidenService _raidenService;
//        private readonly string path;
//        private readonly string cyrax_AppCode;
//        private readonly CloudWatchLogger _cwLogger;
//        public CyraxService(IHttpContextAccessor accessor, RaidenService raidenService, CloudWatchLogger CWLogger, Serilog.Core.Logger logger)
//        {
//            _cwLogger = CWLogger;
//            httpContextAccessor = accessor;
//            _session = httpContextAccessor.HttpContext.Session;
//            _raidenService = raidenService;
//            var cyrax_url = Environment.GetEnvironmentVariable("Cyrax_Url");
//            cyrax_AppCode = Environment.GetEnvironmentVariable("Cyrax_AppCode");
//            path = cyrax_url + "/" + cyrax_AppCode;
//        }

//        private async Task<string> SendEmail(string eventCode, string organizationCode, string applicationCode, int retention, int pagination)
//        {
//            try
//            {
//                var token = await _raidenService.GetAppToken();
//                var email = _session.GetString("Email");
//                var appEmail = _session.GetString("ApplicationEmail");
//                var orgEmail = _session.GetString("OrganizationEmail");
//                var client = new HttpClient();
//                var request = new HttpRequestMessage(HttpMethod.Post, path);
//                request.Headers.Add("accept", "text/plain");
//                request.Headers.Add($"Authorization", $"Bearer {token}");
//                var emailData = new List<object>();

//                if (!string.IsNullOrEmpty(email))
//                {
//                    emailData.Add(new
//                    {
//                        recipientEmail = email,
//                        metadata = new
//                        {
//                            orgCode = organizationCode,
//                            appCode = applicationCode,
//                            retention = retention.ToString(),
//                            pagination = pagination.ToString()
//                        }
//                    });
//                }

//                if (!string.IsNullOrEmpty(orgEmail))
//                {
//                    emailData.Add(new
//                    {
//                        recipientEmail = orgEmail,
//                        metadata = new
//                        {
//                            orgCode = organizationCode,
//                            appCode = applicationCode,
//                            retention = retention.ToString(),
//                            pagination = pagination.ToString()
//                        }
//                    });
//                }

//                if (!string.IsNullOrEmpty(appEmail))
//                {
//                    emailData.Add(new
//                    {
//                        recipientEmail = appEmail,
//                        metadata = new
//                        {
//                            orgCode = organizationCode,
//                            appCode = applicationCode,
//                            retention = retention.ToString(),
//                            pagination = pagination.ToString()
//                        }
//                    });
//                }

//                var content = new StringContent(
//                    JsonConvert.SerializeObject(new
//                    {
//                        eventCode,
//                        appCode = this.cyrax_AppCode,
//                        emailData
//                    }),
//                    Encoding.UTF8,
//                    "application/json"
//                );
//                request.Content = content;
//                var response = await client.SendAsync(request);
//                await _cwLogger.log($"Mail request : {request}", null);
//                await _cwLogger.log($"Mail sent successully : {response}", null);
//                response.EnsureSuccessStatusCode();
//                return await response.Content.ReadAsStringAsync();
//            }
//            catch (Exception ex)
//            {
//                await _cwLogger.log($"Could not send mail {ex.Message}", null);
//                return null;
//            }
//        }

//        public async Task<string> SendEmailToOrganizationDeletion(string eventCode, string organizationCode)
//        {
//            return await SendEmail(eventCode, organizationCode, null, 0, 0);
//        }

//        public async Task<string> SendEmailToOrganization(string eventCode, string organizationCode, Plan plan)
//        {
//            var planType = JsonConvert.SerializeObject(plan).ToString();
//            return await SendEmail(eventCode, organizationCode, null, 0, 0);
//        }

//        public async Task<string> SendEmailToApplication(string eventCode, string organizationCode, string applicationCode, int retention, int pagination)
//        {
//            return await SendEmail(eventCode, organizationCode, applicationCode, retention, pagination);
//        }

//        public async Task<string> SendEmailToApplicationDeletion(string eventCode, string organizationCode, string applicationCode)
//        {
//            return await SendEmail(eventCode, organizationCode, applicationCode, 0, 0);
//        }
//    }
//}