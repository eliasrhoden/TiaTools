using System.Windows.Forms;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;


namespace TiaTools
{
    public class TiaToolsContextMenuAddIn : ContextMenuAddIn
    {
        readonly TiaPortal m_TiaPortal;
        private const string s_DisplayNameOfAddIn = "TiaTools";

        private OnlinePrepper onlinepPrepper;

        public TiaToolsContextMenuAddIn(TiaPortal tiaPortal) : base(s_DisplayNameOfAddIn)
        {
            m_TiaPortal = tiaPortal;
            onlinepPrepper = new OnlinePrepper();
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            /*
            Define the context menu when right-clicking in the project tree
            */
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Prepare Go-Online", prepOnlineCallback, pluginActiveCallback);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Export I/O map", export_IO_map_Callback, pluginActiveCallback);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Export IP-addresses", export_IP_Callback, pluginActiveCallback);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Import IP-addresses", import_IP_Callback, pluginActiveCallback);

        }


        private void export_IP_Callback(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            try
            {
                Project project = m_TiaPortal.Projects.First();
                IpExportImport.export_IPs(project);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.ToString(), "Error");
            }
        }
        private void import_IP_Callback(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            try
            {
                Project project = m_TiaPortal.Projects.First();
                IpExportImport.import_IPs(project);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.ToString(), "Error");
            }
        }

        private void export_IO_map_Callback(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {

            try
            {
                Project project = m_TiaPortal.Projects.First();
                ExportProcessImg.exportIMG(project);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.ToString(), "Error");
            }
        }

        public void prepOnlineCallback(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {

            try
            {
                IEnumerable<IEngineeringObject> selectedObjects = (IEnumerable<IEngineeringObject>)menuSelectionProvider.GetSelection();
                onlinepPrepper.prepOnline(selectedObjects);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.ToString(), "Error");
            }
            MessageBox.Show("Completed");
        }

        private MenuStatus pluginActiveCallback(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            // TODO: Change the code here
            // MenuStatus
            //  Enabled  = Visible
            //  Disabled = Visible but not executable
            //  Hidden   = Item will not be shown
            return MenuStatus.Enabled;
        }


        
    }
}

