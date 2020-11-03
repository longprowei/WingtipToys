Imports WingtipToys.Models

Namespace Checkout
    Public Class CheckoutComplete
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If DirectCast(Session("userCheckoutCompleted"), String) <> "true" Then
                    Session("userCheckoutCompleted") = String.Empty
                    Response.Redirect("CheckoutError.aspx?" + "Desc=Unvalidated%20Checkout.")
                End If

                Dim payPalCaller As New NVPAPICaller()
                Dim retMsg As String = ""
                Dim token As String = ""
                Dim finalPaymentAmount As String = ""
                Dim PayerID As String = ""

                Dim decoder As New NVPCodec()

                token = Session("token").ToString()
                PayerID = Session("payerId").ToString()
                finalPaymentAmount = Session("payment_amt").ToString()

                Dim ret As Boolean = payPalCaller.DoCheckoutPayment(finalPaymentAmount, token, PayerID, decoder, retMsg)
                If ret Then
                    Dim PaymentConfirmation As String = decoder("PAYMENTINFO_0_TRANSACTIONID").ToString()
                    TransactionId.Text = PaymentConfirmation

                    Dim _db As New ProductContext()
                    Dim currentOrderId As Integer = -1
                    If Session("currentOrderId") <> String.Empty Then
                        currentOrderId = Convert.ToInt32(Session("currentOrderId"))
                    End If

                    Dim myCurrentOrder As Order
                    If currentOrderId >= 0 Then
                        myCurrentOrder = _db.Orders.Single(Function(o) o.OrderId = currentOrderId)
                        myCurrentOrder.PaymentTransactionId = PaymentConfirmation
                        _db.SaveChanges()
                    End If

                    Using usersShoppingCart As New Logic.ShoppingCartActions()
                        usersShoppingCart.EmptyCart()
                    End Using

                    Session("currentOrderId") = String.Empty
                Else
                    Response.Redirect("CheckoutError.aspx?" + retMsg)
                End If
            End If
        End Sub

        Protected Sub Continue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("~/Default.aspx")
        End Sub
    End Class
End Namespace