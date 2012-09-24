using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace GraphicalVersion
{
    
    /// <summary>
    /// Author: Jervis Muindi
    /// Main Window Form
    /// </summary>
    public partial class Form1 : Form
    {
        /*
        private enum tipStates { ValidDrag, InvalidDrag, Empty };
        private int tipState = (int) tipStates.Empty;
        */
        
        private enum btnState { Convert, Reset};
        private int convertBtnState = (int) btnState.Convert;

        public Form1()
        {
            InitializeComponent();
            init();
        }


        private void init()
        {
            initializeDragDrop();
            initializeTooltip();
            disableConversionBtn();
        }


        private void enableConversionBtn()
        {
            this.button_startConversion.Enabled = true;
            this.toolTip1.SetToolTip(button_startConversion, "");
        }

        private void disableConversionBtn()
        {
            this.button_startConversion.Enabled = false;
            this.toolTip1.SetToolTip(button_startConversion, "You Must add SNP files before you can start the conversion process");  
        }

        private void initializeTooltip()
        {
            this.toolTip1.SetToolTip(listBox_SNPFiles, "Drag Snapshot (.snp) files here to convert them");
            
        }

        private void initializeDragDrop()
        {
            this.listBox_SNPFiles.DragDrop += new DragEventHandler(listBox_SNPFiles_DragDrop);

            this.listBox_SNPFiles.DragEnter += new DragEventHandler(listBox_SNPFiles_DragEnter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox_SNPFiles_DragEnter(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                

                foreach (String file in (string[])e.Data.GetData(DataFormats.FileDrop, false))
                {
                    if(!Path.GetExtension(file).ToLower().Equals(".snp")){ // if any one file is not an SNP file, don't allow drag dropping
                        e.Effect = DragDropEffects.None; 
                        /*ToolTip tip = new ToolTip();
                        String text= "Please only drag (Snapshot) SNP files";
                        tip.ToolTipTitle = text;
                        tip.Show(text, this, 30000);
                        */
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
                
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox_SNPFiles_DragDrop(object sender, DragEventArgs e)
        {

            string[] files; 
            try
            {
                files = (string[]) e.Data.GetData(DataFormats.FileDrop,false);

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        Console.WriteLine("{0}) " + files.GetValue(i), i);
                        
                        
                        // may be add a check to prevent network UNC paths from being added ? 

                        listBox_SNPFiles.Items.Add(files[i]);
                    }

                 // enable the covnerison btn
                    enableConversionBtn();

                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        
        /*
         * private void btnSelectFolder_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = true;

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                string foldername = folderBrowserDialog1.SelectedPath;

                this.textBox_OutputDirectory.Text = foldername; 
            }

        }*/

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private bool isListEmpty()
        {
            if (this.listBox_SNPFiles.Items.Count == 0)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }

        private delegate void dlgShowMsg(string s);

        /// <summary>
        /// Non UI thread safe method. 
        /// </summary>
        /// <param name="msg"></param>
        private void _showMsg(string msg)
        {
            MessageBox.Show(msg,"Success");
        }

        private void showErrorMsg(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new dlgShowErrorMsg(_showErrorMsg), msg); 

            }
            else
            {
                _showErrorMsg(msg);
            }
        
        }


        private delegate void dlgShowErrorMsg(string s); 
        private void _showErrorMsg(string msg)
        {
            MessageBox.Show(msg, "Error"); 
        }

        /// <summary>
        /// UI Thread safe version of showMsg
        /// </summary>
        /// <param name="msg"></param>
        private void showMsg(string msg)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new dlgShowMsg(_showMsg), msg);
            }
            else
            {
                _showMsg(msg); 
            }   
        }

        private void finishProgress()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(_finishProgress),null);
            }
            else
            {
                _finishProgress();
            }
        }

        private void _finishProgress(){
            this.progressBar1.Value = this.progressBar1.Maximum;
        }

        private void _makeProgress()
        {
            this.progressBar1.Value = this.progressBar1.Value + 1;
        }

        /// <summary>
        /// UI Thread safe
        /// </summary>
        private void makeProgress()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(_makeProgress),null);
            }
            else
            {
                _makeProgress(); 
            }
        }

        /// <summary>
        /// UI Thread Safe: Sets Status label to given message. 
        /// </summary>
        /// <param name="msg"></param>
        private void setLabel(String msg)
        {


            if(InvokeRequired)
            {
                dlgSetLabel dsl = new dlgSetLabel(_setLabel); 
                
                BeginInvoke(dsl, msg); 


            } else 
            {
                _setLabel(msg);
            }

        }


        private delegate void dlgSetLabel(string msg); 

        private void _setLabel(string msg)
        {
            this.label_status.Text =  msg;
            this.label_status.Visible = true; 
        }

        private void initProgessBar()
        {
            int total = this.listBox_SNPFiles.Items.Count;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = total + 1;
            progressBar1.Value = 1;
        }




        private void changeBtntoNormalMode()
        {
            convertBtnState = (int) btnState.Convert;
            this.button_startConversion.Text = "Start Conversion";
            groupBox1.Text = "Drag Snapshot Files Below to Convert";
            this.labelHelpText.Visible = true;
        }

        private void changeBtnToResetMode()
        {
            this.button_startConversion.Text = "Perform Another Conversion";
            this.groupBox1.Text = "The files below have been converted successfully to PDFs";
            convertBtnState = (int)btnState.Reset;
            this.labelHelpText.Visible = false;
        }

        private void button_startConversion_Click(object sender, EventArgs e)
        {

            if (convertBtnState == (int) btnState.Reset)
            {
                this.resetUI(); // call the button to reset the UI. 
                
            }
            else
            {    // Ensure that ListItem has at least one item

                if (isListEmpty())
                {
                    return;
                }

                //Hide the convert Button
                this.button_startConversion.Visible = false;

                //show the progress bar 
                this.progressBar1.Visible = true;
                initProgessBar(); // initialize progress bar

                // start the conversion process
                AsyncConvertSNPFiles();
                //string[] files = convertCollectionToArray(this.listBox_SNPFiles.Items);
                //convertSNPFiles(files);
            } 
        }


        private void AsyncConvertSNPFiles()
        {
            string[] files = convertCollectionToArray(this.listBox_SNPFiles.Items);
            convertDlg cDlg = new convertDlg(convertSNPFiles);

            cDlg.BeginInvoke(files, null, null); 

        }

        private void onSuccessCompletion()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(_onSuccessCompletion), null);
            }
            else {
                _onSuccessCompletion();
            }
        }

        /// <summary>
        /// Resets the UI and preps it for another conversion
        /// </summary>
        private void resetUI()
        {
            this.listBox_SNPFiles.Items.Clear(); // remove list of converted files. 
            this.button_startConversion.Enabled = false; // disable the conversion button

            changeBtntoNormalMode();
        }

        private void _onSuccessCompletion()
        {
            changeBtnToResetMode(); 
        }

        private void onConversionComplete()
        {
            if(InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(_onConversionComplete), null); 

            } else {

                _onConversionComplete(); 
            }
        }

        /// <summary>
        /// Runs after conversion is completed to reset the UI elements
        /// </summary>
        private void _onConversionComplete()
        {

            // TBD : Hide the prograss bar and re-show the hidden button. 

            progressBar1.Visible = false;
            


            this.button_startConversion.Visible = true; 


        }

        private string[] convertCollectionToArray(ListBox.ObjectCollection col)
        {
            string[] files = new string[col.Count];
            int i = 0;
            foreach (string s in col)
            {
                files[i++] = s;
            }

            return files; 
        }


        private delegate void convertDlg(string [] files);

        private void convertSNPFiles(string[] files)
        {
            bool success = true;
            
            
            
            foreach (string file in files)
            {
                // Skip over any files which don't exist anymore
                FileInfo f = new FileInfo(file);
                if (!f.Exists)
                {
                    continue;
                }

                // Get Just the file name 
                string filename = Path.GetFileName(file); 
                setLabel("Converting " + filename +" ..."); 

                bool ret = convertSNPFile(file);
                if (!ret)
                {
                    success = false;
                }

                makeProgress();
            }


            finishProgress(); // make progress bar complete
            setLabel(""); // remove status msg

            onConversionComplete(); // Run On conversion complete method to reset the UI 
            

            if (success)
            {
                string  msg = "Conversion Completed Successfully\n";
                        msg += "You can find the PDFs in the same location\n";
                        msg += "as where the original SNP files are located.";

                showMsg(msg);
                onSuccessCompletion(); 
              
            }
            else
            {
                string msg = "Not All SNP files were converted successfully.\n";
                msg += "Please remove some files from the list and try again"; 
                showErrorMsg(msg);
            }
        }


        private bool convertSNPFile(string file)
        {
            string name = Path.GetFileName(file);
            string dir = Path.GetDirectoryName(file);


            bool result = Snapshot.convertToPDF(dir, name);

            return result;
        }

        private void label_status_Click(object sender, EventArgs e)
        {

        }

        private void removeSelectedEntries()
        {
            List<String> newList = new List<String>(10);
            int count = listBox_SNPFiles.SelectedItems.Count; // get count of items to remove

            for (int i = 0; i < count; i++) 
            {
                listBox_SNPFiles.Items.Remove(listBox_SNPFiles.SelectedItems[0]);  //use index 0 so that we get the latest element to del
            }
            
            
        }

        /// <summary>
        /// Called when a key is pressed while on the list box item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyPressed(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                
                removeSelectedEntries();
                //this._showMsg("we got the del key");
            }

        }

        private void setToolTip(object sender, EventArgs e)
        {

            if (listBox_SNPFiles.Items.Count != 0)
            {
                if (listBox_SNPFiles.SelectedItems.Count == 0) // nothing selected
                {
                    this.toolTip1.SetToolTip(listBox_SNPFiles, "You can select entries and press the delete key to remove them");
                }
                else
                {
                    this.toolTip1.SetToolTip(listBox_SNPFiles, "You can remove selected entries by pressing the delete key");
                }
                
            }
            else
            {
                initializeTooltip(); // show the default can drag files msg 
            }
        }

        }
        
    }

//}