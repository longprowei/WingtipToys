Imports WingtipToys.Models
Imports WingtipToys.Logic
Imports System.Collections.Specialized
Imports System.Collections
Imports System.Web.ModelBinding

Public Class ShoppingCart
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Using usersShoppingCart As New ShoppingCartActions()
            Dim cartTotal As Decimal = 0
            cartTotal = usersShoppingCart.GetTotal()
            If cartTotal > 0 Then
                lblTotal.Text = String.Format("{0:c}", cartTotal)
            Else
                LabelTotalText.Text = ""
                lblTotal.Text = ""
                ShoppingCartTitle.InnerText = "Shopping Cart is Empty"
                UpdateBtn.Visible = False
                CheckoutImageBtn.Visible = False
            End If
        End Using
    End Sub

    Public Function GetShoppingCartItems() As List(Of CartItem)
        Dim actions As New ShoppingCartActions()
        Return actions.GetCartItems()
    End Function

    Public Function UpdateCartItems() As List(Of CartItem)
        Using usersShoppingCart As New ShoppingCartActions()
            Dim cartId As String = usersShoppingCart.GetCartId()

            Dim cartUpdates As ShoppingCartActions.ShoppingCartUpdates() = New ShoppingCartActions.ShoppingCartUpdates(CartList.Rows.Count - 1) {}

            For i As Integer = 0 To CartList.Rows.Count - 1
                Dim rowValues As IOrderedDictionary = New OrderedDictionary()
                rowValues = GetValues(CartList.Rows(i))
                cartUpdates(i).ProductId = Convert.ToInt32(rowValues("ProductID"))

                Dim cbRemove As New CheckBox()
                cbRemove = DirectCast(CartList.Rows(i).FindControl("Remove"), CheckBox)
                cartUpdates(i).RemoveItem = cbRemove.Checked

                Dim quantityTextBox As New TextBox
                quantityTextBox = DirectCast(CartList.Rows(i).FindControl("PurchaseQuantity"), TextBox)
                cartUpdates(i).PurchaseQuantity = Convert.ToInt16(quantityTextBox.Text.ToString())
            Next
            usersShoppingCart.UpdateShoppingCartDatabase(cartId, cartUpdates)
            CartList.DataBind()
            lblTotal.Text = String.Format("{0:c}", usersShoppingCart.GetTotal())
            Return usersShoppingCart.GetCartItems()
        End Using
    End Function

    Public Shared Function GetValues(row As GridViewRow) As IOrderedDictionary
        Dim values As IOrderedDictionary = New OrderedDictionary()
        For Each cell As DataControlFieldCell In row.Cells
            If cell.Visible Then
                cell.ContainingField.ExtractValuesFromCell(values, cell, row.RowState, True)
            End If
        Next

        Return values
    End Function

    Protected Sub UpdateBtn_Click(ByVal sender As Object, ByVal e As EventArgs)
        UpdateCartItems()
    End Sub

    Protected Sub CheckoutBtn_Click(sender As Object, e As ImageClickEventArgs)
        Using usersShoppingCart As New ShoppingCartActions()
            Session("payment_amt") = usersShoppingCart.GetTotal()
            Response.Redirect("Checkout/CheckoutStart.aspx")
        End Using
    End Sub

End Class