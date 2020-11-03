Imports System.IO
Imports System.Net
Imports WingtipToys.Models


Public Class NVPAPICaller
    Private Const bSandbox As Boolean = True
    Private Const CVV2 As String = "CVV2"
    Private pEndPointURL As String = "https://api-3t.paypal.com/nvp"
    Private host As String = "www.paypal.com"
    Private pEndPointURL_SB As String = "https://api-3t.sandbox.paypal.com/nvp"
    Private host_SB As String = "www.sandbox.paypal.com"
    Private Const SIGNATURE As String = "SIGNATURE"
    Private Const PWD As String = "PWD"
    Private Const ACCT As String = "ACCT"
    Public APIUsername As String = "sb-wlegw3638857_api1.business.example.com"
    Private APIPassword As String = "QZ8P3H7ZQ5X68DWP"
    Private APISignature As String = "APxsKCC3cSCVLODAWqxKsGzqWE1sASbgEFi.MeilW1db7toCG4AOoG-v"
    Private Subject As String = ""
    Private BNCode As String = "PP-ECWizard"
    Private Const Timeout As Integer = 30000
    Private Shared ReadOnly SECURED_NVPS As String() = New String() {ACCT, CVV2, SIGNATURE, PWD}

    Public Sub SetCredentials(ByVal Userid As String, ByVal Pwd As String, ByVal Signature As String)
        APIUsername = Userid
        APIPassword = Pwd
        APISignature = Signature
    End Sub

    Public Function ShortcutExpressCheckout(ByVal amt As String, ByRef token As String, ByRef retMsg As String) As Boolean
        If bSandbox Then
            pEndPointURL = pEndPointURL_SB
            host = host_SB
        End If

        Dim returnURL As String = "https://localhost:44344/Checkout/CheckoutReview.aspx"
        Dim cancelURL As String = "https://localhost:44344/Checkout/CheckoutCancel.aspx"
        Dim encoder As NVPCodec = New NVPCodec()
        encoder("METHOD") = "SetExpressCheckout"
        encoder("RETURNURL") = returnURL
        encoder("CANCELURL") = cancelURL
        encoder("BRANDNAME") = "Wingtip Toys Sample Application"
        encoder("PAYMENTREQUEST_0_AMT") = amt
        encoder("PAYMENTREQUEST_0_ITEMAMT") = amt
        encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
        encoder("PAYMENTREQUEST_0_CURRENCYCODE") = "USD"

        Using myCartOrders As WingtipToys.Logic.ShoppingCartActions = New WingtipToys.Logic.ShoppingCartActions()
            Dim myOrderList As List(Of CartItem) = myCartOrders.GetCartItems()

            For i As Integer = 0 To myOrderList.Count - 1
                encoder("L_PAYMENTREQUEST_0_NAME" & i) = myOrderList(i).Product.ProductName.ToString()
                encoder("L_PAYMENTREQUEST_0_AMT" & i) = myOrderList(i).Product.UnitPrice.ToString()
                encoder("L_PAYMENTREQUEST_0_QTY" & i) = myOrderList(i).Quantity.ToString()
            Next
        End Using

        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)
        Dim decoder As NVPCodec = New NVPCodec()
        decoder.Decode(pStresponsenvp)
        Dim strAck As String = decoder("ACK").ToLower()

        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            token = decoder("TOKEN")
            Dim ECURL As String = "https://" & host & "/cgi-bin/webscr?cmd=_express-checkout" & "&token=" & token
            retMsg = ECURL
            Return True
        Else
            retMsg = "ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=" & decoder("L_SHORTMESSAGE0") & "&" & "Desc2=" & decoder("L_LONGMESSAGE0")
            Return False
        End If
    End Function

    Public Function GetCheckoutDetails(ByVal token As String, ByRef PayerID As String, ByRef decoder As NVPCodec, ByRef retMsg As String) As Boolean
        If bSandbox Then
            pEndPointURL = pEndPointURL_SB
        End If

        Dim encoder As NVPCodec = New NVPCodec()
        encoder("METHOD") = "GetExpressCheckoutDetails"
        encoder("TOKEN") = token
        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)
        decoder = New NVPCodec()
        decoder.Decode(pStresponsenvp)
        Dim strAck As String = decoder("ACK").ToLower()

        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            PayerID = decoder("PAYERID")
            Return True
        Else
            retMsg = "ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=" & decoder("L_SHORTMESSAGE0") & "&" & "Desc2=" & decoder("L_LONGMESSAGE0")
            Return False
        End If
    End Function

    Public Function DoCheckoutPayment(ByVal finalPaymentAmount As String, ByVal token As String, ByVal PayerID As String, ByRef decoder As NVPCodec, ByRef retMsg As String) As Boolean
        If bSandbox Then
            pEndPointURL = pEndPointURL_SB
        End If

        Dim encoder As NVPCodec = New NVPCodec()
        encoder("METHOD") = "DoExpressCheckoutPayment"
        encoder("TOKEN") = token
        encoder("PAYERID") = PayerID
        encoder("PAYMENTREQUEST_0_AMT") = finalPaymentAmount
        encoder("PAYMENTREQUEST_0_CURRENCYCODE") = "USD"
        encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)
        decoder = New NVPCodec()
        decoder.Decode(pStresponsenvp)
        Dim strAck As String = decoder("ACK").ToLower()

        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            Return True
        Else
            retMsg = "ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=" & decoder("L_SHORTMESSAGE0") & "&" & "Desc2=" & decoder("L_LONGMESSAGE0")
            Return False
        End If
    End Function

    Public Function HttpCall(ByVal NvpRequest As String) As String
        Dim url As String = pEndPointURL
        Dim strPost As String = NvpRequest & "&" & buildCredentialsNVPString()
        strPost = strPost & "&BUTTONSOURCE=" & HttpUtility.UrlEncode(BNCode)
        Dim objRequest As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        objRequest.Timeout = Timeout
        objRequest.Method = "POST"
        objRequest.ContentLength = strPost.Length

        Try

            Using myWriter As StreamWriter = New StreamWriter(objRequest.GetRequestStream())
                myWriter.Write(strPost)
            End Using

        Catch __unusedException1__ As Exception
        End Try

        Dim objResponse As HttpWebResponse = CType(objRequest.GetResponse(), HttpWebResponse)
        Dim result As String

        Using sr As StreamReader = New StreamReader(objResponse.GetResponseStream())
            result = sr.ReadToEnd()
        End Using

        Return result
    End Function

    Private Function buildCredentialsNVPString() As String
        Dim codec As NVPCodec = New NVPCodec()
        If Not IsEmpty(APIUsername) Then codec("USER") = APIUsername
        If Not IsEmpty(APIPassword) Then codec(PWD, 1) = APIPassword
        If Not IsEmpty(APISignature) Then codec(SIGNATURE) = APISignature
        If Not IsEmpty(Subject) Then codec("SUBJECT") = Subject
        codec("VERSION", 4) = "88.0"
        Return codec.Encode()
    End Function

    Public Shared Function IsEmpty(ByVal s As String) As Boolean
        Return s Is Nothing OrElse s.Trim() = String.Empty
    End Function
End Class

Public NotInheritable Class NVPCodec
    Inherits NameValueCollection

    Private Const AMPERSAND As String = "&"
    Private Const EQUALS As String = "="
    Private Shared ReadOnly AMPERSAND_CHAR_ARRAY As Char() = AMPERSAND.ToCharArray()
    Private Shared ReadOnly EQUALS_CHAR_ARRAY As Char() = EQUALS.ToCharArray()

    Public Function Encode() As String
        Dim sb As StringBuilder = New StringBuilder()
        Dim firstPair As Boolean = True

        For Each kv As String In AllKeys
            Dim name As String = HttpUtility.UrlEncode(kv)
            Dim value As String = HttpUtility.UrlEncode(Me(kv))

            If Not firstPair Then
                sb.Append(AMPERSAND)
            End If

            sb.Append(name).Append(EQUALS).Append(value)
            firstPair = False
        Next

        Return sb.ToString()
    End Function

    Public Sub Decode(ByVal nvpstring As String)
        Clear()

        For Each nvp As String In nvpstring.Split(AMPERSAND_CHAR_ARRAY)
            Dim tokens As String() = nvp.Split(EQUALS_CHAR_ARRAY)

            If tokens.Length >= 2 Then
                Dim name As String = HttpUtility.UrlDecode(tokens(0))
                Dim value As String = HttpUtility.UrlDecode(tokens(1))
                Add(name, value)
            End If
        Next
    End Sub

    Public Overloads Sub Add(ByVal name As String, ByVal value As String, ByVal index As Integer)
        Me.Add(GetArrayName(index, name), value)
    End Sub

    Public Overloads Sub Remove(ByVal arrayName As String, ByVal index As Integer)
        Me.Remove(GetArrayName(index, arrayName))
    End Sub

    Default Public Overloads Property Item(ByVal name As String, ByVal index As Integer) As String
        Get
            Return Me(GetArrayName(index, name))
        End Get
        Set(ByVal value As String)
            Me(GetArrayName(index, name)) = value
        End Set
    End Property

    Private Shared Function GetArrayName(ByVal index As Integer, ByVal name As String) As String
        If index < 0 Then
            Throw New ArgumentOutOfRangeException("index", "index cannot be negative : " & index)
        End If

        Return name & index
    End Function
End Class
