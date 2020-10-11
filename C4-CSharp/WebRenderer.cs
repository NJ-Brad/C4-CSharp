using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace C4_CSharp
{
    public class WebRenderer
    {
        public async Task<byte[]> RenderAsync(string code)
        {
            string renderUrl = $"http://plantuml.com/plantuml/png/{code}";

            using (HttpClient httpClient = new HttpClient())
            {
                var result = await httpClient.GetAsync(renderUrl).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                }

                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    var messages = result.Headers.GetValues("X-PlantUML-Diagram-Error");
                    throw new Exception(string.Join(Environment.NewLine, messages));
                }

                throw new HttpRequestException(result.ReasonPhrase);
            }
        }

        public byte[] Render(string code)
        {
            return RenderAsync(code).GetAwaiter().GetResult();
        }
    }
}
