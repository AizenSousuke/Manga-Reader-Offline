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
        List<string> imagesArray;
        //Chapter list which stores the images array to be used when calculating the chapters
        List<List<string>> chaptersArray;

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

            //Clear the chapters array if there is one. If not, create a new list
            if (chaptersArray == null)
            {
                chaptersArray = new List<List<string>>();
                Console.WriteLine("Creating new Chapters Array");
            }
            else
            {
                chaptersArray.Clear();
                Console.WriteLine("Clearing Chapters Array");
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

                Console.WriteLine("Predicted max chapter before checking for images: " + maxChapter.ToString());

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
                                //currentChapter += 1;
                                //break;
                            }
                            else
                            {
                                Console.WriteLine("File doesn't contain supported image formats!");
                                //currentChapter += 1;
                            }
                        }
                    }

                    //After checking every folder, if there are images, consolidate them as a chapter. If not, clear the list and check the next folder, if any.
                    if (imagesArray.Count() > 0)
                    {
                        //Add chapter
                        chaptersArray.Add(imagesArray);
                        currentChapter += 1;

                        //Clear images array for the next chapter
                        //imagesArray.Clear();  //If I clear this here chaptersArray[0] will be 0.

                        //Creates a new imagesArray for the next chapter
                        imagesArray = new List<string>();
                    }
                }

                //Calculate how many chapters (with images) there actually are.
                Console.WriteLine("Max Chapter after searching: " + currentChapter.ToString());
                maxChapter = currentChapter;

                //Load all the images from the first directory with images
                imageFound = true;      //Moved from the top so that the program checks for all the images

                //Toolstrip change
                currentChapter = 1;
                CurrentChapter.Text = "1";
                MaxChapter.Text = maxChapter.ToString();

                //Set the first picture to load
                currentPicture = 1;
                CurrentPage.Text = currentPicture.ToString();
                maxPicture = chaptersArray[currentChapter - 1].Count();
                MaxPage.Text = maxPicture.ToString();
                /*
                //maxPicture = filePaths.Count();
                maxPicture = imagesArray.Count();
                Console.WriteLine("Images found: " + imagesArray.Count().ToString());
                MaxPage.Text = maxPicture.ToString();
                Console.WriteLine("Files found: " + filePaths.Length.ToString());
                */

                //Load the first picture file in the directory. If no picture is found, show the background image
                if (imageFound)
                {
                    //PictureBox.Load(filePaths[currentPicture - 1].ToString());

                    //Load the image from the imagesArray
                    //PictureBox.Load(imagesArray[currentPicture - 1].ToString());

                    //Load the image from the imagesArray in the chaptersArray according to which chapter is currently being viewed.
                    Console.WriteLine("Chapters Array at [0]: " + chaptersArray[0].Count().ToString());
                    PictureBox.Load(chaptersArray[currentChapter - 1][currentPicture - 1].ToString());
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
                //Check if the picture is the last one in the current chapter
                if (currentPicture < maxPicture)
                {
                    currentPicture += 1;
                }
                else
                {
                    if (currentChapter < maxChapter)
                    {
                        //Get the max picture of the next chapter
                        maxPicture = chaptersArray[currentChapter].Count();
                        currentPicture = 1;
                        currentChapter += 1;
                    }
                }

                Console.WriteLine("Current Picture: " + currentPicture.ToString());
                Console.WriteLine("Current Chapter: " + currentChapter.ToString());
                CurrentPage.Text = currentPicture.ToString();
                MaxPage.Text = chaptersArray[currentChapter - 1].Count().ToString();
                CurrentChapter.Text = currentChapter.ToString();

                //Check if object is an image before loading
                string file = chaptersArray[currentChapter - 1][currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(chaptersArray[currentChapter - 1][currentPicture - 1].ToString());
                }
            }

            //Left
            if (e.KeyCode == Keys.Left)
            {
                //Check if the picture is the first one in the current chapter
                if (currentPicture > 1)
                {
                    currentPicture -= 1;
                }
                else
                {
                    if (currentChapter > 1)
                    {
                        //Get the max picture of the last chapter
                        maxPicture = chaptersArray[currentChapter - 2].Count();
                        currentPicture = maxPicture;
                        currentChapter -= 1;
                    }
                }

                Console.WriteLine("Current Picture: " + currentPicture.ToString());
                Console.WriteLine("Current Chapter: " + currentChapter.ToString());
                CurrentPage.Text = currentPicture.ToString();
                MaxPage.Text = chaptersArray[currentChapter - 1].Count().ToString();
                CurrentChapter.Text = currentChapter.ToString();

                //Check if object is an image before loading
                string file = chaptersArray[currentChapter - 1][currentPicture - 1].ToString();
                if (supportedFormats.Any(file.Contains))
                {
                    PictureBox.Load(chaptersArray[currentChapter - 1][currentPicture - 1].ToString());
                }
            }
        }
    }
}