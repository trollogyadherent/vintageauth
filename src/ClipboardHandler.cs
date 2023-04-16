using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VintageAuth {
    public class ClipboardHandler {
        public static void CopyText(string text) {
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                CopyTextSync(text);
            }).Start();
        }
        public static void CopyTextSync(string text)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                System.Windows.Forms.Clipboard.SetText(text);
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                bool xselInstalled = false;
                bool wlCopyInstalled = false;
                try
                {
                    // Try running xsel to see if it is installed
                    var xselProcess = new Process();
                    xselProcess.StartInfo.FileName = "xsel";
                    xselProcess.StartInfo.Arguments = "-ib"; // copy to clipboard
                    xselProcess.StartInfo.UseShellExecute = false;
                    xselProcess.StartInfo.RedirectStandardInput = true;
                    xselProcess.Start();
                    xselInstalled = true;
                    xselProcess.StandardInput.Write(text);
                    xselProcess.StandardInput.Close();
                    xselProcess.WaitForExit();
                }
                catch (Exception)
                {
                    // xsel is not installed or failed to run
                    xselInstalled = false;
                }

                if (!xselInstalled)
                {
                    try
                    {
                        // Try running wl-copy to see if it is installed
                        var wlCopyProcess = new Process();
                        wlCopyProcess.StartInfo.FileName = "wl-copy";
                        wlCopyProcess.StartInfo.UseShellExecute = false;
                        wlCopyProcess.StartInfo.RedirectStandardInput = true;
                        wlCopyProcess.Start();
                        wlCopyInstalled = true;
                        wlCopyProcess.StandardInput.Write(text);
                        wlCopyProcess.StandardInput.Close();
                        wlCopyProcess.WaitForExit();
                    }
                    catch (Exception)
                    {
                        // wl-copy is not installed or failed to run
                        wlCopyInstalled = false;
                    }
                }

                if (xselInstalled || wlCopyInstalled)
                {
                    Console.WriteLine("VintageAuth: Text copied to clipboard!");
                }
                else
                {
                    Console.WriteLine("VintageAuth: Could not copy text to clipboard: neither xsel nor wl-copy is installed.");
                }
            }
        }
    }
}