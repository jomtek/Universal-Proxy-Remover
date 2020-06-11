Imports System.Text
Imports dnlib.DotNet
Imports dnlib.DotNet.Emit
Public Class MainForm
    Dim failedMethodAccesses As Integer = 0
    Dim removedProxies As Integer = 0



    Private Sub RemoveProxiesBTN_Click(sender As Object, e As EventArgs) Handles RemoveProxiesBTN.Click
        failedMethodAccesses = 0

        If PathTXT.Text.Trim.Length > 0 Then
            Dim modCtx As ModuleContext = ModuleDef.CreateModuleContext()
            Dim module_ As ModuleDefMD = ModuleDefMD.Load(PathTXT.Text, modCtx)
            Dim moduleTypes As IEnumerable(Of TypeDef) = module_.GetTypes

            If Not module_.IsILOnly Then
                MsgBox("Module contains non-IL code. Aborting.")
                Exit Sub
            End If

            Dim removedMethodsCount As Integer =
                ProxyRemove.RemoveProxiesFromTypes(moduleTypes, CyclesNUD.Value)

            module_.Write(PathTXT.Text & "_fixed.exe")

            MsgBox("Removed " & removedMethodsCount & " useless methods :p", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub PathTXT_DragOver(sender As Object, e As DragEventArgs) Handles PathTXT.DragOver
        e.Effect = DragDropEffects.All
        PathTXT.Text = e.Data.GetData(DataFormats.FileDrop)(0)
    End Sub

    Private Sub ExitBTN_Click(sender As Object, e As EventArgs) Handles ExitBTN.Click
        Application.Exit()
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MsgBox("First version, does not support all proxies (todo) and only removes literal proxies")
    End Sub
End Class
