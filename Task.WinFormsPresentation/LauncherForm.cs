using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Task.WinFormsPresentation.Forms;

namespace Task.WinFormsPresentation;

public partial class LauncherForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public LauncherForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Task Management - Choose Form";
        this.Size = new Size(400, 200);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblTitle = new Label
        {
            Text = "Choose which form to launch:",
            Location = new Point(50, 30),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var btnTestForm = new Button
        {
            Text = "Test Form\n(Register, Login, Tasks)",
            Location = new Point(50, 80),
            Size = new Size(150, 60),
            Font = new Font("Segoe UI", 9),
            UseVisualStyleBackColor = true
        };
        btnTestForm.Click += (s, e) => LaunchTestForm();

        var btnMainForm = new Button
        {
            Text = "Main Form\n(Original Form)",
            Location = new Point(220, 80),
            Size = new Size(150, 60),
            Font = new Font("Segoe UI", 9),
            UseVisualStyleBackColor = true
        };
        btnMainForm.Click += (s, e) => LaunchMainForm();

        this.Controls.Add(lblTitle);
        this.Controls.Add(btnTestForm);
        this.Controls.Add(btnMainForm);
    }

    private void LaunchTestForm()
    {
        this.Hide();
        var testForm = _serviceProvider.GetRequiredService<TestForm>();
        testForm.FormClosed += (s, e) => this.Close();
        testForm.Show();
    }

    private void LaunchMainForm()
    {
        this.Hide();
        var mainForm = _serviceProvider.GetRequiredService<MainForm>();
        mainForm.FormClosed += (s, e) => this.Close();
        mainForm.Show();
    }
}
