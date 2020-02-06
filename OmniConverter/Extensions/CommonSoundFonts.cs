using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace OmniConverter
{
    class CommonSoundFonts
    {
        private static string CSFFixedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Common SoundFonts\\SoundFontList.csflist";

        private static string GetName(string value)
        {
            int A = value.IndexOf(" = ");
            if (A == -1) return value;
            return value.Substring(0, A);
        }

        private static string GetValue(string value)
        {
            int A = value.LastIndexOf(" = ");
            if (A == -1) return "";
            int A2 = A + (" = ").Length;
            if (A2 >= value.Length) return "";
            return value.Substring(A2);
        }

        public static bool LoadCSF()
        {
            Program.SFArray.List = new List<SoundFont>();

            Debug.PrintToConsole("ok", "Checking if Common SoundFonts list exists...");
            if (File.Exists(CSFFixedPath))
            {
                Debug.PrintToConsole("ok", "It does, loading to memory...");

                MemoryStream MS = new MemoryStream(File.ReadAllBytes(CSFFixedPath));
                using (StreamReader SFR = new StreamReader(MS))
                {
                    string SF = null;
                    bool AI = false, ES = false, XG = false;
                    int BV = -1, PV = -1, DBV = 0, DPV = -1, DBLSBV = 0;

                    SoundFont iSF;

                    String L;
                    Int64 Count = 1;
                    while ((L = SFR.ReadLine()) != null)
                    {
                        try
                        {
                            if (L.Equals("sf.start"))
                            {
                                if (AI) continue;

                                // Start of SoundFont item detected
                                AI = true;
                            }
                            else if (L.Equals("sf.end"))
                            {
                                if (!AI) continue;

                                if (ES)
                                {
                                    Debug.PrintToConsole("ok", "============================");
                                    Debug.PrintToConsole("ok", String.Format("SoundFont {0}", Count));
                                    Debug.PrintToConsole("ok", String.Format("SFP = {0}", SF));
                                    Debug.PrintToConsole("ok", String.Format("SP = {0}", PV));
                                    Debug.PrintToConsole("ok", String.Format("SB = {0}", BV));
                                    Debug.PrintToConsole("ok", String.Format("DP = {0}", DPV));
                                    Debug.PrintToConsole("ok", String.Format("DB = {0}", DBV));
                                    Debug.PrintToConsole("ok", String.Format("DBLSB = {0}", DBLSBV));
                                    Debug.PrintToConsole("ok", String.Format("XG = {0}", XG));
                                    Debug.PrintToConsole("ok", String.Format("Enabled = {0}", ES));
                                    Debug.PrintToConsole("ok", "============================");

                                    // Add to the list
                                    iSF = new SoundFont(SF, PV, BV, DPV, DBV, DBLSBV, ES, XG);
                                    Program.SFArray.List.Add(iSF);

                                    Count++;
                                }

                                SF = null;
                            }
                            else if (GetName(L).Equals("sf.path"))
                            {
                                if (!AI | SF != null) continue;

                                SF = GetValue(L);
                            }
                            else if (GetName(L).Equals("sf.enabled"))
                            {
                                if (!AI) continue;

                                ES = Convert.ToBoolean(Convert.ToInt32(GetValue(L)));
                            }
                            else if (GetName(L).Equals("sf.xgdrums"))
                            {
                                if (!AI) continue;

                                XG = Convert.ToBoolean(Convert.ToInt32(GetValue(L)));
                            }
                            else if (GetName(L).Equals("sf.srcb"))
                            {
                                if (!AI) continue;

                                BV = Convert.ToInt32(GetValue(L));
                            }
                            else if (GetName(L).Equals("sf.srcp"))
                            {
                                if (!AI) continue;

                                PV = Convert.ToInt32(GetValue(L));
                            }
                            else if (GetName(L).Equals("sf.desb"))
                            {
                                if (!AI) continue;

                                DBV = Convert.ToInt32(GetValue(L));
                            }
                            else if (GetName(L).Equals("sf.desp"))
                            {
                                if (!AI) continue;

                                DPV = Convert.ToInt32(GetValue(L));
                            }
                            else if (GetName(L).Equals("sf.desblsb"))
                            {
                                if (!AI) continue;

                                DBLSBV = Convert.ToInt32(GetValue(L));
                            }
                            else if (L.Contains("//") || L.Contains('#') || String.IsNullOrWhiteSpace(L)) continue;
                        }
                        catch { }
                    }
                }
                MS.Dispose();

                Debug.PrintToConsole("ok", "Common SoundFonts list successfully loaded.");
                return true;
            }

            Debug.PrintToConsole("ok", "It doesn't.");
            return false;
        }
    }
}
