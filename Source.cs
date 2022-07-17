using GetMouse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiTool
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*
            if (args.Length == 0)
            {
                args = new string[] { "CLIPFTP" };
            }
            */

            CommandTools.CommandLine.CommandLine cl2 = new CommandTools.CommandLine.CommandLine(args);
            CommandTools.CommandLine.CommandLineFunctionTime TimeCommands = CommandTools.CommandLine.CommandLineFunctionTime.Use(cl2);
            cl2.AddCommand("CLIPFTP", "CLIPFTP,FTPCLIP");
            cl2.AddCommand("GOLD", "GOLD");
            cl2.AddCommand("PERMGOLD", "PERMGOLD");
            cl2.AddCommand("SENDKEY", "SENDKEY");
            cl2.AddCommand("FILM", "FILM");
            cl2.AddCommand("NEXTFILM", "NEXTFILM");
            cl2.AddCommand("NEXTSERIE", "NEXTSERIE");
            cl2.AddCommand("ENTZAUBERN", "ENTZAUBERN");

            TimeCommands.Calc(true);

            bool showWindow = true;

            if (cl2.Command("SENDKEY").UsedCount > 0)
            {
                string sendkey_cmd = "";
                string sendkey_parName = "";
                string sendkey_parValue = "";
                if (cl2.Command("SENDKEY").UsedParameter.Count > 0)
                {
                    foreach (CommandTools.CommandLine.Parameter skpara in cl2.Command("SENDKEY").UsedParameter)
                    {
                        if (skpara.Name == "Unknown")
                        {
                            sendkey_cmd = skpara.Value;
                        }
                        else
                        {
                            sendkey_parName = skpara.Name;
                            sendkey_parValue = skpara.Value;
                        }
                    }
                }
                if (sendkey_cmd == "PAUSE")
                {
                    SendKeys.SendWait("+^%{F9}");
                    SendKeys.SendWait("+^%{F5}");
                    SendKeys.SendWait("+^%{F5}");
                }
                else if (sendkey_cmd == "PLAY")
                {
                    SendKeys.SendWait("+^%{F11}");
                }
                else if (sendkey_cmd == "LEISER")
                {
                    SendKeys.SendWait("+^%{F6}");
                    SendKeys.SendWait("+^%{F6}");
                    SendKeys.SendWait("+^%{F6}");
                }
                else if (sendkey_cmd == "STUMM")
                {
                    SendKeys.SendWait("+^%{F7}");
                }
                else if (sendkey_cmd == "LAUTER")
                {
                    SendKeys.SendWait("+^%{F8}");
                    SendKeys.SendWait("+^%{F8}");
                    SendKeys.SendWait("+^%{F8}");
                }

                if (sendkey_parName == "CODE" || sendkey_parName == "KEYS" || sendkey_parName == "SEND")
                {
                    SendKeys.SendWait(sendkey_parValue);
                }
                showWindow = false;
            }
            if (cl2.Command("FILM").UsedCount > 0)
            {
                string[] filmListe = FilmListe();

                List<string> liste = new List<string>();

                foreach (string filmPath in filmListe)
                {
                    if (System.IO.Directory.Exists(filmPath))
                    {
                        AllFiles(filmPath, liste);
                    }
                }

                liste.Sort();
                if (liste.Count > 0)
                {
                    StartFilm(liste[0]);
                }

                showWindow = false;
            }
            if (cl2.Command("NEXTFILM").UsedCount > 0)
            {
                string[] filmListe = FilmListe();

                FilmCollection filmCol = new FilmCollection();

                string lastGP = "";
                foreach (string filmPath in filmListe)
                {
                    if (System.IO.Directory.Exists(filmPath))
                    {
                        AllFilesWithGesehenPath(filmPath, filmCol, ref lastGP);
                    }
                }
                List<Film> sorted = filmCol.SortedList();
                if (sorted.Count > 0)
                {
                    // Move Item 1 -> GesehenPath
                    string filename = System.IO.Path.GetFileName(sorted[0].Path);
                    string destFilename = System.IO.Path.Combine(sorted[0].gesehenPath, filename);

                    System.IO.File.Move(sorted[0].Path, destFilename);

                    if (sorted.Count > 1)
                    {
                        StartFilm(sorted[1].Path);
                    }
                }
                showWindow = false;
            }
            if (cl2.Command("NEXTSERIE").UsedCount>0 )
            {
                GetHTTP.Request();
            }
            if (cl2.Command("CLIPFTP").UsedCount > 0)
            {
                // Bild aus Zwischenablage 
                string filename = StringTools.TempFilename(".jpg");
                string filepath = Consts.LOCAL_TEMPPATH + filename;
                try
                {
                    Clipboard.SaveImage(filepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler : " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Online.UploadFile(filepath, Consts.FTP_USERNAME, Consts.FTP_PASSWORD, Consts.FTP_SERVERURI + "/www/grepolis/img/" + filename);
                System.Windows.Forms.Clipboard.SetText("[img]" + Consts.HTTP_ADDRESS + "/grepolis/img/" + filename + "[/img]");
                showWindow = false;
                MessageBox.Show("File uploadet - URL in Clipboard", "Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (cl2.Command("GOLD").UsedCount > 0)
            {
                CheckingGold1Time(1, true);

                showWindow = false;
            }
            if (cl2.Command("PERMGOLD").UsedCount > 0)
            {
                // Endlos prüfen bis Checkbox set
                Stop stopWin = new Stop();
                stopWin.TopMost = true;
                stopWin.Show();
                stopWin.Left = 0;
                stopWin.Top = 0;

                while (!stopWin.checkBox1.Checked)
                {
                    DateTime start = DateTime.Now;
                    if (CheckingGold1Time(1, false))
                    {
                        if (!frmMain.Continue)
                        {
                            break;
                        }
                    }

                    while (true)
                    {
                        if ((DateTime.Now - start).TotalSeconds >= 10 || stopWin.checkBox1.Checked)
                        {
                            break;
                        }
                        Application.DoEvents();
                    }

                }
                stopWin.Close();
                showWindow = false;
                Application.Exit();
            }
            if (cl2.Command("ENTZAUBERN").UsedCount > 0)
            {
                if (cl2.Command("ENTZAUBERN").UsedCount > 0)
                {
                    showWindow = false;
                    int itemStartX = 672;
                    int itemStartY = 672;
                    int itemDif = 72;

                    int EZx = 1100;
                    int EZy = 395;
                    int Zx = 815;
                    int Zy = 350;

                    
                        

    }
            }
            if (showWindow)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }

        private static string[] FilmListe()
        {
            string[] splitter = { "\r\n" };

            return System.IO.File.ReadAllText(@"D:\film_liste.txt").Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void StartFilm(string path)
        {
            Process.Start(@"C:\Program Files\VideoLAN\VLC\vlc.exe", "\"" + path + "\"");
            Thread.Sleep(5000);
            SendKeys.SendWait("+^%{F4}");
            Thread.Sleep(200);
            for (int i = 0; i < 22; i++)
            {
                SendKeys.SendWait("+^%{F6}");
            }
            for (int i = 0; i < 20; i++)
            {
                SendKeys.SendWait("+^%{F8}");
            }
        }

        private static void AllFilesWithGesehenPath(string path, FilmCollection filmCol, ref string lastGP)
        {
            string[] dirs = System.IO.Directory.GetDirectories(path);

            bool gpFound = false;
            foreach (string dir in dirs)
            {
                if (System.IO.Directory.Exists(dir))
                {
                    if ((dir.ToUpper().Contains("GESEHEN")))
                    {
                        gpFound = true;
                        lastGP = dir;
                    }
                }
            }
            if (!gpFound)
            {
                // kein Gesehen-Ordner, einen erstellen
                lastGP = System.IO.Path.Combine(path, "gesehen");
                System.IO.Directory.CreateDirectory(lastGP);
            }

            foreach (string dir in dirs)
            {
                if (System.IO.Directory.Exists(dir))
                {
                    if ((dir.ToUpper().Contains("GESEHEN")))
                    {
                        lastGP = dir;
                    }
                    else
                    {
                        if (!(dir.ToUpper().Contains("_HIDE")))
                        {
                            AllFilesWithGesehenPath(dir, filmCol, ref lastGP);
                        }
                    }
                }
            }

            string[] files = System.IO.Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    string ext = System.IO.Path.GetExtension(file).ToUpper();
                    if (ext == ".AVI" || ext == ".MKV" || ext == ".MPG" || ext == ".MOV" || ext == ".MP4")
                    {
                        if (!(file is null))
                        {
                            filmCol.AddFilm(file, lastGP);
                        }
                    }
                }
            }
        }

        private static void AllFiles(string path, List<string> liste)
        {
            string[] dirs = System.IO.Directory.GetDirectories(path);

            foreach (string dir in dirs)
            {
                if (System.IO.Directory.Exists(dir))
                {
                    if (!(dir.ToUpper().Contains("GESEHEN")))
                    {
                        if (!(dir.ToUpper().Contains("_HIDE")))
                        {
                            AllFiles(dir, liste);
                        }
                    }
                }
            }

            string[] files = System.IO.Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    string ext = System.IO.Path.GetExtension(file).ToUpper();
                    if (ext == ".AVI" || ext == ".MKV" || ext == ".MPG" || ext == ".MOV")
                    {
                        if (!(file is null))
                        {
                            liste.Add(file);
                        }
                    }
                }
            }
        }
        static int GOLDDIFFALARM = 200;
        private static bool CheckingGold1Time(int alertCount, bool speakNix)
        {
            // Von 593, 363 bis 695, 363 markieren
            CommandTools.Windows.PCControl.LeftClick(535, 266);
            Thread.Sleep(15);

            int foundHolz = 0;
            int foundStein = 0;
            int foundSilber = 0;
            System.Speech.Synthesis.SpeechSynthesizer sp = new System.Speech.Synthesis.SpeechSynthesizer();

            foundHolz = CheckGold(593, 363, 695);
            foundStein = CheckGold(761, 363, 862);
            foundSilber = CheckGold(930, 363, 1032);

            string st = "";

            int maxHolz = Math.Abs(foundHolz);
            int maxStein = Math.Abs(foundStein);
            int maxSiler = Math.Abs(foundSilber);

            Debug.Print("HOLZ = " + foundHolz.ToString() + " / STEIN = " + foundStein.ToString() + " / SILBER = " + foundSilber.ToString());

            if (foundHolz < 0)
                foundHolz = 0;
            if (foundStein < 0)
                foundStein = 0;
            if (foundSilber < 0)
                foundSilber = 0;

            Debug.Print(">> HOLZ = " + foundHolz.ToString() + " / STEIN = " + foundStein.ToString() + " / SILBER = " + foundSilber.ToString());

            if (foundHolz + foundStein + foundSilber > 0)
            {
                int kapa = CheckKapazitaet(sp);

                GoldInfo gi = new GoldInfo(foundHolz.ToString(), foundStein.ToString(), foundSilber.ToString());
                gi.Show();
                gi.Left = 540;
                gi.Top = 228;
                gi.TopMost = true;

                Application.DoEvents();

                ClearAllGold();
                if (foundHolz > foundSilber)
                {
                    // entweder Holz oder Stein
                    if (foundHolz > foundStein)
                    {
                        // HOLZ
                        CommandTools.Windows.PCControl.LeftClick(593 + 50, 389);
                        Application.DoEvents();
                        Thread.Sleep(15);
                        SendKeys.SendWait(foundHolz.ToString());
                        st = "Am meisten Holz mit " + foundHolz.ToString();
                        if (foundStein > 0)
                        {
                            st += " und " + foundStein.ToString() + " Stein";
                        }
                        if (foundSilber > 0)
                        {
                            st += " und " + foundSilber.ToString() + " Silber";
                        }
                    }
                    else
                    {
                        // STEIN
                        CommandTools.Windows.PCControl.LeftClick(761 + 50, 389);
                        Application.DoEvents();
                        Thread.Sleep(15);
                        SendKeys.SendWait(foundStein.ToString());
                        st = "Am meisten Stein mit " + foundStein.ToString();
                        if (foundHolz > 0)
                        {
                            st += " und " + foundHolz.ToString() + " Holz";
                        }
                        if (foundSilber > 0)
                        {
                            st += " und " + foundSilber.ToString() + " Silber";
                        }
                    }
                }
                else
                {
                    // entweder Silber oder Stein
                    if (foundSilber > foundStein)
                    {
                        // SILBER
                        CommandTools.Windows.PCControl.LeftClick(930 + 50, 389);
                        Application.DoEvents();
                        Thread.Sleep(15);
                        SendKeys.SendWait(foundSilber.ToString());
                        st = "Am meisten Silber mit " + foundSilber.ToString();
                        if (foundStein > 0)
                        {
                            st += " und " + foundStein.ToString() + " Stein";
                        }
                        if (foundHolz > 0)
                        {
                            st += " und " + foundHolz.ToString() + " Holz";
                        }
                    }
                    else
                    {
                        // STEIN
                        CommandTools.Windows.PCControl.LeftClick(761 + 50, 389);
                        Application.DoEvents();
                        Thread.Sleep(15);
                        SendKeys.SendWait(foundStein.ToString());
                        st = "Am meisten Stein mit " + foundStein.ToString();
                        if (foundHolz > 0)
                        {
                            st += " und " + foundHolz.ToString() + " Holz";
                        }
                        if (foundSilber > 0)
                        {
                            st += " und " + foundSilber.ToString() + " Silber";
                        }
                    }
                }

                sp.SpeakAsync(st);
                Application.DoEvents();
                for (int i = 0; i < alertCount; i++)
                {
                    System.Media.SystemSounds.Beep.Play();
                    if (i < alertCount - 1)
                    {
                        Thread.Sleep(1000);
                    }
                }

                DateTime closeTimer = DateTime.Now;
                DateTime pingTimer = DateTime.Now;

                frmMain.Continue = false;
                while (true)
                {
                    if (!gi.Visible)
                    {
                        break;
                    }
                    Application.DoEvents();
                    Thread.Sleep(50);
                    if ((DateTime.Now - closeTimer).TotalSeconds > 60)
                    {
                        frmMain.Continue = true;
                        gi.Close();
                    }
                    TimeSpan pingSpan = DateTime.Now - pingTimer;
                    if (pingSpan.TotalSeconds >= 5)
                    {
                        pingTimer = DateTime.Now;
                        System.Media.SystemSounds.Beep.Play();
                    }
                }

                return !frmMain.Continue;
            }
            else
            {
                if (speakNix)
                {
                    int sum = maxHolz + maxStein + maxSiler;

                    if (sum < 100)
                    {
                        st = "Nein " + (maxHolz + maxStein + maxSiler).ToString();
                    }
                    else
                    {
                        sum /= 100;
                        st = "Nein " + sum.ToString() + " Hundert";
                    }
                    sp.Speak(st);
                }
            }

            return false;
        }

        private static void ClearAllGold()
        {
            CommandTools.Windows.PCControl.LeftClick(593 + 50, 389);
            CommandTools.Windows.PCControl.LeftClick(593 + 50, 389);
            Application.DoEvents();
            Thread.Sleep(50);
            SendKeys.SendWait("{del}");
            Application.DoEvents();
            Thread.Sleep(50);

            CommandTools.Windows.PCControl.LeftClick(761 + 50, 389);
            CommandTools.Windows.PCControl.LeftClick(761 + 50, 389);
            Application.DoEvents();
            Thread.Sleep(50);
            SendKeys.SendWait("{del}");
            Application.DoEvents();
            Thread.Sleep(50);

            CommandTools.Windows.PCControl.LeftClick(930 + 50, 389);
            CommandTools.Windows.PCControl.LeftClick(930 + 50, 389);
            Application.DoEvents();
            Thread.Sleep(50);
            SendKeys.SendWait("{del}");
            Application.DoEvents();
            Thread.Sleep(50);

        }

        private static int CheckGold(int x1, int y, int x2)
        {
            CommandTools.Windows.PCControl.Move(x1, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.LeftDown(x1, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.Move(x2, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.LeftUp(x2, y);
            Application.DoEvents();
            Thread.Sleep(25);
            SendKeys.SendWait("^c");
            Application.DoEvents();
            Thread.Sleep(25);
            string text = System.Windows.Forms.Clipboard.GetText();
            Thread.Sleep(20);
            return CheckGold(text);
        }

        private static int CheckKapazitaet(System.Speech.Synthesis.SpeechSynthesizer sp)
        {
            int x1 = 737;
            int y = 319;
            int x2 = 926;

            int k = 0;

            CommandTools.Windows.PCControl.Move(x1, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.LeftDown(x1, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.Move(x2, y);
            Application.DoEvents();
            Thread.Sleep(25);
            CommandTools.Windows.PCControl.LeftUp(x2, y);
            Application.DoEvents();
            Thread.Sleep(25);
            SendKeys.SendWait("^c");
            Application.DoEvents();
            Thread.Sleep(25);
            string text = System.Windows.Forms.Clipboard.GetText();
            Thread.Sleep(20);
            if (text.Contains(":"))
            {
                text = text.Substring(text.IndexOf(":") + 1).Trim();
                string[] kapa = text.Split(new char[] { '/' });
                if (kapa.Length == 2)
                {
                    int kapaIst, kapaMax;
                    bool ok = int.TryParse(kapa[0], out kapaIst);
                    if (ok)
                    {
                        k = kapaIst;
                        ok = int.TryParse(kapa[1], out kapaMax);
                        if (ok)
                        {
                            if (kapaIst >= kapaMax * 0.9)
                            {
                                // 90% oder mehr
                                sp.SpeakAsync("Kapazität gut");
                            }
                            else if (kapaIst >= kapaMax * 0.5)
                            {
                                // 50% - 90%
                                sp.SpeakAsync("Kapazität mittel");
                            }
                            else if (kapaIst >= kapaMax * 0.2)
                            {
                                sp.SpeakAsync("Kapazität gering");
                            }
                            else
                            {
                                sp.SpeakAsync("Keine Kapazität!");
                            }
                        }
                    }

                }
            }
            return k;
        }

        private static int CheckGold(string werte)
        {
            if (werte != null)
            {
                if (werte.Contains("/"))
                {
                    string[] zahlen = werte.Split(new char[] { '/' });
                    if (zahlen.Length == 2)
                    {
                        bool ok;
                        int zahl1, zahl2;
                        ok = int.TryParse(zahlen[0], out zahl1);
                        if (ok)
                        {
                            ok = int.TryParse(zahlen[1], out zahl2);
                            if (ok)
                            {
                                int diff = zahl2 - zahl1;

                                if (diff > GOLDDIFFALARM)
                                {
                                    return diff;
                                }
                                else
                                {
                                    return 0 - diff;
                                }
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}
