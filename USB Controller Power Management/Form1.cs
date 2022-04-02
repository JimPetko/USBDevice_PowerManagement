using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.Diagnostics;

namespace USB_Controller_Power_Management
{
    public partial class Form1 : Form
    {


        private int count;      

        public Form1()
        {

            InitializeComponent();

        }
        /// <summary>
        /// Disables Selective suspend with system utility powercfg.exe in the system dir.
        /// </summary>
        /// <param name="ACDC"></param>
        /// <param name="enable"></param>
        private void SelectiveSuspend(string ACDC, int enable)
        {
            if (ACDC == "AC" && enable < 2)
            {
                Process p_AC_PWR = new Process();
                p_AC_PWR.StartInfo.FileName = "powercfg.exe";
                p_AC_PWR.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                p_AC_PWR.StartInfo.UseShellExecute = false;
                p_AC_PWR.StartInfo.Arguments = @"/SETACVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 " + enable;
                p_AC_PWR.Start();
            }
            if (ACDC == "DC" && enable < 2)
            {
                Process p_DC_PWR = new Process();
                p_DC_PWR.StartInfo.FileName = "powercfg.exe";
                p_DC_PWR.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                p_DC_PWR.StartInfo.UseShellExecute = false;
                p_DC_PWR.StartInfo.Arguments = @"/SETDCVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 " + enable;
                p_DC_PWR.Start();
                p_DC_PWR.WaitForExit();
            }
        }
       

        private void but_EnablePM_Click(object sender, EventArgs e)
        {
            TogglePowerManagement(true);
        }

        private void but_DisablePM_Click(object sender, EventArgs e)
        {
            TogglePowerManagement(false);
        }
        /// <summary>
        /// Disables or Enables power managment based on which button was clicked.
        /// </summary>
        /// <param name="enable"></param>
        private void TogglePowerManagement(bool enable)
        {
            string PropertyValue;
            int keyValue, selectiveSuspend;
            if (enable)
            {
                PropertyValue = "True";
                keyValue = 0;
                selectiveSuspend = 1; 
            }
            else
            {
                PropertyValue = "False";
                keyValue = 1;
                selectiveSuspend = 0;
            }
            try
            {
                ManagementObjectSearcher PMSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSPower_DeviceEnable");//Where the selective suspend power management values are stored.
                ManagementObjectSearcher HubSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_USBHub");//Get the HWID of the USBHubs which will also have power management available.  
                ManagementObjectSearcher ControllerSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_USBController");// Get HWID of the main controllers, typically USB 3.x will appear here.
                foreach (ManagementObject PMQueryObj in PMSearcher.Get())
                {
                    string s_ControllerName = PMQueryObj["InstanceName"].ToString().ToUpper();
                    foreach (ManagementObject HubQueryObj in HubSearcher.Get())
                    {
                        string PnpDeviceID = HubQueryObj["PNPDeviceID"].ToString();
                        if (s_ControllerName.Contains(PnpDeviceID))
                        {
                            PMQueryObj.SetPropertyValue("Enable", PropertyValue);//Sets Selective Suspend for the current index of Hub
                            PMQueryObj.Put();//Saves Value in Windows Management Engine.
                        }
                    }
                    foreach (ManagementObject ControllerQueryObj in ControllerSearcher.Get())
                    {
                        string PnpDeviceID = ControllerQueryObj["PNPDeviceID"].ToString();
                        if (s_ControllerName.Contains(PnpDeviceID))
                        {
                            PMQueryObj.SetPropertyValue("Enable", PropertyValue);//Sets Selective Suspend for the current index of Controller
                            PMQueryObj.Put();
                        }
                    }
                }
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\USB");
                key.SetValue("DisableSelectiveSuspend", keyValue, RegistryValueKind.DWord);
                
                SelectiveSuspend("AC", selectiveSuspend);
                if (cb_BatteryCheck.Checked)
                    SelectiveSuspend("DC", selectiveSuspend);

                MessageBox.Show(this, "Successfully Disabled All Power Manamgement Settings.");
            }
            catch (Exception ex)
            {

                MessageBox.Show("Unable to make modifications, Please Run this Application as Administrator", ex.Message);

                MessageBox.Show(this, "Unable to make modifications, Please Run this Application as Administrator");
            }
        }

        private void but_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void but_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ManagementObjectSearcher HubSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_USBHub");

            foreach (ManagementObject HubQueryObj in HubSearcher.Get())
            {
                if(!HubQueryObj["Name"].ToString().Contains("Composite Device"))
                listBox1.Items.Add(HubQueryObj["Name"]);

            }
            ManagementObjectSearcher ControllerSearcher = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_USBController");
            foreach (ManagementObject ControllerQueryObj in ControllerSearcher.Get()) 
            {
                listBox1.Items.Add(ControllerQueryObj["Name"]);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/JimPetko");
        }
    }
}
