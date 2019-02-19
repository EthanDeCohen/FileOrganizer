using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FileNameOrganizer
{
    public partial class Form1 : Form
    {
        System.String[] CurrentDir = null;
        System.String previousDir = null;
        System.String currentDirPath = null;
        List<string> listboxfiles = new List<string>();
        protected System.String[] TopDir = System.IO.Directory.GetLogicalDrives();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] Root = System.IO.Directory.GetLogicalDrives();
            this.comboBox1.Items.AddRange(Root);
            this.textBox6.Text = "ROOT";
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            PopulateCheckBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //This Button is design to navigate backwards.
            this.listboxfiles.Clear();
            this.comboBox1.Items.Clear();  //Clear both Combobox and the Listbox
            this.checkedListBox1.Items.Clear();
            try //Try to navigate backwards
            {


                if (this.currentDirPath == "C:\\" || this.currentDirPath.Length <= 4 || this.previousDir == "ROOT") //If your are currently at the Top of the File Tree
                {
                    string[] Root = System.IO.Directory.GetLogicalDrives();  //You can only navigate back to the Drives
                    this.comboBox1.Items.AddRange(Root);                    //Populate Combobox with Directories
                    this.textBox6.Text = "ROOT";                            //Tell User that you're in the root
                    this.CurrentDir = Root;                                 //Current Directory update
                    return;
                }
                this.currentDirPath = this.previousDir;
                string[] DirFiles = System.IO.Directory.GetFiles(this.previousDir);         //Grab files for previous Directory
                string[] Cdirectories = System.IO.Directory.GetDirectories(this.previousDir);
                this.CurrentDir = System.IO.Directory.GetDirectories(this.previousDir);     //Grab final destination (Current)
                this.comboBox1.Items.AddRange(this.CurrentDir);                             //Populate Combo and...
                this.checkedListBox1.Items.AddRange(DirFiles);                                     //...list box
                Check_Or_Uncheck_All();//Check the List.
                this.listboxfiles.AddRange(DirFiles);
                this.textBox6.Text = this.previousDir;                                      //Tell user where we're at


                if ((this.previousDir.Length >= 4) && this.previousDir != "ROOT")//If previous Directory Isn't the top of the tree then...
                    this.previousDir = System.IO.Directory.GetParent(this.previousDir).ToString();               //... update previous directory.
                if (this.currentDirPath.Length <= 4)
                {

                    this.comboBox1.Items.Clear();  //Clear both Combobox and the Listbox
                    this.checkedListBox1.Items.Clear();

                    this.previousDir = "ROOT";
                    this.comboBox1.Items.AddRange(Cdirectories);
                    this.checkedListBox1.Items.AddRange(DirFiles);
                    Check_Or_Uncheck_All();//Check the List.
                    this.listboxfiles.AddRange(DirFiles);
                    this.textBox6.Text = this.currentDirPath;

                }
            }
            catch (System.Exception) //You're at the top of the File Tree
            {
                this.comboBox1.Items.Clear(); //Clear out the Combo-Box
                this.comboBox1.Items.AddRange(this.TopDir); //Populate Combo box with Directories
                this.textBox6.Text = "ROOT"; // Tell User where you are
                MessageBox.Show("You cannot go back any futher"); //Stop trying to navigate backwards. You cannot!
                this.checkedListBox1.Items.Clear(); //Clear everything Just it case.
            }
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            this.comboBox1.Text = this.textBox6.Text;
        }

        public void GetFileTypes(String s, params string[] values)
        {

            if (!string.IsNullOrEmpty(s) || values.Length > 0)
            {
                foreach (string value in values)
                {
                    if (s.Contains(value))
                        return;
                }
            }
        }

        private void DoesTypeExist(string[] file)
        {
            List<String> Types = new List<string>();
            foreach (string extension in file)
            {
                if (!Types.Contains(System.IO.Path.GetExtension(extension)))
                    Types.Add(System.IO.Path.GetExtension(extension));
            }
            this.comboBox2.Items.AddRange(Types.ToArray());
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            this.comboBox2.Items.Clear();
            try
            {
                DoesTypeExist(this.listboxfiles.ToArray());
            }
            catch (System.Exception)
            {
                MessageBox.Show("There are no available Files");
            }
        }

        private void MakeChange_Click(object sender, EventArgs e)
        {            
            int i = 1;
            if (this.comboBox2.Text != null)
            {
                try
                {
                    var FilesToQuery = this.listboxfiles.ToArray();
                    int count= stringMatches(FilesToQuery, 
                            this.textBox1.Text,
                        System.IO.Path.GetExtension(
                            this.comboBox2.Text));//Count how many occurences of file name that exists already
                    for (i = 0; i <= (this.checkedListBox1.Items.Count - 1); i++)
                    {   
                        if (this.checkedListBox1.GetItemChecked(i))
                        {
                            if (System.IO.Path.GetExtension(this.checkedListBox1.Items[i].ToString()) == this.comboBox2.Text.ToString())
                            {
                                if (count>0)
                                {
                                    count++;
                                    System.IO.File.Move(this.checkedListBox1.Items[i].ToString(), this.currentDirPath + "\\" + this.textBox1.Text + count +
                                    System.IO.Path.GetExtension(this.checkedListBox1.Items[i].ToString()));
                                }
                                else
                                {

                                System.IO.File.Move(this.checkedListBox1.Items[i].ToString(), this.currentDirPath + "\\" + this.textBox1.Text +
                                    System.IO.Path.GetExtension(this.checkedListBox1.Items[i].ToString()));
                                    count++;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Unable to change File name");
                    return;
                }
                this.checkedListBox1.Items.Clear();
                string[] newDirectories = System.IO.Directory.GetDirectories(this.currentDirPath); //Look inside the directory and save its children directories
                string[] newfiles = System.IO.Directory.GetFiles(this.currentDirPath);             //Look inside the directory and save its member files

                this.CurrentDir = newDirectories;                                                   //Save the current child directories to keep track of progress
                this.comboBox1.Items.AddRange(newDirectories);                        //Populate the Combox with Child Directories
                this.checkedListBox1.Items.AddRange(newfiles);                               //Populate the Listbox with child files.
                Check_Or_Uncheck_All();//Check the List.
                this.listboxfiles.AddRange(newfiles);

            }
        }
        
        private void OpenCurrentDirectory_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            OpenFileDialog x = new OpenFileDialog();
            x.InitialDirectory = this.currentDirPath;
            x.Multiselect = true;
            x.ShowDialog();
            string[] result = x.FileNames;
            checkedListBox1.Items.AddRange(result);
            SelectAll();
                
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void PopulateCheckBox()
        {
            System.String newDir = this.comboBox1.Text; //This line saves the directory that the user has chosen.
            this.listboxfiles.Clear();
            this.textBox6.Text = this.comboBox1.Text; //This updates the Current directory Text box 
            this.comboBox1.Items.Clear();             //This clears the Combobox
            this.checkedListBox1.Items.Clear();              //This clears the listbox with the files we're interested in.
            try //Try to gain access to files
            {
                string[] newDirectories = System.IO.Directory.GetDirectories(newDir); //Look inside the directory and save its children directories
                 string[] newfiles = System.IO.Directory.GetFiles(newDir);             //Look inside the directory and save its member files

                this.CurrentDir = newDirectories;                                     //Save the current child directories to keep track of progress
                this.comboBox1.Items.AddRange(newDirectories);                        //Populate the Combox with Child Directories
                this.checkedListBox1.Items.AddRange(newfiles);                               //Populate the Listbox with child files.
                //Check_Or_Uncheck_All();//Check the List.
                this.listboxfiles.AddRange(newfiles);

                this.currentDirPath = this.textBox6.Text;                             //Update current Location
            }
            catch (System.UnauthorizedAccessException) //Access Denied to directory. 
            {
                MessageBox.Show("You are Unauthorized to Access this File");
            }

            try //If a previous Directory Exists, Try to assign it.
            {
                this.previousDir = System.IO.Directory.GetParent(this.currentDirPath).ToString();      //Update previous location
            }
            catch (System.Exception) //Previous directory doesn't exist. Assign root.
            {
                this.previousDir = "ROOT";
            }
            //this.checkedListBox1.Items.AddRange(this.listboxfiles.ToArray());
        }

        private void Check_Or_Uncheck_All()
        { 
            try
            {   if (this.checkedListBox1.Items.Count <= 0) return;
                foreach (object item in this.checkedListBox1.Items)
                {
                    ;
                }
            }catch(System.Exception)
            {
                MessageBox.Show("Something Went Wrong");
            }
        }

        private void WhichItemsAreChecked()
        {
            int i;
            string s;
            s = "Checked items:\n";
            for (i = 0; i <= (checkedListBox1.Items.Count - 1); i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    s = s + "Item " + (i + 1).ToString() + " = " + checkedListBox1.Items[i].ToString() + "\n";
                }
            }
            MessageBox.Show(s);
        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void SelectAll()
        {
            int i;
            string s;
            s = "Checked items:\n";
            for (i = 0; i <= (checkedListBox1.Items.Count - 1); i++)
            {
                if (!checkedListBox1.GetItemChecked(i))
                {
                    checkedListBox1.SetItemChecked(i, true);
                    //s = s + "Item " + (i + 1).ToString() + " = " + checkedListBox1.Items[i].ToString() + "\n";
                }
            }
        }

        private int stringMatches(string[] textToQuery, String FileName,string ext)
        {
            int count = 0;
            Regex Copy = new Regex(FileName.Split('.')[0] + @"\(\d+\)" + ext);
            Regex Copy1 = new Regex(FileName.Split('.')[0] + @"\d+" + ext);
            Regex Original = new Regex(FileName.Split('.')[0] + ext);
            foreach(string file in textToQuery)
            {
                var origincheck = Original.Match(file);
                var NumericalCheck = Copy1.Match(file);
                var ParentheticalCheck = Copy.Match(file);
                if (ParentheticalCheck.Success|| origincheck.Success ||NumericalCheck.Success)
                    count++;                
            }
            return count;
        }
    }
}
