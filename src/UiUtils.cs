using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace TiaTools
{
    public class UiUtils
    {

        public static string AskUserForXmlFile(string title)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = title;
                dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                dlg.Multiselect = false;

                return dlg.ShowDialog() == DialogResult.OK
                    ? dlg.FileName
                    : null;
            }
        }

        public static string PromptUserSelection(string prompt, IList<string> options)
        {
            var combo = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            combo.Items.AddRange(options.Cast<object>().ToArray());
            combo.SelectedIndex = 0;

            var ok = new Button { Text = "OK", Dock = DockStyle.Bottom, DialogResult = DialogResult.OK };

            var form = new Form
            {
                Text = prompt,
                Size = new Size(300, 110),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                AcceptButton = ok
            };

            form.Controls.AddRange(new Control[] { ok, combo });

            return form.ShowDialog() == DialogResult.OK
                ? combo.SelectedItem.ToString()
                : throw new OperationCanceledException("User cancelled.");
        }
    }
    }