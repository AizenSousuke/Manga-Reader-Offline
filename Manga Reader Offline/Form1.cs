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

        //Formats which are supported
        string[] supportedFormats = { ".jpg", ".png" };

        //All the directories that should be checked
        string[] directoryPaths;
        //All the paths of the files in the directory
        string[] filePaths;
        //Images which are stored in memory to be loaded so that the program only load the images
        List<String> imagesArray;

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

            //Clear the images array list if there is one. If not, create a new list
            if (imagesArray == null)
            {
                imagesArray = new List<string>();
                Console.WriteLine("Creating new Images Array");
            }
            else
            {
                imagesArray.Clear();
                Console.WriteLine("Clearing Images Array");
            }

            //Reset toolbar strip
            CurrentChapter.Text = "0";
            MaxChapter.Text = "0";
            CurrentPage.Text = "0";
            MaxPage.Text = "0";

            //Make sure there's a path before proceeding
            if (folderDialog.SelectedPath != "" && folderDialog.SelectedPath != null)
            {
                //Get the number of directories
                Console.WriteLine("Folderpath: " + folderDialog.SelectedPath.ToString());
                directoryPaths = Directory.GetDirectories(folderDialog.SelectedPath);
                currentChapter = 0;
                maxChapter = directoryPaths.Count();

                Console.WriteLine("Max chapter before checking for images: " + maxChapter.ToString());

                //For every directory, check for images until it is found. If not, check the root directory for images
                for (int i = 0; i <= maxChapter; i++)
                {
                    //The program will only check till the first image is found
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
                                //imageFound = true;
                                //Put the image in the imagesArray
                                imagesArray.Add(filePaths[j]);
                                currentChapter += 1;
                                //break;
                            }
                            else
                            {
                                Console.WriteLine("File doesn't contain supported image formats!");
                                currentChapter += 1;
                            }
                        }
                    }
                }

                //Load all the images from the first directory with images
                imageFound = true;      //Moved from the top so that the program checks for all the images

                //Toolstrip change
                CurrentChapter.Text = "1";
                MaxChapter.Text = maxChapter.ToString();

                //Set the first picture to load
                currentPicture = 1;
                CurrentPage.Text = currentPicture.ToString();
                //maxPicture = filePaths.Count();
                maxPicture = imagesArray.Count();
                Console.WriteLine("Images found: " + imagesArray.Count().ToString());
                MaxPage.Text = maxPicture.ToString();
                Console.WriteLine("Files found: " + filePaths.Length.ToString());

                //Load the first picture file in the directory. If no picture is found, show the background image
                if (imageFound)
                {
                    //PictureBox.Load(filePaths[currentPicture - 1].ToString());

                    //Load the image from the imagesArray
                    PictureBox.Load(imagesArray[currentPicture - 1].ToString());
                }
                else
                {
                    PictureBox.Image = null;
                }
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
        /// Focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Panel_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
            Console.WriteLine("Picturebox Focus!");
        }

        public void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse wheel scrolled");
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
                string file = imagesArray[currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(imagesArray[currentPicture - 1].ToString());
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
                string file = imagesArray[currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(imagesArray[currentPicture - 1].ToString());
                }
            }
        }
    }
}