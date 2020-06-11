<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RemoveProxiesBTN = New System.Windows.Forms.Button()
        Me.PathTXT = New System.Windows.Forms.TextBox()
        Me.ExitBTN = New System.Windows.Forms.Button()
        Me.CyclesNUD = New System.Windows.Forms.NumericUpDown()
        Me.CyclesLBL = New System.Windows.Forms.Label()
        CType(Me.CyclesNUD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RemoveProxiesBTN
        '
        Me.RemoveProxiesBTN.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RemoveProxiesBTN.Location = New System.Drawing.Point(268, 37)
        Me.RemoveProxiesBTN.Name = "RemoveProxiesBTN"
        Me.RemoveProxiesBTN.Size = New System.Drawing.Size(148, 32)
        Me.RemoveProxiesBTN.TabIndex = 0
        Me.RemoveProxiesBTN.Text = "Remove Proxies"
        Me.RemoveProxiesBTN.UseVisualStyleBackColor = True
        '
        'PathTXT
        '
        Me.PathTXT.AllowDrop = True
        Me.PathTXT.Font = New System.Drawing.Font("Miriam CLM", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
        Me.PathTXT.Location = New System.Drawing.Point(12, 12)
        Me.PathTXT.Name = "PathTXT"
        Me.PathTXT.Size = New System.Drawing.Size(513, 22)
        Me.PathTXT.TabIndex = 1
        '
        'ExitBTN
        '
        Me.ExitBTN.Location = New System.Drawing.Point(422, 37)
        Me.ExitBTN.Name = "ExitBTN"
        Me.ExitBTN.Size = New System.Drawing.Size(103, 32)
        Me.ExitBTN.TabIndex = 3
        Me.ExitBTN.Text = "Exit"
        Me.ExitBTN.UseVisualStyleBackColor = True
        '
        'CyclesNUD
        '
        Me.CyclesNUD.Location = New System.Drawing.Point(229, 42)
        Me.CyclesNUD.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.CyclesNUD.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.CyclesNUD.Name = "CyclesNUD"
        Me.CyclesNUD.Size = New System.Drawing.Size(32, 20)
        Me.CyclesNUD.TabIndex = 4
        Me.CyclesNUD.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'CyclesLBL
        '
        Me.CyclesLBL.AutoSize = True
        Me.CyclesLBL.Font = New System.Drawing.Font("Miriam Libre", 9.749999!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CyclesLBL.Location = New System.Drawing.Point(181, 43)
        Me.CyclesLBL.Name = "CyclesLBL"
        Me.CyclesLBL.Size = New System.Drawing.Size(45, 17)
        Me.CyclesLBL.TabIndex = 5
        Me.CyclesLBL.Text = "Depth"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(537, 83)
        Me.Controls.Add(Me.CyclesLBL)
        Me.Controls.Add(Me.CyclesNUD)
        Me.Controls.Add(Me.ExitBTN)
        Me.Controls.Add(Me.PathTXT)
        Me.Controls.Add(Me.RemoveProxiesBTN)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.ShowIcon = False
        Me.Text = "Universal Proxy Remover by Jomtek - BETA"
        CType(Me.CyclesNUD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents RemoveProxiesBTN As Button
    Friend WithEvents PathTXT As TextBox
    Friend WithEvents ExitBTN As Button
    Friend WithEvents CyclesNUD As NumericUpDown
    Friend WithEvents CyclesLBL As Label
End Class
