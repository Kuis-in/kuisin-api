using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace Kuisin.Infrastructure.Extensions
{
    internal class CosmosJsonTextSerializer : CosmosSerializer
    {
        private readonly JsonSerializerOptions? _serializerOptions;

        public CosmosJsonTextSerializer(JsonSerializerOptions? serializerOptions = null)
        {
            _serializerOptions = serializerOptions;
        }

        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                return JsonSerializer.Deserialize<T>(stream, _serializerOptions)!;
            }
        }

        public override Stream ToStream<T>(T input)
        {
            var outputStream = new MemoryStream();

            JsonSerializer.Serialize(outputStream, input, _serializerOptions);

            outputStream.Position = 0;
            return outputStream;
        }
    }
}
