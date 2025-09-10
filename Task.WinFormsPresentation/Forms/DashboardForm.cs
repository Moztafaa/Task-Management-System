using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Task.Application.ServiceInterface;
using Task.Application.Services;
using Task.Domain.Entities;
using TaskStatus = Task.Domain.Entities.TaskStatus;
using TaskPriority = Task.Domain.Entities.TaskPriority;

namespace Task.WinFormsPresentation.Forms;

[DesignerCategory("")]
public partial class DashboardForm : Form
{
    private readonly ITaskService _taskService;
    private readonly IAuthService _authService;
    private readonly IReportService _reportService;
    private readonly ICategoryService _categoryService;

    // Main Layout
    private Panel topPanel = null!;
    private Panel sidePanel = null!;
    private Panel contentPanel = null!;

    // Top Panel Controls
    private Label lblWelcome = null!;
    private Button btnProfile = null!;
    private Button btnLogout = null!;

    // Side Panel Controls
    private Button btnDashboard = null!;
    private Button btnTasks = null!;
    private Button btnAddTask = null!;
    private Button btnReports = null!;
    private Button btnSearch = null!;
    private Button btnSideLogout = null!;

    // Content Area
    private Panel dashboardContent = null!;
    private Panel tasksContent = null!;
    private Panel addTaskContent = null!;
    private Panel reportsContent = null!;
    private Panel searchContent = null!;

    // Dashboard widgets
    private Panel statsPanel = null!;
    private Panel recentTasksPanel = null!;
    private Panel quickActionsPanel = null!;

    // Current user
    private User? _currentUser;

    public DashboardForm(ITaskService taskService, IAuthService authService, IReportService reportService, ICategoryService categoryService)
    {
        _taskService = taskService;
        _authService = authService;
        _reportService = reportService;
        _categoryService = categoryService;
        _currentUser = SessionManager.Instance.CurrentUser;

        InitializeComponent();
        ApplyModernStyling();
        LoadDashboard();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(1400, 900);
        this.Text = "Task Management Dashboard";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.FromArgb(248, 249, 250);

        CreateLayout();
        CreateTopPanel();
        CreateSidePanel();
        CreateContentPanels();

        this.Controls.Add(topPanel);
        this.Controls.Add(sidePanel);
        this.Controls.Add(contentPanel);

        // Handle resize events
        this.Resize += DashboardForm_Resize;
    }

    private void DashboardForm_Resize(object? sender, EventArgs e)
    {
        if (contentPanel != null)
        {
            contentPanel.Size = new Size(this.Width - 250, this.Height - 80);

            // Refresh all content panel sizes
            var availableWidth = contentPanel.Width - 40;
            var availableHeight = contentPanel.Height - 40;

            if (dashboardContent != null)
                dashboardContent.Size = new Size(availableWidth, availableHeight);
            if (tasksContent != null)
                tasksContent.Size = new Size(availableWidth, availableHeight);
            if (addTaskContent != null)
                addTaskContent.Size = new Size(availableWidth, availableHeight);
            if (reportsContent != null)
                reportsContent.Size = new Size(availableWidth, availableHeight);
            if (searchContent != null)
                searchContent.Size = new Size(availableWidth, availableHeight);

            // Refresh dashboard layout if currently visible
            if (dashboardContent != null && dashboardContent.Visible)
            {
                RefreshDashboardLayout();
            }
        }
    }
    private void RefreshDashboardLayout()
    {
        if (statsPanel != null)
        {
            statsPanel.Size = new Size(contentPanel.Width - 40, 120);
        }

        if (recentTasksPanel != null && quickActionsPanel != null)
        {
            var panelWidth = (contentPanel.Width - 60) / 2;
            recentTasksPanel.Size = new Size(panelWidth, 450);
            quickActionsPanel.Location = new Point(panelWidth + 10, 200);
            quickActionsPanel.Size = new Size(panelWidth, 450);
        }
    }

    private void RefreshTasksLayout()
    {
        if (tasksContent == null) return;

        // Find and resize the filter panel and tasks list panel
        foreach (Control control in tasksContent.Controls)
        {
            if (control is Panel panel)
            {
                if (panel.BorderStyle == BorderStyle.FixedSingle && !panel.AutoScroll)
                {
                    // This is the filter panel
                    panel.Size = new Size(tasksContent.Width - 40, 60);
                }
                else if (panel.BorderStyle == BorderStyle.FixedSingle && panel.AutoScroll)
                {
                    // This is the tasks list panel
                    panel.Size = new Size(tasksContent.Width - 40, tasksContent.Height - 160);
                }
            }
        }
    }

    private void RefreshSearchLayout()
    {
        if (searchContent == null) return;

        // Find and resize the search panel and results panel
        foreach (Control control in searchContent.Controls)
        {
            if (control is Panel panel)
            {
                if (panel.Location.Y == 60) // Search panel
                {
                    panel.Size = new Size(searchContent.Width - 40, 80);
                }
                else if (panel.Location.Y == 160) // Results panel
                {
                    panel.Size = new Size(searchContent.Width - 40, searchContent.Height - 180);
                }
            }
        }
    }

    private void CreateLayout()
    {
        // Top Panel
        topPanel = new Panel
        {
            Height = 80,
            Dock = DockStyle.Top,
            BackColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        // Side Panel
        sidePanel = new Panel
        {
            Width = 250,
            Dock = DockStyle.Left,
            BackColor = Color.FromArgb(52, 73, 94),
            BorderStyle = BorderStyle.None
        };

        // Content Panel - positioned to avoid overlap with sidebar and top panel
        contentPanel = new Panel
        {
            Location = new Point(250, 80), // Start after sidebar and top panel
            Size = new Size(this.Width - 250, this.Height - 80), // Fill remaining space
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(20),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };
    }

    private void CreateTopPanel()
    {
        // Welcome Label
        lblWelcome = new Label
        {
            Text = $"Welcome back, {_currentUser?.Username ?? "User"}!",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(20, 25),
            Size = new Size(400, 30),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Profile Button
        btnProfile = new Button
        {
            Text = "👤 Profile",
            Size = new Size(100, 40),
            Location = new Point(this.Width - 240, 20),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnProfile.FlatAppearance.BorderSize = 0;
        btnProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // Logout Button
        btnLogout = new Button
        {
            Text = "🚪 Logout",
            Size = new Size(100, 40),
            Location = new Point(this.Width - 130, 20),
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnLogout.Click += BtnLogout_Click;

        topPanel.Controls.Add(lblWelcome);
        topPanel.Controls.Add(btnProfile);
        topPanel.Controls.Add(btnLogout);

        // Add bottom border to top panel
        topPanel.Paint += (s, e) =>
        {
            using (var pen = new Pen(Color.FromArgb(236, 240, 241), 2))
            {
                e.Graphics.DrawLine(pen, 0, topPanel.Height - 1, topPanel.Width, topPanel.Height - 1);
            }
        };
    }

    private void CreateSidePanel()
    {
        // Logo/Title
        var lblLogo = new Label
        {
            Text = "📋 TaskMaster",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 30),
            Size = new Size(200, 40),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Navigation Buttons
        btnDashboard = CreateNavButton("🏠 Dashboard", 100);
        btnTasks = CreateNavButton("📝 My Tasks", 160);
        btnAddTask = CreateNavButton("➕ Add Task", 220);
        btnReports = CreateNavButton("📊 Reports", 280);
        btnSearch = CreateNavButton("🔍 Search", 340);

        // Categories button
        var btnCategories = CreateNavButton("📂 Categories", 400);
        btnCategories.Click += BtnCategories_Click;

        // Logout button at the bottom of the side panel
        btnSideLogout = new Button
        {
            Text = "🚪 Logout",
            Size = new Size(210, 50),
            Location = new Point(20, sidePanel.Height - 80), // Position at bottom with some padding
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        btnSideLogout.FlatAppearance.BorderSize = 0;
        btnSideLogout.Click += BtnLogout_Click;

        // Select dashboard by default
        SelectNavButton(btnDashboard);

        // Event handlers
        btnDashboard.Click += (s, e) => ShowContent("dashboard");
        btnTasks.Click += (s, e) => ShowContent("tasks");
        btnAddTask.Click += (s, e) => ShowContent("addtask");
        btnReports.Click += (s, e) => ShowContent("reports");
        btnSearch.Click += (s, e) => ShowContent("search");

        sidePanel.Controls.Add(lblLogo);
        sidePanel.Controls.Add(btnDashboard);
        sidePanel.Controls.Add(btnTasks);
        sidePanel.Controls.Add(btnAddTask);
        sidePanel.Controls.Add(btnReports);
        sidePanel.Controls.Add(btnSearch);
        sidePanel.Controls.Add(btnCategories);
        sidePanel.Controls.Add(btnSideLogout);
    }

    private Button CreateNavButton(string text, int y)
    {
        return new Button
        {
            Text = text,
            Size = new Size(210, 50),
            Location = new Point(20, y),
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(189, 195, 199),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }

    private void SelectNavButton(Button selected)
    {
        // Reset all buttons (except logout button which has its own styling)
        foreach (Control control in sidePanel.Controls)
        {
            if (control is Button btn && btn != selected && btn != btnSideLogout)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(189, 195, 199);
            }
        }

        // Highlight selected (but don't change logout button styling)
        if (selected != btnSideLogout)
        {
            selected.BackColor = Color.FromArgb(44, 62, 80);
            selected.ForeColor = Color.White;
        }
    }

    private void CreateContentPanels()
    {
        // Calculate the available size for content panels
        var availableWidth = contentPanel.Width - 40; // Account for padding
        var availableHeight = contentPanel.Height - 40; // Account for padding

        // Dashboard Content
        dashboardContent = new Panel
        {
            Size = new Size(availableWidth, availableHeight),
            Location = new Point(0, 0),
            BackColor = Color.Transparent
        };
        CreateDashboardContent();

        // Tasks Content
        tasksContent = new Panel
        {
            Size = new Size(availableWidth, availableHeight),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
            Visible = false
        };
        CreateTasksContent();

        // Add Task Content
        addTaskContent = new Panel
        {
            Size = new Size(availableWidth, availableHeight),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
            Visible = false
        };
        CreateAddTaskContent();

        // Reports Content
        reportsContent = new Panel
        {
            Size = new Size(availableWidth, availableHeight),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
            Visible = false
        };
        CreateReportsContent();

        // Search Content
        searchContent = new Panel
        {
            Size = new Size(availableWidth, availableHeight),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
            Visible = false
        };
        CreateSearchContent();

        contentPanel.Controls.Add(dashboardContent);
        contentPanel.Controls.Add(tasksContent);
        contentPanel.Controls.Add(addTaskContent);
        contentPanel.Controls.Add(reportsContent);
        contentPanel.Controls.Add(searchContent);
    }

    private void CreateDashboardContent()
    {
        // Page Title
        var lblTitle = new Label
        {
            Text = "Dashboard Overview",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(0, 0),
            Size = new Size(400, 40)
        };

        // Statistics Panel
        statsPanel = new Panel
        {
            Location = new Point(0, 60),
            Size = new Size(contentPanel.Width - 40, 120), // Use contentPanel width minus padding
            BackColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        _ = CreateStatsCards(); // Fire and forget for initial load

        // Recent Tasks Panel
        recentTasksPanel = new Panel
        {
            Location = new Point(0, 200),
            Size = new Size((contentPanel.Width - 60) / 2, 400), // Use contentPanel width minus padding, divided by 2
            BackColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        CreateRecentTasksPanel();

        // Quick Actions Panel
        quickActionsPanel = new Panel
        {
            Location = new Point((contentPanel.Width - 40) / 2 + 10, 200), // Position next to recent tasks panel
            Size = new Size((contentPanel.Width - 60) / 2, 400),
            BackColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        CreateQuickActionsPanel();

        dashboardContent.Controls.Add(lblTitle);
        dashboardContent.Controls.Add(statsPanel);
        dashboardContent.Controls.Add(recentTasksPanel);
        dashboardContent.Controls.Add(quickActionsPanel);
    }

    private async System.Threading.Tasks.Task CreateStatsCards()
    {
        try
        {
            var userTasks = await _taskService.GetUserTasksAsync(SessionManager.Instance.UserId);
            var tasks = userTasks.ToList();

            var totalTasks = tasks.Count;
            var pendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending);
            var inProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress);
            var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);

            // Get category count
            var categories = _categoryService.GetAllCategories();
            var totalCategories = categories?.Count() ?? 0;

            CreateStatCard("Total Tasks", totalTasks.ToString(), Color.FromArgb(52, 152, 219), 0);
            CreateStatCard("Pending", pendingTasks.ToString(), Color.FromArgb(230, 126, 34), 200);
            CreateStatCard("In Progress", inProgressTasks.ToString(), Color.FromArgb(241, 196, 15), 400);
            CreateStatCard("Completed", completedTasks.ToString(), Color.FromArgb(46, 204, 113), 600);
            CreateStatCard("Categories", totalCategories.ToString(), Color.FromArgb(142, 68, 173), 800);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateStatCard(string title, string value, Color color, int x)
    {
        var card = new Panel
        {
            Location = new Point(x, 0),
            Size = new Size(180, 120),
            BackColor = color,
            BorderStyle = BorderStyle.None,
            Cursor = title == "Categories" ? Cursors.Hand : Cursors.Default
        };

        // Add click event for Categories card
        if (title == "Categories")
        {
            card.Click += (s, e) => BtnCategories_Click(s, e);
        }

        var lblValue = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 20),
            Size = new Size(140, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = title == "Categories" ? Cursors.Hand : Cursors.Default
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.White,
            Location = new Point(20, 70),
            Size = new Size(140, 20),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = title == "Categories" ? Cursors.Hand : Cursors.Default
        };

        // Add click events for labels too when it's Categories card
        if (title == "Categories")
        {
            lblValue.Click += (s, e) => BtnCategories_Click(s, e);
            lblTitle.Click += (s, e) => BtnCategories_Click(s, e);
        }

        card.Controls.Add(lblValue);
        card.Controls.Add(lblTitle);
        statsPanel.Controls.Add(card);
    }

    private void CreateRecentTasksPanel()
    {
        var lblTitle = new Label
        {
            Text = "Recent Tasks",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(20, 20),
            Size = new Size(200, 30)
        };

        var taskList = new ListBox
        {
            Location = new Point(20, 60),
            Size = new Size(recentTasksPanel.Width - 40, 320),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(248, 249, 250)
        };

        // Load recent tasks
        var recentTasks = _currentUser != null
            ? _taskService.GetUserTasks(_currentUser.Id)?.OrderByDescending(t => t.CreatedAt).Take(10).ToList() ?? new List<TaskItem>()
            : new List<TaskItem>();
        foreach (var task in recentTasks)
        {
            taskList.Items.Add($"{task.Title} - {task.Status}");
        }

        recentTasksPanel.Controls.Add(lblTitle);
        recentTasksPanel.Controls.Add(taskList);
    }

    private void CreateQuickActionsPanel()
    {
        var lblTitle = new Label
        {
            Text = "Quick Actions",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(20, 20),
            Size = new Size(200, 30)
        };

        var btnQuickAdd = CreateActionButton("➕ Quick Add Task", 60, Color.FromArgb(46, 204, 113));
        btnQuickAdd.Click += (s, e) => ShowContent("addtask");

        var btnViewAll = CreateActionButton("📝 View All Tasks", 120, Color.FromArgb(52, 152, 219));
        btnViewAll.Click += (s, e) => ShowContent("tasks");

        var btnGenReport = CreateActionButton("📊 Generate Report", 180, Color.FromArgb(155, 89, 182));
        btnGenReport.Click += (s, e) => ShowContent("reports");

        var btnSearchAction = CreateActionButton("🔍 Search Tasks", 240, Color.FromArgb(230, 126, 34));
        btnSearchAction.Click += (s, e) => ShowContent("search");

        var btnManageCategories = CreateActionButton("📂 Manage Categories", 300, Color.FromArgb(142, 68, 173));
        btnManageCategories.Click += BtnCategories_Click;

        quickActionsPanel.Controls.Add(lblTitle);
        quickActionsPanel.Controls.Add(btnQuickAdd);
        quickActionsPanel.Controls.Add(btnViewAll);
        quickActionsPanel.Controls.Add(btnGenReport);
        quickActionsPanel.Controls.Add(btnSearchAction);
        quickActionsPanel.Controls.Add(btnManageCategories);
    }

    private Button CreateActionButton(string text, int y, Color color)
    {
        return new Button
        {
            Text = text,
            Location = new Point(20, y),
            Size = new Size(quickActionsPanel.Width - 40, 50),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }

    private void CreateTasksContent()
    {
        // Header with more space
        var lblTitle = new Label
        {
            Text = "My Tasks",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(20, 20),
            Size = new Size(200, 35) // Increased height
        };

        // Filter Panel - increased height and moved down more for better spacing
        var filterPanel = new Panel
        {
            Location = new Point(20, 70),
            Size = new Size(Math.Max(600, tasksContent.Width - 40), 80), // Increased height from 70 to 80
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var cmbStatus = new ComboBox
        {
            Location = new Point(10, 25), // Moved down from 20 to 25
            Size = new Size(120, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbStatus.Items.AddRange(new object[] { "All", "Pending", "In Progress", "Completed" });
        cmbStatus.SelectedIndex = 0;

        var cmbPriority = new ComboBox
        {
            Location = new Point(150, 25), // Moved down from 20 to 25
            Size = new Size(120, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbPriority.Items.AddRange(new object[] { "All", "Low", "Medium", "High" });
        cmbPriority.SelectedIndex = 0;

        var cmbCategoryFilter = new ComboBox
        {
            Location = new Point(290, 25), // Moved down from 20 to 25
            Size = new Size(130, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        LoadCategoryFilterComboBox(cmbCategoryFilter);

        var btnRefresh = new Button
        {
            Text = "🔄 Refresh",
            Location = new Point(440, 25), // Moved down from 20 to 25
            Size = new Size(100, 30),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;

        filterPanel.Controls.Add(cmbStatus);
        filterPanel.Controls.Add(cmbPriority);
        filterPanel.Controls.Add(cmbCategoryFilter);
        filterPanel.Controls.Add(btnRefresh);

        // Tasks List Panel - adjusted position for increased filter panel height
        var tasksListPanel = new Panel
        {
            Location = new Point(20, 165), // Moved down from 155 to 165
            Size = new Size(Math.Max(600, tasksContent.Width - 40), Math.Max(400, tasksContent.Height - 185)), // Adjusted height calculation
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        var lblNoTasks = new Label
        {
            Text = "Loading tasks...",
            Font = new Font("Segoe UI", 14),
            ForeColor = Color.Gray,
            Location = new Point(50, 50),
            Size = new Size(200, 30)
        };
        tasksListPanel.Controls.Add(lblNoTasks);

        tasksContent.Controls.Add(lblTitle);
        tasksContent.Controls.Add(filterPanel);
        tasksContent.Controls.Add(tasksListPanel);

        // Load tasks asynchronously
        btnRefresh.Click += async (s, e) => await LoadUserTasks(tasksListPanel, cmbStatus.Text, cmbPriority.Text, GetCategoryFilterValue(cmbCategoryFilter));
        _ = LoadUserTasks(tasksListPanel, "All", "All", null); // Initial load
    }

    private void LoadCategoryFilterComboBox(ComboBox cmbCategoryFilter)
    {
        try
        {
            var categories = _categoryService.GetAllCategories();
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add("All Categories");
            cmbCategoryFilter.Items.Add("No Category");

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    cmbCategoryFilter.Items.Add(new CategoryItem { Id = category.Id, Name = category.Name });
                }
            }

            cmbCategoryFilter.SelectedIndex = 0; // Default to "All Categories"
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add("All Categories");
            cmbCategoryFilter.SelectedIndex = 0;
        }
    }

    private static Guid? GetCategoryFilterValue(ComboBox cmbCategoryFilter)
    {
        if (cmbCategoryFilter.SelectedItem is CategoryItem categoryItem)
        {
            return categoryItem.Id;
        }
        return cmbCategoryFilter.SelectedIndex == 1 ? Guid.Empty : null; // Guid.Empty represents "No Category"
    }

    private async System.Threading.Tasks.Task LoadUserTasks(Panel container, string statusFilter, string priorityFilter, Guid? categoryFilter)
    {
        try
        {
            container.Controls.Clear();

            var loadingLabel = new Label
            {
                Text = "Loading tasks...",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Gray,
                Location = new Point(20, 20),
                Size = new Size(200, 30)
            };
            container.Controls.Add(loadingLabel);

            var userTasks = await _taskService.GetUserTasksAsync(SessionManager.Instance.UserId);
            var tasks = userTasks.ToList();

            // Apply filters
            if (statusFilter != "All")
            {
                var status = Enum.Parse<TaskStatus>(statusFilter.Replace(" ", ""));
                tasks = tasks.Where(t => t.Status == status).ToList();
            }

            if (priorityFilter != "All")
            {
                var priority = Enum.Parse<TaskPriority>(priorityFilter);
                tasks = tasks.Where(t => t.Priority == priority).ToList();
            }

            // Apply category filter
            if (categoryFilter.HasValue)
            {
                if (categoryFilter.Value == Guid.Empty) // "No Category" selected
                {
                    tasks = tasks.Where(t => !t.CategoryId.HasValue).ToList();
                }
                else // Specific category selected
                {
                    tasks = tasks.Where(t => t.CategoryId == categoryFilter.Value).ToList();
                }
            }

            container.Controls.Clear();

            if (!tasks.Any())
            {
                var noTasksLabel = new Label
                {
                    Text = "No tasks found matching the selected filters.",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    Location = new Point(20, 20),
                    Size = new Size(400, 30)
                };
                container.Controls.Add(noTasksLabel);
                return;
            }

            int yPos = 10;
            foreach (var task in tasks.OrderBy(t => t.DueDate))
            {
                var taskCard = CreateTaskCard(task, yPos);
                container.Controls.Add(taskCard);
                yPos += taskCard.Height + 10;
            }
        }
        catch (Exception ex)
        {
            container.Controls.Clear();
            var errorLabel = new Label
            {
                Text = $"Error loading tasks: {ex.Message}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Red,
                Location = new Point(20, 20),
                Size = new Size(400, 60)
            };
            container.Controls.Add(errorLabel);
        }
    }

    private Panel CreateTaskCard(TaskItem task, int yPos)
    {
        var cardWidth = Math.Max(600, contentPanel.Width - 120); // Responsive width
        var card = new Panel
        {
            Location = new Point(10, yPos),
            Size = new Size(cardWidth, 110), // Increased height from 100 to 110
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Priority color indicator - adjusted height for taller card
        var priorityIndicator = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(5, card.Height), // Height will now be 110
            BackColor = GetPriorityColor(task.Priority)
        };

        // Task title
        var lblTitle = new Label
        {
            Text = task.Title,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(15, 10),
            Size = new Size(cardWidth - 150, 25), // Responsive width
            ForeColor = Color.FromArgb(44, 62, 80)
        };

        // Task description
        var lblDescription = new Label
        {
            Text = task.Description ?? "No description",
            Font = new Font("Segoe UI", 10),
            Location = new Point(15, 35),
            Size = new Size(cardWidth - 150, 20), // Responsive width
            ForeColor = Color.Gray
        };

        // Due date
        var lblDueDate = new Label
        {
            Text = task.DueDate?.ToString("MMM dd, yyyy") ?? "No due date",
            Font = new Font("Segoe UI", 9),
            Location = new Point(15, 60),
            Size = new Size(150, 20),
            ForeColor = task.DueDate < DateTime.Now ? Color.Red : Color.FromArgb(52, 152, 219)
        };

        // Category display - moved down to better align with other elements
        var lblCategory = new Label
        {
            Text = GetCategoryDisplayName(task.CategoryId),
            Font = new Font("Segoe UI", 9),
            Location = new Point(170, 75), // Moved down from 60 to 75
            Size = new Size(120, 20),
            ForeColor = Color.FromArgb(142, 68, 173),
            BackColor = Color.FromArgb(245, 245, 245),
            TextAlign = ContentAlignment.MiddleCenter,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Status dropdown for quick edit
        var cmbStatus = new ComboBox
        {
            Location = new Point(cardWidth - 120, 15),
            Size = new Size(100, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        cmbStatus.Items.AddRange(new object[] { "Pending", "In Progress", "Completed" });
        cmbStatus.SelectedItem = task.Status.ToString().Replace("InProgress", "In Progress");
        cmbStatus.SelectedIndexChanged += async (s, e) => await UpdateTaskStatus(task, cmbStatus.SelectedItem?.ToString() ?? "");

        // Priority dropdown for quick edit
        var cmbPriority = new ComboBox
        {
            Location = new Point(cardWidth - 120, 45),
            Size = new Size(100, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
        cmbPriority.SelectedItem = task.Priority.ToString();
        cmbPriority.SelectedIndexChanged += async (s, e) => await UpdateTaskPriority(task, cmbPriority.SelectedItem?.ToString() ?? "");

        // Category dropdown for quick edit - moved down to avoid overlap with priority
        var cmbTaskCategory = new ComboBox
        {
            Location = new Point(cardWidth - 240, 70), // Moved down from 45 to 70
            Size = new Size(110, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        LoadTaskCategoryComboBox(cmbTaskCategory, task.CategoryId);
        cmbTaskCategory.SelectedIndexChanged += async (s, e) => await UpdateTaskCategory(task, cmbTaskCategory);

        // Delete button
        var btnDelete = new Button
        {
            Text = "🗑️",
            Location = new Point(cardWidth - 120, 75),
            Size = new Size(50, 20),
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8)
        };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += async (s, e) => await DeleteTask(task);

        card.Controls.Add(priorityIndicator);
        card.Controls.Add(lblTitle);
        card.Controls.Add(lblDescription);
        card.Controls.Add(lblDueDate);
        card.Controls.Add(lblCategory);
        card.Controls.Add(cmbStatus);
        card.Controls.Add(cmbPriority);
        card.Controls.Add(cmbTaskCategory);
        card.Controls.Add(btnDelete);

        return card;
    }

    private string GetCategoryDisplayName(Guid? categoryId)
    {
        if (!categoryId.HasValue)
            return "No Category";

        try
        {
            var category = _categoryService.GetCategoryById(categoryId.Value);
            return category?.Name ?? "Unknown Category";
        }
        catch
        {
            return "Unknown Category";
        }
    }

    private void LoadTaskCategoryComboBox(ComboBox cmbTaskCategory, Guid? currentCategoryId)
    {
        try
        {
            var categories = _categoryService.GetAllCategories();
            cmbTaskCategory.Items.Clear();
            cmbTaskCategory.Items.Add("No Category");

            var selectedIndex = 0;

            if (categories != null)
            {
                var index = 1;
                foreach (var category in categories)
                {
                    cmbTaskCategory.Items.Add(new CategoryItem { Id = category.Id, Name = category.Name });
                    if (currentCategoryId.HasValue && category.Id == currentCategoryId.Value)
                    {
                        selectedIndex = index;
                    }
                    index++;
                }
            }

            cmbTaskCategory.SelectedIndex = selectedIndex;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbTaskCategory.Items.Clear();
            cmbTaskCategory.Items.Add("No Category");
            cmbTaskCategory.SelectedIndex = 0;
        }
    }

    private async System.Threading.Tasks.Task UpdateTaskCategory(TaskItem task, ComboBox cmbTaskCategory)
    {
        try
        {
            Guid? newCategoryId = null;
            if (cmbTaskCategory.SelectedItem is CategoryItem categoryItem)
            {
                newCategoryId = categoryItem.Id;
            }

            task.CategoryId = newCategoryId;
            await _taskService.UpdateTaskAsync(task);

            // Refresh the current view
            RefreshCurrentTaskView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating task category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnCategories_Click(object? sender, EventArgs e)
    {
        try
        {
            var categoryForm = Program.ServiceProvider.GetRequiredService<CategoryManagementForm>();
            categoryForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening category management: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowQuickCategoryDialog(ComboBox cmbCategory)
    {
        try
        {
            // Simple input dialog for quick category creation
            var inputDialog = new QuickCategoryInputDialog();
            var result = inputDialog.ShowDialog(this);

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(inputDialog.CategoryName))
            {
                var newCategory = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = inputDialog.CategoryName.Trim()
                };

                _categoryService.AddCategory(newCategory);

                // Refresh the combobox
                LoadCategoriesIntoComboBox(cmbCategory);

                // Select the newly created category
                foreach (var item in cmbCategory.Items)
                {
                    if (item is CategoryItem categoryItem && categoryItem.Name == newCategory.Name)
                    {
                        cmbCategory.SelectedItem = item;
                        break;
                    }
                }

                MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Quick input dialog for category creation
    private sealed class QuickCategoryInputDialog : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CategoryName { get; private set; } = string.Empty;

        public QuickCategoryInputDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 150);
            this.Text = "Add New Category";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var lblPrompt = new Label
            {
                Text = "Category Name:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 12)
            };

            var txtCategoryName = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11)
            };

            var btnOK = new Button
            {
                Text = "OK",
                Location = new Point(160, 90),
                Size = new Size(70, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnOK.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, 90),
                Size = new Size(70, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnOK.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    CategoryName = txtCategoryName.Text.Trim();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                }
            };

            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lblPrompt);
            this.Controls.Add(txtCategoryName);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            txtCategoryName.Focus();
        }
    }

    private async System.Threading.Tasks.Task UpdateTaskStatus(TaskItem task, string newStatus)
    {
        try
        {
            var statusEnum = newStatus.Replace(" ", "") switch
            {
                "Pending" => TaskStatus.Pending,
                "InProgress" => TaskStatus.InProgress,
                "Completed" => TaskStatus.Completed,
                _ => task.Status
            };

            task.Status = statusEnum;
            await _taskService.UpdateTaskAsync(task);

            // Refresh the current view
            RefreshCurrentTaskView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating task status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async System.Threading.Tasks.Task UpdateTaskPriority(TaskItem task, string newPriority)
    {
        try
        {
            var priorityEnum = newPriority switch
            {
                "Low" => TaskPriority.Low,
                "Medium" => TaskPriority.Medium,
                "High" => TaskPriority.High,
                _ => task.Priority
            };

            task.Priority = priorityEnum;
            await _taskService.UpdateTaskAsync(task);

            // Refresh the current view
            RefreshCurrentTaskView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating task priority: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async System.Threading.Tasks.Task DeleteTask(TaskItem task)
    {
        try
        {
            var result = MessageBox.Show($"Are you sure you want to delete the task '{task.Title}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await _taskService.DeleteTaskAsync(task.Id);
                RefreshCurrentTaskView();
                MessageBox.Show("Task deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshCurrentTaskView()
    {
        // Refresh both dashboard statistics and task list if currently visible
        if (dashboardContent != null && dashboardContent.Visible)
        {
            _ = LoadDashboard();
        }

        if (tasksContent != null && tasksContent.Visible)
        {
            // Find the tasks list panel and reload it
            foreach (Control control in tasksContent.Controls)
            {
                if (control is Panel panel && panel.BorderStyle == BorderStyle.FixedSingle && panel.AutoScroll)
                {
                    _ = LoadUserTasks(panel, "All", "All", null);
                    break;
                }
            }
        }
    }

    private static Color GetPriorityColor(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.High => Color.FromArgb(231, 76, 60),
            TaskPriority.Medium => Color.FromArgb(241, 196, 15),
            TaskPriority.Low => Color.FromArgb(46, 204, 113),
            _ => Color.Gray
        };
    }

    private void CreateAddTaskContent()
    {
        // Header with more space
        var lblTitle = new Label
        {
            Text = "Add New Task",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(50, 30),
            Size = new Size(250, 35) // Increased height from 30 to 35
        };

        // Form Panel - adjusted height for the more compact layout
        var formPanel = new Panel
        {
            Location = new Point(50, 80),
            Size = new Size(600, 420), // Reduced from 520 to 420
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Task Title
        var lblTaskTitle = new Label
        {
            Text = "Task Title *",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(30, 30),
            Size = new Size(100, 25)
        };

        var txtTitle = new TextBox
        {
            Location = new Point(30, 55),
            Size = new Size(540, 30),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Description - reduced height to make more room
        var lblDescription = new Label
        {
            Text = "Description",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(30, 100),
            Size = new Size(100, 25)
        };

        var txtDescription = new TextBox
        {
            Location = new Point(30, 125),
            Size = new Size(540, 60), // Reduced from 80 to 60
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        // Priority - moved up
        var lblPriority = new Label
        {
            Text = "Priority *",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(30, 200), // Moved up from 220 to 200
            Size = new Size(100, 25)
        };

        var cmbPriority = new ComboBox
        {
            Location = new Point(30, 225), // Moved up from 245 to 225
            Size = new Size(150, 30),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11)
        };
        cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
        cmbPriority.SelectedIndex = 1; // Default to Medium

        // Category - moved to second row, better positioned
        var lblCategory = new Label
        {
            Text = "Category",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(200, 200), // Moved left and up
            Size = new Size(100, 25)
        };

        var cmbCategory = new ComboBox
        {
            Location = new Point(200, 225), // Moved left and up
            Size = new Size(150, 30), // Made wider for better usability
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11)
        };

        // Add Category button (small + button) - positioned right next to category dropdown
        var btnAddCategory = new Button
        {
            Text = "+",
            Location = new Point(355, 225), // Positioned right after category dropdown
            Size = new Size(25, 30),
            BackColor = Color.FromArgb(142, 68, 173),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAddCategory.FlatAppearance.BorderSize = 0;
        btnAddCategory.Click += (s, e) => ShowQuickCategoryDialog(cmbCategory);

        // Load categories
        LoadCategoriesIntoComboBox(cmbCategory);

        // Due Date - moved up to accommodate the layout changes
        var lblDueDate = new Label
        {
            Text = "Due Date",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(30, 270), // Moved up from 280 to 270
            Size = new Size(100, 25)
        };

        var dtpDueDate = new DateTimePicker
        {
            Location = new Point(30, 295), // Moved up from 305 to 295
            Size = new Size(200, 30),
            Font = new Font("Segoe UI", 11),
            Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today
        };

        var chkNoDueDate = new CheckBox
        {
            Text = "No due date",
            Location = new Point(250, 298), // Moved up from 308 to 298
            Size = new Size(120, 25),
            Font = new Font("Segoe UI", 10)
        };

        chkNoDueDate.CheckedChanged += (s, e) => dtpDueDate.Enabled = !chkNoDueDate.Checked;

        // Buttons - moved up to fit the new layout
        var btnSave = new Button
        {
            Text = "💾 Save Task",
            Location = new Point(30, 340), // Moved up from 360 to 340
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;

        var btnClear = new Button
        {
            Text = "🗑️ Clear",
            Location = new Point(200, 340), // Moved up from 360 to 340
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnClear.FlatAppearance.BorderSize = 0;

        // Event handlers
        btnSave.Click += async (s, e) => await SaveNewTask(txtTitle, txtDescription, cmbPriority, cmbCategory, dtpDueDate, chkNoDueDate);
        btnClear.Click += (s, e) => ClearAddTaskForm(txtTitle, txtDescription, cmbPriority, cmbCategory, dtpDueDate, chkNoDueDate);

        formPanel.Controls.Add(lblTaskTitle);
        formPanel.Controls.Add(txtTitle);
        formPanel.Controls.Add(lblDescription);
        formPanel.Controls.Add(txtDescription);
        formPanel.Controls.Add(lblPriority);
        formPanel.Controls.Add(cmbPriority);
        formPanel.Controls.Add(lblCategory);
        formPanel.Controls.Add(cmbCategory);
        formPanel.Controls.Add(btnAddCategory);
        formPanel.Controls.Add(lblDueDate);
        formPanel.Controls.Add(dtpDueDate);
        formPanel.Controls.Add(chkNoDueDate);
        formPanel.Controls.Add(btnSave);
        formPanel.Controls.Add(btnClear);

        addTaskContent.Controls.Add(lblTitle);
        addTaskContent.Controls.Add(formPanel);
    }

    private void LoadCategoriesIntoComboBox(ComboBox cmbCategory)
    {
        try
        {
            var categories = _categoryService.GetAllCategories();
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("No Category");

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    cmbCategory.Items.Add(new CategoryItem { Id = category.Id, Name = category.Name });
                }
            }

            cmbCategory.SelectedIndex = 0; // Default to "No Category"
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("No Category");
            cmbCategory.SelectedIndex = 0;
        }
    }

    // Helper class for category ComboBox items
    private class CategoryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }

    private async System.Threading.Tasks.Task SaveNewTask(TextBox txtTitle, TextBox txtDescription, ComboBox cmbPriority, ComboBox cmbCategory, DateTimePicker dtpDueDate, CheckBox chkNoDueDate)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Task title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            // Determine category ID
            Guid? categoryId = null;
            if (cmbCategory.SelectedItem is CategoryItem categoryItem)
            {
                categoryId = categoryItem.Id;
            }

            var newTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = txtTitle.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                Priority = Enum.Parse<TaskPriority>(cmbPriority.Text),
                Status = TaskStatus.Pending,
                CategoryId = categoryId,
                DueDate = chkNoDueDate.Checked ? null : DateTime.SpecifyKind(dtpDueDate.Value, DateTimeKind.Local).ToUniversalTime(),
                CreatedAt = DateTime.UtcNow,
                UserId = SessionManager.Instance.UserId
            };

            await _taskService.AddTaskAsync(newTask);

            MessageBox.Show("Task created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearAddTaskForm(txtTitle, txtDescription, cmbPriority, cmbCategory, dtpDueDate, chkNoDueDate);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ClearAddTaskForm(TextBox txtTitle, TextBox txtDescription, ComboBox cmbPriority, ComboBox cmbCategory, DateTimePicker dtpDueDate, CheckBox chkNoDueDate)
    {
        txtTitle.Clear();
        txtDescription.Clear();
        cmbPriority.SelectedIndex = 1; // Medium
        cmbCategory.SelectedIndex = 0; // No Category
        dtpDueDate.Value = DateTime.Today;
        chkNoDueDate.Checked = false;
        dtpDueDate.Enabled = true;
    }

    private void CreateReportsContent()
    {
        // Header with more space
        var lblTitle = new Label
        {
            Text = "Task Reports",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(50, 30),
            Size = new Size(250, 35) // Increased height from 30 to 35
        };

        // Report Options Panel - moved down to give more space after title
        var optionsPanel = new Panel
        {
            Location = new Point(50, 80), // Keeping same position as it was reasonable
            Size = new Size(700, 150),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblReportType = new Label
        {
            Text = "Select Report Type:",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 20),
            Size = new Size(150, 25)
        };

        var rbTaskSummary = new RadioButton
        {
            Text = "📊 Task Summary Report",
            Location = new Point(20, 60),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 11),
            Checked = true
        };

        var rbProductivity = new RadioButton
        {
            Text = "📈 Productivity Report",
            Location = new Point(250, 60),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 11)
        };

        var rbOverdue = new RadioButton
        {
            Text = "⚠️ Overdue Tasks Report",
            Location = new Point(480, 60),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 11)
        };

        var btnGenerate = new Button
        {
            Text = "📋 Generate Report",
            Location = new Point(20, 100),
            Size = new Size(180, 35),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnGenerate.FlatAppearance.BorderSize = 0;

        var btnExport = new Button
        {
            Text = "💾 Export to File",
            Location = new Point(220, 100),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnExport.FlatAppearance.BorderSize = 0;

        // Report Display Panel
        var reportPanel = new Panel
        {
            Location = new Point(50, 250),
            Size = new Size(700, 300),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        var lblReportContent = new Label
        {
            Text = "Click 'Generate Report' to view task statistics and analytics.",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.Gray,
            Location = new Point(20, 20),
            Size = new Size(400, 30)
        };
        reportPanel.Controls.Add(lblReportContent);

        // Event handlers
        btnGenerate.Click += async (s, e) => await GenerateReport(reportPanel, rbTaskSummary.Checked, rbProductivity.Checked, rbOverdue.Checked);
        btnExport.Click += (s, e) => ExportReport();

        optionsPanel.Controls.Add(lblReportType);
        optionsPanel.Controls.Add(rbTaskSummary);
        optionsPanel.Controls.Add(rbProductivity);
        optionsPanel.Controls.Add(rbOverdue);
        optionsPanel.Controls.Add(btnGenerate);
        optionsPanel.Controls.Add(btnExport);

        reportsContent.Controls.Add(lblTitle);
        reportsContent.Controls.Add(optionsPanel);
        reportsContent.Controls.Add(reportPanel);
    }

    private async System.Threading.Tasks.Task GenerateReport(Panel reportPanel, bool taskSummary, bool productivity, bool overdue)
    {
        try
        {
            reportPanel.Controls.Clear();

            var loadingLabel = new Label
            {
                Text = "Generating report...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 20),
                Size = new Size(200, 30)
            };
            reportPanel.Controls.Add(loadingLabel);

            var userTasks = await _taskService.GetUserTasksAsync(SessionManager.Instance.UserId);
            var tasks = userTasks.ToList();

            reportPanel.Controls.Clear();

            int yPos = 20;

            if (taskSummary)
            {
                CreateTaskSummaryReport(reportPanel, tasks, yPos);
            }
            else if (productivity)
            {
                CreateProductivityReport(reportPanel, tasks, yPos);
            }
            else if (overdue)
            {
                CreateOverdueReport(reportPanel, tasks, yPos);
            }
        }
        catch (Exception ex)
        {
            reportPanel.Controls.Clear();
            var errorLabel = new Label
            {
                Text = $"Error generating report: {ex.Message}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Red,
                Location = new Point(20, 20),
                Size = new Size(400, 60)
            };
            reportPanel.Controls.Add(errorLabel);
        }
    }

    private static void CreateTaskSummaryReport(Panel panel, List<TaskItem> tasks, int yPos)
    {
        var lblTitle = new Label
        {
            Text = "📊 Task Summary Report",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, yPos),
            Size = new Size(300, 30),
            ForeColor = Color.FromArgb(44, 62, 80)
        };
        panel.Controls.Add(lblTitle);
        yPos += 40;

        var totalTasks = tasks.Count;
        var pendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending);
        var inProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);

        var stats = new[]
        {
            $"Total Tasks: {totalTasks}",
            $"Pending: {pendingTasks} ({(totalTasks > 0 ? pendingTasks * 100 / totalTasks : 0)}%)",
            $"In Progress: {inProgressTasks} ({(totalTasks > 0 ? inProgressTasks * 100 / totalTasks : 0)}%)",
            $"Completed: {completedTasks} ({(totalTasks > 0 ? completedTasks * 100 / totalTasks : 0)}%)"
        };

        foreach (var stat in stats)
        {
            var lblStat = new Label
            {
                Text = stat,
                Font = new Font("Segoe UI", 12),
                Location = new Point(40, yPos),
                Size = new Size(400, 25)
            };
            panel.Controls.Add(lblStat);
            yPos += 30;
        }
    }

    private static void CreateProductivityReport(Panel panel, List<TaskItem> tasks, int yPos)
    {
        var lblTitle = new Label
        {
            Text = "📈 Productivity Report",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, yPos),
            Size = new Size(300, 30),
            ForeColor = Color.FromArgb(44, 62, 80)
        };
        panel.Controls.Add(lblTitle);
        yPos += 40;

        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);
        var completedThisWeek = tasks.Count(t => t.Status == TaskStatus.Completed &&
            t.CreatedAt >= DateTime.Now.AddDays(-7));

        var avgCompletionTime = tasks.Where(t => t.Status == TaskStatus.Completed && t.DueDate.HasValue)
            .Select(t => t.DueDate!.Value.Subtract(t.CreatedAt).Days)
            .DefaultIfEmpty(0)
            .Average();

        var productivityStats = new[]
        {
            $"Tasks Completed: {completedTasks}",
            $"Completed This Week: {completedThisWeek}",
            $"Average Completion Time: {avgCompletionTime:F1} days",
            $"Productivity Score: {(completedTasks > 0 ? "Good" : "Needs Improvement")}"
        };

        foreach (var stat in productivityStats)
        {
            var lblStat = new Label
            {
                Text = stat,
                Font = new Font("Segoe UI", 12),
                Location = new Point(40, yPos),
                Size = new Size(400, 25)
            };
            panel.Controls.Add(lblStat);
            yPos += 30;
        }
    }

    private static void CreateOverdueReport(Panel panel, List<TaskItem> tasks, int yPos)
    {
        var lblTitle = new Label
        {
            Text = "⚠️ Overdue Tasks Report",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, yPos),
            Size = new Size(300, 30),
            ForeColor = Color.FromArgb(231, 76, 60)
        };
        panel.Controls.Add(lblTitle);
        yPos += 40;

        var overdueTasks = tasks.Where(t => t.Status != TaskStatus.Completed &&
            t.DueDate.HasValue && t.DueDate < DateTime.Now).ToList();

        if (!overdueTasks.Any())
        {
            var lblNoOverdue = new Label
            {
                Text = "🎉 Great! No overdue tasks found.",
                Font = new Font("Segoe UI", 12),
                Location = new Point(40, yPos),
                Size = new Size(300, 25),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            panel.Controls.Add(lblNoOverdue);
            return;
        }

        foreach (var task in overdueTasks.OrderBy(t => t.DueDate))
        {
            var daysPastDue = (DateTime.Now - task.DueDate!.Value).Days;
            var lblTask = new Label
            {
                Text = $"• {task.Title} - {daysPastDue} day(s) overdue",
                Font = new Font("Segoe UI", 11),
                Location = new Point(40, yPos),
                Size = new Size(500, 25),
                ForeColor = Color.FromArgb(231, 76, 60)
            };
            panel.Controls.Add(lblTask);
            yPos += 25;
        }
    }

    private static void ExportReport()
    {
        MessageBox.Show("Export functionality will be implemented in the next version.",
            "Export Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CreateSearchContent()
    {
        // Header with more space
        var lblTitle = new Label
        {
            Text = "Search Tasks",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(20, 20),
            Size = new Size(250, 35) // Increased height from 30 to 35
        };

        // Search Panel - moved down to give more space after title
        var searchPanel = new Panel
        {
            Location = new Point(20, 70), // Moved down from 60 to 70
            Size = new Size(Math.Max(600, searchContent.Width - 40), 80),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblSearch = new Label
        {
            Text = "Search Term:",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(10, 15),
            Size = new Size(100, 25)
        };

        var txtSearch = new TextBox
        {
            Location = new Point(120, 15),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 11),
            PlaceholderText = "Enter task title or description..."
        };

        var btnSearchTasks = new Button
        {
            Text = "🔍 Search",
            Location = new Point(340, 15),
            Size = new Size(100, 30),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnSearchTasks.FlatAppearance.BorderSize = 0;

        var btnClear = new Button
        {
            Text = "🗑️ Clear",
            Location = new Point(450, 15),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnClear.FlatAppearance.BorderSize = 0;

        searchPanel.Controls.Add(lblSearch);
        searchPanel.Controls.Add(txtSearch);
        searchPanel.Controls.Add(btnSearchTasks);
        searchPanel.Controls.Add(btnClear);

        // Results Panel - adjusted position for increased search panel location
        var resultsPanel = new Panel
        {
            Location = new Point(20, 170), // Moved down from 160 to 170
            Size = new Size(Math.Max(600, searchContent.Width - 40), Math.Max(400, searchContent.Height - 190)), // Adjusted height calculation
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        var lblInstructions = new Label
        {
            Text = "Enter a search term and click 'Search' to find tasks.",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.Gray,
            Location = new Point(20, 20),
            Size = new Size(400, 30)
        };
        resultsPanel.Controls.Add(lblInstructions);

        // Event handlers
        btnSearchTasks.Click += async (s, e) => await SearchTasks(txtSearch.Text, resultsPanel);
        btnClear.Click += (s, e) =>
        {
            txtSearch.Clear();
            resultsPanel.Controls.Clear();
            resultsPanel.Controls.Add(lblInstructions);
        };

        txtSearch.KeyPress += async (s, e) =>
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                await SearchTasks(txtSearch.Text, resultsPanel);
            }
        };

        searchContent.Controls.Add(lblTitle);
        searchContent.Controls.Add(searchPanel);
        searchContent.Controls.Add(resultsPanel);
    }

    private async System.Threading.Tasks.Task SearchTasks(string searchTerm, Panel resultsPanel)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            resultsPanel.Controls.Clear();
            var loadingLabel = new Label
            {
                Text = "Searching tasks...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 20),
                Size = new Size(200, 30)
            };
            resultsPanel.Controls.Add(loadingLabel);

            var userTasks = await _taskService.GetUserTasksAsync(SessionManager.Instance.UserId);
            var searchResults = userTasks.Where(t =>
                (t.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            resultsPanel.Controls.Clear();

            if (!searchResults.Any())
            {
                var noResultsLabel = new Label
                {
                    Text = $"No tasks found matching '{searchTerm}'.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    Location = new Point(20, 20),
                    Size = new Size(400, 30)
                };
                resultsPanel.Controls.Add(noResultsLabel);
                return;
            }

            var resultCountLabel = new Label
            {
                Text = $"Found {searchResults.Count} task(s) matching '{searchTerm}':",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 10),
                Size = new Size(400, 25)
            };
            resultsPanel.Controls.Add(resultCountLabel);

            int yPos = 45;
            foreach (var task in searchResults.OrderBy(t => t.DueDate))
            {
                var taskCard = CreateTaskCard(task, yPos);
                resultsPanel.Controls.Add(taskCard);
                yPos += taskCard.Height + 10;
            }
        }
        catch (Exception ex)
        {
            resultsPanel.Controls.Clear();
            var errorLabel = new Label
            {
                Text = $"Error searching tasks: {ex.Message}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Red,
                Location = new Point(20, 20),
                Size = new Size(400, 60)
            };
            resultsPanel.Controls.Add(errorLabel);
        }
    }

    private void ShowContent(string contentType)
    {
        // Hide all content panels
        dashboardContent.Visible = false;
        tasksContent.Visible = false;
        addTaskContent.Visible = false;
        reportsContent.Visible = false;
        searchContent.Visible = false;

        // Reset navigation buttons
        foreach (Control control in sidePanel.Controls)
        {
            if (control is Button btn)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(189, 195, 199);
            }
        }

        // Show selected content and highlight nav button
        switch (contentType.ToLower())
        {
            case "dashboard":
                dashboardContent.Visible = true;
                SelectNavButton(btnDashboard);
                _ = LoadDashboard(); // Fire and forget for UI responsiveness
                break;
            case "tasks":
                tasksContent.Visible = true;
                SelectNavButton(btnTasks);
                RefreshTasksLayout();
                break;
            case "addtask":
                addTaskContent.Visible = true;
                SelectNavButton(btnAddTask);
                break;
            case "reports":
                reportsContent.Visible = true;
                SelectNavButton(btnReports);
                break;
            case "search":
                searchContent.Visible = true;
                SelectNavButton(btnSearch);
                RefreshSearchLayout();
                break;
        }
    }

    private async System.Threading.Tasks.Task LoadDashboard()
    {
        // Refresh dashboard data
        if (statsPanel != null)
        {
            statsPanel.Controls.Clear();
            await CreateStatsCards();
        }
    }

    private void ApplyModernStyling()
    {
        // Add subtle shadows and rounded corners effect
        this.Paint += (s, e) =>
        {
            // Draw subtle shadow for panels
            var shadowColor = Color.FromArgb(30, 0, 0, 0);
            using (var brush = new SolidBrush(shadowColor))
            {
                // Side panel shadow
                var shadowRect = new Rectangle(sidePanel.Width, 0, 5, this.Height);
                e.Graphics.FillRectangle(brush, shadowRect);
            }
        };
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            SessionManager.Instance.Logout();
            this.DialogResult = DialogResult.Retry; // Indicates logout and return to login
            this.Close();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Only exit application if DialogResult is not Retry (logout)
        if (this.DialogResult != DialogResult.Retry)
        {
            System.Windows.Forms.Application.Exit();
        }
        base.OnFormClosed(e);
    }
}
