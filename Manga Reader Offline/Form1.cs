using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Manga_Reader_Offline
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Variables
        /// </summary>
        bool imageFound = false;
        int currentChapter = 0;
        int maxChapter = 0;
        int currentPicture = 0;
        int maxPicture = 0;
        bool panning = false;
        Point startingPoint = Point.Empty;
        Point movingPoint = Point.Empty;
        float zoom = 100;

        string[] supportedFormats = { ".jpg", ".png" };

        string[] directoryPaths;
        string[] filePaths;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save data and progress first before exiting the application
            DialogResult reallyQuit = MessageBox.Show("Are you sure you want to quit?\nDisable this prompt in the settings.", "Quitting...", MessageBoxButtons.OKCancel);
            if (reallyQuit == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void loadDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Load the directory of the manga and take the lowest number folder and image first to view
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult folderDialogResult = folderDialog.ShowDialog();

            //Reset image found
            imageFound = false;

            //Reset toolbar strip
            CurrentChapter.Text = "0";
            MaxChapter.Text = "0";

            //Get the number of directories
            directoryPaths = Directory.GetDirectories(folderDialog.SelectedPath);
            currentChapter = 1;
            CurrentChapter.Text = currentChapter.ToString();
            maxChapter = directoryPaths.Count();
            MaxChapter.Text = maxChapter.ToString();

            Console.WriteLine("Max chapter: " + maxChapter.ToString());
            
            //For every directory, check for images until it is found. If not, check the root directory for images
            for (int i = 0; i <= maxChapter; i++)
            {
                if (imageFound == false)
                {
                    //Get the files in the directory's folder if there's any. If not, just get the files in the current directory.
                    if (maxChapter != 0 && i != maxChapter)
                    {
                        Console.WriteLine("There is a directory and its path is " + directoryPaths[i].ToString());
                        Console.WriteLine("Found directories");
                            filePaths = Directory.GetFiles(directoryPaths[i].ToString());
                    }
                    else
                    {
                        Console.WriteLine("Does not find directories");
                        filePaths = Directory.GetFiles(folderDialog.SelectedPath);
                    }

                    //When the program is done checking the directories, check the root folder for images
                    if (i == maxChapter)
                    {
                        Console.WriteLine("Directories have been checked for images but didn't find one. Checking root directory now...");
                        filePaths = Directory.GetFiles(folderDialog.SelectedPath);
                    }

                    //For every file in the folder, if there's no image file, proceed to check the next directory
                    Console.WriteLine("Filepath Count: " + filePaths.Count().ToString());
                    for (int j = 0; j < filePaths.Count(); j++)
                    {
                        Console.WriteLine("File Checking: " + filePaths[j].ToString());

                        if (supportedFormats.Any(filePaths[j].Contains))
                        {
                            Console.WriteLine("File contain supported image formats!");
                            imageFound = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("File doesn't contain supported image formats!");
                            currentChapter += 1;
                        }
                    }
                }
            }

            //Set the first picture to load
            currentPicture = 1;
            maxPicture = filePaths.Count();
            MaxPage.Text = maxPicture.ToString();
            Console.WriteLine("Files found: " + filePaths.Length.ToString());

            //Load the first picture file in the directory. If no picture is found, show the background image
            if (imageFound)
            {
                PictureBox.Load(filePaths[currentPicture - 1].ToString());
            }
            else
            {
                PictureBox.Image = null;
            }
        }

        /// <summary>
        /// Code to pan the image and zoom in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            panning = true;
            //Console.WriteLine(panning.ToString());
            startingPoint = new Point(e.Location.X - movingPoint.X, e.Location.Y - movingPoint.Y);
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            panning = false;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (panning)
            {
                movingPoint = new Point(e.Location.X - startingPoint.X, e.Location.Y - startingPoint.Y);
                //Console.WriteLine(movingPoint.ToString());
                PictureBox.Invalidate();
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (PictureBox.Image != null)
            {
                e.Graphics.Clear(Color.Black);
                e.Graphics.DrawImage(PictureBox.Image, movingPoint);
            }
        }

        /// <summary>
        /// Press the arrow keys to view the next or the previous image in the directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_OnKeyDown(object sender, KeyEventArgs e)
        {
            //Right
            if (e.KeyCode == Keys.Right)
            {
                if (currentPicture < maxPicture)
                {
                    currentPicture += 1;
                }
                else
                {
                    currentPicture = 1;
                }
                CurrentPage.Text = currentPicture.ToString();

                //Check if object is an image before loading
                string file = filePaths[currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(filePaths[currentPicture - 1].ToString());
                }
            }
            //Left
            if (e.KeyCode == Keys.Left)
            {
                if (currentPicture > 1)
                {
                    currentPicture -= 1;
                }
                else
                {
                    currentPicture = maxPicture;
                }
                CurrentPage.Text = currentPicture.ToString();

                //Check if object is an image before loading
                string file = filePaths[currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(filePaths[currentPicture - 1].ToString());
                }
            }
        }
    }
}
