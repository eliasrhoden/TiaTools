using System.Windows.Forms;
using Siemens.Engineering;
using System.Collections.Generic;
using Siemens.Engineering.Connection;
using Siemens.Engineering.HW;
using Siemens.Engineering.Online;


namespace TiaTools
{
    public class OnlinePrepper 
    {

        private string selectedNetworkAdapter = "";
        private string selectedConnectionMode = "";
        private string selectedTargetIF = "";

        public void prepOnline(IEnumerable<IEngineeringObject> selectedObjects)
        {
            selectedNetworkAdapter = "";
            selectedConnectionMode = "";
            selectedTargetIF = "";


            foreach (var obj in selectedObjects)
            {
                // Handle DeviceItem selections (CPU, HMI, etc.)
                if (obj is DeviceItem deviceItem)
                {
                    prepDeviceItem(deviceItem);
                }

                // Handle top-level Device selections
                if (obj is Device device)
                {
                    foreach (DeviceItem item in device.DeviceItems)
                    {
                        prepDeviceItem(item);
                    }
                }
            }
        }


        private void prepDeviceItem(DeviceItem deviceItem)
        {

            var onlineProvider = deviceItem.GetService<OnlineProvider>();

            if (onlineProvider != null)
            {

                ConnectionConfiguration configuration = onlineProvider.Configuration;

                // Check mode (PN/Profibus/S7-USB)
                if (selectedConnectionMode == "")
                {
                    List<string> modes = new List<string>();
                    foreach (var m in configuration.Modes)
                    {
                        modes.Add(m.Name);
                    }

                    selectedConnectionMode = UiUtils.PromptUserSelection("Which mode?", modes);
                }

                ConfigurationMode mode = configuration.Modes.Find(selectedConnectionMode);

                // check network adapter
                if (selectedNetworkAdapter == "")
                {
                    List<string> adapters = new List<string>();
                    foreach (var pa in mode.PcInterfaces)
                    {
                        adapters.Add(pa.Name);
                    }

                    selectedNetworkAdapter = UiUtils.PromptUserSelection("Which adapter?", adapters);
                }

                ConfigurationPcInterface pcInterface = mode.PcInterfaces.Find(selectedNetworkAdapter, 1);

                // Target port/interface
                if(selectedTargetIF == "")
                {
                    List<string> targetIFs = new List<string>();
                    foreach (var tif in pcInterface.TargetInterfaces)
                    {
                        targetIFs.Add(tif.Name);
                    }

                    selectedTargetIF = UiUtils.PromptUserSelection("Which target interface?", targetIFs);
                }


                //ConfigurationTargetInterface targetConfiguration = pcInterface.TargetInterfaces[0];
                ConfigurationTargetInterface targetConfiguration = pcInterface.TargetInterfaces.Find(selectedTargetIF);

                configuration.ApplyConfiguration(targetConfiguration);

                if (!configuration.IsConfigured)
                {
                    MessageBox.Show("Failed to configure: " + deviceItem.Name, "Error");
                }
            }

            foreach (DeviceItem d in deviceItem.DeviceItems)
            {
                prepDeviceItem(d);
            }
        }
    }
}