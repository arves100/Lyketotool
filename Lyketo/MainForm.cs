using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lyketo
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void outputFormatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void convertMobProtoButton_Click(object sender, EventArgs e)
        {

        }

        private void convertItemProtoButton_Click(object sender, EventArgs e)
        {
            if (metroComboBox2.SelectedIndex == 0)
            {
                MessageBox.Show(this, "Please select an output format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //string filter = "LZO item proto (item_proto)|item_proto|XML files (*.xml)|*.xml|CSV files (*.txt;*.csv)|*.txt;*.csv|QuantumCore Protobuf (";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
