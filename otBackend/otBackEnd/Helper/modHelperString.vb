'************
'************ Helper for Implementing some string handling functions 
'************


Public Module modHelperString

    '************
    '************ splitMultiByChar : Split  "|xx|yy|" -> "xx","yy"
    '************
    Function SplitMultbyChar(ByVal text As String, ByVal DelimChar As String) As String()
        ''''''''''''''''''''''''''''''''
        ' if Text is empty, get out
        ''''''''''''''''''''''''''''''''
        If Len(text) = 0 Then
            Exit Function
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''
        ' if DelimChars is empty, return original text
        ''''''''''''''''''''''''''''''''''''''''''''''
        If String.IsNullOrEmpty(DelimChar) Or DelimChar = vbNullString Then
            SplitMultbyChar = New String() {text}
            Exit Function
        End If

        Dim interim() As String = text.Split(DelimChar)
        Dim result() As String
        Dim j As UShort = 0
        Dim i As UShort
        Dim u As UShort = UBound(interim)
        Dim l As UShort = LBound(interim)


        ' if start is like '|*' leave the first out
        If text.Substring(0, 1) = DelimChar Then
            l += 1
        End If
        If text.Substring(text.Length - 1, 1) = DelimChar Then
            u -= 1
        End If

        ' go through
        For i = l To u

            ReDim Preserve result(j)
            result(j) = interim(i)
            j += 1

        Next

        ' return the result
        Return result

    End Function
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' SplitMutliCharEmb: -> Embedded Version e.g. |xx|yy| -> xx,yy
    '
    ' This function splits Text into an array of substrings, each substring
    ' delimited by any character in DelimChars. Only a single character
    ' may be a delimiter between two substrings, but DelimChars may
    ' contain any number of delimiter characters. If you need multiple
    ' character delimiters, use the SplitMultiDelimsEX function. It returns
    ' an unallocated array it Text is empty, a single element array
    ' containing all of text if DelimChars is empty, or a 1 or greater
    ' element array if the Text is successfully split into substrings.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function SplitMultiDelimsEmb(ByVal text As String, ByVal DelimChars As String) As String()

        Dim Pos1 As Long
        Dim n As Long
        Dim m As Long
        Dim s As Long
        Dim Arr() As String
        Dim i As Long

        ''''''''''''''''''''''''''''''''
        ' if Text is empty, get out
        ''''''''''''''''''''''''''''''''
        If Len(text) = 0 Then
            Exit Function
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''
        ' if DelimChars is empty, return original text
        ''''''''''''''''''''''''''''''''''''''''''''''
        If String.IsNullOrEmpty(DelimChars) Or DelimChars = vbNullString Then
            SplitMultiDelimsEmb = New String() {text}
            Exit Function
        End If





        ' check if we start with delimiter
        If Mid(text, 1, 1) = DelimChars Then
            s = 1
        Else
            s = 0
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''
        ' oversize the array, we'll shrink it later so
        ' we don't need to use Redim Preserve
        '''''''''''''''''''''''''''''''''''''''''''''''
        ReDim Arr(0 To Len(text) - 1)

        i = 0
        n = 0
        Pos1 = s

        For n = s To Len(text)
            For m = 1 To Len(DelimChars)
                If StrComp(Mid(text, n, 1), Mid(DelimChars, m, 1), vbTextCompare) = 0 Then
                    i = i + 1
                    Arr(i - 1) = Mid(text, Pos1, n - Pos1)
                    Pos1 = n + 1
                    n = n + 1
                End If
            Next m
        Next n

        If Pos1 <= Len(text) Then
            i = i + 1
            Arr(i - 1) = Mid(text, Pos1)
        End If

        ''''''''''''''''''''''''''''''''''''''
        ' chop off unused array elements
        ''''''''''''''''''''''''''''''''''''''
        ReDim Preserve Arr(0 To i - 1)
        SplitMultiDelimsEmb = Arr

    End Function
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' SplitMutliChar
    ' This function splits Text into an array of substrings, each substring
    ' delimited by any character in DelimChars. Only a single character
    ' may be a delimiter between two substrings, but DelimChars may
    ' contain any number of delimiter characters. If you need multiple
    ' character delimiters, use the SplitMultiDelimsEX function. It returns
    ' an unallocated array it Text is empty, a single element array
    ' containing all of text if DelimChars is empty, or a 1 or greater
    ' element array if the Text is successfully split into substrings.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    Function SplitMultiDelims(ByVal text As String, ByVal DelimChars As String) As String()

        Dim Pos1 As Long
        Dim n As Long
        Dim m As Long
        Dim Arr() As String
        Dim i As Long

        ''''''''''''''''''''''''''''''''
        ' if Text is empty, get out
        ''''''''''''''''''''''''''''''''
        If Len(text) = 0 Then
            Exit Function
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''
        ' if DelimChars is empty, return original text
        ''''''''''''''''''''''''''''''''''''''''''''''
        If DelimChars = vbNullString Then
            SplitMultiDelims = New String() {text}
            Exit Function
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''
        ' oversize the array, we'll shrink it later so
        ' we don't need to use Redim Preserve
        '''''''''''''''''''''''''''''''''''''''''''''''
        ReDim Arr(0 To Len(text) - 1)

        System.Diagnostics.Debug.Assert(False)
        i = 0
        n = 0
        Pos1 = 0

        For n = 1 To Len(text)
            For m = 1 To Len(DelimChars)
                If StrComp(Mid(text, n, 1), Mid(DelimChars, m, 1), vbTextCompare) = 0 Then
                    i = i + 1
                    Arr(i - 1) = Mid(text, Pos1, n - Pos1)
                    Pos1 = n + 1
                    n = n + 1
                End If
            Next m
        Next n

        If Pos1 <= Len(text) Then
            i = i + 1
            Arr(i - 1) = Mid(text, Pos1)
        End If

        ''''''''''''''''''''''''''''''''''''''
        ' chop off unused array elements
        ''''''''''''''''''''''''''''''''''''''
        ReDim Preserve Arr(0 To i - 1)
        SplitMultiDelims = Arr

    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' SplitMultiDelimsEX
    ' This function is like VBA's Split function or the SplitMultiDelims
    ' function, also in this module. It accepts any number of multiple-
    ' character delimiter strings and splits Text into substrings based
    ' on the delimiter strings. It returns an unallocated array if Text
    ' is empty, a single-element array if DelimStrings is empty, or a
    ' 1 or greater element array if Text was successfully split into
    ' substrings based on the DelimStrings delimiters.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    Function SplitMultiDelimsEX(ByVal text As String, ByVal DelimStrings As String, _
                                ByVal DelimStringsSep As String) As String()

        Dim Pos1 As Long
        Dim n As Long
        Dim m As Long
        Dim Arr() As String
        Dim i As Long
        Dim DelimWords() As String
        Dim DelimNdx As Long
        Dim DelimWord As String

        '''''''''''''''''''''''''''''
        ' if Text is empty, get out
        '''''''''''''''''''''''''''''
        If Len(text) = 0 Then
            Exit Function
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''
        ' if there are no delimiters, return the whole text
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        If DelimStrings = vbNullString Then
            SplitMultiDelimsEX = New String() {text}
            Exit Function
        End If

        ''''''''''''''''''''''''''''''''''''''''''
        ' if there is no delim separator, get out
        ''''''''''''''''''''''''''''''''''''''''''
        If DelimStringsSep = vbNullString Then
            Exit Function
        End If

        DelimWords = Split(DelimStrings, DelimStringsSep)
        If IsArrayInitialized(DelimWords) = False Then
            Exit Function
        End If

        ''''''''''''''''''''''''''''''''''''''''''''''''
        ' oversize the array, we'll shrink it later so
        ' we don't need to use Redim Preserve
        ''''''''''''''''''''''''''''''''''''''''''''''''
        ReDim Arr(0 To Len(text) - 1)

        i = 0
        n = 0
        Pos1 = 0

        For n = Pos1 To Len(text)
            For DelimNdx = LBound(DelimWords) To UBound(DelimWords)
                DelimWord = DelimWords(DelimNdx)
                If StrComp(Mid(text, n, Len(DelimWord)), DelimWord, vbBinaryCompare) = 0 Then
                    i = i + 1
                    Arr(i - 1) = Mid(text, Pos1, n - Pos1)
                    Pos1 = n + Len(DelimWord)
                    n = Pos1
                End If
            Next DelimNdx
        Next n

        If Pos1 <= Len(text) Then
            i = i + 1
            Arr(i - 1) = Mid(text, Pos1)
        End If

        ''''''''''''''''''''''''''''''''''''
        ' chop off unused array elements
        ''''''''''''''''''''''''''''''''''''
        ReDim Preserve Arr(0 To i - 1)
        SplitMultiDelimsEX = Arr

    End Function


End Module
