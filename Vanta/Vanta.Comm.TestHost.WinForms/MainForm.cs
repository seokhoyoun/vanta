using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Device.Melsec;
using Vanta.Comm.Simulation.Profiles;

namespace Vanta.Comm.TestHost.WinForms
{
    public sealed class MainForm : Form
    {
        #region Public Properties

        #endregion

        #region Fields

        private readonly Button _initializeButton;
        private readonly Button _stopButton;
        private readonly Button _readTagButton;
        private readonly Button _writeTagButton;
        private readonly Button _readBlockButton;
        private readonly Button _addTagButton;
        private readonly Button _editTagButton;
        private readonly Button _deleteTagButton;
        private readonly ComboBox _tagComboBox;
        private readonly DataGridView _tagGridView;
        private readonly TextBox _currentValueTextBox;
        private readonly TextBox _writeValueTextBox;
        private readonly TextBox _blockHeadTextBox;
        private readonly TextBox _blockAddressTextBox;
        private readonly TextBox _blockLengthTextBox;
        private readonly TextBox _blockValuesTextBox;
        private readonly TextBox _logTextBox;
        private readonly Label _statusValueLabel;
        private readonly Label _driverValueLabel;
        private readonly Label _tagInfoValueLabel;

        private IDeviceDriver? _driver;
        private DeviceDefinition? _device;
        private BlockDefinition? _block;
        private List<TagDefinition> _tags;
        private int _selectedTagSequence;
        private bool _isSynchronizingSelection;

        #endregion

        #region Constructors

        public MainForm()
        {
            _initializeButton = new Button();
            _stopButton = new Button();
            _readTagButton = new Button();
            _writeTagButton = new Button();
            _readBlockButton = new Button();
            _addTagButton = new Button();
            _editTagButton = new Button();
            _deleteTagButton = new Button();
            _tagComboBox = new ComboBox();
            _tagGridView = new DataGridView();
            _currentValueTextBox = new TextBox();
            _writeValueTextBox = new TextBox();
            _blockHeadTextBox = new TextBox();
            _blockAddressTextBox = new TextBox();
            _blockLengthTextBox = new TextBox();
            _blockValuesTextBox = new TextBox();
            _logTextBox = new TextBox();
            _statusValueLabel = new Label();
            _driverValueLabel = new Label();
            _tagInfoValueLabel = new Label();

            _tags = SampleTestConfiguration.CreateTags();
            _selectedTagSequence = 0;

            Text = "Vanta Comm Test Host";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1140, 930);
            ClientSize = new Size(1140, 930);

            InitializeLayout();
            HookEvents();
            LoadTags();
            SetUiState(false);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Helpers

        private void InitializeLayout()
        {
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            Panel topPanel = CreatePanel(new Rectangle(12, 12, 1098, 92));
            Panel tagManagePanel = CreatePanel(new Rectangle(12, 116, 1098, 304));
            Panel tagTestPanel = CreatePanel(new Rectangle(12, 432, 1098, 174));
            Panel blockPanel = CreatePanel(new Rectangle(12, 618, 1098, 122));
            Panel logPanel = CreatePanel(new Rectangle(12, 752, 1098, 160));

            Controls.Add(topPanel);
            Controls.Add(tagManagePanel);
            Controls.Add(tagTestPanel);
            Controls.Add(blockPanel);
            Controls.Add(logPanel);

            BuildTopPanel(topPanel);
            BuildTagManagePanel(tagManagePanel);
            BuildTagTestPanel(tagTestPanel);
            BuildBlockPanel(blockPanel);
            BuildLogPanel(logPanel);
        }

        private void BuildTopPanel(Panel panel)
        {
            Label titleLabel = CreateLabel("Simulation Driver", new Point(12, 12), 160, true);
            panel.Controls.Add(titleLabel);

            _initializeButton.Text = "Initialize";
            _initializeButton.Location = new Point(16, 42);
            _initializeButton.Size = new Size(110, 32);
            panel.Controls.Add(_initializeButton);

            _stopButton.Text = "Stop";
            _stopButton.Location = new Point(136, 42);
            _stopButton.Size = new Size(110, 32);
            panel.Controls.Add(_stopButton);

            Label statusLabel = CreateLabel("Status", new Point(280, 18), 80, false);
            panel.Controls.Add(statusLabel);

            _statusValueLabel.Location = new Point(280, 44);
            _statusValueLabel.Size = new Size(180, 24);
            _statusValueLabel.Text = "Not initialized";
            panel.Controls.Add(_statusValueLabel);

            Label driverLabel = CreateLabel("Driver", new Point(500, 18), 80, false);
            panel.Controls.Add(driverLabel);

            _driverValueLabel.Location = new Point(500, 44);
            _driverValueLabel.Size = new Size(280, 24);
            _driverValueLabel.Text = "-";
            panel.Controls.Add(_driverValueLabel);
        }

        private void BuildTagManagePanel(Panel panel)
        {
            Label titleLabel = CreateLabel("Tag List", new Point(12, 12), 120, true);
            panel.Controls.Add(titleLabel);

            _addTagButton.Text = "Add";
            _addTagButton.Location = new Point(760, 20);
            _addTagButton.Size = new Size(90, 30);
            panel.Controls.Add(_addTagButton);

            _editTagButton.Text = "Edit";
            _editTagButton.Location = new Point(860, 20);
            _editTagButton.Size = new Size(90, 30);
            panel.Controls.Add(_editTagButton);

            _deleteTagButton.Text = "Delete";
            _deleteTagButton.Location = new Point(960, 20);
            _deleteTagButton.Size = new Size(90, 30);
            panel.Controls.Add(_deleteTagButton);

            _tagGridView.Location = new Point(16, 60);
            _tagGridView.Size = new Size(1060, 224);
            _tagGridView.AllowUserToAddRows = false;
            _tagGridView.AllowUserToDeleteRows = false;
            _tagGridView.AllowUserToResizeRows = false;
            _tagGridView.MultiSelect = false;
            _tagGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _tagGridView.ReadOnly = true;
            _tagGridView.RowHeadersVisible = false;
            _tagGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _tagGridView.Columns.Add(CreateTextColumn("TagSequence", "Seq", 60));
            _tagGridView.Columns.Add(CreateTextColumn("TagName", "Tag Name", 180));
            _tagGridView.Columns.Add(CreateTextColumn("AddressHead", "Head", 60));
            _tagGridView.Columns.Add(CreateTextColumn("TagAddress", "Address", 90));
            _tagGridView.Columns.Add(CreateTextColumn("MemoryKind", "Memory", 80));
            _tagGridView.Columns.Add(CreateTextColumn("DataType", "Data Type", 140));
            _tagGridView.Columns.Add(CreateTextColumn("DataShapeKind", "Shape", 90));
            _tagGridView.Columns.Add(CreateTextColumn("ElementShapeKind", "Element", 90));
            _tagGridView.Columns.Add(CreateTextColumn("AddressLength", "Length", 70));
            _tagGridView.Columns.Add(CreateTextColumn("BitDigit", "Bit", 60));
            panel.Controls.Add(_tagGridView);
        }

        private void BuildTagTestPanel(Panel panel)
        {
            Label titleLabel = CreateLabel("Tag Test", new Point(12, 12), 120, true);
            panel.Controls.Add(titleLabel);

            Label tagSelectLabel = CreateLabel("Tag", new Point(16, 44), 80, false);
            panel.Controls.Add(tagSelectLabel);

            _tagComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _tagComboBox.Location = new Point(16, 68);
            _tagComboBox.Size = new Size(260, 25);
            _tagComboBox.DisplayMember = "TagName";
            panel.Controls.Add(_tagComboBox);

            Label tagInfoLabel = CreateLabel("Definition", new Point(300, 44), 120, false);
            panel.Controls.Add(tagInfoLabel);

            _tagInfoValueLabel.Location = new Point(300, 68);
            _tagInfoValueLabel.Size = new Size(760, 38);
            _tagInfoValueLabel.Text = "-";
            panel.Controls.Add(_tagInfoValueLabel);

            Label currentValueLabel = CreateLabel("Current Value", new Point(16, 104), 120, false);
            panel.Controls.Add(currentValueLabel);

            _currentValueTextBox.Location = new Point(16, 128);
            _currentValueTextBox.Size = new Size(380, 25);
            _currentValueTextBox.ReadOnly = true;
            panel.Controls.Add(_currentValueTextBox);

            _readTagButton.Text = "Read";
            _readTagButton.Location = new Point(410, 126);
            _readTagButton.Size = new Size(100, 30);
            panel.Controls.Add(_readTagButton);

            Label writeValueLabel = CreateLabel("Write Value", new Point(540, 104), 120, false);
            panel.Controls.Add(writeValueLabel);

            _writeValueTextBox.Location = new Point(540, 128);
            _writeValueTextBox.Size = new Size(380, 25);
            panel.Controls.Add(_writeValueTextBox);

            _writeTagButton.Text = "Write";
            _writeTagButton.Location = new Point(934, 126);
            _writeTagButton.Size = new Size(100, 30);
            panel.Controls.Add(_writeTagButton);
        }

        private void BuildBlockPanel(Panel panel)
        {
            Label blockTitleLabel = CreateLabel("Block Memory", new Point(12, 12), 140, true);
            panel.Controls.Add(blockTitleLabel);

            Label blockHeadLabel = CreateLabel("Head", new Point(16, 42), 60, false);
            panel.Controls.Add(blockHeadLabel);

            _blockHeadTextBox.Location = new Point(16, 66);
            _blockHeadTextBox.Size = new Size(80, 25);
            _blockHeadTextBox.Text = "D";
            panel.Controls.Add(_blockHeadTextBox);

            Label blockAddressLabel = CreateLabel("Start", new Point(112, 42), 60, false);
            panel.Controls.Add(blockAddressLabel);

            _blockAddressTextBox.Location = new Point(112, 66);
            _blockAddressTextBox.Size = new Size(90, 25);
            _blockAddressTextBox.Text = "100";
            panel.Controls.Add(_blockAddressTextBox);

            Label blockLengthLabel = CreateLabel("Length", new Point(218, 42), 60, false);
            panel.Controls.Add(blockLengthLabel);

            _blockLengthTextBox.Location = new Point(218, 66);
            _blockLengthTextBox.Size = new Size(90, 25);
            _blockLengthTextBox.Text = "8";
            panel.Controls.Add(_blockLengthTextBox);

            _readBlockButton.Text = "Read Block";
            _readBlockButton.Location = new Point(326, 63);
            _readBlockButton.Size = new Size(110, 30);
            panel.Controls.Add(_readBlockButton);

            _blockValuesTextBox.Location = new Point(460, 66);
            _blockValuesTextBox.Multiline = true;
            _blockValuesTextBox.ScrollBars = ScrollBars.Vertical;
            _blockValuesTextBox.Size = new Size(616, 36);
            _blockValuesTextBox.ReadOnly = true;
            panel.Controls.Add(_blockValuesTextBox);
        }

        private void BuildLogPanel(Panel panel)
        {
            Label logTitleLabel = CreateLabel("Log", new Point(12, 12), 80, true);
            panel.Controls.Add(logTitleLabel);

            _logTextBox.Location = new Point(16, 42);
            _logTextBox.Multiline = true;
            _logTextBox.ScrollBars = ScrollBars.Vertical;
            _logTextBox.ReadOnly = true;
            _logTextBox.Size = new Size(1060, 100);
            panel.Controls.Add(_logTextBox);
        }

        private void HookEvents()
        {
            _initializeButton.Click += InitializeButton_Click;
            _stopButton.Click += StopButton_Click;
            _readTagButton.Click += ReadTagButton_Click;
            _writeTagButton.Click += WriteTagButton_Click;
            _readBlockButton.Click += ReadBlockButton_Click;
            _addTagButton.Click += AddTagButton_Click;
            _editTagButton.Click += EditTagButton_Click;
            _deleteTagButton.Click += DeleteTagButton_Click;
            _tagComboBox.SelectedIndexChanged += TagComboBox_SelectedIndexChanged;
            _tagGridView.SelectionChanged += TagGridView_SelectionChanged;
        }

        private DataGridViewTextBoxColumn CreateTextColumn(string name, string headerText, int minimumWidth)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = headerText;
            column.MinimumWidth = minimumWidth;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            return column;
        }

        private void LoadTags()
        {
            LoadTagComboBox();
            LoadTagGrid();
            SelectTagBySequence(_selectedTagSequence);
            UpdateTagDetails();
        }

        private void LoadTagComboBox()
        {
            _tagComboBox.Items.Clear();

            foreach (TagDefinition tag in _tags)
            {
                _tagComboBox.Items.Add(tag);
            }

            if (_tagComboBox.Items.Count > 0)
            {
                if (_selectedTagSequence <= 0)
                {
                    TagDefinition? firstTag = _tagComboBox.Items[0] as TagDefinition;

                    if (firstTag != null)
                    {
                        _selectedTagSequence = firstTag.TagSequence;
                    }
                }

                SelectComboItemBySequence(_selectedTagSequence);
            }
        }

        private void LoadTagGrid()
        {
            _tagGridView.Rows.Clear();

            foreach (TagDefinition tag in _tags)
            {
                int rowIndex = _tagGridView.Rows.Add();
                DataGridViewRow row = _tagGridView.Rows[rowIndex];
                row.Cells["TagSequence"].Value = tag.TagSequence;
                row.Cells["TagName"].Value = tag.TagName;
                row.Cells["AddressHead"].Value = tag.AddressHead;
                row.Cells["TagAddress"].Value = tag.TagAddress;
                row.Cells["MemoryKind"].Value = tag.MemoryKind;
                row.Cells["DataType"].Value = tag.DataType;
                row.Cells["DataShapeKind"].Value = tag.DataShapeKind;
                row.Cells["ElementShapeKind"].Value = tag.ElementShapeKind;
                row.Cells["AddressLength"].Value = tag.AddressLength;
                row.Cells["BitDigit"].Value = tag.BitDigit;
                row.Tag = tag;
            }

            SelectGridRowBySequence(_selectedTagSequence);
        }

        private void SelectTagBySequence(int tagSequence)
        {
            if (_tags.Count == 0)
            {
                _selectedTagSequence = 0;
                _tagComboBox.SelectedItem = null;
                _tagGridView.ClearSelection();
                return;
            }

            if (tagSequence <= 0 || FindTag(tagSequence) == null)
            {
                _selectedTagSequence = _tags[0].TagSequence;
            }
            else
            {
                _selectedTagSequence = tagSequence;
            }

            _isSynchronizingSelection = true;
            SelectComboItemBySequence(_selectedTagSequence);
            SelectGridRowBySequence(_selectedTagSequence);
            _isSynchronizingSelection = false;
        }

        private void SelectComboItemBySequence(int tagSequence)
        {
            int index;

            for (index = 0; index < _tagComboBox.Items.Count; index++)
            {
                TagDefinition? tag = _tagComboBox.Items[index] as TagDefinition;

                if (tag != null && tag.TagSequence == tagSequence)
                {
                    _tagComboBox.SelectedIndex = index;
                    return;
                }
            }

            if (_tagComboBox.Items.Count > 0)
            {
                _tagComboBox.SelectedIndex = 0;
            }
        }

        private void SelectGridRowBySequence(int tagSequence)
        {
            _tagGridView.ClearSelection();

            foreach (DataGridViewRow row in _tagGridView.Rows)
            {
                TagDefinition? tag = row.Tag as TagDefinition;

                if (tag != null && tag.TagSequence == tagSequence)
                {
                    row.Selected = true;
                    _tagGridView.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private void UpdateTagDetails()
        {
            TagDefinition? tag = GetSelectedTag();

            if (tag == null)
            {
                _tagInfoValueLabel.Text = "-";
                _writeValueTextBox.Text = string.Empty;
                _currentValueTextBox.Text = string.Empty;
                return;
            }

            _tagInfoValueLabel.Text =
                "Head=" + tag.AddressHead +
                " Address=" + tag.TagAddress +
                " Type=" + tag.DataType +
                " Shape=" + tag.DataShapeKind +
                " ElementShape=" + tag.ElementShapeKind +
                " Length=" + tag.AddressLength;

            _writeValueTextBox.Text = GetSuggestedWriteValue(tag);
            _currentValueTextBox.Text = string.Empty;
        }

        private TagDefinition? GetSelectedTag()
        {
            return FindTag(_selectedTagSequence);
        }

        private TagDefinition? FindTag(int tagSequence)
        {
            int index;

            for (index = 0; index < _tags.Count; index++)
            {
                if (_tags[index].TagSequence == tagSequence)
                {
                    return _tags[index];
                }
            }

            return null;
        }

        private void SetUiState(bool isRunning)
        {
            _stopButton.Enabled = isRunning;
            _readTagButton.Enabled = isRunning;
            _writeTagButton.Enabled = isRunning;
            _readBlockButton.Enabled = isRunning;
            _tagComboBox.Enabled = _tags.Count > 0;
            _writeValueTextBox.Enabled = isRunning;
            _editTagButton.Enabled = _tags.Count > 0;
            _deleteTagButton.Enabled = _tags.Count > 0;

            if (!isRunning)
            {
                _statusValueLabel.Text = "Stopped";
                _driverValueLabel.Text = "-";
            }
        }

        private async Task InitializeSimulationAsync()
        {
            StopDriver();

            DeviceSimulationProfile profile = SampleTestConfiguration.CreateProfile();
            DeviceDefinition device = SampleTestConfiguration.CreateDevice();
            BlockDefinition block = SampleTestConfiguration.CreateBlock();
            IDeviceDriver driver = MelsecSimulationFactory.CreateDriver(profile);

            await driver.InitializeAsync(device).ConfigureAwait(true);
            await driver.StartAsync().ConfigureAwait(true);
            await driver.ApplyBlockAsync(block).ConfigureAwait(true);

            foreach (TagDefinition tag in _tags)
            {
                await driver.ApplyTagAsync(tag).ConfigureAwait(true);
            }

            _driver = driver;
            _device = device;
            _block = block;
            _driverValueLabel.Text = driver.DriverKey;
            _statusValueLabel.Text = "Running";
            SetUiState(true);
            AppendLog("Simulation driver initialized.");
            AppendLog("Loaded tags: " + _tags.Count);
        }

        private void StopDriver()
        {
            IDeviceDriver? driver = _driver;

            if (driver != null)
            {
                try
                {
                    driver.StopAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    AppendLog("Stop failed: " + ex.Message);
                }
            }

            _driver = null;
            _device = null;
            _block = null;
        }

        private void AddTag()
        {
            TagEditorForm form = new TagEditorForm(null);
            form.ShowInTaskbar = false;

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            TagDefinition tag = form.TagDefinition;

            if (FindTag(tag.TagSequence) != null)
            {
                MessageBox.Show(this, "Tag sequence already exists.", "Duplicate Sequence", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _tags.Add(tag);
            _selectedTagSequence = tag.TagSequence;
            ApplyAddedTag(tag);
            LoadTags();
            SetUiState(_driver != null);
            AppendLog("Added tag " + tag.TagName);
        }

        private void EditTag()
        {
            TagDefinition? sourceTag = GetSelectedTag();

            if (sourceTag == null)
            {
                return;
            }

            TagEditorForm form = new TagEditorForm(sourceTag);
            form.ShowInTaskbar = false;

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            TagDefinition editedTag = form.TagDefinition;
            TagDefinition? duplicateTag = FindTag(editedTag.TagSequence);

            if (duplicateTag != null && duplicateTag.TagSequence != sourceTag.TagSequence)
            {
                MessageBox.Show(this, "Tag sequence already exists.", "Duplicate Sequence", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = _tags.IndexOf(sourceTag);
            if (index < 0)
            {
                return;
            }

            _tags[index] = editedTag;
            _selectedTagSequence = editedTag.TagSequence;
            ApplyEditedTag(sourceTag, editedTag);
            LoadTags();
            AppendLog("Updated tag " + editedTag.TagName);
        }

        private void DeleteTag()
        {
            TagDefinition? tag = GetSelectedTag();

            if (tag == null)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                this,
                "Delete tag " + tag.TagName + "?",
                "Delete Tag",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _tags.Remove(tag);
            RemoveTagFromDriver(tag.TagSequence);
            AppendLog("Deleted tag " + tag.TagName);

            if (_tags.Count > 0)
            {
                _selectedTagSequence = _tags[0].TagSequence;
            }
            else
            {
                _selectedTagSequence = 0;
            }

            LoadTags();
            SetUiState(_driver != null);
        }

        private void ApplyAddedTag(TagDefinition tag)
        {
            if (_driver == null)
            {
                return;
            }

            try
            {
                _driver.ApplyTagAsync(tag).GetAwaiter().GetResult();
                AppendLog("Applied new tag to running driver.");
            }
            catch (Exception ex)
            {
                AppendLog("Apply new tag failed: " + ex.Message);
            }
        }

        private void ApplyEditedTag(TagDefinition sourceTag, TagDefinition editedTag)
        {
            if (_driver == null)
            {
                return;
            }

            try
            {
                if (sourceTag.TagSequence != editedTag.TagSequence)
                {
                    _driver.RemoveTagAsync(sourceTag.TagSequence).GetAwaiter().GetResult();
                }

                _driver.ApplyTagAsync(editedTag).GetAwaiter().GetResult();
                AppendLog("Applied updated tag to running driver.");
            }
            catch (Exception ex)
            {
                AppendLog("Apply updated tag failed: " + ex.Message);
            }
        }

        private void RemoveTagFromDriver(int tagSequence)
        {
            if (_driver == null)
            {
                return;
            }

            try
            {
                _driver.RemoveTagAsync(tagSequence).GetAwaiter().GetResult();
                AppendLog("Removed tag from running driver.");
            }
            catch (Exception ex)
            {
                AppendLog("Remove tag failed: " + ex.Message);
            }
        }

        private void AppendLog(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss") + "  " + message;

            if (_logTextBox.TextLength > 0)
            {
                _logTextBox.AppendText(Environment.NewLine);
            }

            _logTextBox.AppendText(line);
        }

        private static string JoinValues(IReadOnlyList<int> values)
        {
            StringBuilder builder = new StringBuilder();
            int index;

            for (index = 0; index < values.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(values[index]);
            }

            return builder.ToString();
        }

        private static string GetSuggestedWriteValue(TagDefinition tag)
        {
            if (string.Equals(tag.DataType, "bool", StringComparison.OrdinalIgnoreCase))
            {
                return "true";
            }

            if (string.Equals(tag.DataType, "int32", StringComparison.OrdinalIgnoreCase))
            {
                return "654321";
            }

            if (string.Equals(tag.DataType, "float[2]", StringComparison.OrdinalIgnoreCase))
            {
                return "1.25,2.5";
            }

            if (string.Equals(tag.DataType, "string[16]", StringComparison.OrdinalIgnoreCase))
            {
                return "RUN";
            }

            if (string.Equals(tag.DataType, "int16[4]", StringComparison.OrdinalIgnoreCase))
            {
                return "11,22,33,44";
            }

            if (string.Equals(tag.DataType, "double", StringComparison.OrdinalIgnoreCase))
            {
                return "456.789";
            }

            return string.Empty;
        }

        private static Panel CreatePanel(Rectangle bounds)
        {
            Panel panel = new Panel();
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Bounds = bounds;
            return panel;
        }

        private static Label CreateLabel(string text, Point location, int width, bool bold)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.Size = new Size(width, 22);

            if (bold)
            {
                label.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            }

            return label;
        }

        #endregion

        #region Events

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            StopDriver();
            base.OnFormClosed(e);
        }

        private async void InitializeButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            try
            {
                await InitializeSimulationAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                AppendLog("Initialize failed: " + ex.Message);
                MessageBox.Show(this, ex.Message, "Initialize Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            StopDriver();
            SetUiState(false);
            AppendLog("Driver stopped.");
        }

        private async void ReadTagButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            TagDefinition? tag = GetSelectedTag();
            if (_driver == null || tag == null)
            {
                return;
            }

            try
            {
                string? value = await _driver.GetDirectTagValueAsync(tag).ConfigureAwait(true);

                if (value == null)
                {
                    _currentValueTextBox.Text = string.Empty;
                }
                else
                {
                    _currentValueTextBox.Text = value;
                }

                AppendLog("Read tag " + tag.TagName + " = " + _currentValueTextBox.Text);
            }
            catch (Exception ex)
            {
                AppendLog("Read tag failed: " + ex.Message);
            }
        }

        private async void WriteTagButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            TagDefinition? tag = GetSelectedTag();
            if (_driver == null || tag == null)
            {
                return;
            }

            try
            {
                bool success = await _driver.SetDirectTagValueAsync(tag, _writeValueTextBox.Text).ConfigureAwait(true);
                AppendLog("Write tag " + tag.TagName + " = " + _writeValueTextBox.Text + " result " + success);

                if (success)
                {
                    string? value = await _driver.GetDirectTagValueAsync(tag).ConfigureAwait(true);

                    if (value == null)
                    {
                        _currentValueTextBox.Text = string.Empty;
                    }
                    else
                    {
                        _currentValueTextBox.Text = value;
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("Write tag failed: " + ex.Message);
            }
        }

        private async void ReadBlockButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            if (_driver == null)
            {
                return;
            }

            try
            {
                int startAddress;
                int length;

                if (!int.TryParse(_blockAddressTextBox.Text, out startAddress) ||
                    !int.TryParse(_blockLengthTextBox.Text, out length))
                {
                    MessageBox.Show(this, "Start and Length must be integers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int[] values = await _driver.ReadBlockMemoryAsync(_blockHeadTextBox.Text.Trim(), startAddress, length).ConfigureAwait(true);
                _blockValuesTextBox.Text = JoinValues(values);
                AppendLog("Read block " + _blockHeadTextBox.Text.Trim() + startAddress + " length " + length);
            }
            catch (Exception ex)
            {
                AppendLog("Read block failed: " + ex.Message);
            }
        }

        private void AddTagButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;
            AddTag();
        }

        private void EditTagButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;
            EditTag();
        }

        private void DeleteTagButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;
            DeleteTag();
        }

        private void TagComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            if (_isSynchronizingSelection)
            {
                return;
            }

            TagDefinition? tag = _tagComboBox.SelectedItem as TagDefinition;

            if (tag == null)
            {
                return;
            }

            _selectedTagSequence = tag.TagSequence;
            _isSynchronizingSelection = true;
            SelectGridRowBySequence(_selectedTagSequence);
            _isSynchronizingSelection = false;
            UpdateTagDetails();
        }

        private void TagGridView_SelectionChanged(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            if (_isSynchronizingSelection)
            {
                return;
            }

            if (_tagGridView.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = _tagGridView.SelectedRows[0];
            TagDefinition? tag = row.Tag as TagDefinition;

            if (tag == null)
            {
                return;
            }

            _selectedTagSequence = tag.TagSequence;
            _isSynchronizingSelection = true;
            SelectComboItemBySequence(_selectedTagSequence);
            _isSynchronizingSelection = false;
            UpdateTagDetails();
        }

        #endregion
    }
}
