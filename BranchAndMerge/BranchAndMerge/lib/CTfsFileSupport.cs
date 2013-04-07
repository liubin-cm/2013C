
namespace BranchAndMerge.lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using log4net;

    /// <summary>
    /// read config file and write log file
    /// </summary>
    public class CTfsFileSupport
    {

        public string tfsserverurl;
        public CTfsFileSupport()
        {
        }

        public string[] GetProjectNames(string filename)
        {
            List<string> projectname = new List<string>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            tfsserverurl = xmldoc.DocumentElement.Attributes["url"].Value;

            if (topM.Count > 0)
            {
                foreach (XmlElement element in topM)
                {
                    if (element.Name == "project")
                    {
                        string name = element.Attributes["name"].Value;
                        if (name != string.Empty)
                            projectname.Add(name);
                    }
                }
            }
            return projectname.ToArray();
          }
        }

    /// <summary>
    /// log class
    /// </summary>
    public class LogUtil
    {

        private static readonly log4net.ILog bizLog = log4net.LogManager.GetLogger("BusinessLogger");

        public static void Log(string message)
        {
            try
            {
                if (!bizLog.IsInfoEnabled)
                {
                    log4net.Config.XmlConfigurator.Configure();
                }
                bizLog.Info(message);                
                bizLog.Debug(message);
                bizLog.Error(message);
                bizLog.Fatal(message);
                
            }
            catch (Exception ex)
            {
                ex.GetType();
            }
        }
    }


}
