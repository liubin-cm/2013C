// -----------------------------------------------------------------------
// <copyright file="CProcess.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BranchAndMerge.lib
{
    using System;
    using System.Text;
    using System.Threading;
    //fdfds
    class ReadErrorThread
    {
        System.Threading.Thread m_Thread;
        System.Diagnostics.Process m_Process;
        String m_Error;
        bool m_HasExisted;
        object m_LockObj = new object();

        public String Error
        {
            get
            {
                return m_Error;
            }
        }

        public bool HasExisted
        {
            get
            {
                lock (m_LockObj)
                {
                    return m_HasExisted;
                }
            }

            set
            {
                lock (m_LockObj)
                {
                    m_HasExisted = value;
                }
            }
        }

        private void ReadError()
        {
            StringBuilder strError = new StringBuilder();
            while (!m_Process.HasExited)
            {
                strError.Append(m_Process.StandardError.ReadLine());
            }

            strError.Append(m_Process.StandardError.ReadToEnd());

            m_Error = strError.ToString();
            HasExisted = true;
        }

        public ReadErrorThread(System.Diagnostics.Process p)
        {
            HasExisted = false;
            m_Error = "";
            m_Process = p;
            m_Thread = new Thread(new ThreadStart(ReadError));
            m_Thread.Start();
        }

    }

    class CProcess
    {
        private String m_Error;
        private String m_Output;

        public String Error
        {
            get
            {
                return m_Error;
            }
        }

        public String Output
        {
            get
            {
                return m_Output;
            }
        }

        public bool HasError
        {
            get
            {
                return m_Error != "" && m_Error != null;
            }
        }

        public void Run(String fileName, String para, string workingDirectory="")
        {
            StringBuilder outputStr = new StringBuilder();

            if (workingDirectory==string.Empty)
            {
                workingDirectory = System.Environment.CurrentDirectory;
            }
            m_Error = "";
            m_Output = "";
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = para;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = workingDirectory;

                p.Start();

                ReadErrorThread readErrorThread = new ReadErrorThread(p);

                while (!p.HasExited)
                {
                    outputStr.Append(p.StandardOutput.ReadLine() + "\r\n");
                }

                outputStr.Append(p.StandardOutput.ReadToEnd());

                while (!readErrorThread.HasExisted)
                {
                    Thread.Sleep(1);
                }

                m_Error = readErrorThread.Error;
                m_Output = outputStr.ToString();
            }
            catch (Exception e)
            {
                m_Error = e.Message;
            }
        }
    }
}
