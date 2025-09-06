using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task.Application.DTO;
using Task.Application.ServiveInterface;

namespace Task.WinFormsPresentation.Forms;

[DesignerCategory("")]
public partial class RegisterForm : Form
{
    private readonly IAuthService _authService;
    private Panel mainPanel = null!;
    private Panel leftPanel = null!;
    private Panel rightPanel = null!;
    private TextBox txtUsername = null!;
    private TextBox txtEmail = null!;
    private TextBox txtPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private Button btnRegister = null!;
    private Button btnBackToLogin = null!;
    private Label lblStatus = null!;
    private CheckBox chkTerms = null!;

    public RegisterForm(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        ApplyModernStyling();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(1000, 650);
        this.Text = "Task Management - Register";
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
            Size = new Size(500, 650),
            Location = new Point(0, 0),
            BackColor = Color.FromArgb(46, 204, 113)
        };

        rightPanel = new Panel
        {
            Size = new Size(500, 650),
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
            Text = "Join Us Today!",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(50, 150),
            Size = new Size(400, 50),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblSubtitle = new Label
        {
            Text = "Create your account and start managing tasks like a pro",
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(229, 244, 236),
            Location = new Point(50, 210),
            Size = new Size(400, 50),
            TextAlign = ContentAlignment.TopLeft
        };

        // Decorative Elements
        var decorPanel = new Panel
        {
            Size = new Size(80, 4),
            Location = new Point(50, 280),
            BackColor = Color.White
        };

        var benefitsLabel = new Label
        {
            Text = "🚀 Get started in minutes\n📋 Organize unlimited tasks\n📊 Track your productivity\n🎯 Achieve your goals",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = Color.FromArgb(229, 244, 236),
            Location = new Point(50, 320),
            Size = new Size(400, 140),
            TextAlign = ContentAlignment.TopLeft
        };

        leftPanel.Controls.Add(lblWelcome);
        leftPanel.Controls.Add(lblSubtitle);
        leftPanel.Controls.Add(decorPanel);
        leftPanel.Controls.Add(benefitsLabel);
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

        // Register Form Title
        var lblTitle = new Label
        {
            Text = "Create Account",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Location = new Point(50, 60),
            Size = new Size(400, 40),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Username Field
        var lblUsername = new Label
        {
            Text = "Username",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 130),
            Size = new Size(100, 20)
        };

        txtUsername = CreateStyledTextBox(new Point(50, 155));
        var usernamePanel = CreateTextBoxPanel(txtUsername);

        // Email Field
        var lblEmail = new Label
        {
            Text = "Email Address",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 210),
            Size = new Size(100, 20)
        };

        txtEmail = CreateStyledTextBox(new Point(50, 235));
        var emailPanel = CreateTextBoxPanel(txtEmail);

        // Password Field
        var lblPassword = new Label
        {
            Text = "Password",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 290),
            Size = new Size(100, 20)
        };

        txtPassword = CreateStyledTextBox(new Point(50, 315));
        txtPassword.UseSystemPasswordChar = true;
        var passwordPanel = CreateTextBoxPanel(txtPassword);

        // Confirm Password Field
        var lblConfirmPassword = new Label
        {
            Text = "Confirm Password",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(127, 140, 141),
            Location = new Point(50, 370),
            Size = new Size(150, 20)
        };

        txtConfirmPassword = CreateStyledTextBox(new Point(50, 395));
        txtConfirmPassword.UseSystemPasswordChar = true;
        var confirmPasswordPanel = CreateTextBoxPanel(txtConfirmPassword);

        // Terms and Conditions
        chkTerms = new CheckBox
        {
            Text = "I agree to the Terms and Conditions",
            Location = new Point(50, 450),
            Size = new Size(300, 25),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        // Register Button
        btnRegister = new Button
        {
            Text = "Create Account",
            Location = new Point(50, 490),
            Size = new Size(400, 45),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnRegister.FlatAppearance.BorderSize = 0;
        btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
        btnRegister.Click += BtnRegister_Click;

        // Back to Login Link
        var lblLoginText = new Label
        {
            Text = "Already have an account?",
            Location = new Point(50, 560),
            Size = new Size(170, 20),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        btnBackToLogin = new Button
        {
            Text = "Sign In",
            Location = new Point(230, 555),
            Size = new Size(80, 30),
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(46, 204, 113),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnBackToLogin.FlatAppearance.BorderSize = 0;
        btnBackToLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 240, 241);
        btnBackToLogin.Click += BtnBackToLogin_Click;

        // Status Label
        lblStatus = new Label
        {
            Location = new Point(50, 600),
            Size = new Size(400, 40),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(231, 76, 60),
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false
        };

        rightPanel.Controls.Add(btnClose);
        rightPanel.Controls.Add(lblTitle);
        rightPanel.Controls.Add(lblUsername);
        rightPanel.Controls.Add(usernamePanel);
        rightPanel.Controls.Add(lblEmail);
        rightPanel.Controls.Add(emailPanel);
        rightPanel.Controls.Add(lblPassword);
        rightPanel.Controls.Add(passwordPanel);
        rightPanel.Controls.Add(lblConfirmPassword);
        rightPanel.Controls.Add(confirmPasswordPanel);
        rightPanel.Controls.Add(chkTerms);
        rightPanel.Controls.Add(btnRegister);
        rightPanel.Controls.Add(lblLoginText);
        rightPanel.Controls.Add(btnBackToLogin);
        rightPanel.Controls.Add(lblStatus);
    }

    private TextBox CreateStyledTextBox(Point location)
    {
        return new TextBox
        {
            Location = location,
            Size = new Size(400, 35),
            Font = new Font("Segoe UI", 12),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(247, 249, 249)
        };
    }

    private Panel CreateTextBoxPanel(TextBox textBox)
    {
        var panel = new Panel
        {
            Location = textBox.Location,
            Size = textBox.Size,
            BackColor = Color.FromArgb(247, 249, 249),
            BorderStyle = BorderStyle.FixedSingle
        };
        panel.Controls.Add(textBox);
        textBox.Dock = DockStyle.Fill;
        return panel;
    }

    private void ApplyModernStyling()
    {
        // Add focus effects to textboxes
        foreach (Control control in rightPanel.Controls)
        {
            if (control is Panel panel && panel.Controls.Count > 0 && panel.Controls[0] is TextBox textBox)
            {
                textBox.Enter += (s, e) => panel.BackColor = Color.White;
                textBox.Leave += (s, e) => panel.BackColor = Color.FromArgb(247, 249, 249);
            }
        }

        // Enter key handling for last field
        txtConfirmPassword.KeyPress += (s, e) =>
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnRegister.PerformClick();
                e.Handled = true;
            }
        };
    }

    private async void BtnRegister_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm())
            return;

        btnRegister.Enabled = false;
        btnRegister.Text = "Creating Account...";

        try
        {
            var registerDto = new RegisterUserDto
            {
                Username = txtUsername.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text,
                ConfirmPassword = txtConfirmPassword.Text
            };

            var user = _authService.RegisterUser(registerDto);

            ShowStatus("Account created successfully! You can now sign in.", false);

            // Wait a moment to show success message
            await System.Threading.Tasks.Task.Delay(2000);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            ShowStatus($"Registration failed: {ex.Message}", true);
        }
        finally
        {
            btnRegister.Enabled = true;
            btnRegister.Text = "Create Account";
        }
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            ShowStatus("Please enter a username.", true);
            txtUsername.Focus();
            return false;
        }

        if (txtUsername.Text.Length < 3)
        {
            ShowStatus("Username must be at least 3 characters.", true);
            txtUsername.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            ShowStatus("Please enter an email address.", true);
            txtEmail.Focus();
            return false;
        }

        if (!IsValidEmail(txtEmail.Text))
        {
            ShowStatus("Please enter a valid email address.", true);
            txtEmail.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            ShowStatus("Please enter a password.", true);
            txtPassword.Focus();
            return false;
        }

        if (txtPassword.Text.Length < 6)
        {
            ShowStatus("Password must be at least 6 characters.", true);
            txtPassword.Focus();
            return false;
        }

        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            ShowStatus("Passwords do not match.", true);
            txtConfirmPassword.Focus();
            return false;
        }

        if (!chkTerms.Checked)
        {
            ShowStatus("Please accept the Terms and Conditions.", true);
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void BtnBackToLogin_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
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
