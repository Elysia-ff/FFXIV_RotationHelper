using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public class RotationData
    {
        public string URL { get; private set; } = string.Empty;

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("sequence")]
        public List<int> Sequence { get; set; }

        public void SetURL(string url)
        {
            URL = url;
        }
    }

    public class SequenceConverter : JsonConverter<List<int>>
    {
        private const char separator = ',';

        public override void WriteJson(JsonWriter writer, List<int> value, JsonSerializer serializer)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < value.Count; ++i)
            {
                stringBuilder.Append(value[i]);
                stringBuilder.Append(separator);
            }

            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            writer.WriteValue(stringBuilder.ToString());
        }

        public override List<int> ReadJson(JsonReader reader, Type objectType, List<int> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();

            return value.Split(separator).ToList().ConvertAll((s) => int.Parse(s));
        }
    }
}
