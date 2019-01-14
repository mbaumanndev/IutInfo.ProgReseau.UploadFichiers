using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.UploadFichiers.WebUi.Utilities
{
    public sealed class MergeManager
    {
        private const string PART_TOKEN = ".part_";

        public void Merge(string cheminDeFragment)
        {
            string v_BaseFile = cheminDeFragment.Substring(0, cheminDeFragment.IndexOf(PART_TOKEN));
            string v_Tokens = cheminDeFragment.Substring(cheminDeFragment.IndexOf(PART_TOKEN) + PART_TOKEN.Length);
            
            int total = 0;
            int.TryParse(v_Tokens.Substring(v_Tokens.IndexOf(".") + 1), out total);

            string v_SearchPattern = Path.GetFileName(v_BaseFile) + PART_TOKEN + "*";
            string[] v_Files = Directory.GetFiles(Path.GetDirectoryName(v_BaseFile), v_SearchPattern);

            if (v_Files.Count() == total)
            {
                if (!MergeSingleton.Instance.IsInUse(v_BaseFile))
                {
                    MergeSingleton.Instance.Use(v_BaseFile);

                    List<FichierTrie> v_FichiersTries = new List<FichierTrie>();
                    foreach (var file in v_Files)
                    {
                        v_FichiersTries.Add(new FichierTrie
                        {
                            Chemin = file,
                            NumeroFichier = RecupererNumero(file)
                        });
                    }

                    List<FichierTrie> v_Finale = v_FichiersTries.OrderBy(ft => ft.NumeroFichier).ToList();

                    using (var fichierFinal = new FileStream(v_BaseFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        foreach (var v_Fragement in v_Finale)
                        {
                            int v_Tentatives = 0;

                            while(true)
                            {
                                v_Tentatives++;

                                try
                                {
                                    using (var fragment = new FileStream(v_Fragement.Chemin, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        fragment.CopyTo(fichierFinal);
                                    }

                                    break;
                                }
                                catch (Exception)
                                {
                                    if (v_Tentatives > 10)
                                        throw new Exception();

                                    Thread.Sleep(500);
                                }
                            }
                        }
                    }

                    MergeSingleton.Instance.Free(v_BaseFile);
                }
            }
        }

        private int RecupererNumero(string file)
        {
            string v_Tokens = file.Substring(file.IndexOf(PART_TOKEN) + PART_TOKEN.Length);

            int numero = 0;
            int.TryParse(v_Tokens.Substring(0, v_Tokens.IndexOf(".")), out numero);

            return numero;
        }
    }
}
