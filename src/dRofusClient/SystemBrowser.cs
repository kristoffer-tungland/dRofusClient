using System.Net;
using System.Text;
using System.Diagnostics;
using Duende.IdentityModel.OidcClient.Browser;

namespace dRofusClient
{
    public class SystemBrowser(int port = 7890) : IBrowser
    {
        public int Port { get; } = port;

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            string redirectUri = $"http://127.0.0.1:{Port}/";
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(redirectUri);
                listener.Start();

                // Open system browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = options.StartUrl,
                    UseShellExecute = true
                });

                try
                {
                    var contextTask = listener.GetContextAsync();
                    if (await Task.WhenAny(contextTask, Task.Delay(300000, cancellationToken)) != contextTask)
                    {
                        return new BrowserResult
                        {
                            ResultType = BrowserResultType.Timeout,
                            Error = "Timeout waiting for browser response."
                        };
                    }

                    var context = contextTask.Result;
                    string responseString = "<html><head><script>setTimeout(function(){ window.close(); }, 1000);</script></head><body>You may now return to the app. This window will close automatically.</body></html>";
                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                    context.Response.OutputStream.Close();

                    string rawUrl = context.Request.RawUrl ?? "";
                    string fullUrl = redirectUri.TrimEnd('/') + rawUrl;

                    return new BrowserResult
                    {
                        Response = fullUrl,
                        ResultType = BrowserResultType.Success
                    };
                }
                catch (Exception ex)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = ex.Message
                    };
                }
                finally
                {
                    listener.Stop();
                }
            }
        }
    }
}