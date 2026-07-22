using System.Windows.Forms;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Siemens.Engineering.Connection;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.Online;
using Siemens.Engineering.SW;

namespace TiaTools
{
    public class ExportProcessImg 
    {

        public static void exportIMG(Project project )
        {

            var res = getAllAddressEntries(project);
            List<AddressEntry> entries = res.entries;

            string userFilesDir = Path.Combine(project.Path.Directory.ToString(), "UserFiles");
            try
            {
                Directory.CreateDirectory(userFilesDir); // safe if it already exists
            }
            catch
            {

            }

            string filePath = Path.Combine(userFilesDir, "io.xml");
            MessageBox.Show($"No of entries: {entries.Count} under file: {filePath}");

            string xml_str = AddressEntry.ListToXml(entries);
            File.WriteAllText(filePath, xml_str);

        }

        private static (List<AddressEntry> entries, List<Address> tiaRefs) getAllAddressEntries(Project project)
        {
            List<AddressEntry> entries = new List<AddressEntry>();
            List<Address> projAddresses = new List<Address>();

            foreach (Device d in project.Devices)
            {
                ProcessDevice(d, entries, projAddresses);

            }

            foreach (DeviceUserGroup group in project.DeviceGroups)
            {
                foreach (Device d in group.Devices)
                {
                    ProcessDevice(d, entries, projAddresses);
                }
            }

            var ungrouped = project.UngroupedDevicesGroup;
            foreach (Device d in ungrouped.Devices)
            {
                ProcessDevice(d, entries, projAddresses);
            }

            return (entries, projAddresses);
        }

        private static void ProcessDevice(Device device, List<AddressEntry> entires, List<Address> projAddresses)
        {
            foreach (DeviceItem item in device.DeviceItems)
            {
                ProcessDeviceItem(item, entires, device.Name, projAddresses);
            }

        }


        private static void tryToParseDriveTelegrams(DeviceItem device, List<AddressEntry> entires, string path, List<Address> projAddresses)
        {

            var driveObjCont = device.GetService<DriveObjectContainer>();

            if (driveObjCont == null)
            {
                return;
            }

            foreach (var drvObj in driveObjCont.DriveObjects)
            {
                foreach (var telegram in drvObj.Telegrams)
                {
                    foreach (var address in telegram.Addresses)
                    {
                        string tempPath = path + $"/{telegram.Type} Nr {telegram.TelegramNumber}/{address.IoType.ToString()}";
                        var ae = createAddressEntry(address, tempPath);
                        entires.Add(ae);
                        projAddresses.Add(address);
                    }
                }
            }
        }

        private static void ProcessDeviceItem(DeviceItem device, List<AddressEntry> entires, string path, List<Address> projAddresses)
        {
            string path_new = path + "/" + device.Name;

            tryToParseDriveTelegrams(device, entires, path_new, projAddresses);

            if (device.Addresses.Count > 0)
            {
                foreach (Address a in device.Addresses)
                {
                    if (a.StartAddress < 0)
                    {
                        continue;
                    }

                    string path_new_a = path_new + "/" + a.IoType.ToString();
                    var ae = createAddressEntry(a, path_new_a);
                    entires.Add(ae);
                    projAddresses.Add(a);
                }
            }

            foreach (DeviceItem item in device.DeviceItems)
            {
                ProcessDeviceItem(item, entires, path_new, projAddresses);
            }
        }

        private static AddressEntry createAddressEntry(Address a, string path)
        {
            var addEntry = new AddressEntry();

            addEntry.Path = path;
            addEntry.StartAddr = a.StartAddress;
            addEntry.Length = a.Length / 8;

            return addEntry;
        }

        private static string addr2Str(DeviceItem d)
        {
            string res = "";

            foreach (Address a in d.Addresses)
            {
                if (a.StartAddress > 0)
                {

                    res += $"parent:{a.Parent.GetAttribute("Name")} type:{a.IoType.ToString()} start:{a.StartAddress} length:{a.Length / 8} \n";
                }

            }

            return res;
        }



        
    }
}