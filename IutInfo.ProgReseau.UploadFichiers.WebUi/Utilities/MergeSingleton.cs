using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.UploadFichiers.WebUi.Utilities
{
    public class MergeSingleton
    {
        private MergeSingleton()
        {
            try
            {
                Files = new List<string>();
            }
            catch (Exception) { }
        }

        internal static readonly Lazy<MergeSingleton> lazyInstance =
            new Lazy<MergeSingleton>(() => new MergeSingleton());

        public static MergeSingleton Instance => lazyInstance.Value;

        private List<string> Files { get; }

        public bool IsInUse(string p_Path)
        {
            return Files.Contains(p_Path);
        }

        public void Use(string p_Path)
        {
            Files.Add(p_Path);
        }

        public void Free(string p_Path)
        {
            Files.Remove(p_Path);
        }
    }
}
