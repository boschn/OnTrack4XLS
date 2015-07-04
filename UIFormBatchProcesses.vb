
Imports System.ComponentModel

Public Class UIFormBatchProcesses
    Private WithEvents _batchworker As BackgroundWorker
    Private Delegate Sub SetProgressCallback([percentage] As Integer, [text] As String)
    Private Delegate Sub SetStatusCallback([text] As String)
    Private Delegate Sub SetAfterWorkCallback()

    Private Enum Batchprog
        updategaps = 1
        buildnet = 10
        buildcluster = 11
        checkDepend = 12
    End Enum

    Private _currentbatch As Batchprog = 0

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _batchworker = New BackgroundWorker
        Me.CancelButton.Enabled = False

    End Sub
    Private Sub UpdateGapsButton_Click(sender As Object, e As EventArgs) Handles UpdateGaps.Click
        If Not _batchworker.IsBusy Then
            Me.CancelButton.Enabled = True
            Me.TileGroupElement1.Enabled = False
            Me.TileGroupElement2.Enabled = False
            Me.Refresh()
            _batchworker.WorkerReportsProgress = True
            ' run
            _currentbatch = Batchprog.updategaps
            _batchworker.RunWorkerAsync()
        Else
            Me.StatusLabel.Text = "OnTrack is busy please standby"
        End If

    End Sub


    Private Sub buildDependNet_Click(sender As Object, e As EventArgs) Handles buildDependNet.Click
        If Not _batchworker.IsBusy Then
            Me.CancelButton.Enabled = True
            Me.TileGroupElement1.Enabled = False
            Me.TileGroupElement2.Enabled = False
            Me.Refresh()
            _batchworker.WorkerReportsProgress = True
            ' run
            _currentbatch = Batchprog.buildnet
            _batchworker.RunWorkerAsync()
        Else
            Me.StatusLabel.Text = "OnTrack is busy please standby"
        End If
    End Sub
    Private Sub checkCluster_click(sender As Object, e As EventArgs) Handles CheckDepend.Click
        If Not _batchworker.IsBusy Then
            Me.CancelButton.Enabled = True
            Me.TileGroupElement1.Enabled = False
            Me.TileGroupElement2.Enabled = False
            Me.Refresh()
            _batchworker.WorkerReportsProgress = True
            ' run
            _currentbatch = Batchprog.checkDepend
            _batchworker.RunWorkerAsync()
        Else
            Me.StatusLabel.Text = "OnTrack is busy please standby"
        End If
    End Sub
    Private Sub buildDynCluster_Click(sender As Object, e As EventArgs) Handles BuildCluster.Click
        If Not _batchworker.IsBusy Then
            Me.CancelButton.Enabled = True
            Me.TileGroupElement1.Enabled = False
            Me.TileGroupElement2.Enabled = False
            Me.Refresh()
            _batchworker.WorkerReportsProgress = True
            ' run
            _currentbatch = Batchprog.buildcluster
            _batchworker.RunWorkerAsync()
        Else
            Me.StatusLabel.Text = "OnTrack is busy please standby"
        End If
    End Sub
    ''' <summary>
    ''' Run Batch Processing
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Run(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _batchworker.DoWork

        Select Case _currentbatch
            Case Batchprog.updategaps
                e.Result = Deliverables.Track.UpdateAllTracks(workerthread:=_batchworker)
            Case Batchprog.buildcluster
                'e.Result = Scheduling.Dependency.BuildDynamicDependencyCluster(workerthread:=_batchworker)
            Case Batchprog.buildnet
                'e.Result = Scheduling.Dependency.BuildDependencyNet(workerthread:=_batchworker)
            Case Batchprog.checkDepend
                'e.Result = Scheduling.Dependency.CheckAllDependencies(workerthread:=_batchworker)
        End Select


    End Sub
    ' This method demonstrates a pattern for making thread-safe 
    ' calls on a Windows Forms control.  
    ' 
    ' If the calling thread is different from the thread that 
    ' created the control, this method creates a 
    ' Callback and calls itself asynchronously using the 
    ' Invoke method. 
    ' 
    ' If the calling thread is the same as the thread that created 
    ' the  control, the  properties are/is set directly.  
    ''' <summary>
    ''' Set the Progress
    ''' </summary>
    ''' <param name="percentage"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub SetProgress(ByVal [percentage] As Integer, ByVal [text] As String)

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.StatusStrip.InvokeRequired Then
            Dim d As New SetProgressCallback(AddressOf SetProgress)
            Me.Invoke(d, New Object() {[percentage], [text]})
        Else
            Me.StatusProgress.Value1 = [percentage]
            Me.StatusLabel.Text = [text]
            Me.StatusStrip.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.StatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.CancelButton.Enabled = False
            Me.TileGroupElement1.Enabled = True
            Me.TileGroupElement2.Enabled = True
            'Me.Enabled = True
            Me.StatusStrip.Refresh()
        End If


    End Sub
    ''' <summary>
    ''' Progress EventHandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReplicationProgress(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _batchworker.ProgressChanged

        Dim perc As Integer
        Dim text As String

        perc = e.ProgressPercentage
        If Not e.UserState Is Nothing Then
            text = CType(e.UserState, String)
        Else
            text = String.empty
        End If

        Call SetProgress(perc, text)

    End Sub
    ''' <summary>
    '''  End of the Preprocess Eventhandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EndOfReplication(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
        Handles _batchworker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso e.Result = True Then
            Me.StatusLabel.Text = "finished sucessfully"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso e.Result = False Then
            Me.StatusLabel.Text = "finished with no success"
        End If

        Call SetAfterWork()

    End Sub


End Class
