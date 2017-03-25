using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Windows.Media;

namespace OSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 静态变量初始化 全局对象实例化
        public static bool Loading = true;
        public static bool FirstStart = false, firsttimeloadlanguage = true;
        public static bool from_UEFI = false;
        public static string partition = null, identifier = null, local = null;
        public static string UEFI_Lable_Content = null, BIOS_Label_Content = null;
        public static string advancedcontent = null, normalcontent = null;
        public static string LangXMLPaht = null;
        public static string editing_System = null;
        public static bool isnormalmode = true;
        public static bool showingmessage = false;
        public static string successfully = null; //save
        public static string failed = null;
        public static string successfully_edit = null;
        public static string successfully_settodefault;
        public static string erroroccured;
        public static string copyto;
        public static string deletesuc;
        public static string erroredit;
        public static string unknownos;
        public static string unknownosname;
        public static int Bootmenuselectedindex = -1;
        public static bool verbcd = true;
        AnOS thisOS = null;
        public static string[] OSs = null;
        public static string result = null;
        Login loginwindow = new Login();
        Powerbutton pow = new Powerbutton();

        System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

        #endregion

        #region 程序宏观控制函数

        //告知系统列表中所有系统当前选择的是哪个
        public void Refresh_Menuitem_Sel()
        {
            AnOSX.Bootmenuselectedindex___ = Bootmenuselectedindex;
            int i = 0;
            Dispatcher.Invoke(new Action(() =>
            {
                foreach (AnOSX a in Bootmenu.Children)
                {
                    a.meindex = i;
                    i++;
                }
            }));
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //任意位置拖动
            if (e.LeftButton == MouseButtonState.Pressed && drag.IsChecked == true)
            {
                DragMove();
            }
        }
        private void ShowCaution()
        {
            for (byte r = 106, g = 121, b = 136; r > 0 && g < 182 && b > 17;)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    downdraw.Color = Color.FromArgb(255, r, g, b);
                }));
                r -= 10;
                g += 6;
                b -= 12;
                Thread.Sleep(25);
            }
            Thread.Sleep(1600);
            Dispatcher.Invoke(new Action(() =>
            {
                Cautiontext.Content = "";
            }));
            for (byte r = 0, g = 182, b = 17; r < 106 && g > 121 && b < 136;)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    downdraw.Color = Color.FromArgb(255, r, g, b);
                }));
                r += 10;
                g -= 6;
                b += 12;
                Thread.Sleep(20);

            }
            Dispatcher.Invoke(new Action(() =>
            {
                downdraw.Color = Color.FromArgb(255, 106, 121, 136);
            }));
            showingmessage = false;
        }
        private void ShowFail()
        {
            for (byte r = 106, g = 121, b = 136; r < 234 && g > 49 && b > 20;)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    downdraw.Color = Color.FromArgb(255, r, g, b);
                }));
                r += 13;
                g -= 7;
                b -= 12;
                Thread.Sleep(25);
            }
            Thread.Sleep(1600);
            Dispatcher.Invoke(new Action(() =>
            {
                Cautiontext.Content = "";
            }));
            for (byte r = 234, g = 49, b = 20; r > 106 && g < 121 && b < 136;)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    downdraw.Color = Color.FromArgb(255, r, g, b);
                }));
                r -= 13;
                g += 7;
                b += 12;
                Thread.Sleep(20);

            }
            Dispatcher.Invoke(new Action(() =>
            {
                downdraw.Color = Color.FromArgb(255, 106, 121, 136);
            }));
            showingmessage = false;
        }
        public void Caution(string contentf)
        {
            if (showingmessage)
            {
                return;
            }

            showingmessage = true;
            Dispatcher.Invoke(new Action(() =>
            {
                Cautiontext.Content = contentf.Replace("\r", "").Replace("\n", "").Trim();
            }));
            Thread cau = new Thread(ShowCaution);
            cau.Start();
        }
        public void Fail(string contentf)
        {
            if (showingmessage)
            {
                return;
            }
            showingmessage = true;
            Dispatcher.Invoke(new Action(() =>
            {
                Cautiontext.Content = contentf.Replace("\r", "").Replace("\n", "").Trim();
            }));
            Thread cau = new Thread(ShowFail);
            cau.Start();
        }
        //登录按钮的代码
        private void But_Signin_Click(object sender, RoutedEventArgs e)
        {
            loginwindow.readytodo = true;
            loginwindow.draggable = false;
            if (WindowState == WindowState.Maximized)
            {
                loginwindow.setpoint(new Point(-7, SystemParameters.PrimaryScreenWidth - 840), this, LangXMLPaht);
            }
            else
            {
                loginwindow.setpoint(new Point(Top, Left + ActualWidth - 840), this, LangXMLPaht);
            }
            loginwindow.Show();
        }
        //右下角重启按钮
        private void restart_Click_1(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                pow.setpoint(new Point(SystemParameters.PrimaryScreenHeight, SystemParameters.PrimaryScreenWidth - 840), LangXMLPaht, 0);
            }
            else
            {
                pow.setpoint(new Point(Top + 2, Left + ActualWidth - 838), LangXMLPaht, this.Height);
            }
            pow.Show();
        }
        //控制窗口缩放
        private void MainWindow1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stretch.Header = CoreEdit.count2space((int) (this.Width + 200) / 20);
            if (!(Width < 690))
            {
                return;
            }
            stretch.Header = "";
        }
        //加载当前高级和小白状态，并切换
        private void OSHead_Copy1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isnormalmode = !isnormalmode;
            Loadworkingmode(isnormalmode);
        }
        private void Loadworkingmode(bool isnormal)
        {
            //Boot_Menu_Item.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            if (isnormal == true)
            {
                show_active_os.IsChecked = true;
            }

            Disk_Item.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            Firmware_Item.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            Settings_Item.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            OSHead_Copy1.Text = isnormal ? advancedcontent : normalcontent;
            check_Copy.Visibility = Advanced_Mode.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            refresh.Visibility = Advanced_Mode.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            Advaninsettings.IsChecked = isnormal == false;
            ban1.Visibility = ban2.Visibility = ban3.Visibility = list1_Copy3.Visibility = list4.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            image.Visibility = list4_Copy1.Visibility = list1_Copy5.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
            Security_Item.Visibility = isnormal ? Visibility.Hidden : Visibility.Visible;
        }
        private void staticrefresh()
        {
            Refresh();
            foreach (AnOS nowOS in OS_List_Grid.Children)
            {
                if (nowOS.Identifiervalve_String == editing_System)
                {
                    test(nowOS, null);
                }
            }
        }

        #endregion

        #region 程序启动后一路执行，包含刷新等核心函数

        public MainWindow()
        {
            InitializeComponent();
        }
        //窗体加载
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //test();

            //下面这句话可以加载系统显示语言，请注意使用!
            //MessageBox.Show(System.Globalization.CultureInfo.InstalledUICulture.DisplayName);
            //更新汇编号
            Versionlab.Content = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Thread Load = new Thread(Encodpass);
            Load.Start();
        }
        //新功能测试函数
        private void test()
        {

        }
        //从文件中初始化所有设置和语言信息
        private void Encodpass()
        {
            LoadLanguageFiles();
            LoadSettings();
            LoadLanguage();
            Loading = true;
            Refresh();
            if (from_UEFI)
            {
                RefreshFirmware();
            }
            LoadStorePics();
            if (FirstStart)
            {
                //启动首次启动欢迎界面
            }
            Loading = false;
        }

        private void LoadLanguageFiles()
        {
            string[] fn = Directory.GetFiles("Lang\\");
            foreach (string s in fn)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    LangCombobox.Items.Add(s.Substring(5).Replace(".xml", ""));
                }));
            }
        }
        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSettings()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo dri in allDrives)
                Dispatcher.Invoke(new Action(() =>
                {
                    partitions_combo.Items.Add(dri.Name);
                }));
            //打开对应设置文件
            string pathXml = "user.xml";
            try
            {
                StreamReader reader = new StreamReader(pathXml);
                Settings u = Xml.XmlHelper.DeSerialize<Settings>(reader);
                int Langcombindex = u.Selected_Lang_Index;
                bool ischeck = CoreEdit.string2bool(u.ischeck);
                bool show_active_os_ischeck = CoreEdit.string2bool(u.show_active_os);
                bool isdrag = CoreEdit.string2bool(u.drag);
                bool mulprocess = CoreEdit.string2bool(u.multiprocess);
                bool loadisnormalmode = CoreEdit.string2bool(u.isnormalmode);
                Dispatcher.Invoke(new Action(() =>
                {
                    //隐藏正常模式不显示的项
                    isnormalmode = loadisnormalmode;
                    Loadworkingmode(isnormalmode);
                    LangCombobox.SelectedIndex = Langcombindex;
                    LangXMLPaht = "Lang/" + LangCombobox.Text + ".xml";
                    verbcd = ischeck;
                    check.IsChecked = ischeck;
                    show_active_os.IsChecked = show_active_os_ischeck;
                    drag.IsChecked = isdrag;
                    Multiprocessallow.IsChecked = mulprocess;
                }));
                reader.Close();
            }
            //这里是默认设置
            catch
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    isnormalmode = true;
                    Loadworkingmode(isnormalmode);
                    LangCombobox.SelectedIndex = 0;
                    LangXMLPaht = "Lang/" + LangCombobox.Text + ".xml";
                    check.IsChecked = true;
                    show_active_os.IsChecked = true;
                    drag.IsChecked = true;
                    Multiprocessallow.IsChecked = false;
                }));
                FirstStart = true;
            }
        }
        /// <summary>
        /// 加载语言包
        /// </summary>
        private void LoadLanguage()
        {
            //打开对应语言包
            StreamReader LangReader = null;
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    LangXMLPaht = "Lang/" + LangCombobox.Text + ".xml";
                }));
                LangReader = new StreamReader(LangXMLPaht);
                Language Lang = Xml.XmlHelper.DeSerialize<Language>(LangReader);
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        //将对应语言包内容写入UI
                        string tittle = Xml.DESHelper.Decrypt(Lang.Heading);
                        Lab_OSpace.Content = tittle;
                        MainWindow1.Title = tittle;
                        Home_Item.Header = Xml.DESHelper.Decrypt(Lang.Item1);
                        OS_List_Item.Header = Xml.DESHelper.Decrypt(Lang.Item2);
                        OSList.Text = Xml.DESHelper.Decrypt(Lang.Item2);
                        Boot_Menu_Item.Header = Xml.DESHelper.Decrypt(Lang.Item3);
                        Disk_Item.Header = Xml.DESHelper.Decrypt(Lang.Item4);
                        Settings_Item.Header = Xml.DESHelper.Decrypt(Lang.Item5);
                        But_Signin.Content = Xml.DESHelper.Decrypt(Lang.Signin);
                        Lab_QuickAccess.Content = Xml.DESHelper.Decrypt(Lang.Quick_Access);
                        ChannelInforDescripe.Text = Xml.DESHelper.Decrypt(Lang.describe);
                        Auto_diagnose.Content = Xml.DESHelper.Decrypt(Lang.Auto_diagnose);
                        Standing_by.Content = Xml.DESHelper.Decrypt(Lang.Standing_by);
                        UEFI_Lable_Content = Xml.DESHelper.Decrypt(Lang.UEFI_working_mode);
                        BIOS_Label_Content = Xml.DESHelper.Decrypt(Lang.BIOS_working_mode);
                        Lab_access1.Text = Xml.DESHelper.Decrypt(Lang.access1);
                        Lab_access2.Text = Xml.DESHelper.Decrypt(Lang.access2);
                        Lab_access3.Text = Xml.DESHelper.Decrypt(Lang.access3);
                        Lab_access4.Text = Xml.DESHelper.Decrypt(Lang.access4);
                        Lab_program_edition.Content = Xml.DESHelper.Decrypt(Lang.program_edition);
                        OSHead_Copy1.Text = Xml.DESHelper.Decrypt(Lang.changemode);
                        Wrap.Text = Xml.DESHelper.Decrypt(Lang.statement);
                        list1.Text = Xml.DESHelper.Decrypt(Lang.list1);
                        list2.Text = Xml.DESHelper.Decrypt(Lang.list2);
                        list3.Text = Xml.DESHelper.Decrypt(Lang.list3);
                        list4.Text = Xml.DESHelper.Decrypt(Lang.list4);
                        Tex_content1.Text = Xml.DESHelper.Decrypt(Lang.content1);
                        list1_Copy.Text = Xml.DESHelper.Decrypt(Lang.list1_Copy);
                        list1_Copy1.Text = Xml.DESHelper.Decrypt(Lang.list1_Copy1);
                        list1_Copy2.Text = Xml.DESHelper.Decrypt(Lang.list1_Copy2);
                        list1_Copy3.Text = Xml.DESHelper.Decrypt(Lang.list1_Copy3);
                        show_active_os.Content = Xml.DESHelper.Decrypt(Lang.show_active_os);
                        Show_ALL_OS.Content = Xml.DESHelper.Decrypt(Lang.Show_ALL_OS);
                        partition = Xml.DESHelper.Decrypt(Lang.partition);
                        identifier = Xml.DESHelper.Decrypt(Lang.identifier);
                        local = Xml.DESHelper.Decrypt(Lang.local);
                        Header.Content = Xml.DESHelper.Decrypt(Lang.os_type);
                        Lab_inherit.Content = Xml.DESHelper.Decrypt(Lang.inherit);
                        Lab_path.Content = Xml.DESHelper.Decrypt(Lang.path);
                        Lab_local.Content = Xml.DESHelper.Decrypt(Lang.Lab_local);
                        Lab_device.Content = Xml.DESHelper.Decrypt(Lang.device);
                        Lab_identifier.Content = Xml.DESHelper.Decrypt(Lang.Lab_identifier);
                        Advanced_Mode.Text = Xml.DESHelper.Decrypt(Lang.advancedmode);
                        Lab_Systemroot.Content = Xml.DESHelper.Decrypt(Lang.system_root);
                        copy.Content = Xml.DESHelper.Decrypt(Lang.But_copy);
                        delete.Content = Xml.DESHelper.Decrypt(Lang.But_delete);
                        _return.Content = " " + Xml.DESHelper.Decrypt(Lang.But_return);
                        Copy_description.Text = Xml.DESHelper.Decrypt(Lang.copy_description);
                        Delete_Description.Text = Xml.DESHelper.Decrypt(Lang.delete_description);
                        But_remove.Content = Xml.DESHelper.Decrypt(Lang.But_remove);
                        But_delete_files.Content = Xml.DESHelper.Decrypt(Lang.But_delete_files);
                        Cancel.Content = Xml.DESHelper.Decrypt(Lang.Cancel);
                        Pro.Content = Xml.DESHelper.Decrypt(Lang.Pro);
                        Appearance.Header = Xml.DESHelper.Decrypt(Lang.Appearance);
                        Core.Header = Xml.DESHelper.Decrypt(Lang.Core);
                        About.Header = Xml.DESHelper.Decrypt(Lang.about);
                        Advanced_Mode1.Header = Xml.DESHelper.Decrypt(Lang.Advanced_mode);
                        Language.Header = Xml.DESHelper.Decrypt(Lang.Language_choose);
                        Target_Property.Content = Xml.DESHelper.Decrypt(Lang.Target_Property);
                        Valve.Content = Xml.DESHelper.Decrypt(Lang.Valve);
                        Display_name.Content = Xml.DESHelper.Decrypt(Lang.Display_name);
                        Diskusage.Content = Xml.DESHelper.Decrypt(Lang.Diskusage);
                        Anywhere_draggable.Content = Xml.DESHelper.Decrypt(Lang.Anywhere_draggable);
                        textBlock_copy1.Text = Xml.DESHelper.Decrypt(Lang.textBlock_copy1);
                        textBlock_copy2.Text = Xml.DESHelper.Decrypt(Lang.textBlock_copy2);
                        advancedcontent = Xml.DESHelper.Decrypt(Lang.advancedmodebutton);
                        normalcontent = Xml.DESHelper.Decrypt(Lang.normalmodebutton);
                        Firmware_Item.Header = Xml.DESHelper.Decrypt(Lang.Firmware_Item);
                        Phe.Content = Xml.DESHelper.Decrypt(Lang.Phe);
                        DEP.Content = Xml.DESHelper.Decrypt(Lang.DHP);
                        Debugging_windows.Content = Xml.DESHelper.Decrypt(Lang.Debugging_windows);
                        Lab_sos.Content = Xml.DESHelper.Decrypt(Lang.Lab_sos);
                        Lab_decline_RAM.Content = Xml.DESHelper.Decrypt(Lang.Lab_decline_RAM);
                        Lab_MaxCPU.Content = Xml.DESHelper.Decrypt(Lang.Lab_MaxCPU);
                        successfully = Xml.DESHelper.Decrypt(Lang.successfully);
                        failed = Xml.DESHelper.Decrypt(Lang.failed);
                        successfully_edit = Xml.DESHelper.Decrypt(Lang.successfully_edit);
                        //successfully_refresh = Xml.DESHelper.Decrypt(Lang.successfully_refresh);
                        Safe.Content = Xml.DESHelper.Decrypt(Lang.safe);
                        SaveRam.Content = Xml.DESHelper.Decrypt(Lang.save);
                        SaveCPU.Content = Xml.DESHelper.Decrypt(Lang.save);
                        button_Save.Content = Xml.DESHelper.Decrypt(Lang.save);
                        button_CoreMode.Content = Xml.DESHelper.Decrypt(Lang.coremode);
                        button_Previous.Content = Xml.DESHelper.Decrypt(Lang.previous);
                        resetall.Content = Xml.DESHelper.Decrypt(Lang.settodefault);
                        Store_settings.Header = Xml.DESHelper.Decrypt(Lang.storesettings);
                        safemode.Items[0] = Xml.DESHelper.Decrypt(Lang.default_item);
                        safemode.Items[1] = Xml.DESHelper.Decrypt(Lang.save_item);
                        safemode.Items[2] = Xml.DESHelper.Decrypt(Lang.save_net_item);
                        safemode.Items[3] = Xml.DESHelper.Decrypt(Lang.save_com_item);
                        safemode.Items[4] = Xml.DESHelper.Decrypt(Lang.basic_vga);
                        PAE.Items[0] = Xml.DESHelper.Decrypt(Lang.default_p_item);
                        PAE.Items[1] = Xml.DESHelper.Decrypt(Lang.on_item);
                        PAE.Items[2] = Xml.DESHelper.Decrypt(Lang.off_item);
                        DEP_Value.Items[0] = Xml.DESHelper.Decrypt(Lang.optional_on);
                        DEP_Value.Items[1] = Xml.DESHelper.Decrypt(Lang.optional_of);
                        DEP_Value.Items[2] = Xml.DESHelper.Decrypt(Lang.always_on);
                        DEP_Value.Items[3] = Xml.DESHelper.Decrypt(Lang.always_off);
                        erroroccured = Xml.DESHelper.Decrypt(Lang.erroroccured);
                        copyto = Xml.DESHelper.Decrypt(Lang.copyto);
                        deletesuc = Xml.DESHelper.Decrypt(Lang.deletesuc);
                        erroredit = Xml.DESHelper.Decrypt(Lang.erroredit);
                        Multiprocess.Content = Xml.DESHelper.Decrypt(Lang.MultyProcess);
                        OSHead_Copy1.Text = !isnormalmode ? normalcontent : advancedcontent;
                        successfully_settodefault = Xml.DESHelper.Decrypt(Lang.settodefaultsuc);
                        unknownos = Xml.DESHelper.Decrypt(Lang.unknownos);
                        unknownosname = Xml.DESHelper.Decrypt(Lang.unknownosname);
                        authorize.Header = Xml.DESHelper.Decrypt(Lang.Authorize);
                        Advaninset.Content = Xml.DESHelper.Decrypt(Lang.AdvancedMode);
                        Version.Content = Xml.DESHelper.Decrypt(Lang.Version);
                        FastEditTech.Content = Xml.DESHelper.Decrypt(Lang.FastEditTech);
                        list4_Copy.Text = Xml.DESHelper.Decrypt(Lang.MakeBootableUSB);
                        list4_Copy1.Text = Xml.DESHelper.Decrypt(Lang.AddLinux);
                        backup.Content = Xml.DESHelper.Decrypt(Lang.backupBootSetting);
                        rest.Content = Xml.DESHelper.Decrypt(Lang.restoreBootSetting);
                        backupnow.Content = Xml.DESHelper.Decrypt(Lang.backup);
                        restorenow.Content = Xml.DESHelper.Decrypt(Lang.restore);
                        select.Content = Xml.DESHelper.Decrypt(Lang.select);
                        select2.Content = Xml.DESHelper.Decrypt(Lang.select);
                        //SelVHD.Content = Xml.DESHelper.Decrypt(Lang.select);
                        SaveVHD.Content = Xml.DESHelper.Decrypt(Lang.select);
                        NEWVHD.Content = Xml.DESHelper.Decrypt(Lang.CreateAttachnewVD);
                        Attach.Content = Xml.DESHelper.Decrypt(Lang.AttachDetachexiVD);
                        Create.Content = Xml.DESHelper.Decrypt(Lang.create);
                        Attachbut.Content = Xml.DESHelper.Decrypt(Lang.attach);
                        Attachbut_Copy.Content = Xml.DESHelper.Decrypt(Lang.detach);


                        foreach (AnOS a in OS_List_Grid.Children)
                        {
                            a.Partition_String = partition;
                            a.Identifier_String = identifier;
                            a.Local_String = local;
                        }
                        foreach (AnOSX a in Bootmenu.Children)
                        {
                            a.identifierlabel = identifier;
                        }
                    }
                    catch (Exception e)
                    {
                        Fail(e.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                Fail(ex.ToString());
            }
            LangReader.Close();
            if (!firsttimeloadlanguage)
                Dispatcher.Invoke(new Action(() =>
                {
                    Lab_UEFI_working_mode.Text = from_UEFI ? UEFI_Lable_Content : BIOS_Label_Content;
                }));
            firsttimeloadlanguage = false;
            Loading = false;
        }


        //最关键的 刷新全部系统的函数
        /// <summary>
        /// 注意：在所有设置项修改时都应该执行此函数！线程内支持
        /// </summary>
        private void Refresh()
        {
            bool Show_ALL = false;
            Dispatcher.Invoke(new Action(() =>
            {
                all_clear();
                Show_ALL = Show_ALL_OS.IsChecked == true;
                makeactivebutton.Visibility = Show_ALL_OS.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
                MainTab.IsEnabled = false;
                Loadinglabel.Content = "Loading...";
                Loadinglabel.Visibility = Visibility.Visible;
            }));
            result = CoreEdit.bcdedit(" /enum all", verbcd);
            //对检索结果添加入列表
            //try
            //{
                OSs = result.Split(new string[1] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] OSnow = CoreEdit.get_profile_List(CoreEdit.get_OS(OSs, "{bootmgr}"), "\r\ndisplayorder");
                //将系统添加进系统列表
                if (!Show_ALL)
                {
                    //得到显示的系统列表
                    //对于每一个要显示的系统
                    string OS_Profiles;
                    foreach (string osindisplaylist in OSnow)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            OS_Profiles = CoreEdit.get_OS(OSs, osindisplaylist);
                            AddtoList(OS_Profiles, false, osindisplaylist);
                        }));
                    }
                }
                else
                {
                    //对于每个系统
                    foreach (string osindisplaylist in OSs)
                        Dispatcher.Invoke(new Action(() =>
                        {
                            AddtoList(osindisplaylist, true, CoreEdit.get_profile(osindisplaylist, "identifier"));
                        }));
                }
                //Bootmenu
                foreach (string osindisplaylist in OSnow)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        AddtoMenulist(osindisplaylist, CoreEdit.get_profile(CoreEdit.get_OS(OSs, osindisplaylist), "description"));
                    }));
                }
                Refresh_Menuitem_Sel();
            //}
            //常规异常
            //catch
            //{
            //    Fail("打开启动引导错误!请按下自动修复键.");
            //    return;
            //}
            //得到UEFI和BIOS区别项
            if (CoreEdit.get_OS(OSs, "{fwbootmgr}") == "No such OS!")
            {
                from_UEFI = false;
            }
            else
            {
                from_UEFI = true;
            }

            Dispatcher.Invoke(new Action(() =>
            {
                MBR.IsChecked = from_UEFI == false;
                GPT.IsChecked = from_UEFI == true;
                Lab_UEFI_working_mode.Text = from_UEFI ? UEFI_Lable_Content : BIOS_Label_Content;
                //解锁线程抑制
                MainTab.IsEnabled = true;
                Loadinglabel.Visibility = Visibility.Hidden;
                Loadinglabel.Content = Standing_by.Content;
            }));
            return;
        }

        private void RefreshFirmware()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Loadinglabel.Content = "Loading...";
                Loadinglabel.Visibility = Visibility.Visible;
                all_clear_Firmware();
            }));
            try
            {
                string[] FWnow = CoreEdit.get_profile_List(CoreEdit.get_OS(OSs, "{fwbootmgr}"), "\r\ndisplayorder");
                //对于每一个要显示的固件应用程序
                string FW_Profiles;
                foreach (string osindisplaylist in FWnow)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        FW_Profiles = CoreEdit.get_OS(OSs, osindisplaylist);
                        Addfwtolist(FW_Profiles, osindisplaylist);
                        //MessageBox.Show(FW_Profiles);
                    }));
                }
            }
            catch
            {

            }
            Dispatcher.Invoke(new Action(() =>
            {
                Loadinglabel.Visibility = Visibility.Hidden;
                Loadinglabel.Content = Standing_by.Content;
            }));
        }
        private void LoadStorePics()
        {
            //try
            //{
            //    string folderPath = "Tempimag";
            //    DirectoryInfo di = new DirectoryInfo(folderPath);
            //    FileInfo[] fis = di.GetFiles();
            //    if (fis == null)
            //    {
            //        Fail("没有图片！");
            //        Application.Current.Shutdown();
            //    }
            //    else
            //    {
            //        foreach (FileInfo fi in fis)
            //        {
            //            OSdisplayinSotre TempOS = new OSdisplayinSotre();
            //            TempOS.Name = fi.Name.Substring(0, fi.Name.Length - 4);
            //            TempOS.Photo = @"persons/" + fi.Name;
            //        }
            //    }
            //}
            //catch { }
        }
        #endregion

        #region 高级的动态增加函数、动态事件
        /// <summary>
        ///将一个系统添加到列表
        /// </summary>
        /// <param name="OS_Profiles"></param>
        /// <param name="playwithheader"></param>
        private void AddtoList(string OS_Profiles, bool playwithheader, string identi)
        {
            //实例系统对象并初始化其值
            AnOS a = null;
            if (playwithheader)
            {
                a = new AnOS(CoreEdit.get_profile(OS_Profiles) + " : " + CoreEdit.get_profile(OS_Profiles, "description").Replace("\r", ""));
            }
            else
            {
                a = new AnOS(CoreEdit.get_profile(OS_Profiles, "description").Replace("\r", ""));
            }
            a.all_Properties = OS_Profiles;
            a.Description_string = CoreEdit.get_profile(OS_Profiles, "description");
            a.Partitionvalve_String = CoreEdit.get_profile(OS_Profiles, "device").Replace("partition=", "");
            a.Identifiervalve_String = identi;
            a.Localvalve_String = CoreEdit.get_profile(OS_Profiles, "locale");
            a.path_string = CoreEdit.get_profile(OS_Profiles, "path");
            a.inherit_string = CoreEdit.get_profile(OS_Profiles, "inherit");
            a.osdevice_string = CoreEdit.get_profile(OS_Profiles, "osdevice");
            a.systemroot_string = CoreEdit.get_profile(OS_Profiles, "systemroot");
            a.Header_string = CoreEdit.get_profile(OS_Profiles);
            a.Cursor = Cursors.Hand;
            a.MouseLeftButtonUp += new MouseButtonEventHandler(test);
            //加载语言
            a.Partition_String = partition;
            a.Identifier_String = identifier;
            a.Local_String = local;
            OS_List_Grid.Children.Add(a);
        }
        private void AddtoMenulist(string iden, string namein)
        {
            AnOSX a = new AnOSX(this);
            a.identi = iden;
            a.identifierlabel = identifier;
            a.name = namein;
            Bootmenu.Children.Add(a);
        }
        private void Addfwtolist(string FW_pro, string FW_identi)
        {
            AnFW a = null;
            a = new AnFW();
            a.fwidenvalue = FW_identi;
            a.fwproterites = FW_pro;
            a.fwnamevalue = CoreEdit.get_profile(FW_pro, "description");
            firmwareapplication.Children.Add(a);
        }
        /// <summary>
        ///单击了一个系统发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void test(object sender, MouseEventArgs e)
        {
            thisOS = sender as AnOS;
            //翻页、头版
            OS_Properties_Grid.Text = (thisOS.Description_string as string);
            OS_list_or_properties.SelectedIndex = 1;
            //处理磁盘空间数据
            string volume = thisOS.Partitionvalve_String.Replace(":", "");
            double fullspace = CoreEdit.GetHardDiskSpace(volume), freespace = CoreEdit.GetFreeSpace(volume);
            DiskSpace.Value = (fullspace - freespace) * 100 / fullspace;
            string valve = ((fullspace - freespace) * 100 / fullspace).ToString();
            Unknown.Content = valve.Substring(0, Math.Min(5, valve.Length)) + "%";
            Header_Valve.Content = thisOS.Header_string;
            path_value.Content = thisOS.path_string;
            inherit_value.Content = thisOS.inherit_string;
            osdevice_value.Content = thisOS.Partitionvalve_String;
            systemroot_value.Content = thisOS.systemroot_string;
            Identifier_value.Content = thisOS.Identifiervalve_String;
            editing_System = thisOS.Identifiervalve_String;
            Local_Valve.Content = thisOS.Localvalve_String;
            Display_name_valve.Content = thisOS.Description_string;
            editname.Text = (thisOS.Description_string as string);
            editname.Visibility = Visibility.Hidden;
            button1.Content = "Rename";
            listBox.Items.Clear();
            string[] property = thisOS.all_Properties.Split('\n');
            foreach (string word in property)
            {
                OneProperty oneproperty = new OneProperty(word);
                listBox.Items.Add(oneproperty);
            }
            Loading = true;
            cleancoreedit();
            loadcoreedit();
            Loading = false;
        }
        #endregion

        #region 主页部分的相关UI控制事件、函数
        /// <summary>
        /// 自动修复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Auto_diagnose_Click(object sender, RoutedEventArgs e)
        {
            //在这里准备自动修复全部系统。自动修复算法有待设计
        }
        #endregion

        #region 系统菜单的相关UI控制事件、函数
        //刷新按钮
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            Thread torefresh = new Thread(Refresh);
            torefresh.Start();
            //Caution(successfully_refresh);
        }
        //彻底清除菜单
        private void all_clear()
        {
            OS_List_Grid.Children.Clear();
            Bootmenu.Children.Clear();
        }
        //清除固件菜单
        private void all_clear_Firmware()
        {
            firmwareapplication.Children.Clear();
        }
        //添加Windows系统
        private void list1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OS_list_or_properties.SelectedIndex = 3;
        }
        //添加可启动外部文件
        private void list2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OS_list_or_properties.SelectedIndex = 4;
        }
        //进入添加自定义项面板
        private void list4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OS_list_or_properties.SelectedIndex = 5;
        }
        //进入系统商店
        private void list3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainTab.SelectedIndex = 2;
        }

        #region 普通模式的系统信息
        //删除键
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            delete.Visibility = Visibility.Hidden;
            HiddenDeletePanel.Visibility = Visibility.Visible;
            Copy_description.Visibility = Visibility.Hidden;
            Delete_Description.Visibility = Visibility.Hidden;
            copy.Visibility = Visibility.Hidden;
        }
        //取消删除
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            delete.Visibility = Visibility.Visible;
            HiddenDeletePanel.Visibility = Visibility.Hidden;
            Copy_description.Visibility = Visibility.Visible;
            Delete_Description.Visibility = Visibility.Visible;
            copy.Visibility = Visibility.Visible;
        }
        //进入编辑模式
        private void check_Copy_Checked(object sender, RoutedEventArgs e)
        {
            resetbutton();
            OS_list_or_properties.SelectedIndex = 2;
            textBox.Clear();
            textBox_Copy.Clear();
        }
        //复制当前系统
        private void copy_Click(object sender, RoutedEventArgs e)
        {
            string result = CoreEdit.bcdedit(" /copy " + thisOS.Identifiervalve_String + " /d \"" + thisOS.Name_Label + " (Copy)\"", verbcd);
            string iden = CoreEdit.get_only_one_Identifier(result)[0];
            Refresh();
            string[] resultfromrefresh = OSs;
            if (CoreEdit.get_profile(CoreEdit.get_OS(resultfromrefresh, iden), "description") == thisOS.Name_Label + " (Copy)")
            {
                Caution(copyto + thisOS.Name_Label + " (Copy)");
            }
            else
            {
                Fail(erroroccured);
            }
            Button_Click(null, null);
        }
        //激活当前系统
        private void makeactivebutton_Click(object sender, RoutedEventArgs e)
        {
            CoreEdit.bcdedit(" /displayorder " + thisOS.Identifiervalve_String + " /addlast", true);
            Refresh();
            if (CoreEdit.stringlistcontainstring(CoreEdit.get_profile_List(CoreEdit.get_OS(OSs, "{bootmgr}"), "\r\ndisplayorder"), thisOS.Identifiervalve_String))
            {
                Caution("Successfully actived");
            }
            else
            {
                Fail("Failed to active.");
            }
        }
        //非编辑模式修改系统名称
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (button1.Content.Equals("Save"))
            {
                CoreEdit.bcdedit(" /set " + thisOS.Identifiervalve_String + " description \"" + editname.Text + "\"", true);
                staticrefresh();
                editname.Visibility = Visibility.Hidden;
                button1.Content = "Rename";
                return;
            }
            editname.Visibility = Visibility.Visible;
            button1.Content = "Save";
            editname.Focus();
        }
        //删除当前系统
        private void But_remove_Click(object sender, RoutedEventArgs e)
        {
            if (CoreEdit.get_profile_List(CoreEdit.get_OS(OSs, "{bootmgr}"), "\r\ndisplayorder").GetLength(0) <= 1 && show_active_os.IsChecked == true)
            {
                if (MessageBox.Show("This is the last active os in the list. If you delete this os, your PC may not be able to boot. Sure to delete?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            deteting.IsIndeterminate = true;
            deteting.Visibility = Visibility.Visible;
            MainTab.IsEnabled = false;
            Thread startdeleting = new Thread(deletethisOS);
            startdeleting.Start();
        }
        private void deletethisOS()
        {
            string Identifiervalve_Stringtemp = "";
            Dispatcher.Invoke(new Action(() =>
            {
                Identifiervalve_Stringtemp = thisOS.Identifiervalve_String;
            }));
            CoreEdit.bcdedit(" /delete " + Identifiervalve_Stringtemp + " /f", false);
            CoreEdit.bcdedit(" /displayorder /remove " + Identifiervalve_Stringtemp, true);
            Refresh();
            RefreshFirmware();
            if (CoreEdit.get_OS(OSs, Identifiervalve_Stringtemp) == "No such OS!")
            {
                Caution(deletesuc);
            }
            else
            {
                Fail(erroroccured);
            }
            Dispatcher.Invoke(new Action(() =>
            {
                deteting.IsIndeterminate = false;
                deteting.Visibility = Visibility.Hidden;
                MainTab.IsEnabled = true;
                Button_Click(null, null);
            }));
        }
        //删除当前系统并删除文件
        private void But_delete_files_Click(object sender, RoutedEventArgs e)
        {
            //请执行OSP脚本
        }

        #endregion

        #region 编辑模式的系统信息

        //通用返回键
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel_Click(null, null);
            OS_list_or_properties.SelectedIndex = 0;
        }
        //返回初级模式
        private void check_Copy_Advanced_Checked(object sender, RoutedEventArgs e)
        {
            OS_list_or_properties.SelectedIndex = 1;
            resetbutton();
        }
        //重置高级按钮和普通按钮
        private void resetbutton()
        {
            check_Copy.IsChecked = false;
            check_Copy_Advanced.IsChecked = true;
            coreorproperty.SelectedIndex = 0;
        }
        //单击列表中的属性后发生
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex > 2)
            {
                try
                {
                    string property = (listBox.SelectedItem as OneProperty).label.Content.ToString().Substring(0, 17).Trim();
                    textBox.Text = property;
                    textBox_Copy.Text = CoreEdit.get_profile((thisOS as AnOS).all_Properties, property);
                    button_Save.IsEnabled = button_Previous.IsEnabled = false;
                }
                catch
                {
                    textBox.Clear();
                    textBox_Copy.Clear();
                }
            }
            else
            {
                listBox.SelectedIndex = -1;
            }
        }

        #region 普通编辑
        //修改了一个属性的文本，使两个按钮可用
        private void textBox_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Loading)
            {
                return;
            }
            button_Save.IsEnabled = button_Previous.IsEnabled = true;
        }
        //恢复属性按钮
        private void button_Previous_Click(object sender, RoutedEventArgs e)
        {
            listBox_SelectionChanged(sender, null);
        }
        //按下保存属性按钮
        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox_Copy.Text.Trim();
            Loading = true;
            int tempi = listBox.SelectedIndex;
            CoreEdit.bcdedit(" /set " + thisOS.Identifiervalve_String + " " + textBox.Text + " \"" + textBox_Copy.Text + "\"", verbcd);
            //在完成一次修改后，应该重新加载此属性。
            //进行一次刷新，重置主列表。
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            //加载选择项。
            listBox.SelectedIndex = tempi;
            if (textBox_Copy.Text.ToLower() != text.ToLower())
                Fail(erroredit);
            else
                Caution(successfully_edit);
            Loading = false;
        }
        //按下删除按钮
        private void button_delpro_Click(object sender, RoutedEventArgs e)
        {
            CoreEdit.deleteproperty(thisOS.Identifiervalve_String, textBox.Text);
            Refresh();
            staticrefresh();
            Caution("successfully delete");
            OS_list_or_properties.SelectedIndex = 2;
        }
        #endregion

        #region 核心编辑
        /// <summary>
        ///清理核心编辑工作区
        /// </summary>
        private void cleancoreedit()
        {
            //safemode.Text = safemode.Items[0].ToString();
            safemode.SelectedIndex = 0;
            //PAE.Text = PAE.Items[0].ToString();
            PAE.SelectedIndex = 0;
            //DEP_Value.Text = DEP_Value.Items[0].ToString();
            DEP_Value.SelectedIndex = 0;
            Debuggingwindows.IsChecked = false;
            SOS.IsChecked = false;
            checkBox_Copy1.IsChecked = false;
            checkBox_Copy2.IsChecked = false;
            textBox_ram.Clear();
            textBox_CPU.Clear();
        }
        //加载核心编辑工作区
        private void loadcoreedit()
        {
            try
            {
                if (CoreEdit.get_profile(thisOS.all_Properties, "safeboot").Contains("Minimal"))
                {
                    safemode.SelectedIndex = 1;
                    if (CoreEdit.get_profile(thisOS.all_Properties, "safebootalternateshell").Contains("Yes"))
                        safemode.SelectedIndex = 3;
                }
                if (CoreEdit.get_profile(thisOS.all_Properties, "safeboot").Contains("Network"))
                    safemode.SelectedIndex = 2;
                if (CoreEdit.get_profile(thisOS.all_Properties, "vga").Contains("Yes"))
                    safemode.SelectedIndex = 4;
                //PAE选项加载
                switch (CoreEdit.get_profile(thisOS.all_Properties, "pae"))
                {
                    case "ForceEnable":
                        PAE.SelectedIndex = 1;
                        break;
                    case "ForceDisable":
                        PAE.SelectedIndex = 2;
                        break;
                    case "Default":
                        PAE.SelectedIndex = 0;
                        break;
                    default:
                        break;
                }
                //DEP选项加载
                switch (CoreEdit.get_profile(thisOS.all_Properties, "nx"))
                {
                    case "OptIn":
                        DEP_Value.SelectedIndex = 0;
                        break;
                    case "OptOut":
                        DEP_Value.SelectedIndex = 1;
                        break;
                    case "AlwaysOn":
                        DEP_Value.SelectedIndex = 2;
                        break;
                    case "AlwaysOff":
                        DEP_Value.SelectedIndex = 3;
                        break;
                    default:
                        break;
                }
                Debuggingwindows.IsChecked = CoreEdit.get_profile(thisOS.all_Properties, "debug") == "Yes";
                SOS.IsChecked = CoreEdit.get_profile(thisOS.all_Properties, "sos") == "Yes";
                if (CoreEdit.get_profile(thisOS.all_Properties, "removememory") != "Null")
                {
                    checkBox_Copy1.IsChecked = true;
                    textBox_ram.Text = CoreEdit.get_profile(thisOS.all_Properties, "removememory");
                }
                else
                    checkBox_Copy1.IsChecked = false;
                if (CoreEdit.get_profile(thisOS.all_Properties, "numproc") != "Null")
                {
                    checkBox_Copy2.IsChecked = true;
                    textBox_CPU.Text = CoreEdit.get_profile(thisOS.all_Properties, "numproc");
                }
                else
                    checkBox_Copy2.IsChecked = false;
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }
        }
        /// <summary>
        /// 将核心编辑区恢复默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reset_core()
        {
            Loading = true;
            Dispatcher.Invoke(new Action(() =>
            {
                resetall.IsEnabled = false;
                string nowidentifier = thisOS.Identifiervalve_String;
                CoreEdit.setproperty(nowidentifier, "nx", "Optin");
                CoreEdit.deleteproperty(nowidentifier, "safeboot");
                CoreEdit.deleteproperty(nowidentifier, "safebootalternateshell");
                CoreEdit.deleteproperty(nowidentifier, "vga");
                CoreEdit.deleteproperty(nowidentifier, "debug");
                CoreEdit.deleteproperty(nowidentifier, "sos");
                CoreEdit.deleteproperty(nowidentifier, "pae");
                CoreEdit.deleteproperty(nowidentifier, "numproc");
                CoreEdit.deleteproperty(nowidentifier, "removememory");
            }));
            Refresh();
            //执行一次静态刷新
            Dispatcher.Invoke(new Action(() =>
            {
                staticrefresh();
                OS_list_or_properties.SelectedIndex = 2;
                loadcoreedit();
                resetall.IsEnabled = true;
            }));
            Loading = false;
            Caution(successfully_settodefault);
        }
        private void resetall_Click(object sender, RoutedEventArgs e)
        {
            Thread resetthread = new Thread(reset_core);
            resetthread.Start();
        }
        private void Debuggingwindows_Click(object sender, RoutedEventArgs e)
        {
            bool check = Debuggingwindows.IsChecked == true;
            if (check)
            {
                CoreEdit.setproperty(thisOS.Identifiervalve_String, "debug", "Yes");
            }
            else
            {
                CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "debug");
            }
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (Debuggingwindows.IsChecked == check)
                Caution(successfully_edit);
            else
                Fail(failed);
        }
        private void SOS_Click(object sender, RoutedEventArgs e)
        {
            bool check = SOS.IsChecked == true;
            if (check)
                CoreEdit.setproperty(thisOS.Identifiervalve_String, "sos", "Yes");
            else
                CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "sos");
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (SOS.IsChecked == check)
                Caution(successfully_edit);
            else
                Fail(failed);
        }

        private void SaveRam_Click(object sender, RoutedEventArgs e)
        {
            bool itchecked = checkBox_Copy1.IsChecked == true;
            string nowram = textBox_ram.Text.ToString();
            if (itchecked)
                CoreEdit.setproperty(thisOS.Identifiervalve_String, "removememory", nowram);
            else
                CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "removememory");
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (itchecked == false && CoreEdit.get_profile(thisOS.all_Properties, "removememory") == "Null")
                Caution(successfully);
            else if (CoreEdit.get_profile(thisOS.all_Properties, "removememory") == nowram)
                Caution(successfully);
            else
                Fail(failed);
        }
        private void SaveCPU_Click(object sender, RoutedEventArgs e)
        {
            bool itchecked = checkBox_Copy2.IsChecked == true;
            string nowcpu = textBox_CPU.Text.ToString();
            if (itchecked)
                CoreEdit.setproperty(thisOS.Identifiervalve_String, "numproc", nowcpu);
            else
                CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "numproc");
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (itchecked == false && CoreEdit.get_profile(thisOS.all_Properties, "numproc") == "Null")
                Caution(successfully);
            else if (CoreEdit.get_profile(thisOS.all_Properties, "numproc") == nowcpu)
                Caution(successfully);
            else
                Fail(failed);

        }
        private void checkBox_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox_Copy1.IsChecked == false)
                SaveRam_Click(null, null);
        }
        //cpu削减修改勾
        private void checkBox_Copy2_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox_Copy2.IsChecked == false)
                SaveCPU_Click(null, null);
        }
        //进入核心编辑模式
        private void button_CoreMode_Click(object sender, RoutedEventArgs e)
        {
            coreorproperty.SelectedIndex = 1;
        }
        //应用安全模式选项
        private void safemode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Loading)
                return;
            int check = safemode.SelectedIndex;
            switch (check)
            {
                case 0:
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "safeboot");
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "vga");
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "safebootalternateshell");
                    break;
                case 1:
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "vga");
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "safebootalternateshell");
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "safeboot", "Minimal");
                    break;
                case 2:
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "vga");
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "safebootalternateshell");
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "safeboot", "Network");
                    break;
                case 3:
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "vga");
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "safeboot", "Minimal");
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "safebootalternateshell", "Yes");
                    break;
                case 4:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "safeboot", "Minimal");
                    CoreEdit.deleteproperty(thisOS.Identifiervalve_String, "safebootalternateshell");
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "vga", "Yes");
                    break;
                default:
                    break;
            }
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (safemode.SelectedIndex == check)
            {
                Caution(successfully_edit);
            }
            else
            {
                Fail(failed);
            }
        }

        private void PAE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Loading)
            {
                return;
            }
            int check = PAE.SelectedIndex;
            switch (check)
            {
                case 0:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "pae", "Default");
                    break;
                case 1:
                    //ForceEnable
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "pae", "ForceEnable");
                    break;
                case 2:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "pae", "ForceDisable");
                    break;
                default:
                    break;
            }
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (PAE.SelectedIndex == check)
            {
                Caution(successfully_edit);
            }
            else
            {
                Fail(failed);
            }
        }

        private void DEP_Value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Loading)
                return;
            int check = DEP_Value.SelectedIndex;
            switch (check)
            {
                case 0:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "nx", "OptIn");
                    break;
                case 1:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "nx", "OptOut");
                    break;
                case 2:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "nx", "AlwaysOn");
                    break;
                case 3:
                    CoreEdit.setproperty(thisOS.Identifiervalve_String, "nx", "AlwaysOff");
                    break;
                default:
                    break;
            }
            Refresh();
            staticrefresh();
            OS_list_or_properties.SelectedIndex = 2;
            if (DEP_Value.SelectedIndex == check)
            {
                Caution(successfully_edit);
            }
            else
            {
                Fail(failed);
            }
        }




        //从核心编辑模式返回
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            coreorproperty.SelectedIndex = 0;
        }
        #endregion

        #endregion

        #region 新建Windows的部分代码
        //在这里控制增加一个操作系统。
        private void ADDnow_Click(object sender, RoutedEventArgs e)
        {
            if (partitions_combo.SelectedIndex == -1)
            {
                Fail(unknownos);
                return;
            }
            string targetdiscription = displaynamecontent.Text.Replace("\"", "").Replace("\r", "").Replace("\n", "").Trim(), text;
            if (targetdiscription == "")
            {
                Fail(unknownosname);
                return;
            }
            text = CoreEdit.bcdedit(" /create /d \"" + targetdiscription + "\" /application osloader", true);
            string returnidentity = CoreEdit.get_only_one_Identifier(text)[0];
            CoreEdit.bcdedit(" /set " + returnidentity + " device \"partition=" + (partitions_combo.Items[partitions_combo.SelectedIndex] as string).Replace(@"\", "") + "\"", true);
            if (!from_UEFI)
            {
                CoreEdit.bcdedit(" /set " + returnidentity + " path \"\\Windows\\system32\\winload.exe\"", true);
            }
            else
            {
                CoreEdit.bcdedit(" /set " + returnidentity + " path \"\\Windows\\system32\\winload.efi\"", true);
            }
            CoreEdit.bcdedit(" /set " + returnidentity + " systemroot \"\\Windows\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " osdevice \"partition=" + (partitions_combo.Items[partitions_combo.SelectedIndex] as string).Replace(@"\", "") + "\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " locale \"en-US\"", true);
            CoreEdit.bcdedit(" /displayorder " + returnidentity + " /addlast", true);
            Refresh();
            if (CoreEdit.get_profile(CoreEdit.get_OS(OSs, returnidentity), "description") == targetdiscription)
            {
                Caution(targetdiscription + " successfully added!");
            }
            else
            {
                CoreEdit.bcdedit(" /delete " + returnidentity, true);
                Fail("Error when adding!");
            }
            OS_list_or_properties.SelectedIndex = 0;
        }
        //自动判断各个盘符的系统,将第一个没有成功添加到列表的系统选中.
        private void autodetect_Click(object sender, RoutedEventArgs e)
        {
            bool alreadyexist = true;
            foreach (string apartition in partitions_combo.Items)
            {
                alreadyexist = false;
                try
                {
                    string path = apartition + @"Windows\explorer.exe";
                    CoreEdit.PrintFileVersionInfo(path);
                }
                catch
                {
                    continue;
                }
                foreach (string aOS in OSs)
                {
                    if (CoreEdit.get_profile(aOS, "device") == "Null")
                        continue;
                    if (CoreEdit.get_profile(aOS, "device") == "partition=" + apartition.Replace(@"\", ""))
                    {
                        alreadyexist = true;
                        continue;
                    }
                }
                if (!alreadyexist)
                {
                    partitions_combo.Text = apartition;
                    return;
                }
            }
            Fail("All Windows are installed.");
        }
        //在用户选择盘符后对该盘符下的Windows版本进行判断。
        private void partitions_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CoreEdit.version osverion;
            osverion.ProductBuildPart = 0;
            osverion.ProductMajorPart = 0;
            osverion.ProductMinorPart = 0;
            osverion.ProductPrivatePart = 0;
            try
            {
                string path = partitions_combo.Items[partitions_combo.SelectedIndex] as string + @"Windows\explorer.exe";
                osverion = CoreEdit.PrintFileVersionInfo(path);
            }
            catch (Exception ee)
            {
                if (ee.Message == "File doesn't exist")
                    displaynamecontent.Text = "Unknown Windows";
                return;
            }
            if (osverion.ProductMajorPart == 10)
                displaynamecontent.Text = "Windows 10";
            else if (osverion.ProductMajorPart == 6 && osverion.ProductMinorPart == 3)
                displaynamecontent.Text = "Windows 8.1";
            else if (osverion.ProductMajorPart == 6 && osverion.ProductMinorPart == 2)
                displaynamecontent.Text = "Windows 8";
            else if (osverion.ProductMajorPart == 6 && osverion.ProductMinorPart == 1)
                displaynamecontent.Text = "Windows 7";
            else if (osverion.ProductMajorPart == 6 && osverion.ProductMinorPart == 0)
                displaynamecontent.Text = "Windows vista";
            else
                displaynamecontent.Text = "Windows " + osverion.ProductMajorPart + "." + osverion.ProductMinorPart;
        }
        #endregion

        #region 新建可启动外部文件的部分代码
        //选择WIM文件地址
        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            string file = "";
            ofd.DefaultExt = ".*";
            ofd.Filter = "Wim PE (WIM)|*.wim";
            var result = ofd.ShowDialog();
            file = ofd.FileName;
            PElocation.Text = file;
        }
        //将WIM添加到启动菜单
        private void addpenow_Click(object sender, RoutedEventArgs e)
        {
            if (PElocation.Text.Replace(" ", "") == "")
            {
                Fail("Unknown .wim file!");
                return;
            }
            string create = CoreEdit.bcdedit(" /create /device", true), cmd = "";
            try
            {
                create = CoreEdit.get_only_one_Identifier(create)[0];
                string curlp = Environment.CurrentDirectory;
                CoreEdit.bcdedit(" /set " + create + " ramdisksdipath \"" + (curlp.Substring(2) + "\\Core\\boot.sdi") + "\"", true);
                CoreEdit.bcdedit("/set " + create + " ramdisksdidevice " + "partition=" + curlp.Substring(0, 1) + ":", true);
                string doit = CoreEdit.bcdedit(" /create /d \"" + wimpedisplaynamecontent.Text + "\" /application osloader", true);
                cmd = CoreEdit.get_only_one_Identifier(doit)[0];
                CoreEdit.bcdedit(" /displayorder " + cmd + " /addlast", true);
                string resu = CoreEdit.bcdedit(" /set " + cmd + " device ramdisk=\"[" + PElocation.Text.Substring(0, 1) + ":]" + PElocation.Text.Substring(2) + "\"," + create, true);
                CoreEdit.bcdedit(" /set " + cmd + " osdevice ramdisk=\"[" + PElocation.Text.Substring(0, 1) + ":]" + PElocation.Text.Substring(2) + "\"," + create, true);
                CoreEdit.bcdedit(" /set " + cmd + " winpe yes", true);
                CoreEdit.bcdedit(" /set " + cmd + " detecthal yes", true);
                if (!from_UEFI)
                    CoreEdit.bcdedit(" /set " + cmd + " path \\Windows\\System32\\Boot\\winload.exe", true);
                else
                    CoreEdit.bcdedit(" /set " + cmd + " path \\Windows\\System32\\Boot\\winload.efi", true);
                CoreEdit.bcdedit(" /set " + cmd + " locale en-US", true);
                CoreEdit.bcdedit(" /set " + cmd + " systemroot \\Windows", true);
            }
            catch
            {
                CoreEdit.bcdedit(" /delete " + create + " /f", true);
                CoreEdit.bcdedit(" /delete " + cmd + " /f", true);
                Refresh();
                Fail("WIM file wrong!");
                return;
            }
            Refresh();
            OS_list_or_properties.SelectedIndex = 0;
            if (CoreEdit.get_profile(CoreEdit.get_OS(OSs, cmd), "description") == wimpedisplaynamecontent.Text)
                Caution("Successfully Added!");
            else
                Fail("An error occured!");
        }
        //选择VHD文件地址
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string file = "";
            ofd.DefaultExt = ".*";
            ofd.Filter = "VHD File (vhd)|*.vhd;*.vhdx";
            var result = ofd.ShowDialog();
            file = ofd.FileName;
            vhdlocation.Text = file;
        }
        //立即添加VHD文件
        private void addvhdnow_Click(object sender, RoutedEventArgs e)
        {
            string targetdiscription = vhdpedisplaynamecontent.Text.Replace("\"", "").Replace("\r", "").Replace("\n", "").Trim(), text;
            if (targetdiscription == "")
            {
                Fail("Unknown VHD name!");
                return;
            }
            text = CoreEdit.bcdedit(" /create /d \"" + targetdiscription + "\" /application osloader", true);
            string returnidentity = CoreEdit.get_only_one_Identifier(text)[0];
            CoreEdit.bcdedit(" /set " + returnidentity + " device \"vhd=[" + vhdlocation.Text.Substring(0, 1) + ":]" + vhdlocation.Text.Substring(2) + "\"", true);
            if (!from_UEFI)
                CoreEdit.bcdedit(" /set " + returnidentity + " path \"\\Windows\\system32\\winload.exe\"", true);
            else
                CoreEdit.bcdedit(" /set " + returnidentity + " path \"\\Windows\\system32\\winload.efi\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " systemroot \"\\Windows\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " osdevice  \"vhd=[" + vhdlocation.Text.Substring(0, 1) + ":]" + vhdlocation.Text.Substring(2) + "\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " locale \"en-US\"", true);
            CoreEdit.bcdedit(" /set " + returnidentity + " detecthal yes", true);
            CoreEdit.bcdedit(" /displayorder " + returnidentity + " /addlast", true);
            Refresh();
            OS_list_or_properties.SelectedIndex = 0;
            if (CoreEdit.get_profile(CoreEdit.get_OS(OSs, returnidentity), "description") == targetdiscription)
                Caution("Successfully Added!");
            else
                Fail("An error occured!");

        }
        //立即添加自定义项
        private void Addcustom_Click(object sender, RoutedEventArgs e)
        {
            string resultincus, resultidenincus;
            switch (comboBoxcustom.SelectedIndex)
            {
                case 0:
                    resultincus = CoreEdit.bcdedit("/create /d \"Real Mode Item\" /application bootsector", verbcd);
                    Caution(resultincus);
                    resultidenincus = CoreEdit.get_only_one_Identifier(resultincus)[0];
                    CoreEdit.bcdedit("/displayorder " + resultidenincus + " /addlast", true);
                    break;
                case 1:
                    resultincus = CoreEdit.bcdedit("/create {ntldr} /d \"Earlier Windows OS Loader\"", verbcd);
                    Caution(resultincus);
                    resultidenincus = CoreEdit.get_only_one_Identifier(resultincus)[0];
                    CoreEdit.bcdedit("/displayorder " + resultidenincus + " /addlast", true);
                    break;
                case 2:
                    resultincus = CoreEdit.bcdedit("/create /d \"Windows Loader\" /application osloader", verbcd);
                    Caution(resultincus);
                    resultidenincus = CoreEdit.get_only_one_Identifier(resultincus)[0];
                    CoreEdit.bcdedit("/displayorder " + resultidenincus + " /addlast", true);
                    break;
                case 3:
                    resultincus = CoreEdit.bcdedit("/create {ramdiskoptions} /d \"RAM disk options\" ", verbcd);
                    Caution(resultincus);
                    resultidenincus = CoreEdit.get_only_one_Identifier(resultincus)[0];
                    CoreEdit.bcdedit("/displayorder " + resultidenincus + " /addlast", true);
                    break;
                case 4:
                    resultincus = CoreEdit.bcdedit("/create {dbgsettings} /d \"Debugger\" ", verbcd);
                    Caution(resultincus);
                    resultidenincus = CoreEdit.get_only_one_Identifier(resultincus)[0];
                    CoreEdit.bcdedit("/displayorder " + resultidenincus + " /addlast", true);
                    break;
                default:
                    break;

            }
            OS_list_or_properties.SelectedIndex = 0;
            Thread torefresh = new Thread(Refresh);
            torefresh.Start();
        }
        #endregion

        #endregion

        #region 磁盘管理相关UI控制事件、函数
        //选择目标VHD路径
        private void SaveVHD_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog();
            sf.Filter = "VHD file(*.vhd)|*.vhd|VHDX file|*.vhdx";
            sf.FilterIndex = 2;
            sf.ShowDialog();
            VHDtoattachlocation.Text = VHDlocation.Text = (sf.FileName.ToString());
            if (VHDlocation.Text.Trim() != "")
            {
                Attachbut.IsEnabled = true;
                Attachbut_Copy.IsEnabled = true;
                Create.IsEnabled = true;
            }
            else
            {
                Attachbut.IsEnabled = false;
                Attachbut_Copy.IsEnabled = false;
                Create.IsEnabled = false;
            }
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Create.IsEnabled = false;
            Thread createdisk = new Thread(CreateDISK);
            createdisk.Start();
        }
        private void CreateDISK()
        {
            try
            {
                int maximum = 0;
                string VHDlocationtemp = "";
                bool extendable = true;
                bool gptcheck = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    creatingdiskprogress.IsIndeterminate = true;
                    creatingdiskprogress.Visibility = Visibility.Visible;
                    maximum = Convert.ToInt32(VHDSize.Text);
                    VHDlocationtemp = VHDlocation.Text.Trim();
                    extendable = extencheck.IsChecked == true;
                    gptcheck = GPT.IsChecked == true;
                }));

                CoreEdit.Diskpart(new string[] {
                    "create vdisk file=\"" +VHDlocationtemp  + "\" maximum=" + maximum.ToString() + " type=" + (extendable? "expandable" : "fixed")
                    , "sel vdisk" + " file =\"" + VHDlocationtemp + "\""
                    , "attach vdisk"
                    , "convert " +(gptcheck ? "GPT" : "MBR")
                    ,"create part primary"
                    ,"format fs=ntfs quick"
                    ,"assign"
                });
            }
            catch (Exception ee)
            {
                Fail(ee.Message);
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                creatingdiskprogress.IsIndeterminate = false;
                creatingdiskprogress.Visibility = Visibility.Hidden;
                Create.IsEnabled = true;
            }));
            Caution("Successfully created virtual disk!");
        }
        private void SelVHD_Click(object sender, RoutedEventArgs e)
        {

            string file = "";
            ofd.DefaultExt = ".vhd|.vhdx";
            ofd.Filter = "Virtual disk|*.vhd;*.vhdx";
            var result = ofd.ShowDialog();
            file = ofd.FileName;
            VHDtoattachlocation.Text = file;
            if (file.Trim() != "")
            {
                Attachbut.IsEnabled = true;
                Attachbut_Copy.IsEnabled = true;
                Create.IsEnabled = true;
            }
            else
            {
                Attachbut.IsEnabled = false;
                Attachbut_Copy.IsEnabled = false;
                Create.IsEnabled = false;
            }
        }

        private void Attachbut_Click(object sender, RoutedEventArgs e)
        {
            CoreEdit.Diskpart(new string[] {
                "sel vdisk" + " file =\"" + VHDtoattachlocation.Text.Trim() + "\""
                   , "attach vdisk"
            });
            Caution("Successfully attached virtual disk!");
        }

        private void Attachbut_Copy_Click(object sender, RoutedEventArgs e)
        {
            CoreEdit.Diskpart(new string[] {
                  "sel vdisk" + " file =\"" + VHDtoattachlocation.Text.Trim() + "\""
                 , "detach vdisk"
                 });
            Caution("Successfully detached virtual disk!");
        }

        #endregion

        #region 固件设置的相关UI控制事件、函数
        private void refreshFW_Click(object sender, RoutedEventArgs e)
        {

            Thread startfwrefresh = new Thread(RefreshFirmware);
            startfwrefresh.Start();
        }

        #endregion

        #region 安全选项的相关UI控制事件、函数
        //选择备份目录
        private void select_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog1.ShowDialog();
            textBox1.Text = FolderBrowserDialog1.SelectedPath;
        }
        //执行备份
        private void backupnow_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text == "")
                return;
            CoreEdit.direct_command("md \"" + textBox1.Text + "\"");
            string analyse = CoreEdit.bcdedit(" /export \"" + textBox1.Text + "\\" + "bk.bcd" + "\"", verbcd);
            Caution("Successfully backed up at " + textBox1.Text + "\\bk.bcd!");
        }
        //选择还原目录
        private void select2_Click(object sender, RoutedEventArgs e)
        {
            string file = "";
            ofd.DefaultExt = ".*";
            ofd.Filter = "Windows BCD (BCD)|*.*";
            var result = ofd.ShowDialog();
            file = ofd.FileName;
            textBoxres.Text = file;
        }
        //执行还原
        private void restorenow_Click(object sender, RoutedEventArgs e)
        {
            restorenow.IsEnabled = false;
            retoring.Visibility = Visibility.Visible;
            retoring.IsIndeterminate = true;
            MainTab.IsEnabled = false;
            Thread begin = new Thread(restorebcd);
            begin.Start();
        }

        //还原线程
        public void restorebcd()
        {
            string analyse, filename = null;
            Dispatcher.Invoke(new Action(() =>
            {
                filename = textBoxres.Text;
            }));
            analyse = CoreEdit.bcdedit(" /import \"" + filename + "\"", false);
            Caution(analyse);
            verbcd = false;
            Refresh();
            Dispatcher.Invoke(new Action(() =>
            {
                verbcd = (check.IsChecked == true);
                restorenow.IsEnabled = true;
                retoring.Visibility = Visibility.Hidden;
                retoring.IsIndeterminate = false;
                MainTab.IsEnabled = true;
            }));
        }
        #endregion

        #region 系统设置的相关UI控制事件、函数
        /// <summary>
        /// 修改语言时程序反应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LangCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Loading)
                return;
            Loading = true;
            Thread Load = new Thread(LoadLanguage);
            Load.Start();
        }









        //系统显示状态改变
        private void Show_ALL_OS_Checked(object sender, RoutedEventArgs e)
        {
            if (Loading)
                return;
            OS_list_or_properties.SelectedIndex = 0;
            Thread torefresh = new Thread(Refresh);
            torefresh.Start();
        }
        //修改高速模式和兼容模式
        private void check_Checked(object sender, RoutedEventArgs e)
        {
            if (Loading)
                return;
            verbcd = check.IsChecked == true;
            OS_list_or_properties.SelectedIndex = 0;
            Thread torefresh = new Thread(Refresh);
            torefresh.Start();
        }
        //高级模式和小白模式的第二选取
        private void Advaninsettings_Click(object sender, RoutedEventArgs e)
        {
            OSHead_Copy1_MouseUp(null, null);
        }
        #endregion

        #region 窗口退出的一路执行代码

        //保存选项信息
        private void WriteCode()
        {
            string pathXml = "user.xml";
            Settings u = new Settings();
            Dispatcher.Invoke(new Action(() =>
            {
                u.Selected_Lang_Index = LangCombobox.SelectedIndex;
                u.ischeck = check.IsChecked.ToString();
                u.show_active_os = show_active_os.IsChecked.ToString();
                u.drag = drag.IsChecked.ToString();
                u.multiprocess = Multiprocessallow.IsChecked.ToString();
                u.isnormalmode = isnormalmode.ToString();
            }));
            Xml.XmlHelper.Serialize(u, pathXml);
        }
        //窗体退出
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            WriteCode();
            loginwindow.Close();
            pow.Close();
            Environment.Exit(0);
        }

        #endregion

    }
}