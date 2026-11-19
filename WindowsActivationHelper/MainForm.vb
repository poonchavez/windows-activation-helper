Imports System.Diagnostics
Imports System.Drawing
Imports System.Security.Principal
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Friend NotInheritable Class MainForm
    Inherits Form

    Private ReadOnly statusLabel As New Label()
    Private ReadOnly keyBox As New TextBox()
    Private ReadOnly outputBox As New TextBox()

    Public Sub New()
        Text = "Windows Activation Helper"
        StartPosition = FormStartPosition.CenterScreen
        MinimumSize = New Size(620, 500)
        Size = New Size(720, 560)
        Font = New Font("Segoe UI", 10)

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill, .Padding = New Padding(24), .ColumnCount = 1,
            .RowCount = 8, .BackColor = Color.FromArgb(248, 249, 252)
        }
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Controls.Add(layout)

        Dim title As New Label With {
            .Text = "Windows activation", .AutoSize = True,
            .Font = New Font("Segoe UI Semibold", 22, FontStyle.Bold),
            .ForeColor = Color.FromArgb(25, 48, 79), .Margin = New Padding(0, 0, 0, 4)
        }
        layout.Controls.Add(title)
        Dim intro As New Label With {
            .Text = "Check activation or use a genuine product key you own.",
            .AutoSize = True, .Margin = New Padding(0, 0, 0, 18)
        }
        layout.Controls.Add(intro)

        statusLabel.Text = "Activation status has not been checked yet."
        statusLabel.AutoSize = True
        statusLabel.Margin = New Padding(0, 0, 0, 12)
        layout.Controls.Add(statusLabel)

        Dim buttons As New FlowLayoutPanel With {.AutoSize = True, .Margin = New Padding(0, 0, 0, 18)}
        Dim checkButton = MakeButton("Check status", AddressOf CheckStatus)
        Dim settingsButton = MakeButton("Open Activation settings", AddressOf OpenActivationSettings)
        buttons.Controls.Add(checkButton)
        buttons.Controls.Add(settingsButton)
        layout.Controls.Add(buttons)

        Dim keyCaption As New Label With {.Text = "Install your licensed product key", .AutoSize = True, .Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold)}
        layout.Controls.Add(keyCaption)
        keyBox.PlaceholderText = "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX"
        keyBox.Dock = DockStyle.Top
        keyBox.Margin = New Padding(0, 6, 0, 10)
        layout.Controls.Add(keyBox)

        Dim bottom As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
        Dim installButton = MakeButton("Install key and activate", AddressOf InstallKey)
        bottom.Controls.Add(installButton)
        outputBox.Multiline = True
        outputBox.ReadOnly = True
        outputBox.ScrollBars = ScrollBars.Vertical
        outputBox.Dock = DockStyle.Fill
        outputBox.BackColor = Color.White
        outputBox.Margin = New Padding(0, 10, 0, 0)
        bottom.Controls.Add(outputBox)
        layout.Controls.Add(bottom)

        Dim footer As New Label With {
            .Text = "This tool uses Windows licensing commands and Microsoft Activation settings. It does not provide keys or connect to third-party activation servers.",
            .AutoSize = True, .MaximumSize = New Size(650, 0), .ForeColor = Color.DimGray,
            .Margin = New Padding(0, 12, 0, 0)
        }
        layout.Controls.Add(footer)
    End Sub

    Private Function MakeButton(caption As String, handler As EventHandler) As Button
        Dim button As New Button With {.Text = caption, .AutoSize = True, .Padding = New Padding(8, 5, 8, 5), .Margin = New Padding(0, 0, 10, 0)}
        AddHandler button.Click, handler
        Return button
    End Function

    Private Sub CheckStatus(sender As Object, e As EventArgs)
        Try
            Dim result = RunSlmgr("/xpr")
            statusLabel.Text = If(String.IsNullOrWhiteSpace(result), "Windows returned no activation details. Open Activation settings for more information.", result.Trim())
            outputBox.Text = result
        Catch ex As Exception
            ShowError(ex.Message)
        End Try
    End Sub

    Private Sub OpenActivationSettings(sender As Object, e As EventArgs)
        Try
            Process.Start(New ProcessStartInfo("ms-settings:activation") With {.UseShellExecute = True})
        Catch ex As Exception
            ShowError("Could not open Activation settings: " & ex.Message)
        End Try
    End Sub

    Private Sub InstallKey(sender As Object, e As EventArgs)
        Dim key = keyBox.Text.Trim().ToUpperInvariant()
        If Not Regex.IsMatch(key, "^[A-Z0-9]{5}(-[A-Z0-9]{5}){4}$") Then
            MessageBox.Show("Enter a product key in the format XXXXX-XXXXX-XXXXX-XXXXX-XXXXX.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If Not IsAdministrator() Then
            MessageBox.Show("Installing a product key requires administrator access. Close this app, then choose Run as administrator.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If MessageBox.Show("This will install the product key you entered and ask Windows to activate online. Continue only if you are licensed to use this key.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            outputBox.Text = RunSlmgr("/ipk " & key) & Environment.NewLine & Environment.NewLine & RunSlmgr("/ato")
            statusLabel.Text = "Activation request finished. Review the result below or check Activation settings."
        Catch ex As Exception
            ShowError(ex.Message)
        End Try
    End Sub

    Private Function RunSlmgr(arguments As String) As String
        Dim systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)
        Dim scriptPath = IO.Path.Combine(systemDirectory, "slmgr.vbs")
        Dim startInfo As New ProcessStartInfo With {
            .FileName = "cscript.exe", .Arguments = "//NoLogo """ & scriptPath & """ " & arguments,
            .UseShellExecute = False, .CreateNoWindow = True,
            .RedirectStandardOutput = True, .RedirectStandardError = True
        }
        Using proc As Process = System.Diagnostics.Process.Start(startInfo)
            If proc Is Nothing Then Throw New InvalidOperationException("Could not start the Windows licensing script.")
            Dim stdout = proc.StandardOutput.ReadToEnd()
            Dim stderr = proc.StandardError.ReadToEnd()
            proc.WaitForExit()
            Return If(String.IsNullOrWhiteSpace(stderr), stdout, stdout & Environment.NewLine & stderr)
        End Using
    End Function

    Private Shared Function IsAdministrator() As Boolean
        Using identity = WindowsIdentity.GetCurrent()
            Return New WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator)
        End Using
    End Function

    Private Sub ShowError(message As String)
        MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub
End Class
