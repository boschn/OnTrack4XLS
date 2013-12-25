Imports System.Data
Imports OnTrack
Imports OnTrack.XChange

'************
'************ MQFFeedWizardDataModel is a DataTable Representation of the MessageQueue (file)

Public Class UIMQFDataModel
    Inherits DataTable

    Private _mqf As clsOTDBMessageQueue

    ReadOnly Property MessageQueue As clsOTDBMessageQueue
        Get
            MessageQueue = _mqf
        End Get
    End Property

    Public Sub New(aMQF As clsOTDBMessageQueue)
        MyBase.New()

        _mqf = aMQF
        '
        '
        'MQFTable
        '
        Me.TableName = "MQFTable"
    End Sub
End Class
