REM ***********************************************************************************************************************************************''' <summary>
REM *********** Nano XML Parser and Writer
REM *********** code based on http://www.codeproject.com/Tips/682245/NanoXML-Simple-and-fast-XML-parser
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2015-04-09
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Collections.Generic

Namespace NanoXML
    ''' <summary>
    ''' Base class containing usefull features for all XML classes
    ''' </summary>
    Public Class NanoXMLBase
        Protected Shared Function IsSpace(c As Char) As Boolean
            Return c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.Lf OrElse c = ControlChars.Cr
        End Function

        Protected Shared Sub SkipSpaces(str As String, ByRef i As Integer)
            While i < str.Length
                If Not IsSpace(str(i)) Then
                    If str(i) = "<"c AndAlso i + 4 < str.Length AndAlso str(i + 1) = "!"c AndAlso str(i + 2) = "-"c AndAlso str(i + 3) = "-"c Then
                        i += 4
                        ' skip <!--
                        While i + 2 < str.Length AndAlso Not (str(i) = "-"c AndAlso str(i + 1) = "-"c)
                            i += 1
                        End While

                        ' skip --
                        i += 2
                    Else
                        Exit While
                    End If
                End If

                i += 1
            End While
        End Sub

        Protected Shared Function GetValue(str As String, ByRef i As Integer, endChar As Char, endChar2 As Char, stopOnSpace As Boolean) As String
            Dim start As Integer = i
            While (Not stopOnSpace OrElse Not IsSpace(str(i))) AndAlso str(i) <> endChar AndAlso str(i) <> endChar2
                i += 1
            End While

            Return str.Substring(start, i - start)
        End Function

        Protected Shared Function IsQuote(c As Char) As Boolean
            Return c = """"c OrElse c = "'"c
        End Function

        ' returns name
        Protected Shared Function ParseAttributes(str As String, ByRef i As Integer, attributes As List(Of NanoXMLAttribute), endChar As Char, endChar2 As Char) As String
            SkipSpaces(str, i)
            Dim name As String = GetValue(str, i, endChar, endChar2, True)

            SkipSpaces(str, i)

            While str(i) <> endChar AndAlso str(i) <> endChar2
                Dim attrName As String = GetValue(str, i, "="c, ControlChars.NullChar, True)

                SkipSpaces(str, i)
                i += 1
                ' skip '='
                SkipSpaces(str, i)

                Dim quote As Char = str(i)
                If Not IsQuote(quote) Then
                    Throw New XMLParsingException(Convert.ToString("Unexpected token after ") & attrName)
                End If

                i += 1
                ' skip quote
                Dim attrValue As String = GetValue(str, i, quote, ControlChars.NullChar, False)
                i += 1
                ' skip quote
                attributes.Add(New NanoXMLAttribute(attrName, attrValue))

                SkipSpaces(str, i)
            End While

            Return name
        End Function
    End Class

    ''' <summary>
    ''' Class representing whole DOM XML document
    ''' </summary>
    Public Class NanoXMLDocument
        Inherits NanoXMLBase
        Private m_rootNode As NanoXMLNode
        Private m_declarations As New List(Of NanoXMLAttribute)()
        ''' <summary>
        ''' Public constructor. Loads xml document from raw string
        ''' </summary>
        ''' <param name="xmlString">String with xml</param>
        Public Sub New(xmlString As String)
            Dim i As Integer = 0

            While True
                SkipSpaces(xmlString, i)

                If xmlString(i) <> "<"c Then
                    Throw New XMLParsingException("Unexpected token")
                End If

                i += 1
                ' skip <
                If xmlString(i) = "?"c Then
                    ' declaration
                    i += 1
                    ' skip ?
                    ParseAttributes(xmlString, i, m_declarations, "?"c, ">"c)
                    i += 1
                    ' skip ending ?
                    i += 1
                    ' skip ending >
                    Continue While
                End If

                If xmlString(i) = "!"c Then
                    ' doctype
                    While xmlString(i) <> ">"c
                        ' skip doctype
                        i += 1
                    End While

                    i += 1
                    ' skip >
                    Continue While
                End If

                m_rootNode = New NanoXMLNode(xmlString, i)
                Exit While
            End While
        End Sub
        ''' <summary>
        ''' Root document element
        ''' </summary>
        Public ReadOnly Property RootNode() As NanoXMLNode
            Get
                Return m_rootNode
            End Get
        End Property
        ''' <summary>
        ''' List of XML Declarations as <see cref="NanoXMLAttribute"/>
        ''' </summary>
        Public ReadOnly Property Declarations() As IEnumerable(Of NanoXMLAttribute)
            Get
                Return m_declarations
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Element node of document
    ''' </summary>
    Public Class NanoXMLNode
        Inherits NanoXMLBase
        Private m_value As String
        Private m_name As String

        Private m_subNodes As New List(Of NanoXMLNode)()
        Private m_attributes As New List(Of NanoXMLAttribute)()

        Friend Sub New(str As String, ByRef i As Integer)
            m_name = ParseAttributes(str, i, m_attributes, ">"c, "/"c)

            If str(i) = "/"c Then
                ' if this node has nothing inside
                i += 1
                ' skip /
                i += 1
                ' skip >
                Return
            End If

            i += 1
            ' skip >
            ' temporary. to include all whitespaces into value, if any
            Dim tempI As Integer = i

            SkipSpaces(str, tempI)

            If str(tempI) = "<"c Then
                i = tempI

                While str(i + 1) <> "/"c
                    ' parse subnodes
                    i += 1
                    ' skip <
                    m_subNodes.Add(New NanoXMLNode(str, i))

                    SkipSpaces(str, i)

                    If i >= str.Length Then
                        Return
                    End If
                    ' EOF
                    If str(i) <> "<"c Then
                        Throw New XMLParsingException("Unexpected token")
                    End If
                End While

                ' skip <
                i += 1
            Else
                ' parse value
                m_value = GetValue(str, i, "<"c, ControlChars.NullChar, False)
                i += 1
                ' skip <
                If str(i) <> "/"c Then
                    Throw New XMLParsingException(Convert.ToString("Invalid ending on tag ") & m_name)
                End If
            End If

            i += 1
            ' skip /
            SkipSpaces(str, i)

            Dim endName As String = GetValue(str, i, ">"c, ControlChars.NullChar, True)
            If endName <> m_name Then
                Throw New XMLParsingException(Convert.ToString((Convert.ToString("Start/end tag name mismatch: ") & m_name) + " and ") & endName)
            End If
            SkipSpaces(str, i)

            If str(i) <> ">"c Then
                Throw New XMLParsingException(Convert.ToString("Invalid ending on tag ") & m_name)
            End If

            ' skip >
            i += 1
        End Sub
        ''' <summary>
        ''' Element value
        ''' </summary>
        Public ReadOnly Property Value() As String
            Get
                Return m_value
            End Get
        End Property
        ''' <summary>
        ''' Element name
        ''' </summary>
        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property
        ''' <summary>
        ''' List of subelements
        ''' </summary>
        Public ReadOnly Property SubNodes() As IEnumerable(Of NanoXMLNode)
            Get
                Return m_subNodes
            End Get
        End Property
        ''' <summary>
        ''' List of attributes
        ''' </summary>
        Public ReadOnly Property Attributes() As IEnumerable(Of NanoXMLAttribute)
            Get
                Return m_attributes
            End Get
        End Property
        ''' <summary>
        ''' Returns subelement by given name
        ''' </summary>
        ''' <param name="nodeName">Name of subelement to get</param>
        ''' <returns>First subelement with given name or NULL if no such element</returns>
        Default Public ReadOnly Property Item(nodeName As String) As NanoXMLNode
            Get
                For Each nanoXmlNode As NanoXMLNode In m_subNodes
                    If nanoXmlNode.Name = nodeName Then
                        Return nanoXmlNode
                    End If
                Next

                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Returns attribute by given name
        ''' </summary>
        ''' <param name="attributeName">Attribute name to get</param>
        ''' <returns><see cref="NanoXMLAttribute"/> with given name or null if no such attribute</returns>
        Public Function GetAttribute(attributeName As String) As NanoXMLAttribute
            For Each nanoXmlAttribute As NanoXMLAttribute In m_attributes
                If nanoXmlAttribute.Name = attributeName Then
                    Return nanoXmlAttribute
                End If
            Next

            Return Nothing
        End Function
    End Class

    ''' <summary>
    ''' XML element attribute
    ''' </summary>
    Public Class NanoXMLAttribute
        Private m_name As String
        Private m_value As String
        ''' <summary>
        ''' Attribute name
        ''' </summary>
        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property
        ''' <summary>
        ''' Attribtue value
        ''' </summary>
        Public ReadOnly Property Value() As String
            Get
                Return m_value
            End Get
        End Property

        Friend Sub New(name As String, value As String)
            Me.m_name = name
            Me.m_value = value
        End Sub
    End Class

    ''' <summary>
    ''' describes a xml parsing exception
    ''' </summary>
    ''' <remarks></remarks>
    Public Class XMLParsingException
        Inherits Exception
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace

