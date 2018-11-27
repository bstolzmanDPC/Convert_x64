using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using FeatureCAM;
using System.Threading;

namespace Convert_x64
{
    
    public class FC_Addin
    {
        #region FeatureCAM Software Prep Stuff

        static public FeatureCAM.Application Application { get; set; }

        static public IFMDllAddIn Addin { get; set; }
       
        static public void OnConnect(object obj, tagFMAddInFlags flags)
        {
            Application.CommandBars.CreateButton("Utilities", "Convert_x64", tagFMMacroButtonFaceId.eMBFID_VerticalScroll);
        }

        static public void OnDisConnect(tagFMAddInFlags flags)
        {
            Application.CommandBars.DeleteButton("Utilities", "Convert_x64");
        }

        static public void AttachEventHandlers()
        {
            ((_IFMDllAddInEvents_Event)Addin).OnConnect += new _IFMDllAddInEvents_OnConnectEventHandler(OnConnect);
            ((_IFMDllAddInEvents_Event)Addin).OnDisConnect += new _IFMDllAddInEvents_OnDisConnectEventHandler(OnDisConnect);
        }

        static public void RemoveEventHandlers()
        {
            ((_IFMDllAddInEvents_Event)Addin).OnConnect -= new _IFMDllAddInEvents_OnConnectEventHandler(OnConnect);
            ((_IFMDllAddInEvents_Event)Addin).OnDisConnect -= new _IFMDllAddInEvents_OnDisConnectEventHandler(OnDisConnect);
        }

        #endregion

        static private StreamWriter Attempted, Saved;
        static private FMDocument doc;
        static private FMProgressBar PB;

        [STAThread]  
        static public void Convert_x64()
        {
            GetFC();

            Dialog dlg = new Dialog();

            if(dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File f = new File();

                    f = dlg.GetFileInfo();

                    Attempted = new StreamWriter(f.LogPath + "\\files_attemped.log");
                    Saved = new StreamWriter(f.LogPath + "\\files_saved.log");

                    SaveAllFiles(f.Path);

                    Attempted.Close();
                    Saved.Close();
                }
                catch(OperationCanceledException)
                {
                    return;
                }
                catch(Exception)
                {
                    string message = "There was an internal error that caused this Addin to malfunction. Sorry for the inconvenience.";
                    string caption = "Internal Error";
                    MessageBoxIcon icon = MessageBoxIcon.Error;
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons, icon);
                }
                finally
                {
                    Thread.Sleep(1500);
                    PB.Finished();
                }

            }


        }
       
        static private void GetFC()
        {
            Application = (FeatureCAM.Application)Marshal.GetActiveObject("FeatureCAM.Application");
        }

        static private void SaveAllFiles(string folder)
        {
            //Recursive function that loops through the specified directory and opens each of the
            //featurecam files. Each file gets logged in the attempted log file, if its saved then
            //it gets logged into the saved file

            string[] dirs = Directory.GetDirectories(folder);

            //Recursively call the function until at base subdirectory
            foreach(string dir in dirs)
            {
                SaveAllFiles(dir);
            }

            //GEt collection of files in the directory
            var files = Directory.GetFiles(folder);

            ShowPB(); 
            InitializePB(folder);

            //loop through each file in the collection and attempt to save it.
            foreach(string file in files)
            {
                UpdatePB(file);

                string extension = Path.GetExtension(file);

                //Could have used a big 'if' statement condition but didnt feel like writing it out.
                switch(extension.ToLower())
                {
                    case ".fm":
                    case ".tsf":
                    case ".mf":
                    case ".md":
                        Attempted.WriteLine(file);
                        doc = (FMDocument)Application.Documents.Open(file);

                        if(doc != null)
                        {
                            doc.Save();
                            doc.Close();
                            Saved.WriteLine(file);
                        }
                        break;
                }
            }
        }

        static private void ShowPB()
        {
            PB = Application.ShowProgressBar();
            PB.DialogTitle = "Conversion Progress";
        }

        static private void InitializePB(string folder)
        {
            PB.DialogTitle = "Converting files in " + Path.GetDirectoryName(folder);
            PB.Range = Directory.GetFiles(folder).Length;
            PB.position = 1;
        }

        static private void UpdatePB(string file)
        {
            PB.Text = file;
            PB.increment();
        }
    }
}
