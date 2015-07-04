Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data

Public Class UIFormWorkDataAreas

    Private _DataAreaListItem As New List(Of RadListDataItem)
    Private _list As List(Of XLSDataArea)
    Private _workbook As Microsoft.Office.Interop.Excel.Workbook



    Public Sub New()
        Dim aDataArea As XLSDataArea
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _workbook = Globals.ThisAddIn.Application.ActiveWorkbook

        XLSXChangeMgr.AttachWorkbook(_workbook)
        _list = XLSXChangeMgr.getDataAreas(_workbook, refresh:=True)
        If _list.Count = 0 Then
            Dim aDataItem As New RadListDataItem()
            aDataItem.Text = "new DataArea"
            'aDataItem.Font.Style = Drawing.FontStyle.Italic
            _DataAreaListItem.Add(aDataItem)
            DataAreaListControl.Items.Add(aDataItem)
            _list.Add(New XLSDataArea("new DataArea", _workbook))
        Else
            For Each aDataArea In _list
                Dim aDataItem As New RadListDataItem()
                aDataItem.Text = aDataArea.Name
                AddHandler aDataArea.PropertyChanged, AddressOf DataAreaNameChangedEnvent
                'aDataItem.Font.Style = Drawing.FontStyle.Italic
                _DataAreaListItem.Add(aDataItem)
                DataAreaListControl.Items.Add(aDataItem)
            Next
        End If
        Me.DataAreaListControl.SelectedIndex = _DataAreaListItem.Count - 1
    End Sub

    Private Sub DataAreaNameChangedEnvent(sender As Object, e As EventArgs)
        Me.DataAreaListControl.SelectedItem.Text = DirectCast(sender, XLSDataArea).Name
        Me.Refresh()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click, Me.FormClosing
        RadMessageBox.SetThemeName(Me.ThemeName)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)

        If ds = Windows.Forms.DialogResult.Yes Then
            'Me.Disposing(sender, e)
            Me.Dispose()
        Else
            Dim FormClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
            If Not FormClosingArgs Is Nothing Then
                FormClosingArgs.Cancel = True
            End If
            Exit Sub
        End If
    End Sub

    Private Sub DataAreaListControl_SelectedIndexChanged(sender As Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles DataAreaListControl.SelectedIndexChanged
        Me.DataAreaPropertyGrid.SelectedObject = _list.Item(e.Position)
        Me.Refresh()
    End Sub

    Private Sub AddDataAreaButton_Click(sender As Object, e As EventArgs) Handles AddDataAreaButton.Click
        Dim aDataItem As New RadListDataItem()
        aDataItem.Text = "new DataArea " & _DataAreaListItem.Count
        'aDataItem.Font.Style = Drawing.FontStyle.Italic
        _DataAreaListItem.Add(aDataItem)
        DataAreaListControl.Items.Add(aDataItem)
        Dim aDataArea = New XLSDataArea(aDataItem.Text, _workbook)
        _list.Add(aDataArea)
        Me.DataAreaPropertyGrid.SelectedObject = aDataArea
        Me.DataAreaListControl.SelectedIndex = _DataAreaListItem.Count - 1
        Me.Refresh()
    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        ' for each one we have here
        'For Each aDataarea As XLSDataArea In _list
        'XLSXChangeMgr.addDataArea(_workbook, aDataarea)
        'Next
        ' save to properties
        XLSXChangeMgr.saveDataAreas(_workbook)
        Me.Dispose()
    End Sub
End Class
