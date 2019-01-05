using System;
using System.Collections.Generic;

namespace Pocoyo
{
    public class PocoyoData
    {
        public string DefaultNamespace { get; set; }
        public List<string> Excluded { get; set; }
        public List<string> ExcludedAttributes { get; set; }
        public List<string> KnownTypes { get; set; }

        public List<string> Namespaces { get; set; }

        public Dictionary<string, string> DiscoveredTypes { get; set; }
        public Dictionary<string, string> ExcludedTypes { get; set; }

        public static void LoadFromData(string filePath)
        {
            var data = Json.DeserializeFromFile<PocoyoData>(filePath);
            if (data != null)
            {
                Pocoyo.DefaultNamespace = data.DefaultNamespace;
                Pocoyo.Excluded = data.Excluded;
                Pocoyo.ExcludedAttributes = data.ExcludedAttributes;
                Pocoyo.KnownTypes = data.KnownTypes;
                Pocoyo.Namespaces = data.Namespaces;
                Pocoyo.DiscoveredTypes = data.DiscoveredTypes;
                Pocoyo.ExcludedTypes = data.ExcludedTypes;
            }
        }

        public void SaveToData(string filePath)
        {
            try
            {
                DefaultNamespace = Pocoyo.DefaultNamespace;
                Excluded = Pocoyo.Excluded;
                ExcludedAttributes = Pocoyo.ExcludedAttributes;
                KnownTypes = Pocoyo.KnownTypes;
                Namespaces = Pocoyo.Namespaces;
                DiscoveredTypes = Pocoyo.DiscoveredTypes;
                ExcludedTypes = Pocoyo.ExcludedTypes;

                Json.SerializeToFile(this, filePath);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
