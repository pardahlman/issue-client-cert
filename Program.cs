using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Issue.Client.Cert
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Making an HTTPS call to Google without client certificate.");
            using var googleHttpClient = new HttpClient();
            await googleHttpClient.GetAsync("https://google.com");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(GetLeafCertAndAddIntermediateToStore());
            using var client = new HttpClient(handler);

            try
            {
                await client.GetAsync("https://mss.cpc.getswish.net/swish-cpcapi/api/v1/paymentrequests/5D59DA1B1632424E874DDB219AD54597");
                Console.WriteLine($"The first HTTP request succeeded.");
            }
            catch (Exception e) when(ExceptionIsSslAlertHandshakeFailure(e))
            {
                Console.WriteLine($"The first HTTP request failed with 'sslv3 alert handshake failure'.");
            }

            Console.WriteLine("Waiting for 2 seconds, a direct call will fail again.");
            await Task.Delay(TimeSpan.FromSeconds(2));

            try
            {
                await client.GetAsync("https://mss.cpc.getswish.net/swish-cpcapi/api/v1/paymentrequests/5D59DA1B1632424E874DDB219AD54597");
                Console.WriteLine($"The second HTTP request succeeded.");
            }
            catch (Exception e) when(ExceptionIsSslAlertHandshakeFailure(e))
            {
                Console.WriteLine($"The second HTTP request failed with 'sslv3 alert handshake failure'.");
            }
        }

        private static bool ExceptionIsSslAlertHandshakeFailure(Exception e)
            => e.InnerException?.InnerException?.InnerException?.Message?.Contains("sslv3 alert handshake failure") ?? false;

        private static X509Certificate2 GetLeafCertAndAddIntermediateToStore()
        {
            var certificateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "swish_client_certificate_test.p12");
            Console.WriteLine($"Loading certificate from {certificateFilePath}.");

            var certificateChain = new X509Certificate2Collection();
            certificateChain.Import(certificateFilePath, "swish", X509KeyStorageFlags.DefaultKeySet);
            X509Certificate2 clientCertificate = default;

            using var userCertificateAuthorityStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
            userCertificateAuthorityStore.Open(OpenFlags.ReadWrite);
            Console.WriteLine($"Certificate chain consists of {certificateChain.Count} entries.");

            foreach (var certificate in certificateChain)
            {
                if (certificate.HasPrivateKey)
                {
                    if (clientCertificate != null)
                    {
                        throw new Exception($"Found more than one client certificate with private key: '{clientCertificate.Subject}' and '{certificate.Subject}'");
                    }

                    clientCertificate = certificate;
                }
                else
                {
                    userCertificateAuthorityStore.Add(certificate);
                }
            }

            if (clientCertificate == null)
            {
                throw new Exception($"Unable to find client certificate in {certificateFilePath}.");
            }

            return clientCertificate;
        }
    }
}
