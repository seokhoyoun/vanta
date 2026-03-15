using System;
using System.Drawing;
using System.Windows.Forms;
using Vanta.Comm.Contracts.Enums;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.TestHost.WinForms
{
    public sealed class TagEditorForm : Form
    {
        #region Public Properties

        public TagDefinition TagDefinition
        {
            get
            {
                return _tagDefinition;
            }
        }

        #endregion

        #region Fields

        private readonly NumericUpDown _tagSequenceNumericUpDown;
        private readonly NumericUpDown _deviceIdNumericUpDown;
        private readonly NumericUpDown _blockSequenceNumericUpDown;
        private readonly NumericUpDown _addressLengthNumericUpDown;
        private readonly NumericUpDown _bitDigitNumericUpDown;
        private readonly NumericUpDown _elementCountNumericUpDown;
        private readonly TextBox _tagNameTextBox;
        private readonly TextBox _addressHeadTextBox;
        private readonly TextBox _tagAddressTextBox;
        private readonly TextBox _dataTypeTextBox;
        private readonly ComboBox _memoryKindComboBox;
        private readonly ComboBox _dataShapeKindComboBox;
        private readonly ComboBox _elementShapeKindComboBox;
        private readonly CheckBox _isReadOnlyCheckBox;
        private readonly CheckBox _isEnabledCheckBox;
        private readonly Button _saveButton;
        private readonly Button _cancelButton;
        private readonly TagDefinition _tagDefinition;

        #endregion

        #region Constructors

        public TagEditorForm(TagDefinition? sourceTag)
        {
            _tagDefinition = CloneTag(sourceTag);
            _tagSequenceNumericUpDown = new NumericUpDown();
            _deviceIdNumericUpDown = new NumericUpDown();
            _blockSequenceNumericUpDown = new NumericUpDown();
            _addressLengthNumericUpDown = new NumericUpDown();
            _bitDigitNumericUpDown = new NumericUpDown();
            _elementCountNumericUpDown = new NumericUpDown();
            _tagNameTextBox = new TextBox();
            _addressHeadTextBox = new TextBox();
            _tagAddressTextBox = new TextBox();
            _dataTypeTextBox = new TextBox();
            _memoryKindComboBox = new ComboBox();
            _dataShapeKindComboBox = new ComboBox();
            _elementShapeKindComboBox = new ComboBox();
            _isReadOnlyCheckBox = new CheckBox();
            _isEnabledCheckBox = new CheckBox();
            _saveButton = new Button();
            _cancelButton = new Button();

            InitializeLayout();
            LoadTag();
            HookEvents();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Helpers

        private void InitializeLayout()
        {
            Text = "Tag Editor";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(520, 430);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            ConfigureNumeric(_tagSequenceNumericUpDown, 16, 38, 120);
            ConfigureNumeric(_deviceIdNumericUpDown, 190, 38, 120);
            ConfigureNumeric(_blockSequenceNumericUpDown, 364, 38, 120);
            ConfigureTextBox(_tagNameTextBox, 16, 82, 468);
            ConfigureNumeric(_addressLengthNumericUpDown, 16, 126, 120);
            ConfigureNumeric(_bitDigitNumericUpDown, 190, 126, 120);
            ConfigureNumeric(_elementCountNumericUpDown, 364, 126, 120);
            ConfigureTextBox(_addressHeadTextBox, 16, 170, 120);
            ConfigureTextBox(_tagAddressTextBox, 190, 170, 120);
            ConfigureComboBox(_memoryKindComboBox, 364, 170, 120, typeof(MemoryKind));
            ConfigureComboBox(_dataShapeKindComboBox, 16, 214, 120, typeof(DataShapeKind));
            ConfigureComboBox(_elementShapeKindComboBox, 190, 214, 120, typeof(DataShapeKind));
            ConfigureTextBox(_dataTypeTextBox, 16, 258, 468);

            _isReadOnlyCheckBox.Location = new Point(364, 214);
            _isReadOnlyCheckBox.Size = new Size(120, 24);
            _isReadOnlyCheckBox.Text = "Read Only";
            Controls.Add(_isReadOnlyCheckBox);

            _isEnabledCheckBox.Location = new Point(364, 258);
            _isEnabledCheckBox.Size = new Size(120, 24);
            _isEnabledCheckBox.Text = "Enabled";
            Controls.Add(_isEnabledCheckBox);

            _saveButton.Text = "Save";
            _saveButton.Location = new Point(284, 374);
            _saveButton.Size = new Size(96, 30);
            Controls.Add(_saveButton);

            _cancelButton.Text = "Cancel";
            _cancelButton.Location = new Point(388, 374);
            _cancelButton.Size = new Size(96, 30);
            _cancelButton.DialogResult = DialogResult.Cancel;
            Controls.Add(_cancelButton);

            Controls.Add(CreateLabel("Tag Sequence", 16, 16, 120));
            Controls.Add(CreateLabel("Device Id", 190, 16, 120));
            Controls.Add(CreateLabel("Block Sequence", 364, 16, 120));
            Controls.Add(CreateLabel("Tag Name", 16, 60, 120));
            Controls.Add(CreateLabel("Address Length", 16, 104, 120));
            Controls.Add(CreateLabel("Bit Digit", 190, 104, 120));
            Controls.Add(CreateLabel("Element Count", 364, 104, 120));
            Controls.Add(CreateLabel("Address Head", 16, 148, 120));
            Controls.Add(CreateLabel("Tag Address", 190, 148, 120));
            Controls.Add(CreateLabel("Memory Kind", 364, 148, 120));
            Controls.Add(CreateLabel("Data Shape", 16, 192, 120));
            Controls.Add(CreateLabel("Element Shape", 190, 192, 120));
            Controls.Add(CreateLabel("Data Type", 16, 236, 120));
            Controls.Add(CreateLabel("Examples: bool, int32, float[2], string[16]", 16, 294, 320));
        }

        private void HookEvents()
        {
            _saveButton.Click += SaveButton_Click;
        }

        private void LoadTag()
        {
            _tagSequenceNumericUpDown.Value = _tagDefinition.TagSequence;
            _deviceIdNumericUpDown.Value = _tagDefinition.DeviceId;
            _blockSequenceNumericUpDown.Value = _tagDefinition.BlockSequence;
            _addressLengthNumericUpDown.Value = _tagDefinition.AddressLength;
            _bitDigitNumericUpDown.Value = _tagDefinition.BitDigit;
            _elementCountNumericUpDown.Value = _tagDefinition.ElementCount;
            _tagNameTextBox.Text = _tagDefinition.TagName;
            _addressHeadTextBox.Text = _tagDefinition.AddressHead;
            _tagAddressTextBox.Text = _tagDefinition.TagAddress;
            _dataTypeTextBox.Text = _tagDefinition.DataType;
            _memoryKindComboBox.SelectedItem = _tagDefinition.MemoryKind;
            _dataShapeKindComboBox.SelectedItem = _tagDefinition.DataShapeKind;
            _elementShapeKindComboBox.SelectedItem = _tagDefinition.ElementShapeKind;
            _isReadOnlyCheckBox.Checked = _tagDefinition.IsReadOnly;
            _isEnabledCheckBox.Checked = _tagDefinition.IsEnabled;
        }

        private void SaveToModel()
        {
            object? memoryKindItem = _memoryKindComboBox.SelectedItem;
            object? dataShapeKindItem = _dataShapeKindComboBox.SelectedItem;
            object? elementShapeKindItem = _elementShapeKindComboBox.SelectedItem;

            _tagDefinition.TagSequence = Decimal.ToInt32(_tagSequenceNumericUpDown.Value);
            _tagDefinition.DeviceId = Decimal.ToInt32(_deviceIdNumericUpDown.Value);
            _tagDefinition.BlockSequence = Decimal.ToInt32(_blockSequenceNumericUpDown.Value);
            _tagDefinition.AddressLength = Decimal.ToInt32(_addressLengthNumericUpDown.Value);
            _tagDefinition.BitDigit = Decimal.ToInt32(_bitDigitNumericUpDown.Value);
            _tagDefinition.ElementCount = Decimal.ToInt32(_elementCountNumericUpDown.Value);
            _tagDefinition.TagName = _tagNameTextBox.Text.Trim();
            _tagDefinition.AddressHead = _addressHeadTextBox.Text.Trim();
            _tagDefinition.TagAddress = _tagAddressTextBox.Text.Trim();
            _tagDefinition.DataType = _dataTypeTextBox.Text.Trim();

            if (memoryKindItem is MemoryKind)
            {
                _tagDefinition.MemoryKind = (MemoryKind)memoryKindItem;
            }

            if (dataShapeKindItem is DataShapeKind)
            {
                _tagDefinition.DataShapeKind = (DataShapeKind)dataShapeKindItem;
            }

            if (elementShapeKindItem is DataShapeKind)
            {
                _tagDefinition.ElementShapeKind = (DataShapeKind)elementShapeKindItem;
            }

            _tagDefinition.IsReadOnly = _isReadOnlyCheckBox.Checked;
            _tagDefinition.IsEnabled = _isEnabledCheckBox.Checked;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(_tagNameTextBox.Text))
            {
                ShowValidationMessage("Tag Name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_addressHeadTextBox.Text))
            {
                ShowValidationMessage("Address Head is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_tagAddressTextBox.Text))
            {
                ShowValidationMessage("Tag Address is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_dataTypeTextBox.Text))
            {
                ShowValidationMessage("Data Type is required.");
                return false;
            }

            return true;
        }

        private void ShowValidationMessage(string message)
        {
            MessageBox.Show(this, message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ConfigureNumeric(NumericUpDown control, int x, int y, int width)
        {
            control.Location = new Point(x, y);
            control.Size = new Size(width, 25);
            control.Maximum = 1000000;
            Controls.Add(control);
        }

        private void ConfigureTextBox(TextBox control, int x, int y, int width)
        {
            control.Location = new Point(x, y);
            control.Size = new Size(width, 25);
            Controls.Add(control);
        }

        private void ConfigureComboBox(ComboBox control, int x, int y, int width, Type enumType)
        {
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            control.Location = new Point(x, y);
            control.Size = new Size(width, 25);
            Array values = Enum.GetValues(enumType);
            int index;

            for (index = 0; index < values.Length; index++)
            {
                object? value = values.GetValue(index);

                if (value != null)
                {
                    control.Items.Add(value);
                }
            }

            if (control.Items.Count > 0)
            {
                control.SelectedIndex = 0;
            }

            Controls.Add(control);
        }

        private static Label CreateLabel(string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(width, 22);
            return label;
        }

        private static TagDefinition CloneTag(TagDefinition? sourceTag)
        {
            TagDefinition tag = new TagDefinition();

            if (sourceTag == null)
            {
                tag.IsEnabled = true;
                tag.DeviceId = 1;
                tag.BlockSequence = 1000;
                tag.DataShapeKind = DataShapeKind.Scalar;
                tag.ElementShapeKind = DataShapeKind.Unknown;
                tag.MemoryKind = MemoryKind.Word;
                return tag;
            }

            tag.TagSequence = sourceTag.TagSequence;
            tag.DeviceId = sourceTag.DeviceId;
            tag.BlockSequence = sourceTag.BlockSequence;
            tag.MemoryKind = sourceTag.MemoryKind;
            tag.AddressIndex = sourceTag.AddressIndex;
            tag.AddressLength = sourceTag.AddressLength;
            tag.BitDigit = sourceTag.BitDigit;
            tag.DecimalPosition = sourceTag.DecimalPosition;
            tag.MinimumValue = sourceTag.MinimumValue;
            tag.MaximumValue = sourceTag.MaximumValue;
            tag.AnalogMinimum = sourceTag.AnalogMinimum;
            tag.AnalogMaximum = sourceTag.AnalogMaximum;
            tag.EventUnit = sourceTag.EventUnit;
            tag.ElementCount = sourceTag.ElementCount;
            tag.CompositeTypeId = sourceTag.CompositeTypeId;
            tag.IsReadOnly = sourceTag.IsReadOnly;
            tag.IsEnabled = sourceTag.IsEnabled;
            tag.DataShapeKind = sourceTag.DataShapeKind;
            tag.ElementShapeKind = sourceTag.ElementShapeKind;
            tag.TagGroup = sourceTag.TagGroup;
            tag.TagName = sourceTag.TagName;
            tag.DataType = sourceTag.DataType;
            tag.CompositeTypeName = sourceTag.CompositeTypeName;
            tag.RawValue = sourceTag.RawValue;
            tag.Unit = sourceTag.Unit;
            tag.AddressHead = sourceTag.AddressHead;
            tag.TagAddress = sourceTag.TagAddress;
            tag.Description = sourceTag.Description;
            tag.EventGroup = sourceTag.EventGroup;
            return tag;
        }

        #endregion

        #region Events

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            _ = sender;
            _ = e;

            if (!ValidateInput())
            {
                return;
            }

            SaveToModel();
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}
