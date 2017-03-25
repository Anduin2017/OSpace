using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
namespace OSpace
{
    /// <summary>
    /// 核心编辑类,将给出所有静态的可用于直接调用的函数
    /// </summary>
    public static class CoreEdit
    {
        //直接执行命令
        public static string direct_command(string cmd)
        {
            ProcessStartInfo start = new ProcessStartInfo("cmd.exe");
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            Process pro = Process.Start(start);
            pro.StartInfo.Arguments = "";//启动参数
            pro.StandardInput.WriteLine(cmd);
            pro.StandardInput.WriteLine("exit");
            string result = pro.StandardOutput.ReadToEnd();
            return result;
        }
        /// <summary>
        /// Diskpart核心编辑函数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="fastedition"></param>
        /// <returns></returns>
        public static string Diskpart(string[] cmd)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo("diskpart.exe");
                start.RedirectStandardInput = true;
                start.RedirectStandardOutput = true;
                start.UseShellExecute = false;
                start.CreateNoWindow = true;
                Process pro = Process.Start(start);
                foreach(string cmd1 in cmd)
                    pro.StandardInput.WriteLine(cmd1);
                pro.StandardInput.WriteLine("exit");
                string res = (pro.StandardOutput.ReadToEnd());
                return res;
            }
            catch
            {
                MessageBox.Show("Critical Error!!!!");
                return "";
            }
        }
        #region 对BCD进行编辑的核心控制函数组
        /// <summary>
        /// 核心编辑函数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="fastedition"></param>
        /// <returns></returns>
        public static string bcdedit(string cmd, bool fastedition)
        {
            try
            {
                //MessageBox.Show(cmd);
                //bcdedit高效版和兼容版区分
                string bcdedit_edition;
                if (fastedition)
                    bcdedit_edition = "bcdedit.exe";
                else
                    bcdedit_edition = "bcdedit2.exe";
                ProcessStartInfo start = new ProcessStartInfo(Environment.CurrentDirectory + "\\Core\\" + bcdedit_edition, " " + cmd);
                start.RedirectStandardInput = true;
                start.RedirectStandardOutput = true;
                start.UseShellExecute = false;
                start.CreateNoWindow = true;
                Process pro = Process.Start(start);
                string res = (pro.StandardOutput.ReadToEnd());
                //MessageBox.Show(cmd + res);
                return res;
            }
            catch
            {
                MessageBox.Show("Critical Error!!!!");
                return "";
            }
        }

        /// <summary>
        /// 删除特定系统的属性值
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="propertyname"></param>
        public static string deleteproperty(string identifier, string propertyname)
        {
            return bcdedit("/deletevalue " + identifier + " " + propertyname, true);
        }

        /// <summary>
        /// 直接对特定系统的属性值进行修改
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="propertyname"></param>
        /// <param name="propertyvalue"></param>
        public static void setproperty(string identifier, string propertyname, string propertyvalue)
        {
            bcdedit(" /set " + identifier + " " + propertyname + " " + "\"" + propertyvalue + "\"", true);
        }

        /// <summary>
        ///API：在一大群OS中,找出特点标识符的OS,代入一大群OS数组,所求的OS标识符,返回OS的属性表
        /// </summary>
        /// <param name="一大群OS数组"></param>
        /// <param name="所求的OS标识符"></param>
        /// <returns>OS的属性表</returns>
        public static string get_OS(string[] OSs, string identifier)
        {
            foreach (string os in OSs)
                if (get_profile(os, "identifier") == identifier)
                    return os;
            return "No such OS!";
            //throw new Exception("No such OS!");
        }

        /// <summary>
        ///API：在一个OS的一大群属性中,找出特定的属性,代入属性表,所求的属性名,返回对应属性值
        /// </summary>
        /// <param name="属性表"></param>
        /// <param name="所求的属性名"></param>
        /// <returns>对应属性值</returns>
        public static string get_profile(string OS_Profiles, string Target_Profile)
        {
            int position = (OS_Profiles.IndexOf('-'));
            if (position == -1)
                return "Null";
            OS_Profiles = OS_Profiles.Substring(position);
            Regex regidentifier = new Regex(@"(?im)(?<=^\s*" + Target_Profile + @" \s*).+$");
            foreach (Match reg in regidentifier.Matches(OS_Profiles))
                return reg.Value.Trim();
            return "Null";
        }

        /// <summary>
        ///API：在一个OS的一大群属性中,找出表头,代入属性表,返回对应表头。和上面函数+1重载
        /// </summary>
        /// <param name="属性表"></param>
        /// <returns>对应表头</returns>
        public static string get_profile(string OS_Profiles)
        {
            try
            {
                int stop = OS_Profiles.IndexOf("\r\n-");
                OS_Profiles = OS_Profiles.Substring(0, stop);
                return OS_Profiles.Trim();
            }
            catch
            {
                return "Null";
                //throw new Exception("无法获取到表头,代入内容不合法！");
            }
        }

        /// <summary>
        ///API：在一个OS的一大群属性中,找出特定的属性,代入属性表,所求的属性名,返回对应属性值数组类型
        /// </summary>
        /// <param name="属性表"></param>
        /// <param name="所求的属性名"></param>
        /// <returns>对应属性值数组类型</returns>
        public static string[] get_profile_List(string OS_Profiles, string Target_Profile)
        {
            try
            {
                int position = OS_Profiles.IndexOf(Target_Profile.Trim());
                OS_Profiles = OS_Profiles.Substring(position);
                List<string> returnvalve = new List<string>();
                OS_Profiles = OS_Profiles.Substring(24).Replace("\r", "").Replace(" ", "").Replace("\n", "");
                string[] words = OS_Profiles.Split('}');
                foreach (string word in words)
                    if (word.Substring(0, 1) == "{")
                        returnvalve.Add(word + "}");
                    else
                        break;
                return returnvalve.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("该属性提取失败,发生" + e.ToString() + "错误");
            }
        }
        /// <summary>
        /// 代入字符串,返回里面存在的唯一的标识符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string[] get_only_one_Identifier(string Input)
        {
            try
            {
                int position = Input.IndexOf('{');
                Input = Input.Substring(position);
                List<string> returnvalve = new List<string>();
                Input = Input.Replace("\r", "").Replace(" ", "").Replace("\n", "");
                string[] words = Input.Split('}');
                foreach (string word in words)
                    if (word.Substring(0, 1) == "{")
                        returnvalve.Add(word + "}");
                    else
                        break;
                return returnvalve.ToArray();
            }
            catch
            {
                return new string[1] { "" };
                MessageBox.Show("Critical Error");

            }
        }
        #endregion

        public struct version
        {
            public int ProductMajorPart;
            public int ProductMinorPart;
            public int ProductBuildPart;
            public int ProductPrivatePart;
        }
        #region 相关系统资源处理函数组
        /// <summary>
        /// 获取剩余空间,单位B
        /// </summary>
        /// <param name="driveDirectoryName"></param>
        /// <returns></returns>
        public static long GetFreeSpace(string driveDirectoryName)
        {
            try
            {
                long freefreeBytesAvailable = 0;
                DriveInfo drive = new DriveInfo(driveDirectoryName);
                freefreeBytesAvailable = drive.AvailableFreeSpace;
                return freefreeBytesAvailable;
            }
            catch
            {
                return 1;
            }
        }
        /// <summary>
        /// 获取总空间,单位B
        /// </summary>
        /// <param name="str_HardDiskName"></param>
        /// <returns></returns>
        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            str_HardDiskName += ":\\";
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
                if (drive.Name == str_HardDiskName)
                    return drive.TotalSize;
            return 1;
        }
        /// <summary>
        /// 得到系统函数的版本号,用户判断系统版本。
        /// </summary>
        /// <param name="path"></param>
        /// <returns>结构体version,含有四个int型变量用于描述版本号</returns>
        public static version PrintFileVersionInfo(string path)
        {
            version returnversion;
            returnversion.ProductMajorPart = 0;
            returnversion.ProductMinorPart = 0;
            returnversion.ProductBuildPart = 0;
            returnversion.ProductPrivatePart = 0;
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(path);
            }
            catch
            {
                return returnversion;
            }
            // 如果文件存在
            if (fileInfo != null && fileInfo.Exists)
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
                Console.WriteLine("系统显示文件版本：" + info.ProductMajorPart + '.' + info.ProductMinorPart + '.' + info.ProductBuildPart + '.' + info.ProductPrivatePart);
                returnversion.ProductMajorPart = info.ProductMajorPart;
                returnversion.ProductMinorPart = info.ProductMinorPart;
                returnversion.ProductBuildPart = info.ProductBuildPart;
                returnversion.ProductPrivatePart = info.ProductPrivatePart;
                return returnversion;
            }
            else
                throw new Exception("File doesn't exist");
        }
        #endregion

        #region 内部算法函数组
        /// <summary>
        /// string到bool类型的类型转换
        /// </summary>
        /// <param name="string"></param>
        /// <returns>Bool</returns>
        public static bool string2bool(string input)
        {
            return input == "True" ? true : false;
        }
        //得到这么多的空格数
        public static string count2space(int i)
        {
            string re = "";
            for (int j = 0; j < i; j++)
                re += " ";
            return re;
        }
        //判断string[]中是否包含一个string
        public static bool stringlistcontainstring(string[] list, string input)
        {
            foreach (string li in list)
                if (li.Trim() == input.Trim())
                    return true;
            return false;
        }
        #endregion
    }
}
