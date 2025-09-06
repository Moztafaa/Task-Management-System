using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Task.Application.DTO;
using Task.Application.ServiveInterface;

namespace Task.WinFormsPresentation.Forms;

[DesignerCategory("")]
public partial class LoginForm : Form
{
    private readonly IAuthService _authService;
    private Panel mainPanel = null!;
    private Panel leftPanel = null!;
    private Panel rightPanel = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnRegister = null!;
    private Label lblStatus = null!;
    private CheckBox chkRememberMe = null!;

    public LoginForm(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        ApplyModernStyling();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(1000, 600);
        this.Text = "Task Management - Login";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.FromArgb(240, 240, 240);

        CreateMainLayout();
        CreateLeftPanel();
        CreateRightPanel();

        this.Controls.Add(mainPanel);
    }

    private void CreateMainLayout()
    {
        mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        leftPanel = new Panel
        {
            Size = new Size(500, 600),
            Location = new Point(0, 0),
            BackColor = Color.FromArgb(52, 73, 94)
        };

        rightPanel = new Panel
        {
            Size = new Size(500, 600),
            Location = new Point(500, 0),
            BackColor = Color.White
        };

        mainPanel.Controls.Add(leftPanel);
        mainPanel.Controls.Add(rightPanel);
    }

    private void CreateLeftPanel()
    {
        // Welcome Section
        var lblWelcome = new Label
        {
            Text = "Welcome Back!",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(50, 150),
            Size = new Size(400, 50),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblSubtitle = new Label
        {
            Text = "Sign in to manage your tasks efficiently",
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(189, 195, 199),
            Location = new Point(50, 210),
            Size = new Size(400, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Decorative Elements
        var decorPanel = new Panel
        {
            Size = new Size(80, 4),
            Location = new Point(50, 250),
            BackColor = Color.FromArgb(52, 152, 219)
        };

        var featuresLabel = new Label
        {
            Text = "✓ Organize your tasks\n✓ Track progress\n✓ Set priorities\n✓ Generate reports",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = Color.FromArgb(189, 195, 199),
            Location = new Point(50, 300),
            Size = new Size(400, 120),
            TextAlign = ContentAlignment.TopLeft
        };

        leftPanel.Controls.AddRange(new Control[] { lblWelcome, lblSubtitle, decorPanel, featuresLabel });
    }

    private void CreateRightPanel()
    {
        // Close Button
        var btnClose = new Button
        {
            Text = "✕",
            Size = new Size(40, 40),
            Location = new Point(450, 10),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(149, 165, 166),
            Cursor = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
        btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 57, 43);
        btnClose.Click += (s, e) => this.Close();

        // Login Form Title
        var lblTitle = new Label
        {
            Text = "Sign In",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(50, 80),
            Size = new Size(400, 40),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Username Field
        var lblUsername = new Label
        {
            Text = "Username",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 160),
            Size = new Size(100, 20)
        };

        txtUsername = new TextBox
        {
            Location = new Point(50, 185),
            Size = new Size(400, 35),
            Font = new Font("Segoe UI", 12),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(247, 249, 249)
        };

        var usernamePanel = new Panel
        {
            Location = new Point(50, 185),
            Size = new Size(400, 35),
            BackColor = Color.FromArgb(247, 249, 249),
            BorderStyle = BorderStyle.FixedSingle
        };
        usernamePanel.Controls.Add(txtUsername);
        txtUsername.Dock = DockStyle.Fill;

        // Password Field
        var lblPassword = new Label
        {
            Text = "Password",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 250),
            Size = new Size(100, 20)
        };

        txtPassword = new TextBox
        {
            Location = new Point(50, 275),
            Size = new Size(400, 35),
            Font = new Font("Segoe UI", 12),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(247, 249, 249),
            UseSystemPasswordChar = true
        };

        var passwordPanel = new Panel
        {
            Location = new Point(50, 275),
            Size = new Size(400, 35),
            BackColor = Color.FromArgb(247, 249, 249),
            BorderStyle = BorderStyle.FixedSingle
        };
        passwordPanel.Controls.Add(txtPassword);
        txtPassword.Dock = DockStyle.Fill;

        // Remember Me
        chkRememberMe = new CheckBox
        {
            Text = "Remember me",
            Location = new Point(50, 330),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        // Login Button
        btnLogin = new Button
        {
            Text = "Sign In",
            Location = new Point(50, 380),
            Size = new Size(400, 45),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
        btnLogin.Click += BtnLogin_Click;

        // Register Link
        var lblRegisterText = new Label
        {
            Text = "Don't have an account?",
            Location = new Point(50, 450),
            Size = new Size(150, 20),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        btnRegister = new Button
        {
            Text = "Create Account",
            Location = new Point(210, 445),
            Size = new Size(120, 30),
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(52, 152, 219),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnRegister.FlatAppearance.BorderSize = 0;
        btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 240, 241);
        btnRegister.Click += BtnRegister_Click;

        // Status Label
        lblStatus = new Label
        {
            Location = new Point(50, 490),
            Size = new Size(400, 40),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(231, 76, 60),
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false
        };

        rightPanel.Controls.AddRange(new Control[] {
            btnClose, lblTitle, lblUsername, usernamePanel, lblPassword, passwordPanel,
            chkRememberMe, btnLogin, lblRegisterText, btnRegister, lblStatus
        });
    }

    private void ApplyModernStyling()
    {
        // Add shadow effect
        this.Paint += (s, e) =>
        {
            var rect = new Rectangle(0, 0, this.Width, this.Height);
            using (var brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        };

        // Add focus effects to textboxes
        txtUsername.Enter += (s, e) => txtUsername.Parent.BackColor = Color.White;
        txtUsername.Leave += (s, e) => txtUsername.Parent.BackColor = Color.FromArgb(247, 249, 249);
        txtPassword.Enter += (s, e) => txtPassword.Parent.BackColor = Color.White;
        txtPassword.Leave += (s, e) => txtPassword.Parent.BackColor = Color.FromArgb(247, 249, 249);

        // Enter key handling
        txtPassword.KeyPress += (s, e) =>
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
                e.Handled = true;
            }
        };
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            ShowStatus("Please fill in all fields.", true);
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "Signing In...";

        try
        {
            var loginDto = new LoginUserDto
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text
            };

            var user = _authService.LoginUser(loginDto);

            if (user != null)
            {
                ShowStatus("Login successful! Redirecting...", false);

                // Wait a moment to show success message
                await System.Threading.Tasks.Task.Delay(1000);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowStatus("Invalid username or password.", true);
            }
        }
        catch (Exception ex)
        {
            ShowStatus($"Login failed: {ex.Message}", true);
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Sign In";
        }
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        this.Hide();
        var registerForm = Program.ServiceProvider.GetRequiredService<RegisterForm>();
        var result = registerForm.ShowDialog();

        if (result == DialogResult.OK)
        {
            ShowStatus("Registration successful! Please sign in.", false);
        }

        this.Show();
    }

    private async void ShowStatus(string message, bool isError)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = isError ? Color.FromArgb(231, 76, 60) : Color.FromArgb(39, 174, 96);
        lblStatus.Visible = true;

        // Auto-hide after 3 seconds
        await System.Threading.Tasks.Task.Delay(3000);
        lblStatus.Visible = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Draw subtle border
        using (var pen = new Pen(Color.FromArgb(189, 195, 199), 1))
        {
            e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
        }
        base.OnPaint(e);
    }
}
