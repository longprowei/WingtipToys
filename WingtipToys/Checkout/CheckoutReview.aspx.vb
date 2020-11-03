Imports WingtipToys.Models

Namespace Checkout

    Partial Public Class CheckoutReview
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim payPalCaller As New NVPAPICaller()

                Dim retMsg As String = ""
                Dim token As String = ""
                Dim PayerID As String = ""
                Dim decoder As New NVPCodec()
                token = Session("token").ToString()

                Dim ret As Boolean = payPalCaller.GetCheckoutDetails(token, PayerID, decoder, retMsg)
                If ret Then
                    Session("payerId") = PayerID
                    Dim myOrder As New Order()
                    myOrder.OrderDate = Convert.ToDateTime(decoder("TIMESTAMP").ToString())
                    myOrder.Username = User.Identity.Name
                    myOrder.FirstName = decoder("FIRSTNAME").ToString()
                    myOrder.LastName = decoder("LASTNAME").ToString()
                    myOrder.Address = decoder("SHIPTOSTREET").ToString()
                    myOrder.City = decoder("SHIPTOCITY").ToString()
                    myOrder.State = decoder("SHIPTOSTATE").ToString()
                    myOrder.PostalCode = decoder("SHIPTOZIP").ToString()
                    myOrder.Country = decoder("SHIPTOCOUNTRYCODE").ToString()
                    myOrder.Email = decoder("EMAIL").ToString()
                    myOrder.Total = Convert.ToDecimal(decoder("AMT").ToString())

                    Try
                        Dim paymentAmountOnCheckout As Decimal = Convert.ToDecimal(Session("payment_amt").ToString())
                        Dim paymentAmoutFromPayPal As Decimal = Convert.ToDecimal(decoder("AMT").ToString())
                        If paymentAmountOnCheckout <> paymentAmoutFromPayPal Then
                            Response.Redirect("CheckoutError.aspx?" + "Desc=Amount%20total%20mismatch.")
                        End If
                    Catch ex As Exception
                        Response.Redirect("CheckoutError.aspx?" + "Desc=Amount%20total%20mismatch.")
                    End Try

                    Dim _db As New ProductContext()

                    _db.Orders.Add(myOrder)
                    _db.SaveChanges()

                    Using usersShoppingCart As New Logic.ShoppingCartActions()
                        Dim myOrderList As List(Of CartItem) = usersShoppingCart.GetCartItems()
                        For i As Integer = 0 To myOrderList.Count - 1
                            Dim myOrderDetail As New OrderDetail()
                            myOrderDetail.OrderId = myOrder.OrderId
                            myOrderDetail.Username = User.Identity.Name
                            myOrderDetail.ProductId = myOrderList(i).ProductId
                            myOrderDetail.Quantity = myOrderList(i).Quantity
                            myOrderDetail.UnitPrice = myOrderList(i).Product.UnitPrice

                            _db.OrderDetails.Add(myOrderDetail)
                            _db.SaveChanges()
                        Next

                        Session("currentOrderId") = myOrder.OrderId

                        Dim orderList As New List(Of Order)
                        orderList.Add(myOrder)
                        ShipInfo.DataSource = orderList
                        ShipInfo.DataBind()

                        OrderItemList.DataSource = myOrderList
                        OrderItemList.DataBind()
                    End Using
                Else
                    Response.Redirect("CheckoutError.aspx?" + retMsg)
                End If
            End If
        End Sub

        Protected Sub CheckoutConfirm_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Session("userCheckoutCompleted") = "true"
            Response.Redirect("~/Checkout/CheckoutComplete.aspx")
        End Sub
    End Class

End Namespace