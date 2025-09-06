using System;
using System.ComponentModel;
using System.Windows.Forms;
using Task.Application.DTO;
using Task.Application.ServiceInterface;
using Task.Domain.Entities;
using TaskStatus = Task.Domain.Entities.TaskStatus;
using TaskPriority = Task.Domain.Entities.TaskPriority;

namespace Task.WinFormsPresentation.Forms;

[DesignerCategory("")]
public partial class TestForm : Form
{
    private readonly IAuthService _authService;
    private readonly ITaskService _taskService;
    private User? _currentUser;

    // UI Controls
    private TabControl tabControl = null!;
    private TabPage registerTab = null!;
    private TabPage loginTab = null!;
    private TabPage tasksTab = null!;

    // Register Tab Controls
    private TextBox txtRegUsername = null!;
    private TextBox txtRegEmail = null!;
    private TextBox txtRegPassword = null!;
    private TextBox txtRegConfirmPassword = null!;
    private Button btnRegister = null!;
    private Label lblRegResult = null!;

    // Login Tab Controls
    private TextBox txtLoginUsername = null!;
    private TextBox txtLoginPassword = null!;
    private Button btnLogin = null!;
    private Label lblLoginResult = null!;

    // Tasks Tab Controls
    private TextBox txtTaskTitle = null!;
    private TextBox txtTaskDescription = null!;
    private DateTimePicker dtpDueDate = null!;
    private ComboBox cmbPriority = null!;
    private ComboBox cmbStatus = null!;
    private Button btnCreateTask = null!;
    private ListBox lstTasks = null!;
    private Button btnRefreshTasks = null!;
    private Button btnDeleteTask = null!;
    private Label lblCurrentUser = null!;

    public TestForm(IAuthService authService, ITaskService taskService)
    {
        _authService = authService;
        _taskService = taskService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Task Management Test Form";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create TabControl
        tabControl = new TabControl
        {
            Dock = DockStyle.Fill
        };

        CreateRegisterTab();
        CreateLoginTab();
        CreateTasksTab();

        this.Controls.Add(tabControl);
    }

    private void CreateRegisterTab()
    {
        registerTab = new TabPage("Register");

        var lblRegUsername = new Label { Text = "Username:", Location = new Point(20, 30), Size = new Size(100, 23) };
        txtRegUsername = new TextBox { Location = new Point(130, 30), Size = new Size(200, 23) };

        var lblRegEmail = new Label { Text = "Email:", Location = new Point(20, 70), Size = new Size(100, 23) };
        txtRegEmail = new TextBox { Location = new Point(130, 70), Size = new Size(200, 23) };

        var lblRegPassword = new Label { Text = "Password:", Location = new Point(20, 110), Size = new Size(100, 23) };
        txtRegPassword = new TextBox { Location = new Point(130, 110), Size = new Size(200, 23), UseSystemPasswordChar = true };

        var lblRegConfirmPassword = new Label { Text = "Confirm Password:", Location = new Point(20, 150), Size = new Size(100, 23) };
        txtRegConfirmPassword = new TextBox { Location = new Point(130, 150), Size = new Size(200, 23), UseSystemPasswordChar = true };

        btnRegister = new Button { Text = "Register", Location = new Point(130, 190), Size = new Size(100, 30) };
        btnRegister.Click += BtnRegister_Click;

        lblRegResult = new Label { Location = new Point(20, 230), Size = new Size(400, 60), ForeColor = Color.Red };

        registerTab.Controls.AddRange(new Control[] {
            lblRegUsername, txtRegUsername,
            lblRegEmail, txtRegEmail,
            lblRegPassword, txtRegPassword,
            lblRegConfirmPassword, txtRegConfirmPassword,
            btnRegister, lblRegResult
        });

        tabControl.TabPages.Add(registerTab);
    }

    private void CreateLoginTab()
    {
        loginTab = new TabPage("Login");

        var lblLoginUsername = new Label { Text = "Username:", Location = new Point(20, 30), Size = new Size(100, 23) };
        txtLoginUsername = new TextBox { Location = new Point(130, 30), Size = new Size(200, 23) };

        var lblLoginPassword = new Label { Text = "Password:", Location = new Point(20, 70), Size = new Size(100, 23) };
        txtLoginPassword = new TextBox { Location = new Point(130, 70), Size = new Size(200, 23), UseSystemPasswordChar = true };

        btnLogin = new Button { Text = "Login", Location = new Point(130, 110), Size = new Size(100, 30) };
        btnLogin.Click += BtnLogin_Click;

        lblLoginResult = new Label { Location = new Point(20, 150), Size = new Size(400, 60), ForeColor = Color.Green };

        loginTab.Controls.AddRange(new Control[] {
            lblLoginUsername, txtLoginUsername,
            lblLoginPassword, txtLoginPassword,
            btnLogin, lblLoginResult
        });

        tabControl.TabPages.Add(loginTab);
    }

    private void CreateTasksTab()
    {
        tasksTab = new TabPage("Tasks");

        lblCurrentUser = new Label { Text = "Not logged in", Location = new Point(20, 10), Size = new Size(300, 23), ForeColor = Color.Blue };

        var lblTaskTitle = new Label { Text = "Title:", Location = new Point(20, 50), Size = new Size(100, 23) };
        txtTaskTitle = new TextBox { Location = new Point(130, 50), Size = new Size(200, 23) };

        var lblTaskDescription = new Label { Text = "Description:", Location = new Point(20, 90), Size = new Size(100, 23) };
        txtTaskDescription = new TextBox { Location = new Point(130, 90), Size = new Size(200, 60), Multiline = true };

        var lblDueDate = new Label { Text = "Due Date:", Location = new Point(20, 170), Size = new Size(100, 23) };
        dtpDueDate = new DateTimePicker { Location = new Point(130, 170), Size = new Size(200, 23) };

        var lblPriority = new Label { Text = "Priority:", Location = new Point(20, 210), Size = new Size(100, 23) };
        cmbPriority = new ComboBox { Location = new Point(130, 210), Size = new Size(200, 23) };
        cmbPriority.Items.AddRange(new[] { "Low", "Medium", "High" });
        cmbPriority.SelectedIndex = 1; // Medium

        var lblStatus = new Label { Text = "Status:", Location = new Point(20, 250), Size = new Size(100, 23) };
        cmbStatus = new ComboBox { Location = new Point(130, 250), Size = new Size(200, 23) };
        cmbStatus.Items.AddRange(new[] { "Pending", "InProgress", "Completed" });
        cmbStatus.SelectedIndex = 0; // Pending

        btnCreateTask = new Button { Text = "Create Task", Location = new Point(130, 290), Size = new Size(100, 30) };
        btnCreateTask.Click += BtnCreateTask_Click;

        lstTasks = new ListBox { Location = new Point(400, 50), Size = new Size(350, 300) };

        btnRefreshTasks = new Button { Text = "Refresh Tasks", Location = new Point(400, 360), Size = new Size(100, 30) };
        btnRefreshTasks.Click += BtnRefreshTasks_Click;

        btnDeleteTask = new Button { Text = "Delete Selected", Location = new Point(520, 360), Size = new Size(100, 30) };
        btnDeleteTask.Click += BtnDeleteTask_Click;

        tasksTab.Controls.AddRange(new Control[] {
            lblCurrentUser,
            lblTaskTitle, txtTaskTitle,
            lblTaskDescription, txtTaskDescription,
            lblDueDate, dtpDueDate,
            lblPriority, cmbPriority,
            lblStatus, cmbStatus,
            btnCreateTask,
            lstTasks, btnRefreshTasks, btnDeleteTask
        });

        tabControl.TabPages.Add(tasksTab);
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        try
        {
            var registerDto = new RegisterUserDto
            {
                Username = txtRegUsername.Text,
                Email = txtRegEmail.Text,
                Password = txtRegPassword.Text,
                ConfirmPassword = txtRegConfirmPassword.Text
            };

            var user = _authService.RegisterUser(registerDto);
            lblRegResult.ForeColor = Color.Green;
            lblRegResult.Text = $"User '{user.Username}' registered successfully!\nID: {user.Id}";

            // Clear form
            txtRegUsername.Clear();
            txtRegEmail.Clear();
            txtRegPassword.Clear();
            txtRegConfirmPassword.Clear();
        }
        catch (Exception ex)
        {
            lblRegResult.ForeColor = Color.Red;
            lblRegResult.Text = $"Registration failed: {ex.Message}";
        }
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        try
        {
            var loginDto = new LoginUserDto
            {
                Username = txtLoginUsername.Text,
                Password = txtLoginPassword.Text
            };

            var user = _authService.LoginUser(loginDto);
            if (user != null)
            {
                _currentUser = user;
                lblLoginResult.ForeColor = Color.Green;
                lblLoginResult.Text = $"Login successful!\nWelcome, {user.Username}";
                lblCurrentUser.Text = $"Logged in as: {user.Username} (ID: {user.Id})";

                // Switch to tasks tab
                tabControl.SelectedTab = tasksTab;
                LoadTasks();
            }
            else
            {
                lblLoginResult.ForeColor = Color.Red;
                lblLoginResult.Text = "Login failed: Invalid username or password";
            }
        }
        catch (Exception ex)
        {
            lblLoginResult.ForeColor = Color.Red;
            lblLoginResult.Text = $"Login error: {ex.Message}";
        }
    }

    private void BtnCreateTask_Click(object? sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            MessageBox.Show("Please login first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = txtTaskTitle.Text,
                Description = txtTaskDescription.Text,
                DueDate = dtpDueDate.Value,
                Priority = (TaskPriority)(cmbPriority.SelectedIndex + 1),
                Status = (TaskStatus)(cmbStatus.SelectedIndex + 1),
                UserId = _currentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            _taskService.AddTask(task);
            MessageBox.Show("Task created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear form and refresh tasks
            txtTaskTitle.Clear();
            txtTaskDescription.Clear();
            dtpDueDate.Value = DateTime.Now;
            cmbPriority.SelectedIndex = 1;
            cmbStatus.SelectedIndex = 0;
            LoadTasks();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRefreshTasks_Click(object? sender, EventArgs e)
    {
        LoadTasks();
    }

    private void BtnDeleteTask_Click(object? sender, EventArgs e)
    {
        if (lstTasks.SelectedItem == null)
        {
            MessageBox.Show("Please select a task to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (lstTasks.SelectedItem is TaskItem selectedTask)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedTask.Title}'?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _taskService.DeleteTask(selectedTask.Id);
                    MessageBox.Show("Task deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTasks();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadTasks()
    {
        if (_currentUser == null) return;

        try
        {
            var tasks = _taskService.GetAllTasks();
            lstTasks.Items.Clear();

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    lstTasks.Items.Add(task);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load tasks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
