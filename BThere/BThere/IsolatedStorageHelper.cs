using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace BThere
{
    class IsolatedStorageHelper
    {

        public string writeResult { get; set; }

        public string readResult { get; set; }

        private IsolatedStorageFile myStorageArea = IsolatedStorageFile.GetUserStoreForApplication(); //creating an isolated storage file object


        /// <summary>
        /// The name for the file
        /// </summary>
        /// <param name="string">file name</param>
        public string filename { get; set; }

        /// <summary>
        /// The name of the folder in storage area
        /// </summary>
        /// <param name="string">folder name</param>
        public string foldername { get; set; }

        /// <summary>
        /// Helper method to write to the file. Returns int as result
        /// </summary>
        /// <param name="contents">String[] Contents</param>
        public int WriteToFile(string[] contents)
        {
            //string[] result = null;

            int result = 0;

            if (filename == "")
            {
                writeResult = "Specify file name";
                result = 1; // file not specified
            }
            if (foldername == "")
            {
                writeResult = "Specify folder name";
                result = 2; // folder not specified
            }
            if(contents.Length<=0)
            {
                writeResult = "Specify file contents";
                result = 3; // file contents not specified
            }

            // write the lines
            try
            {
                //if exist else creating a folder
                if (myStorageArea.DirectoryExists(foldername) == false)
                    myStorageArea.CreateDirectory(foldername);

                //opening or creating a new file
                using (var myIS_Stream = new IsolatedStorageFileStream(foldername + "\\" + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, myStorageArea))
                {
                    // create a stream writer to write data to the file
                    using (var myIS_FileWrite = new System.IO.StreamWriter(myIS_Stream))
                    {

                        foreach (var line in contents)
                        {
                            myIS_FileWrite.WriteLine(line);
                        }
                    }
                }

                writeResult = "Success";
                result = 0;
            }
            catch (Exception e)
            {
                writeResult = "Writing operation failed. Error: " + e.Message;
                result = 4;
            }

            //myStorageArea.Dispose();
            
            return result;
        }

        /// <summary>
        /// Helper method to read from the file. Returns String[] as result
        /// </summary>
        public string[] ReadFromFile()
        {
            string[] result = null;
            List<string> readItems = new List<string>();

            if (filename == "")
            {
                readResult = "Failed. Specify filename";
            }
            if (foldername == "")
            {
                readResult = "Failed. Specify foldername";
            }

            
            //read file
            try
            {
                if (myStorageArea.FileExists(foldername + "\\" + filename))
                {
                    using (var myIS_Stream = new IsolatedStorageFileStream(foldername + "\\" + filename, FileMode.Open, FileAccess.Read, FileShare.Read, myStorageArea))
                    {
                        // create a stream reader to read data from the file
                        using (var myIS_FileRead = new StreamReader(myIS_Stream))
                        {
                            while (myIS_FileRead.EndOfStream)
                            {
                                readItems.Add(myIS_FileRead.ReadLine());
                            }
                        }
                    }

                    if (readItems.Count <= 0)
                    {
                        readResult = "File is empty";
                    }
                    else
                    {
                        readResult = "Success";
                        result = readItems.ToArray();
                    }
                }
                else
                {
                    readResult = "file doesn't exist";
                }
            }
            catch(Exception e) //cannot read the legend file
            {
                //throw e;
                readResult = "Read operation failed. Error: " + e.Message;
            }

            //myStorageArea.Dispose();

            return result;
        }
    }
}
