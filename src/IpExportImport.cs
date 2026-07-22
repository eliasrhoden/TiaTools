using System.Windows.Forms;
using Siemens.Engineering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;

namespace TiaTools
{
    public class IpExportImport 
    {

        public static void export_IPs(Project project)
        {

            var res = getAllIPAddressEntries(project);
            List<IpAddrEntry> entries = res.entries;

            string userFilesDir = Path.Combine(project.Path.Directory.ToString(), "UserFiles");
            try
            {
                Directory.CreateDirectory(userFilesDir); // safe if it already exists
            }
            catch
            {
            }

            string filePath = Path.Combine(userFilesDir, "ips.xml");
            MessageBox.Show($"No of entries: {entries.Count} under file: {filePath}");

            string content = "";
            content = IpAddrEntry.ListToXml(entries);
            File.WriteAllText(filePath, content);

        }


        public static void import_IPs(Project project)
        {

            string filepath = UiUtils.AskUserForXmlFile("Select a file");

            var res = getAllIPAddressEntries(project);
            List<IpAddrEntry> entriesProj = res.entries;
            List<Node> addrRefsProj = res.tiaRefs;

            if (entriesProj.Count != addrRefsProj.Count)
            {
                throw new Exception($"Nr of addresses ({entriesProj.Count}) does not match Nr of addr refs ({addrRefsProj.Count})!");
            }

            // Addresses from user 
            string xml = File.ReadAllText(filepath, Encoding.UTF8);
            List<IpAddrEntry> entriesUser = IpAddrEntry.ListFromXml(xml);
            List<Node> addrRefsuser = new List<Node>();

            // Move references from proj-list to user-list 
            foreach (var userEntry in entriesUser)
            {
                bool foundProjEntry = false;
                for (int i = 0; i < entriesProj.Count; i++)
                {
                    if (entriesProj[i].path == userEntry.path)
                    {
                        addrRefsuser.Add(addrRefsProj[i]);

                        entriesProj.RemoveAt(i);
                        addrRefsProj.RemoveAt(i);

                        foundProjEntry = true;
                        break;
                    }

                    if (!foundProjEntry)
                    {
                        throw new Exception($"Failed to find matching address in project: {userEntry.path}");
                    }

                }
            }

            if (entriesUser.Count != addrRefsuser.Count)
            {
                throw new Exception($"Nr of addresses ({entriesUser.Count}) does not match Nr of addr refs ({addrRefsuser.Count}) in the user map!");
            }


            for (int i = 0; i < entriesUser.Count; i++)
            {
                try
                {
                    addrRefsuser[i].SetAttribute("Address", entriesUser[i].ipAddress);
                    addrRefsuser[i].SetAttribute("Address", entriesUser[i].ipAddress);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed set IP {entriesUser[i].ipAddress} to {entriesUser[i].path} \n" + e.ToString());
                }
            }
            MessageBox.Show("Completed!");
        }

        private static (List<IpAddrEntry> entries, List<Node> tiaRefs) getAllIPAddressEntries(Project project)
        {
            List<IpAddrEntry> entries = new List<IpAddrEntry>();
            List<Node> projAddresses = new List<Node>();

            foreach (Device d in project.Devices)
            {
                ProcessDeviceIP(d, entries, projAddresses);
            }

            foreach (DeviceUserGroup group in project.DeviceGroups)
            {
                foreach (Device d in group.Devices)
                {
                    ProcessDeviceIP(d, entries, projAddresses);
                }
            }

            var ungrouped = project.UngroupedDevicesGroup;
            foreach (Device d in ungrouped.Devices)
            {
                ProcessDeviceIP(d, entries, projAddresses);
            }

            return (entries, projAddresses);
        }



        private static void ProcessDeviceIP(Device device, List<IpAddrEntry> entires, List<Node> projAddresses)
        {
            foreach (DeviceItem item in device.DeviceItems)
            {
                ProcessDeviceItemIP(item, entires, device.Name, projAddresses, device.Name);
            }
        }


        private static void ProcessDeviceItemIP(DeviceItem device, List<IpAddrEntry> entires, string path, List<Node> projAddresses, string deviceName)
        {
            string path_new = path + "/" + device.Name;

            var network = device.GetService<NetworkInterface>();

            if (network != null && network.Nodes.Count > 0)
            {

                foreach (Node n in network.Nodes)
                {
                    try
                    {
                        Subnet subnet = (Subnet)n.GetAttribute("ConnectedSubnet");
                        string address = (string)n.GetAttribute("Address");
                        string profinetCleanDevName = (string)n.GetAttribute("PnDeviceName"); 
                        string profinetDevName = (string)n.GetAttribute("PnDeviceNameConverted");
                        string path_temp = path_new;// + $"/{subnet.Name}/{address}";

                        IpAddrEntry iae = new IpAddrEntry
                        {
                            deviceName = deviceName,
                            path = path_temp,
                            ipAddress = address,
                            profinetDeviceName = profinetDevName,
                            profinetCleanDeviceName = profinetCleanDevName
                        };

                        entires.Add(iae);
                        projAddresses.Add(n);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message + "\n" + e.ToString() + "\n" + path_new, "Error");
                    }
                }
            }

            foreach (DeviceItem item in device.DeviceItems)
            {
                ProcessDeviceItemIP(item, entires, path_new, projAddresses, deviceName);
            }
        }
    }
}
