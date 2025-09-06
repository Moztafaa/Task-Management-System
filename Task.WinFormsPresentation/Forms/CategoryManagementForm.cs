using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Task.Application.ServiceInterface;
using Task.Domain.Entities;

namespace Task.WinFormsPresentation.Forms;

[DesignerCategory("")]
public partial class CategoryManagementForm : Form
{
    private readonly ICategoryService _categoryService;
    private ListBox lstCategories = null!;
    private TextBox txtCategoryName = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnDelete = null!;

    public CategoryManagementForm(ICategoryService categoryService)
    {
        _categoryService = categoryService;
        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(500, 400);
        this.Text = "Category Management";
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        CreateControls();
    }

    private void CreateControls()
    {
        // Title
        var lblTitle = new Label
        {
            Text = "Manage Categories",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, 20),
            Size = new Size(200, 30),
            ForeColor = Color.FromArgb(44, 62, 80)
        };

        // Category list
        var lblCategories = new Label
        {
            Text = "Categories:",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 60),
            Size = new Size(100, 25)
        };

        lstCategories = new ListBox
        {
            Location = new Point(20, 85),
            Size = new Size(280, 200),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Category name input
        var lblCategoryName = new Label
        {
            Text = "Category Name:",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(320, 85),
            Size = new Size(130, 25)
        };

        txtCategoryName = new TextBox
        {
            Location = new Point(320, 110),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Buttons
        btnAdd = new Button
        {
            Text = "➕ Add",
            Location = new Point(320, 150),
            Size = new Size(70, 35),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAdd.FlatAppearance.BorderSize = 0;

        btnEdit = new Button
        {
            Text = "✏️ Edit",
            Location = new Point(400, 150),
            Size = new Size(70, 35),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnEdit.FlatAppearance.BorderSize = 0;

        btnDelete = new Button
        {
            Text = "🗑️ Delete",
            Location = new Point(320, 195),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnDelete.FlatAppearance.BorderSize = 0;

        // Close button
        var btnClose = new Button
        {
            Text = "Close",
            Location = new Point(320, 300),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnClose.FlatAppearance.BorderSize = 0;

        // Event handlers
        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClose.Click += (s, e) => this.Close();
        lstCategories.SelectedIndexChanged += LstCategories_SelectedIndexChanged;

        // Add controls to form
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblCategories);
        this.Controls.Add(lstCategories);
        this.Controls.Add(lblCategoryName);
        this.Controls.Add(txtCategoryName);
        this.Controls.Add(btnAdd);
        this.Controls.Add(btnEdit);
        this.Controls.Add(btnDelete);
        this.Controls.Add(btnClose);
    }

    private void LoadCategories()
    {
        try
        {
            lstCategories.Items.Clear();
            var categories = _categoryService.GetAllCategories();

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    lstCategories.Items.Add(new CategoryDisplayItem { Id = category.Id, Name = category.Name });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
        {
            MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = txtCategoryName.Text.Trim()
            };

            _categoryService.AddCategory(newCategory);
            LoadCategories();
            txtCategoryName.Clear();

            MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (lstCategories.SelectedItem is not CategoryDisplayItem selectedItem)
        {
            MessageBox.Show("Please select a category to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
        {
            MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var category = _categoryService.GetCategoryById(selectedItem.Id);
            if (category != null)
            {
                category.Name = txtCategoryName.Text.Trim();
                _categoryService.UpdateCategory(category);
                LoadCategories();
                txtCategoryName.Clear();

                MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (lstCategories.SelectedItem is not CategoryDisplayItem selectedItem)
        {
            MessageBox.Show("Please select a category to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete the category '{selectedItem.Name}'?",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                _categoryService.DeleteCategory(selectedItem.Id);
                LoadCategories();
                txtCategoryName.Clear();

                MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void LstCategories_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstCategories.SelectedItem is CategoryDisplayItem selectedItem)
        {
            txtCategoryName.Text = selectedItem.Name;
        }
    }

    private sealed class CategoryDisplayItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
