using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using PartsHunter.Models;
using PartsHunter.Repositories.Interfaces;
using Component = PartsHunter.Models.Component;

namespace PartsHunter
{
    public partial class Form1 : Form
    {
        private readonly static Led Led = new Led();

        private readonly IFirebaseRepositorie _firebaseRepositorie;
        private readonly ILocalRepositorie _localRepositorie;

        public Form1(IFirebaseRepositorie firebaseRepositorie, ILocalRepositorie localRepositorie)
        {
            InitializeComponent();

            _firebaseRepositorie = firebaseRepositorie;
            _localRepositorie = localRepositorie;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && SystemForm.Pre_Load_Done == false)
            {
                FIll_ComboBox_Category();
            }

            if (tabControl1.SelectedIndex == 0 && dataGridViewSearch.CurrentRow == null)
            {
            }

            else if (tabControl1.SelectedIndex == 0 && dataGridViewSearch.CurrentRow != null)
            {
                Highligth_From_Results();
            }

            if (tabControl1.SelectedIndex == 2)
            {
                //if (String.IsNullOrEmpty(Firebase_Database_KEY) && String.IsNullOrEmpty(Firebase_Database_URL))
                //{
                buttonShow.Visible = false;
                //}
                //else
                //{
                //    buttonShow.Visible = true;
                //}
            }

        }

        private void CustomizeLed(Led led)
        {
            buttonColor.BackColor = led.Color;

            trackBarTime.Value = led.Time;
            if (led.Time < 1000)
            {
                labelTime.Text = led.Time < 100 ? "always on" : led.Time + "ms blinky";
            }

            else
            {
                labelTime.Text = "1s blinky";
            }

            trackBarBright.Value = led.Brightness;
            int x = (led.Brightness * 100) / 255;
            labelBright.Text = x + "% brightness";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Led led = _localRepositorie.LoadData();
            CustomizeLed(led);

            dataGridViewSearch.Columns[0].Width = (int)(dataGridViewSearch.Width * 0.3);
            dataGridViewSearch.Columns[1].Width = (int)(dataGridViewSearch.Width * 0.6);
            dataGridViewSearch.Columns[2].Width = (int)(dataGridViewSearch.Width * 0.1);

            labelNumberResults.Visible = false;
            comboBoxCategories_SearchTab.Text = comboBoxCategories_SearchTab.Items[0].ToString();
            comboBoxCategories_RegisterTab.Text = comboBoxCategories_RegisterTab.Items[0].ToString();

            buttonSearch.Enabled = false;
            buttonListAll.Enabled = false;
            buttonEdit.Enabled = false;
            buttonDelete.Enabled = false;
            buttonClear.Enabled = false;
            buttonSave.Enabled = false;

            if (_firebaseRepositorie.Autheticate())
            {
                _firebaseRepositorie.Load_Firebase_Database();
                _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
            }

            FIll_ComboBox_Category();
            SystemForm.Pre_Load_Done = true;
        }

        private void FIll_ComboBox_Category()
        {
            comboBoxCategories_RegisterTab.Items.Clear();
            comboBoxCategories_RegisterTab.Items.Add("<Select one or type a new one>");

            int items = comboBoxCategories_RegisterTab.Items.Count;
            int items2 = comboBoxCategories_SearchTab.Items.Count;

            string value = String.Empty;

            if (_firebaseRepositorie.JSON_Firebase != null)
            {
                foreach (dynamic key in _firebaseRepositorie.JSON_Firebase.Keys)
                {
                    bool ignore = false;
                    bool ignore2 = false;
                    for (int i = 0; i < items; i++)
                    {
                        value = comboBoxCategories_RegisterTab.Items[i].ToString();

                        if (key == value || key == "HardwareDevice")
                        {
                            ignore = true;
                        }
                    }
                    if (ignore == false)
                    {
                        comboBoxCategories_RegisterTab.Items.Add(key);
                    }

                    for (int i = 0; i < items2; i++)
                    {


                        string value2 = comboBoxCategories_SearchTab.Items[i].ToString();

                        if (key == value || key == "HardwareDevice" || key == value2)
                        {
                            ignore2 = true;
                        }
                    }
                    if (ignore2 == false)
                    {
                        comboBoxCategories_SearchTab.Items.Add(key);
                    }
                }
            }

            comboBoxCategories_RegisterTab.Text = comboBoxCategories_RegisterTab.Items[0].ToString();
        }

        private async Task SearchByCategory()
        {
            string[] newRow;
            dynamic component = await _firebaseRepositorie.GetComponent(comboBoxCategories_SearchTab.Text);
            dataGridViewSearch.AllowUserToAddRows = true;
            dataGridViewSearch.Rows.Clear();

            foreach (dynamic key in component.Keys)
            {
                if (key != "void" && key != "HardwareDevice")
                {
                    int numberResults = component.Keys.Count;
                    labelNumberResults.Text = numberResults.ToString() + " results found";

                    foreach (DataGridViewRow row in dataGridViewSearch.Rows)
                    {
                        newRow = new string[] { comboBoxCategories_SearchTab.Text, component[key]["Description"], _firebaseRepositorie.JSON_Firebase[key]["Drawer"] };

                        if (row.Cells[0].Value != component[key]["Description"])
                        {
                            dataGridViewSearch.Rows.Add(newRow);
                            break;
                        }
                    }
                }
                else
                {
                    _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
                    labelNumberResults.Text = "0" + " results found";
                }
            }
            dataGridViewSearch.AllowUserToAddRows = false;
        }

        private void SearchByDescription(string input)
        {
            CultureInfo culture = new CultureInfo("es-ES", false);

            string[] newRow;
            int numberResults = 0;
            _firebaseRepositorie.Load_Firebase_Database();

            dataGridViewSearch.AllowUserToAddRows = true;

            dataGridViewSearch.Rows.Clear();

            if (_firebaseRepositorie.JSON_Firebase != null)
            {
                foreach (dynamic key in _firebaseRepositorie.JSON_Firebase.Keys)
                {
                    if (key != "void" && key != "HardwareDevice")
                    {
                        foreach (dynamic key2 in _firebaseRepositorie.JSON_Firebase[key].Keys)
                        {
                            string s = _firebaseRepositorie.JSON_Firebase[key][key2]["Description"].ToString();

                            string[] words = input.Split(' ');



                            foreach (string w in words)
                            {

                                if (culture.CompareInfo.IndexOf(s, w, CompareOptions.IgnoreCase) >= 0)
                                {
                                    foreach (DataGridViewRow row in dataGridViewSearch.Rows)
                                    {
                                        newRow = new string[] { key, _firebaseRepositorie.JSON_Firebase[key][key2]["Description"], _firebaseRepositorie.JSON_Firebase[key][key2]["Drawer"] };

                                        if (row.Cells[0].Value != _firebaseRepositorie.JSON_Firebase[key][key2]["Description"])
                                        {
                                            dataGridViewSearch.Rows.Add(newRow);
                                            numberResults++;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                    else
                    {

                        _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
                        labelNumberResults.Text = "0" + " results found";
                    }
                }
            }

            dataGridViewSearch.AllowUserToAddRows = false;
            labelNumberResults.Text = numberResults.ToString() + " results found";
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {

            SearchByDescription(textBoxSearch.Text);
            SystemForm.Pre_Load_Done = false;
            labelNumberResults.Visible = true;

        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Enter)
            {
                SearchByDescription(textBoxSearch.Text);
                labelNumberResults.Visible = true;
            }

        }

        private void Highligth_From_Results()
        {
            if (dataGridViewSearch.SelectedCells[0].Value != null)
            {

                int x = dataGridViewSearch.SelectedCells[0].RowIndex;


                string drawer = dataGridViewSearch[2, x].Value.ToString();


                string command = drawer + ',' + Led.Color.R + ',' + Led.Color.G + ',' + Led.Color.B + ',' + Led.Brightness + ',' + Led.Time;

                _firebaseRepositorie.SetHardwareDevice(command);
            }

        }

        private void DataGridViewResults_SelectionChanged(object sender, EventArgs e)
        {
            Highligth_From_Results();

        }

        private async void ButtonListAll_Click(object sender, EventArgs e)
        {

            if (comboBoxCategories_SearchTab.SelectedIndex != 0)
            {
                await SearchByCategory();
                SystemForm.Pre_Load_Done = false;
                labelNumberResults.Visible = true;
            }
            else
            {
                MessageBox.Show("Select a category");
            }
        }

        private void ComboBoxCategory_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(comboBoxCategories_RegisterTab.Text) || comboBoxCategories_RegisterTab.Text == comboBoxCategories_RegisterTab.Items[0].ToString())
            {
                SystemForm.ComponentRegisterForm.ValidatedCategory = false;
                errorProvider1.SetError(comboBoxCategories_RegisterTab, "should not be left blank!");
            }
            else
            {
                e.Cancel = false;
                SystemForm.ComponentRegisterForm.ValidatedCategory = true;
                errorProvider1.SetError(comboBoxCategories_RegisterTab, "");
            }
        }

        private void TextBoxDescription_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxDescription.Text))
            {
                SystemForm.ComponentRegisterForm.ValidatedDescription = false;
                errorProvider1.SetError(textBoxDescription, "should not be left blank!");
            }
            else
            {
                e.Cancel = false;
                SystemForm.ComponentRegisterForm.ValidatedDescription = true;
                errorProvider1.SetError(textBoxDescription, "");
            }
        }

        private void TextBoxQuantity_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxDrawer.Text))
            {
                SystemForm.ComponentRegisterForm.ValidatedDrawer = false;
                errorProvider1.SetError(textBoxDrawer, "should not be left blank!");
            }
            else
            {
                e.Cancel = false;
                SystemForm.ComponentRegisterForm.ValidatedDrawer = true;
                errorProvider1.SetError(textBoxDrawer, "");
            }
        }

        private void ReLoad_Fields()
        {
            comboBoxCategories_RegisterTab.Text = comboBoxCategories_RegisterTab.Items[0].ToString();
            textBoxDescription.Text = String.Empty;
            textBoxDrawer.Text = String.Empty;

            _firebaseRepositorie.Load_Firebase_Database();

            FIll_ComboBox_Category();

            SearchByDescription(textBoxSearch.Text);
        }

        private void Highlight_Drawer_From_Box_Selection()
        {
            foreach (dynamic key in _firebaseRepositorie.JSON_Firebase.Keys)
            {
                foreach (dynamic key2 in _firebaseRepositorie.JSON_Firebase[key].Keys)
                {
                    if (_firebaseRepositorie.JSON_Firebase[key][key2]["Box"] == SystemForm.Current_Selected_Box)
                    {
                        switch (_firebaseRepositorie.JSON_Firebase[key][key2]["Drawer"])
                        {

                        }

                    }
                }
            }
        }

        private void ButtonSave_Click_1(object sender, EventArgs e)
        {


            if (SystemForm.ComponentRegisterForm.ValidatedCategory && SystemForm.ComponentRegisterForm.ValidatedDescription && SystemForm.ComponentRegisterForm.ValidatedDrawer)
            {
                string category = comboBoxCategories_RegisterTab.Text.ToUpper();
                string description = textBoxDescription.Text;
                string drawer = textBoxDrawer.Text;
                _firebaseRepositorie.Push_New_Component(category, description, drawer);
                ReLoad_Fields();
                SystemForm.Pre_Load_Done = false;
            }
            else
            {
                MessageBox.Show("Fill all fields!");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (buttonSettings.Text == "Config Highlight")
            {
                groupBoxSettings.Visible = true;
                buttonSettings.Text = "Save";

            }
            else if (buttonSettings.Text == "Save")
            {
                buttonSettings.Text = "Config Highlight";
                groupBoxSettings.Visible = false;
                _localRepositorie.SaveData(new Led() {

                    Color = buttonColor.BackColor,
                    Time = trackBarTime.Value,
                    Brightness = trackBarBright.Value
                });
            }
        }

        private void ButtonColor_Click(object sender, EventArgs e)
        {

            colorDialog1.AllowFullOpen = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = buttonSettings.ForeColor;




            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Led.Color = colorDialog1.Color;
                buttonColor.BackColor = Led.Color;
            }
        }

        private void TrackBarTime_Scroll(object sender, EventArgs e)
        {
            Led.Time = trackBarTime.Value;

            if (trackBarTime.Value < 1000)
            {
                labelTime.Text = trackBarTime.Value < 100 ? "always on" : trackBarTime.Value + "ms blinky";
            }

            else
            {
                labelTime.Text = "1s blinky";
            }
        }

        private void TrackBarBright_Scroll(object sender, EventArgs e)
        {
            Led.Brightness = trackBarBright.Value;

            int x = (trackBarBright.Value * 100) / 255;
            labelBright.Text = x + "% brightness";
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Component oldComponent = new Component();
                Component newComponent = new Component();

                Form2 f2 = new Form2();
                DialogResult result = f2.ShowDialog();

                if (result == DialogResult.OK)
                {
                    newComponent.Category = f2.Category;
                    newComponent.Drawer = f2.Drawer;
                    newComponent.Description = f2.Description;

                    if (oldComponent.Category != newComponent.Category || oldComponent.Drawer != newComponent.Drawer)
                    {
                        _firebaseRepositorie.DeleteComponent(oldComponent.Drawer);
                        _firebaseRepositorie.Push_New_Component(newComponent.Category, newComponent.Description, newComponent.Drawer);
                    }
                    else
                    {
                        _firebaseRepositorie.UpdateComponent(newComponent.Drawer, newComponent.Description);
                    }

                    SystemForm.Pre_Load_Done = false;
                    ReLoad_Fields();
                }
            }
            catch
            {

            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSearch.SelectedCells.Count > 0)
            {
                string drawer = dataGridViewSearch.SelectedCells[2].Value.ToString();
                _firebaseRepositorie.DeleteComponent(drawer);
                ReLoad_Fields();
            }

            SystemForm.Pre_Load_Done = false;
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            if (buttonSettings.Text == "Config Highlight")
            {
                groupBoxSettings.Visible = true;
                buttonSettings.Text = "Save";
            }

            else if (buttonSettings.Text == "Save")
            {
                buttonSettings.Text = "Config Highlight";
                groupBoxSettings.Visible = false;
                _localRepositorie.SaveData(new Led() {

                    Color = buttonColor.BackColor,
                    Time = trackBarTime.Value,
                    Brightness = trackBarBright.Value
                });
            }

            Highligth_From_Results();
        }

        private void ButtonColor_Click_1(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = buttonSettings.ForeColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Led.Color = colorDialog1.Color;
                buttonColor.BackColor = Led.Color;
            }
        }

        private void TrackBarTime_Scroll_1(object sender, EventArgs e)
        {
            Led.Time = trackBarTime.Value;

            if (trackBarTime.Value < 1000)
            {
                labelTime.Text = trackBarTime.Value < 100 ? "always on" : trackBarTime.Value + "ms blinky";
            }

            else
            {
                labelTime.Text = "1s blinky";
            }
        }

        private void TrackBarBright_Scroll_1(object sender, EventArgs e)
        {
            Led.Brightness = trackBarBright.Value;

            int x = (trackBarBright.Value * 100) / 255;
            if (x <= 0)
            {
                x = 1;
            }

            if (Led.Brightness <= 0)
            {
                Led.Brightness = 1;
            }

            labelBright.Text = x + "% brightness";
        }

        private void ButtonClear_Click_1(object sender, EventArgs e)
        {
            _firebaseRepositorie.SetHardwareDevice("-1,0,0,0,0,0");
            dataGridViewSearch.Rows.Clear();
            labelNumberResults.Visible = false;
        }

        private void ButtonFirebase_Save_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxFirebase_URL.Text) && !String.IsNullOrEmpty(textBoxFirebase_Key.Text))
            {
                _firebaseRepositorie.Login(textBoxFirebase_URL.Text, textBoxFirebase_Key.Text);
            }
        }

        private void ButtonShow_Click_2(object sender, EventArgs e)
        {
            if (buttonShow.Text == "Show")
            {
                textBoxFirebase_Key.Text = ""; //Firebase_Database_KEY;
                textBoxFirebase_Key.PasswordChar = '\0';
                buttonShow.Text = "Hide";
            }
            else
            {
                textBoxFirebase_Key.PasswordChar = '*';
                buttonShow.Text = "Show";
            }
        }

        private void ButtonGetFileAddress_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFileLocation.Text = openFileDialog1.FileName;
                SystemForm.BatcFileLocation = openFileDialog1.FileName;
            }
        }

        private void ButtonSaveFromFile_Click(object sender, EventArgs e)
        {
            Component component = new Component();

            if (!String.IsNullOrEmpty(textBoxFileLocation.Text))
            {
                _ = System.IO.File.ReadAllText(SystemForm.BatcFileLocation);
                string[] lines = System.IO.File.ReadAllLines(SystemForm.BatcFileLocation);

                int totalParts = 0;

                progressBar1.Visible = true;
                progressBar1.Maximum = lines.Length;

                try
                {
                    foreach (string l in lines)
                    {
                        string[] words = l.Split(',');

                        int index = 0;
                        foreach (string w in words)
                        {
                            switch (index++)
                            {
                                case 0: component.Category = w; break;
                                case 1: component.Description = w; break;
                                case 2: component.Drawer = w; break;
                            }
                        }
                        _firebaseRepositorie.Push_New_Component(component.Category.ToUpper(), component.Description, component.Drawer);
                        totalParts++;
                        progressBar1.Value++;
                    }

                    ReLoad_Fields();
                    SystemForm.Pre_Load_Done = false;
                    MessageBox.Show("Success! " + totalParts + " Parts added to cloud databese.");
                }
                catch
                {
                    MessageBox.Show("Fail! Check the file and the text formatting rules.");
                }

                progressBar1.Visible = false;
            }
        }
    }
}