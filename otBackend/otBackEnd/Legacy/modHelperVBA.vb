
'************
'************ Helper for Implementing some transition functions from VBA
'************

Public Module modHelperVBA

    '********** Null equals on the DBNullvalue
    '**********
    '**********
    Public Function Null() As Object
        Null = DBNull.Value
    End Function
    '********** isNull compares on the DBNullvalue
    '**********
    '**********
    Public Function IsNull(ByRef value As Object) As Boolean
        IsNull = DBNull.Value.Equals(value)
    End Function
    '********** isEmpty compares on the Nothing
    '**********
    '**********
    ''' <summary>
    ''' returns True if the object is empty - nothing or String.empty or Whitespace
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsEmpty(ByRef value As Object) As Boolean
        If value Is Nothing Then
            Return True
        ElseIf String.IsNullOrWhiteSpace(value) Then
            Return True
        Else
            Return False
        End If

    End Function
    '********** isMissing compares on the Nothing
    '**********
    '**********
    Public Function IsMissing(ByRef value As Object) As Boolean
        If value Is Nothing Then
            IsMissing = True
        ElseIf TypeOf (value) Is String AndAlso String.IsNullOrEmpty(value) Then
            IsMissing = True
        Else
            IsMissing = False
        End If

    End Function
    '************* ArrayIsInitializedV checks if the array is initialized
    '*************
    '*************
    Public Function IsArrayInitialized(ByRef array As Object) As Boolean
        If IsArray(array) AndAlso array.Length > 0 Then
            IsArrayInitialized = True
            Exit Function
        End If
        IsArrayInitialized = False
    End Function


End Module
