using System;
using System.Windows;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

using log4net.Config;
using log4net;
using System.Globalization;

using System.Linq;

namespace DICOMListener
{
    //v1.1

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime ViperPlanDoseLastRaised;
        public DateTime NewDataLastRaised;

        private static log4net.ILog log = null;

        public string viperDosePlanPath = @"S:\Radiation Oncology\Public\Databases\Patient Specific QA\VIPER_Plan_Dose";
        public string newDataPath = @"S:\Radiation Oncology\Public\Databases\Patient Specific QA\NewData";
        public string pdfDirectory = @"S:\Radiation Oncology\Physics\Patient Specific QA\VMAT\ViperResults";//@"S:\Radiation Oncology\Public\Databases\Patient Specific QA\PDF Reports";
        public string replacePDF = "SavePDFFolder\tS:\\Radiation Oncology\\Physics\\Patient Specific QA\\VMAT\\ViperResults";//"SavePDFFolder\tS:\\Radiation Oncology\\Public\\Databases\\Patient Specific QA\\PDF Reports";

        private bool newData = false;



        public string ExitedPatientMRN;
        public string ExitedPdfDirectory;
        public string ExitedNewPatientFolder;
        public string ExitedCreatedDirectory;
        public string ExitedPatientPlanName;

        public string currentRunningMRNmain;
        public string currentRunningMRN;

        int i = 0;
        int imain = 0;

        public MainWindow()
        {
            InitializeComponent();

            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            XmlConfigurator.Configure();
            log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();

            log.Info("Initialize DICOM Listener...");
            log.Info("Start to watch Viper Plan Dose folder...");


            string[] patientFolders = Directory.GetDirectories(viperDosePlanPath);

            if (patientFolders.Length > 0)
            {
                System.Threading.Thread.Sleep(8000);

                ViperPlanDoseWatchingProcess();
            }

            string[] abc = Directory.GetDirectories(newDataPath);
            //DirectoryInfo directory = new DirectoryInfo(newDataPath);
            //DirectoryInfo[] subdirs = directory.GetDirectories();


            if (abc.Length > 0)
            {
                System.Threading.Thread.Sleep(1000);

                NewDataMainProcess();

            }


            ViperPlanDoseWatcher();

            NewDataWatcher();
        }

        private void NewDataMainProcess()
        {
            long bmain = 0;
            string processingPatientMRNmain = null;
            string processingPatientDirNamemain;
            string dcmFileSizeFilemain;
            long dcmFileSizemain = 0;
            FileInfo[] subdirFilesmain;
            string patientPlanFolderMain;

            DirectoryInfo directorymain = new DirectoryInfo(newDataPath);
            DirectoryInfo[] subdirsmain = directorymain.GetDirectories();

            foreach (DirectoryInfo subdir in subdirsmain)
            {
                if (subdir.GetFiles().Length > 4)
                {
                    subdirFilesmain = subdir.GetFiles();

                    foreach (FileInfo subdirFile in subdirFilesmain)
                    {
                        if (subdirFile.Name.Contains("DcmFileSize.txt"))
                        {
                            dcmFileSizeFilemain = subdirFile.FullName;

                            using (var stream = new FileStream(dcmFileSizeFilemain, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                string[] dcmLines = System.IO.File.ReadAllLines(dcmFileSizeFilemain);

                                dcmFileSizemain = Convert.ToInt64(dcmLines[0]);
                            }



                        }
                        else
                        {
                            log.Info("No exsiting patient files.");
                        }
                    }

                      //  foreach (DirectoryInfo b in subdirsmain)
                      //  {

                            processingPatientDirNamemain = subdir.Name.Substring(0, 8);

                            patientPlanFolderMain = subdir.Name.Substring(9, subdir.Name.Length - 9);

                            bmain = DirSize(subdir);

                            if (dcmFileSizemain != 0)
                            {
                                if (bmain == dcmFileSizemain && imain == 0)
                                {
                                    System.Threading.Thread.Sleep(1000);


                                    Console.WriteLine(imain);
                                    log.Info("Process starts...");

                                    currentRunningMRNmain = processingPatientDirNamemain;

                            

                            NewDataWatchingProcess(processingPatientDirNamemain, patientPlanFolderMain);

                            bmain = +bmain;
                            imain = imain + 1;

                        }
                            }

                }
                else
                {
                    log.Info("No patient files available...");
                }

                imain = 0;

            }

            

        }

        private void ViperPlanDoseWatcher()
        { 
            FileSystemWatcher viperPlanDoseWatcher = new FileSystemWatcher();
            viperPlanDoseWatcher.Path = viperDosePlanPath;
            viperPlanDoseWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.Size;//| NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName;// | NotifyFilters.CreationTime;
            viperPlanDoseWatcher.Filter = "*.*";
            viperPlanDoseWatcher.Created += new FileSystemEventHandler(ViperPlanDoseOnCreated);
            viperPlanDoseWatcher.Changed += new FileSystemEventHandler(ViperPlanDoseOnChanged);
            //viperPlanDoseWatcher.Deleted += new FileSystemEventHandler(ViperPlanDoseOnChanged);
            viperPlanDoseWatcher.IncludeSubdirectories = true;
            viperPlanDoseWatcher.EnableRaisingEvents = true;

            ViperPlanDoseLastRaised = DateTime.Now;
            log.Info("ViperPlanDoseWatcher started at " + ViperPlanDoseLastRaised.ToLongTimeString());
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;

            if (d.GetFiles().Length > 0)
            {
                FileInfo[] fis = d.GetFiles();

                foreach (FileInfo fi in fis)
                {
                    if (fi.Name.Contains("RI."))
                    {
                        size += fi.Length;
                    }
                }
            }

            Console.WriteLine("The size {0}", size);

            return size;
        }

        private void NewDataWatcher()
        {
            FileSystemWatcher newDataWatcher = new FileSystemWatcher();
            newDataWatcher.Path = newDataPath;
            newDataWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size | NotifyFilters.DirectoryName;//| NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName;// | NotifyFilters.CreationTime;
            newDataWatcher.Filter = "*.*";
            //newDataWatcher.Changed += new FileSystemEventHandler(NewDataOnChanged);
            //newDataWatcher.EnableRaisingEvents = false;

            newDataWatcher.Changed += new FileSystemEventHandler(NewDataOnChanged);
            newDataWatcher.Deleted += new FileSystemEventHandler(NewDataOnDeleted);
            newDataWatcher.EnableRaisingEvents = true;

            NewDataLastRaised = DateTime.Now;
            log.Info("NewDataWatcher started at " + NewDataLastRaised.ToLongTimeString());
        }

        private void ViperPlanDoseOnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Name);

                log.Info(e.Name + " - " + e.ChangeType);


                //log.Info("Event gaps " + DateTime.Now.Subtract(ViperPlanDoseLastRaised).Seconds.ToString());

                if (DateTime.Now.Subtract(ViperPlanDoseLastRaised).Seconds > 15)
                {
                    string ChangedFileNme = e.Name;
                    FileInfo changedFile = new FileInfo(ChangedFileNme);
                    string extension = changedFile.Extension;
                    string eventToOccured = e.ChangeType.ToString();


                    log.Info("Sleep for 100 miliseconds...");
                    System.Threading.Thread.Sleep(100);

                    log.Info("Process starts...");
                    ViperPlanDoseWatchingProcess();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ViperPlanDoseOnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Name);

                DirectoryInfo directory = new DirectoryInfo(viperDosePlanPath);
                DirectoryInfo[] subdirs = directory.GetDirectories();

                if (subdirs.Length != 0)
                {
                    log.Info("Sleep for 5000 miliseconds...");
                    System.Threading.Thread.Sleep(5000);

                    log.Info(e.Name + " - " + e.ChangeType);
                    //log.Info("Event gaps " + DateTime.Now.Subtract(ViperPlanDoseLastRaised).Seconds.ToString());

                    string ChangedFileNme = e.Name;
                    FileInfo changedFile = new FileInfo(ChangedFileNme);
                    string extension = changedFile.Extension;
                    string eventToOccured = e.ChangeType.ToString();

                    log.Info("Process starts...");
                    ViperPlanDoseWatchingProcess();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void NewDataOnChanged(object source, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(5000);

            //ExitedPatientMRN = "";
            //ExitedPdfDirectory = "";
            //ExitedNewPatientFolder = "";
            //ExitedCreatedDirectory = "";

            long b = 0;
            string processingPatientMRN = null;
            string processingPatientDirName;
            string dcmFileSizeFile;
            long dcmFileSize = 0;
            FileInfo[] subdirFiles;
            string patientPlanFolder;

            DirectoryInfo directory = new DirectoryInfo(newDataPath);
            DirectoryInfo[] subdirs = directory.GetDirectories();

            foreach(DirectoryInfo subdir in subdirs)
            {
                if (subdir.GetFiles().Length > 0)
                {
                    subdirFiles = subdir.GetFiles();

                    foreach (FileInfo subdirFile in subdirFiles)
                    {
                        if (subdirFile.Name.Contains("DcmFileSize.txt"))
                        {
                            dcmFileSizeFile = subdirFile.FullName;

                            using (var stream = new FileStream(dcmFileSizeFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                string[] dcmLines = System.IO.File.ReadAllLines(dcmFileSizeFile);

                                dcmFileSize = Convert.ToInt64(dcmLines[0]);
                            }
                        }
                    }
                }
                else
                {
                    log.Info("No patient files available...");
                }
                
            }

            try
            {
                foreach (DirectoryInfo a in subdirs)
                {
                    processingPatientMRN = a.Name.Substring(0, 8);

                    patientPlanFolder = a.Name.Substring(9, a.Name.Length - 9);

                    //DirSize(subdirs[0]);

                    b = DirSize(a);

                    if (dcmFileSize != 0)
                    {
                        if (b == dcmFileSize && i == 0)
                        {
                            System.Threading.Thread.Sleep(1000);


                            Console.WriteLine(i);
                            log.Info("Process starts...");

                            currentRunningMRN = processingPatientMRN;

                            NewDataWatchingProcess(processingPatientMRN, patientPlanFolder);

                            b = +b;
                            i = i + 1;

                        }
                    }


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void NewDataOnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Deleted file is " + e.Name);
        }

        private void ViperPlanDoseWatchingProcess()
        {
            string selectDirectory = viperDosePlanPath;
            string createdDirectory = newDataPath;

            string patientMRN = null;     
            string sourceFile = null;
            string destinationFile = null;
            string planName = null;
            string newDataDirectory = null;

            string[] patientFolders = Directory.GetDirectories(selectDirectory);

            if (patientFolders.Length > 0)
            {

                foreach (string patientFolder in patientFolders)
                {
                    // Get patient MRN from directory folder
                    string patientDirName = new DirectoryInfo(patientFolder).Name;

                    if (patientDirName.Length == 8 && patientDirName.Substring(0, 1) == "M")
                    {
                        patientMRN = patientDirName;
                        log.Info("Get patient MRN " + patientMRN + " from directory folder in VIPER Plan Dose.");
                    }
                    else
                    {
                        log.Info("Invalid folder name for patient MRN in VIPER Plan Dose folder");
                    }

                    string[] filePaths = Directory.GetFiles(patientFolder);

                    // Check any file exists in the patient folder. e.g. patient plans have been exported into DICOM Listener folder
                    if (filePaths.Length > 0)
                    {
                        int ac = 0;

                        foreach (string filePath in filePaths)
                        {
                            // Check if patient RD plan is available in the patient folder
                            bool bRD = filePath.Contains("RD." + patientMRN);
                            // Check if patient RP plan is available in the patient folder
                            bool bRP = filePath.Contains("RP." + patientMRN);
                            // Check if patient RS plan is available in the patient folder
                            bool bRS = filePath.Contains("RS." + patientMRN);

                            // Get patient RP plan name
                            if (bRP)
                            {
                                int indexRP = System.IO.Path.GetFileName(filePath).IndexOf(".dcm");
                                string planFileName = System.IO.Path.GetFileName(filePath);
                                int lenS = ("RP." + patientMRN).Length + 1;
                                int len = planFileName.Length;

                                // Get plan name
                                planName = planFileName.Substring(lenS, lenS - 12 - (lenS - indexRP));
                                log.Info("Get the plan name " + planName + " from patient RP file.");
                            }
                            else
                            {
                                log.Info("RP plan is not available in the patient folder.");
                            }

                            // Check if all three patient plans are exported into the patient folder
                            if (bRD || bRP || bRS)
                            {
                                ac++;
                            }
                        }

                        // Check if all three patient plans are available in the patient folder
                        if (ac >= 3)
                        {
                            // Create a new patient folder in NewData directory folder 
                            newDataDirectory = createdDirectory + "\\" + patientMRN + "_" + planName;
                            Directory.CreateDirectory(newDataDirectory);
                            Console.WriteLine("Create a new folder for patiient " + patientMRN);
                            log.Info("Create a new patient folder " + patientMRN + " with plan name " + planName + " in NewData folder.");

                            if (filePaths.Length > 0)
                            {
                                foreach (string file in filePaths)
                                {
                                    sourceFile = file;
                                    destinationFile = newDataDirectory + "\\" + System.IO.Path.GetFileName(file);

                                    // Move each file to the new patient folder in NewData 
                                    if (File.Exists(destinationFile))
                                    {
                                        System.IO.File.Delete(destinationFile);
                                        System.IO.File.Move(sourceFile, destinationFile);
                                        Console.WriteLine(destinationFile + " already exists. Delete it before move the new files.");
                                    }
                                    else
                                    {
                                        System.IO.File.Move(sourceFile, destinationFile);
                                        Console.WriteLine(sourceFile + " has been moved into the new patient folder");
                                    }
                                    
                                }

                                log.Info("All files in patient " + patientMRN + " folder have been moved into a new location in NewData folder.");

                                // Delete the patient folder in VIPER Dose Plan folder after moving all the files
                                Directory.Delete(patientFolder, false);
                            }
                            else
                            {
                                log.Info("No any patient file is in the folder.");
                            }
                        }
                        else
                        {
                            log.Info("Not all patient plans are exported into VIPER Dose folder yet.");
                        }
                    }
                }
            }
            else
            {
                log.Info("No patient folder exists in VIPER Dose Plan.");
                Console.WriteLine("No patient foler exists in VIPER Dose Plan.");
            }
        }

        private void NewDataWatchingProcess(string PatientMRN, string PatientPlanName)
        {

            string createdDirectory = newDataPath;

            string[] newFilePaths;
            //string patientMRN = null;

            int countRI = 0;
            int countRD = 0;
            int countRP = 0;
            int countRS = 0;

            string[] newPatientFolders = Directory.GetDirectories(createdDirectory);


            if ((currentRunningMRNmain != currentRunningMRN) && ((!string.IsNullOrEmpty(currentRunningMRN)) || (!string.IsNullOrEmpty(currentRunningMRNmain))))
            {
                // Check if any file has been moved from VIPER Plan Dose to New Data folder
                if (newPatientFolders.Length > 0)
                {
                    foreach (string newPatientFolder in newPatientFolders)
                    {
                        // Get patient MRN from directory folder
                        string newPatientDirName = new DirectoryInfo(newPatientFolder).Name;
                        //patientMRN = newPatientDirName.Substring(0,8);
                        log.Info("Get patient MRN " + PatientMRN + " from new directory folder in New Data.");

                        newFilePaths = Directory.GetFiles(newPatientFolder);

                        if (newFilePaths.Length > 0)
                        {
                            // Check if all patient plans and dcm images are available in New Data folder
                            foreach (string newFilePath in newFilePaths)
                            {
                                bool bRD = newFilePath.Contains("RD.");
                                bool bRP = newFilePath.Contains("RP.");
                                bool bRS = newFilePath.Contains("RS.");
                                bool bRI = newFilePath.Contains("RI.");

                                if (bRD)
                                {
                                    countRD++;
                                }

                                if (bRP)
                                {
                                    countRP++;
                                }

                                if (bRS)
                                {
                                    countRS++;
                                }

                                if (bRI)
                                {
                                    countRI++;
                                }
                            }

                            if (countRD == 1 && countRP == 1 && countRS == 1 && countRI > 1)
                            {
                                log.Info("All patient plans and dcm images are available in New Data folder.");

                                // Get the current directory path
                                string path = Directory.GetCurrentDirectory();
                                // Read autoviper_patient_directory text file in the executable folder
                                string text = System.IO.File.ReadAllText(path + "\\autoviper_patient_directory.txt");
                                // Get destination path
                                string desPath = path + "\\autoviper_patient_directory.txt";
                                // Set a new patient directory path
                                string replaceText = "PatientDir\t" + newPatientFolder;
                                // Get the location of PDF reports
                                string desPDF = path + "\\autoviper_config.txt";

                                // Read all lines in autoviper_config text file
                                string[] pdfReport = System.IO.File.ReadAllLines(path + "\\autoviper_config.txt");

                                // Check if the new patient folder is not written into autoviper_patient_directory
                                if (text != replaceText)
                                {
                                    // Update autoviper_patient_directory
                                    File.WriteAllText(desPath, replaceText);
                                    log.Info("Replace the exsiting patient directory to a new patient directory.");
                                }

                                // Check if the SavePDFFolder line is the correct path
                                if (pdfReport[7] != replacePDF)
                                {
                                    // Update SavePDFFolder path
                                    pdfReport[7] = replacePDF;
                                    File.WriteAllLines(desPDF, pdfReport);
                                    log.Info("The PDF path is updated to " + replacePDF);
                                }

                                // Check if the PDF directory is available in DICOM Listener folder
                                if (!Directory.Exists(pdfDirectory))
                                {
                                    // Create a new PDF directory if it does not exsit in DICOM Listener folder
                                    Directory.CreateDirectory(pdfDirectory);
                                }

                                // Call AutoViper automatically
                                string args = "";
                                string exeName = "AutoViper.exe";

                                ExitedPatientMRN = PatientMRN;
                                ExitedPdfDirectory = pdfDirectory;
                                ExitedNewPatientFolder = newPatientFolder;
                                ExitedCreatedDirectory = createdDirectory;
                                ExitedPatientPlanName = PatientPlanName;

                                //MessageBox.Show("Run Process AutoViper");
                                runProcess(exeName, args);
                                log.Info("Call AutoViper...");

                                //ArchiveProcess(PatientMRN, pdfDirectory, newPatientFolder, createdDirectory);

                                // Check the AutoViper process 

                                //Process[] pname = Process.GetProcessesByName("AutoViper");
                                ////var pname = Process.GetProcessesByName("AutoViper")[0];

                                //if (pname.Length != 0)
                                //{
                                //    pname[0].EnableRaisingEvents = true;
                                //    pname[0].Exited += (s, e) =>
                                //    {
                                //        // AutoViper process is finished and call ArchiveProcess function
                                //        ArchiveProcess(PatientMRN, pdfDirectory, newPatientFolder, createdDirectory);
                                //        log.Info("AutoViper process has exited. Start to archive patient plans and dcm images...");
                                //    };
                                //}
                                //else
                                //{
                                //    log.Info("AutoViper process not running...");
                                //}

                            }
                            else
                            {
                                log.Info("Patient plans or dcm images are not available yet in the New Data folder.");

                                countRD = 0;
                                countRP = 0;
                                countRS = 0;
                                countRI = 0;
                            }
                        }
                        else
                        {
                            log.Info("No patient files in folder.");
                        }
                    }
                }
                else
                {
                    log.Info("No any patient folder is waiting to run AutoViper yet.");
                }
            }
            
                

            i = 0;
            imain = 0;
        }

        #region delete
        //private void WatchingProcess()
        //{
        //    string selectDirectory = viperDosePlanPath;
        //    string createdDirectory = newDataPath;
           
        //    string[] newFilePaths;
        //    string name = "";
        //    string patientMRN = null;

        //    string[] a = new string[1];
        //    string path1 = null;
        //    string path2 = null;
        //    string path3 = null;

        //    string sourceFile = null;
        //    string destinationFile = null;

        //    string planName = null;

        //    string newDataDirectory = null;

        //    string pdfDirectory = null;

        //    int countRI = 0;
        //    int countRD = 0;
        //    int countRP = 0;
        //    int countRS = 0;
        //    string[] patientFolders = Directory.GetDirectories(selectDirectory);

       
        //        if (System.IO.Directory.GetDirectories(createdDirectory).Length > 0)
        //        {
        //            Console.WriteLine("Subdirectory exists. ");
        //            string[] subDirectoryEntries = Directory.GetDirectories(createdDirectory);

        //            foreach (string subDirectory in subDirectoryEntries)
        //            {

        //                bool patientBool = subDirectory.Contains("\\NewData\\");

        //                if (patientBool)
        //                {
        //                    int index = subDirectory.IndexOf("\\NewData\\");
        //                    patientMRN = subDirectory.Substring(index + 9, 8);

        //                }



        //                newDataDirectory = subDirectory;

        //                newFilePaths = Directory.GetFiles(newDataDirectory);

        //                foreach (string newFile in newFilePaths)
        //                {
        //                    bool bRD = newFile.Contains("RD.");
        //                    bool bRP = newFile.Contains("RP.");
        //                    bool bRS = newFile.Contains("RS.");
        //                    bool bRI = newFile.Contains("RI.");

        //                    if (bRD)
        //                    {
        //                        countRD++;
        //                    }

        //                    if (bRP)
        //                    {
        //                        countRP++;
        //                    }

        //                    if (bRS)
        //                    {
        //                        countRS++;
        //                    }

        //                    if (bRI)
        //                    {
        //                        countRI++;
        //                    }


        //                }

        //                if (countRD == 1 && countRP == 1 && countRS == 1 && countRI > 1)
        //                {
        //                    string path = Directory.GetCurrentDirectory();

        //                    string text = System.IO.File.ReadAllText(path + "\\autoviper_patient_directory.txt");
        //                    string desPath = path + "\\autoviper_patient_directory.txt";
        //                    string replaceText = "PatientDir\t" + newDataDirectory;
        //                    string desPDF = path + "\\autoviper_config.txt";

        //                    string[] pdfReport = System.IO.File.ReadAllLines(path + "\\autoviper_config.txt");

        //                    string replacePDF = "SavePDFFolder\tS:\\Radiation Oncology\\Public\\Databases\\Patient Specific QA\\PDF Reports";

        //                    if (text != replaceText)
        //                    {
                   
        //                        File.WriteAllText(desPath, replaceText);
        //                    }

        //                    if (pdfReport[7] != replacePDF)
        //                    {
        //                        pdfReport[7] = replacePDF;
        //                        File.WriteAllLines(desPDF, pdfReport);
        //                    }

        //                    pdfDirectory = @"S:\Radiation Oncology\Public\Databases\Patient Specific QA\PDF Reports";

        //                    if (!Directory.Exists(pdfDirectory))
        //                    {
        //                        Directory.CreateDirectory(pdfDirectory);
        //                    }



        //                    string args = "";
        //                    string exeName = "AutoViper.exe";

        //                    runProcess(exeName, args);


        //                    var pname = Process.GetProcessesByName("AutoViper")[0];

        //                    pname.EnableRaisingEvents = true;
        //                    pname.Exited += (s, e) =>
        //                    {
        //                        ArchiveProcess(patientMRN, pdfDirectory, newDataDirectory, createdDirectory);
        //                    };
        //                }
        //            }
        //        }
        //    //}
                        
        //}
        #endregion

        private void ArchiveProcess(string PatientMRN, string PDFOutput, string NewDataOutput, string ZipDirectory, string Plan)
        {
            DirectoryInfo pdfDi = new DirectoryInfo(PDFOutput);
            FileInfo[] pdfFiles = pdfDi.GetFiles("*.pdf");

            DirectoryInfo matDi = new DirectoryInfo(NewDataOutput);
            FileInfo[] matFiles = matDi.GetFiles("*.mat");

            

            bool isFileLocked;

            // Check if any PDF report exists
            if (pdfFiles.Length > 0)
            {
                if (Directory.GetFiles(PDFOutput).Where(f => f.Contains(ExitedPatientPlanName)).Count() > 0)
                {
                    foreach (var pdf in pdfFiles)
                    {

                        if (pdf.Name.Contains(PatientMRN) && pdf.Name.Contains(ExitedPatientPlanName))
                        {
                            try
                            {
                                using (FileStream stream = pdf.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                                {
                                    stream.Close();
                                }
                            }
                            catch (IOException)
                            {
                                isFileLocked = true;
                                Console.WriteLine("PDF report is locked");
                                log.Info("PDF report is locked.");
                            }

                            isFileLocked = false;
                            Console.WriteLine("PDF Report has been generated for patient " + PatientMRN);
                            log.Info("PDF report for patient " + PatientMRN + " is generated from VIPER.");

                            string sourcePDF = PDFOutput + "\\" + pdf.Name;

                            string destinationPDF = NewDataOutput + "\\" + pdf.Name;

                            // Copy the report to Patient folder in New Data
                            System.IO.File.Copy(sourcePDF, destinationPDF);

                        }
                    }
                }
                else
                {
                    foreach (var pdf in pdfFiles)
                    {

                        if (pdf.Name.Contains(PatientMRN))
                        {
                            try
                            {
                                using (FileStream stream = pdf.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                                {
                                    stream.Close();
                                }
                            }
                            catch (IOException)
                            {
                                isFileLocked = true;
                                Console.WriteLine("PDF report is locked");
                                log.Info("PDF report is locked.");
                            }

                            isFileLocked = false;
                            Console.WriteLine("PDF Report has been generated for patient " + PatientMRN);
                            log.Info("PDF report for patient " + PatientMRN + " is generated from VIPER.");

                            string sourcePDF = PDFOutput + "\\" + pdf.Name;

                            string destinationPDF = NewDataOutput + "\\" + pdf.Name;

                            // Copy the report to Patient folder in New Data
                            System.IO.File.Copy(sourcePDF, destinationPDF);

                        }
                    }

                }
            }
            else
            {
                log.Info("No PDF report in the folder.");
            }

            string startPath = NewDataOutput;
            string foldername = new DirectoryInfo(startPath).Name;
            string zipPath = ZipDirectory + "\\" + foldername + ".zip";
            //string zipPath = ZipDirectory + "\\" + PatientMRN + ".zip";

            if (!File.Exists(zipPath))
            {
                // Create a zip file if not exists
                ZipFile.CreateFromDirectory(startPath, zipPath);
                log.Info("Archive a file for patient " + PatientMRN + ".");
            }
            else
            {
                log.Info("The archive folder already exists for patient " + PatientMRN + ".");
            }

            string sourceZip = zipPath;
            string desZip = @"S:\Radiation Oncology\Physics\Research\VIPER batch analysis\" + foldername + ".zip";//@"S:\Radiation Oncology\Public\Databases\Patient Specific QA\Archives\Research\" + foldername + ".zip";
            //string desZip = @"S:\Radiation Oncology\Public\Databases\Patient Specific QA\Archives\Research\" + PatientMRN + ".zip";

            if (!File.Exists(desZip))
            {
                System.IO.File.Move(sourceZip, desZip);
                log.Info("Move the zip file to archive folder.");
            }
            else 
            {
                DateTime creation = File.GetCreationTime(desZip);
                string creationDate = creation.ToString("yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture);
                string currentDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture);

                if (creationDate != currentDate)
                {
                    desZip = desZip.Substring(0, desZip.Length - 4) + "_" + currentDate.Substring(0, 4) + currentDate.Substring(5, 2) + currentDate.Substring(8, 2) + "_" + currentDate.Substring(11, 2) + currentDate.Substring(14, 2) + ".zip";
                    System.IO.File.Move(sourceZip, desZip);
                    log.Info("There is an existing zip file for the same patient. Add the current date in the folder name.");
                }
                else
                {
                    string desZipBackup = desZip + ".bak";
                    System.IO.File.Replace(sourceZip, desZip, desZipBackup);
                    log.Info("Repalce the current patient folder and create a backup file");
                }
            }

            // Delete patient file in New Data folder after archiving
            if (desZip.Length > 0)
            {
                Directory.Delete(NewDataOutput, true);
                log.Info("Delete the patient file in New Data folder after archiving.");
            }


            i = 0;
            imain = 0;
        }

        #region delete
        //private void BrowseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new System.Windows.Forms.FolderBrowserDialog();
        //    string selectDirectory = SelectDirectoryTextBox.Text;
        //    if (selectDirectory != "")
        //    {
        //        dialog.SelectedPath = selectDirectory;
        //    }

        //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

        //    if (result == System.Windows.Forms.DialogResult.OK)
        //    {
        //        selectDirectory = dialog.SelectedPath;
        //        SelectDirectoryTextBox.Text = dialog.SelectedPath;
        //    }
        //}

        //private void StartWatchingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new System.Windows.Forms.FolderBrowserDialog();

        //    string selectDirectory = SelectDirectoryTextBox.Text;

        //    if (selectDirectory == "")
        //    {
        //        MessageBox.Show("Please select a directory path.");
        //    }
        //    else
        //    {
        //        string[] fileEntries = Directory.GetFiles(selectDirectory + "\\");
        //        string name = "";
        //        string patientMRN = null;

        //        string[] a = new string[1];

        //        int fileExists = fileEntries.Length;

        //        string path1 = null;
        //        string path2 = null;
        //        string path3 = null;

        //        string sourceFile = null;
        //        string destinationFile = null;

        //        if (fileExists > 0)
        //        {

        //            foreach (string fileName in fileEntries)
        //            {
        //                bool b1 = fileName.Contains(".M");

        //                if (b1)
        //                {
        //                    int index = fileName.IndexOf(".M");
        //                    patientMRN = fileName.Substring(index + 1, 8);

        //                    a[0] = patientMRN;
        //                }

        //                Console.WriteLine("Processed file '{0}'.", fileName);

        //                name = name + " " + fileName + " identified. \r";
        //                WatchingLogTextBox.Text = name;
        //            }

        //            bool bRD = name.Contains("RD");
        //            bool bRP = name.Contains("RP");
        //            bool bRS = name.Contains("RS");
        //            bool bPatient = name.Contains(a[0]);

        //            string b = null;

        //            if (bPatient)
        //            {
        //                if (bRD && bRP && bRS)
        //                {
        //                    name = name + " " + patientMRN + " patient plans are ready\r";
        //                    WatchingLogTextBox.Text = name;

        //                    path1 = selectDirectory + "\\";
        //                    path2 = path1 + "NewData";
        //                    path3 = path2 + "\\" + patientMRN;

        //                    if (!Directory.Exists(path2))
        //                    {
        //                        Directory.CreateDirectory(path2);

        //                        name = name + " " + path2 + " folder created";
        //                        WatchingLogTextBox.Text = name;                                
        //                    }


        //                    if (!Directory.Exists(path3))
        //                    {
        //                        Directory.CreateDirectory(path3);

        //                        name = name + " " + path3 + " patient folder created";
        //                        WatchingLogTextBox.Text = name;
        //                    }

        //                    foreach (string fileName in fileEntries)
        //                    {
        //                        b = fileName.Substring(fileName.IndexOf("\\R") + 1, fileName.Length - selectDirectory.Length - 1);
        //                        sourceFile = fileName;
        //                        destinationFile = selectDirectory + "\\NewData\\" + patientMRN + "\\" + b;

        //                        System.IO.File.Move(sourceFile, destinationFile);

        //                        name = name + " " + b + " has been moved to the patient folder\r";
        //                        WatchingLogTextBox.Text = name;

        //                    }

        //                    string[] cineFileEntries = Directory.GetFiles(selectDirectory + "\\NewData\\" + patientMRN + "\\");

        //                    int cineFileExists = cineFileEntries.Length;

        //                    if (cineFileExists > 0)
        //                    {
        //                        name = name + " The patient Cine images exist\r";
        //                        WatchingLogTextBox.Text = name;
        //                    }

        //                    Process.Start("IExplore.exe");

        //                }
        //                else if (bRD == false)
        //                {
        //                    Console.WriteLine("There is no RD plan for '{0}' patient.", a[0]);
        //                }
        //                else if (bRP == false)
        //                {
        //                    Console.WriteLine("There is no RP plan for '{0}' patient.", a[0]);
        //                }
        //                else if (bRS == false)
        //                {
        //                    Console.WriteLine("There is no RS plan for '{0}' patient.", a[0]);
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Invalid patient information.");
        //            }



        //        }
        //        else
        //        {
        //            MessageBox.Show("No files exists.");
        //        }
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string args = "";
        //    string exeName = "AutoViper.exe";

        //    //runProcess(exeName, args);
        //}
        #endregion 

        // A method to run VIPER automatically
        private void runProcess(string executableName, string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C \"" + executableName + " " + arguments + '"';

            Console.WriteLine("arguments " + arguments);

            bool showOutput = false;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = showOutput;
            process.StartInfo.RedirectStandardOutput = showOutput;
            process.StartInfo.RedirectStandardError = !showOutput;
            process.EnableRaisingEvents = true;

            process.Exited += new EventHandler(runProcess_Exited);

            process.Start();

           

            //process.WaitForExit(150);

            //process.Close();
        }

        private void runProcess_Exited(object sender, System.EventArgs e)
        {
            if ((!string.IsNullOrEmpty(ExitedPatientMRN)) && (!string.IsNullOrEmpty(ExitedPdfDirectory)) && (!string.IsNullOrEmpty(ExitedNewPatientFolder)) && (!string.IsNullOrEmpty(ExitedCreatedDirectory)) && (!string.IsNullOrEmpty(ExitedPatientPlanName)))
            {
                ArchiveProcess(ExitedPatientMRN, ExitedPdfDirectory, ExitedNewPatientFolder, ExitedCreatedDirectory, ExitedPatientPlanName);
            }
        }
    }
}
