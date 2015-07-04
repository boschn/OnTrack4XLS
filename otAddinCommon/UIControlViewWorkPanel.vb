Imports System.Windows.Forms

Public Class UIControlViewWorkPanel
    Inherits UserControl
    Implements iUIStatusSender

    

    Public Event OnIssueMessage(sender As Object, e As UIStatusMessageEventArgs) Implements iUIStatusSender.OnIssueMessage

   
End Class
